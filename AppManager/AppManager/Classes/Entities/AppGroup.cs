using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using AppManager.EntityCollection;
using CommonLib;
using CommonLib.IO;


namespace AppManager.Entities
{
	[Serializable]
	public class AppGroup : EntityBase<AppGroup>
	{
		public event EventHandler<ValueEventArgs<AppInfo>> AppInfoDeleted;
		public event EventHandler NeedAppImage;


		protected AppTypeCollection _AppTypes;
		protected int _LastAppInfoID = 1;


		public AppGroup()
		{
			_AppTypes = new AppTypeCollection();
			_AppTypes.CollectionChanged += AppTypesCollectionChanged;
		}


		public string AppGroupName
		{ get; set; }

		public AppTypeCollection AppTypes
		{ get { return _AppTypes; } }

		public int LastAppInfoID
		{
			get { return _LastAppInfoID; }
			set { _LastAppInfoID = value; }
		}

		
		public string GetDefaultTypeName()
		{
			string appTypeName = Strings.APPLICATIONS;
			int i = 0;
			while (AppTypeNameExists(appTypeName))
				appTypeName = Strings.APPLICATIONS + i;

			return appTypeName;
		}

		public bool AppTypeNameExists(string appTypeName)
		{
			foreach (var item in AppTypes)
				if (item.AppTypeInfo == appTypeName)
					return true;

			return false;
		}

		public void RequestAppImage(AppInfo app)
		{
			app.NeedImage += (s, e) => OnNeedAppImage(s as AppInfo);
			app.RequestAppImage();
		}

		public int GetMaxAppCountPerType()
		{
			int maxApps = 1;
			foreach (var item in _AppTypes)
				maxApps = Math.Max(item.AppInfos.Count, maxApps);

			return maxApps;
		}

		public AppInfo FindAppByExecPath(string execPath)
		{
			AppInfoCollection result = new AppInfoCollection();

			foreach (var item in AllApps())
				if (PathHelper.ComparePath(item.ExecPath, execPath))
					return item;

			return null;
		}

		public AppInfoCollection FindApps(string name)
		{
			AppInfoCollection result = new AppInfoCollection();

			foreach (var item in _AppTypes)
				result.AddRange(FindApps(item.AppInfos, name));

			return result;
		}

		public AppInfoCollection FindApps(AppInfoCollection collection, string name)
		{
			AppInfoCollection result = new AppInfoCollection();

			foreach (AppInfo item in collection)
			{
				if (item.AppName.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) >= 0)
					result.Add(item);
			}

			return result;
		}

		public AppType FindAppType(string appTypeName)
		{
			foreach (var item in _AppTypes)
			{
				if (string.Equals(item.AppTypeName, appTypeName, StringComparison.CurrentCultureIgnoreCase))
					return item;
			}

			return null;
		}

		public AppType FindAppType(AppInfo appInfo)
		{
			foreach (var item in _AppTypes)
			{
				int ix = item.AppInfos.IndexOf(appInfo);
				if (ix >= 0)
					return item;
			}

			return null;
		}

		public void GroupByFolders(AppType source)
		{
			if (_AppTypes.Count <= 0)
				return;

			var appSource = source.AppInfos;

			var winGroup = GroupToWindows(appSource);
			if (winGroup.AppInfos.Count > 0)
				_AppTypes.Add(winGroup);

			string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			var groupedApps = new List<int>(appSource.Count);
			for (int i = appSource.Count - 1; i >= 0; )
			{
				string progPath = PathHelper.GetNextPathLevel(appSource[i].AppPath, progFiles);
				if (String.IsNullOrEmpty(progPath))
				{
					i--;
					continue;
				}

				AppType pfApps = new AppType() { AppTypeName = progPath };
				groupedApps.Clear();

				for (int j = appSource.Count - 1; j >= 0; j--)
				{
					string progStartPath = Path.Combine(progFiles, progPath);
					if (appSource[j].AppPath.StartsWith(progStartPath, StringComparison.InvariantCultureIgnoreCase))
					{
						groupedApps.Add(j);
					}
				}

				if (groupedApps.Count > 3)
				{
					for (int k = 0; k < groupedApps.Count; k++)
					{
						pfApps.AppInfos.Add(appSource[groupedApps[k]]);
						appSource.RemoveAt(groupedApps[k]);
					}

					_AppTypes.Add(pfApps);
					i -= groupedApps.Count;
				}
				else
					i--;
			}
		}

