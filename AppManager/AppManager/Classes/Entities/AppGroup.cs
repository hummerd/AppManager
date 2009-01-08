using System;
using System.Collections.Generic;
using System.Text;
using AppManager.EntityCollection;


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


		public AppInfo CreateNewAppInfo(AppType appType)
		{
			AppInfo newInfo = new AppInfo()
			{
				AppName = Strings.NEW_APP,
				AppInfoID = _LastAppInfoID++
			};

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
