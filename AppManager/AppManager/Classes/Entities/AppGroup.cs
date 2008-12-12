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
			clone.CloneSource = this;
			return clone;
		}

		public void MergeEntity(AppGroup source)
		{
			if (AppGroupName != source.AppGroupName)
				AppGroupName = source.AppGroupName;

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