		public AppInfo CreateNewAppInfo(AppType appType)
		{
			return CreateNewAppInfo(appType, String.Empty); ;
		}

		public AppInfo CreateNewAppInfo(AppType appType, string execPath)
		{
			return CreateNewAppInfo(appType, Strings.NEW_APP, execPath, null);
		}

		public AppInfo CreateNewAppInfo(AppType appType, string appName, string execPath, string imagePath)
		{
			if (execPath == null)
				execPath = String.Empty;

			AppInfo newInfo = new AppInfo()
			{
				AppName = appName,
				ID = _LastAppInfoID++
			};

			if (LnkHelper.CompareIconPath(execPath, imagePath))
				imagePath = String.Empty;

			newInfo.NeedImage += (s, e) => OnNeedAppImage(s as AppInfo);
			newInfo.ImagePath = imagePath;
			newInfo.ExecPath = execPath;
			newInfo.SetAutoAppName();

			if (appType != null)
				appType.AppInfos.Add(newInfo);

			return newInfo;
		}

		public void CorrectAppInfoID()
		{
			int maxId = -1;
			foreach (var ai in AllApps())
				maxId = Math.Max(maxId, ai.ID);

			_LastAppInfoID = maxId + 1;

			foreach (var ai in AllApps())
			{
				if (ai.ID <= 0)
					ai.ID = _LastAppInfoID++;
				else if (ai.ID > _LastAppInfoID)
					_LastAppInfoID = ai.ID + 1;
			}
		}

		public void ReInitImages()
		{
			foreach (AppInfo item in AllApps())
				RequestAppImage(item);
		}


		protected void AppTypesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (AppType at in e.NewItems)
				{
					at.AppInfoDeleted += OnAppInfoDeleted;
				}
			}
			else if (
				e.Action == NotifyCollectionChangedAction.Remove ||
				e.Action == NotifyCollectionChangedAction.Replace ||
				e.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (AppType at in e.OldItems)
				{
					at.AppInfoDeleted -= OnAppInfoDeleted;
				}
			}
		}

		protected virtual void OnAppInfoDeleted(object sender, ValueEventArgs<AppInfo> e)
		{
			if (AppInfoDeleted != null)
				AppInfoDeleted(sender, e);
		}

		protected virtual void OnNeedAppImage(AppInfo app)
		{
			if (NeedAppImage != null)
				NeedAppImage(app, EventArgs.Empty);
		}

		protected override void MergeEntity(AppGroup source, bool clone)
		{
			base.MergeEntity(source, clone);

			if (AppGroupName != source.AppGroupName)
				AppGroupName = source.AppGroupName;

			_LastAppInfoID = source._LastAppInfoID;
			AppTypes.Combine(source.AppTypes, clone);

			if (clone)
			{
				ReInitImages();
				//StartLoadImages();
			}
		}

		protected AppType GroupToWindows(AppInfoCollection appSource)
		{
			var winApps = new AppType() { AppTypeName = "Windows" };

			if (_AppTypes.Count <= 0)
				return winApps;

			string winDir = Environment.GetEnvironmentVariable("windir");

			for (int i = appSource.Count - 1; i >= 0; i--)
			{
				if (appSource[i].AppPath.StartsWith(winDir, StringComparison.InvariantCultureIgnoreCase))
				{
					winApps.AppInfos.Add(appSource[i]);
					appSource.RemoveAt(i);
				}
			}

			return winApps;
		}

		protected IEnumerable<AppInfo> AllApps()
		{
			foreach (var type in _AppTypes)
				foreach (AppInfo item in type.AppInfos)
					yield return item;

			yield break;
		}
	}
}
