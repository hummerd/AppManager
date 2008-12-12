using System;
using System.Collections.Generic;
using System.Text;
using AppManager.EntityCollection;
using System.ComponentModel;
using System.Collections.Specialized;


namespace AppManager
{
	public class AppTypeCollection : EntityCollection<AppType>
	{
		public AppTypeCollection()
		{

		}

		public AppTypeCollection(IEnumerable<AppType> collection)
			: base(collection)
		{

		}
	}

	[Serializable]
	public class AppType : IClonableEntity<AppType>, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		protected AppInfoCollection _AppInfos;
		protected string _AppTypeName;


		public AppType()
		{
			_AppInfos = new AppInfoCollection();
			_AppInfos.CollectionChanged += AppInfos_CollectionChanged;
		}

		public AppType(IEnumerable<AppInfo> collection)
		{
			_AppInfos = new AppInfoCollection(collection);
			_AppInfos.CollectionChanged += AppInfos_CollectionChanged;
		}

		
		public string AppTypeName
		{
			get { return _AppTypeName; }
			set { _AppTypeName = value; OnPropertyChanged(new PropertyChangedEventArgs("AppTypeName")); }
		}

		public string AppTypeInfo
		{
			get { return "Приложений в групе: " + _AppInfos.Count; }
		}

		public AppInfoCollection AppInfos
		{ get { return _AppInfos; } }
		

		#region IClonableEntity<AppType> Members

		public AppType CloneSource
		{
			get;
			set;
		}

		public AppType CloneEntity()
		{
			AppType clone = new AppType(AppInfos.Copy());
			clone.AppTypeName = AppTypeName;
			clone.CloneSource = this;
			return clone;
		}

		public void MergeEntity(AppType source)
		{
			if (AppTypeName != source.AppTypeName)
				AppTypeName = source.AppTypeName;

			AppInfos.MergeCollection(source.AppInfos);
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return CloneEntity();
		}

		#endregion

		
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}


		private void AppInfos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(new PropertyChangedEventArgs("AppTypeInfo"));
		}
	}
}
