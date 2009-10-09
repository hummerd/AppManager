using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using AppManager.EntityCollection;


namespace AppManager
{
	public class AppTypeCollection : EntityCollection<AppType>
	{
		public AppTypeCollection()
		{

		}

		//public AppTypeCollection(IEnumerable<AppType> collection)
        //    : base(collection)
        //{

        //}
	}

	[Serializable]
	public class AppType : EntityBase<AppType>
	{
		protected AppInfoCollection _AppInfos;
		protected string _AppTypeName;


		public AppType()
		{
			_AppInfos = new AppInfoCollection();
			_AppInfos.CollectionChanged += (s, e) => OnPropertyChanged("AppTypeInfo");
		}

		
		public string AppTypeName
		{
			get { return _AppTypeName; }
			set { _AppTypeName = value; OnPropertyChanged("AppTypeName"); }
		}

		public string AppTypeInfo
		{
			get { return Strings.APPS_IN_TYPE + " " + _AppInfos.Count; }
		}

		public AppInfoCollection AppInfos
		{ get { return _AppInfos; } }
		

		public override string ToString()
		{
			return _AppTypeName;
		}


		protected override void MergeEntity(AppType source, bool clone)
		{
			base.MergeEntity(source, clone);

			if (AppTypeName != source.AppTypeName)
				AppTypeName = source.AppTypeName;

			AppInfos.Combine(source.AppInfos, clone);
		}
	}
}
