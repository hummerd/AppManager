using System;
using System.Collections.Generic;
using System.Text;
using AppManager.EntityCollection;
using System.Xml.Serialization;


namespace AppManager
{
	[Serializable]
	public class AppGroup : IClonableEntity<AppGroup>
	{
		protected AppTypeCollection _AppTypes;
		protected int _LastAppInfoID = 1;


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
		}

		public AppInfo CreateNewAppInfo(AppType appType)
		{
			return CreateNewAppInfo(appType, String.Empty); ;
		}

		public AppInfo CreateNewAppInfo(AppType appType, string execPath)
		{
			if (execPath == null)
				execPath = String.Empty;
			
			AppInfo newInfo = new AppInfo()
			{
				AppName = Strings.NEW_APP,
				ExecPath = execPath,
				AppInfoID = _LastAppInfoID++
			};

			newInfo.SetAutoAppName();

			if (appType != null)
				appType.AppInfos.Add(newInfo);

			return newInfo;
		}

		public void CorrectAppInfoID()
		{
			foreach (var at in AppTypes)
			{
				foreach (var ai in at.AppInfos)
				{
					if (ai.AppInfoID <= 0)
						ai.AppInfoID = _LastAppInfoID++;
					else if (ai.AppInfoID > _LastAppInfoID)
						_LastAppInfoID = ai.AppInfoID + 1;
				}
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
	}
}
