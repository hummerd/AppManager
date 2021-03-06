﻿using System;
using System.Collections.Specialized;
using AppManager.EntityCollection;
using CommonLib;


namespace AppManager.Entities
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
		public event EventHandler<ValueEventArgs<AppInfo>> AppInfoDeleted;


		protected AppInfoCollection _AppInfos;
		protected string _AppTypeName;


		public AppType()
		{
			_AppInfos = new AppInfoCollection();
			_AppInfos.CollectionChanged += (s, e) => OnPropertyChanged("AppTypeInfo");
			_AppInfos.CollectionChanged += (s, e) =>
				{
					if (
						e.Action == NotifyCollectionChangedAction.Remove ||
						e.Action == NotifyCollectionChangedAction.Replace ||
						e.Action == NotifyCollectionChangedAction.Reset)
					{
						foreach (AppInfo ai in e.OldItems)
						{
							OnAppInfoDeleted(ai);
						}
					}
				};
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


		public AppType CloneWithoutItems()
		{
			var result = new AppType();
			result.MergeEntity(this, true, false);
			return result;
		}

		public override string ToString()
		{
			return _AppTypeName;
		}


		protected virtual void OnAppInfoDeleted(AppInfo appInfo)
		{
			if (AppInfoDeleted != null)
				AppInfoDeleted(this, new ValueEventArgs<AppInfo>(appInfo));
		}

		protected override void MergeEntity(AppType source, bool clone)
		{
			MergeEntity(source, clone, true);
		}

		protected void MergeEntity(AppType source, bool clone, bool cloneAppInfo)
		{
			base.MergeEntity(source, clone);

			if (AppTypeName != source.AppTypeName)
				AppTypeName = source.AppTypeName;

			if (cloneAppInfo)
				AppInfos.Combine(source.AppInfos, clone);
		}
	}
}
