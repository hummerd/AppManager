using System;
using System.Collections.Generic;
using System.IO;
using AppManager.EntityCollection;
using CommonLib;


namespace AppManager
{
	[Serializable]
	public class AppGroup : IClonableEntity<AppGroup>
	{
		protected AppTypeCollection _AppTypes;
		protected int _LastAppInfoID = 1;
		protected AsyncImageLoader _ImageLoader = new AsyncImageLoader();


		public AppGroup()
		{
			_AppTypes = new AppTypeCollection();
		}

		public AppGroup(IEnumerable<AppType> collection)
		{
			_AppTypes = new AppTypeCollection(collection);
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


		public void RequestAppImage(AppInfo app)
		{
			app.NeedImage += (s, e) => RequestImage(s as AppInfo);
			app.RequestAppImage();
		}

		public void StartLoadImages()
		{
			ReInitImages();
			_ImageLoader.StartLoad();
		}

		public int GetMaxAppCountPerType()
		{
			int maxApps = 1;
			foreach (var item in _AppTypes)
				maxApps = Math.Max(item.AppInfos.Count, maxApps);

			return maxApps;
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

		public void GroupByFolders()
		{
			if (_AppTypes.Count <= 0)
				return;

			string winDir = Environment.GetEnvironmentVariable("windir");
			AppType winApps = new AppType() { AppTypeName = "Windows" };
			var appSource = _AppTypes[0].AppInfos;
			for (int i = appSource.Count - 1; i >= 0; i--)
			{
				if (appSource[i].AppPath.StartsWith(winDir, StringComparison.InvariantCultureIgnoreCase))
				{
					winApps.AppInfos.Add(appSource[i]);
					appSource.RemoveAt(i);
				}
			}

			_AppTypes.Add(winApps);

			string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			var groupedApps = new List<int>(appSource.Count);
			for (int i = appSource.Count - 1; i >= 0;)
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
            return CreateNewAppInfo(appType, Strings.NEW_APP, execPath);
        }

		public AppInfo CreateNewAppInfo(AppType appType, string appName, string execPath)
		{
			if (execPath == null)
				execPath = String.Empty;
			
			AppInfo newInfo = new AppInfo()
			{
            AppName = appName,
				AppInfoID = _LastAppInfoID++
			};

			newInfo.SetAutoAppName();

			newInfo.NeedImage += (s, e) => RequestImage(s as AppInfo);
			newInfo.ExecPath = execPath;

			if (appType != null)
				appType.AppInfos.Add(newInfo);

			return newInfo;
		}

		public void CorrectAppInfoID()
		{
			int maxId = -1;
			foreach (var at in AppTypes)
				foreach (var ai in at.AppInfos)
					maxId = Math.Max(maxId, ai.AppInfoID);

			_LastAppInfoID = maxId + 1;

			foreach (var ai in AllApps())
			{
				if (ai.AppInfoID <= 0)
					ai.AppInfoID = _LastAppInfoID++;
				else if (ai.AppInfoID > _LastAppInfoID)
					_LastAppInfoID = ai.AppInfoID + 1;
			}
		}

		#region IClonableEntity<AppGroup> Members

		public AppGroup CloneSource
		{
			get;
			set;
		}

		public AppGroup CloneEntity()
		{
			AppGroup clone = new AppGroup(AppTypes.Copy());
			clone.AppGroupName = AppGroupName;
			clone._LastAppInfoID = _LastAppInfoID;
			clone.CloneSource = this;

			clone.ReInitImages();
			clone.StartLoadImages();

			return clone;
		}

		public void MergeEntity(AppGroup source)
		{
			if (AppGroupName != source.AppGroupName)
				AppGroupName = source.AppGroupName;

			_LastAppInfoID = source._LastAppInfoID;

			AppTypes.MergeCollection(source.AppTypes);
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return CloneEntity();
		}

		#endregion


		protected void RequestImage(AppInfo app)
		{
			if (app == null)
				return;

			_ImageLoader.RequestImage(app);
		}

		protected void ReInitImages()
		{
			foreach (AppInfo item in AllApps())
				RequestAppImage(item);
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
