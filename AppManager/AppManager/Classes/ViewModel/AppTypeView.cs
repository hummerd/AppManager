using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using AppManager.Entities;
using CommonLib;
using System.ComponentModel;


namespace AppManager.Classes.ViewModel
{
	public class AppTypeView : ISourceReference<AppType>
	{
		public event PropertyChangedEventHandler PropertyChanged;


		protected CollectionSyncronizer<AppInfo, AppInfoView> m_AppInfoSync;


		public AppType Source
		{
			get;
			set;
		}

		public ObservableCollection<AppInfoView> AppInfos
		{ get; set; }


		public void Init(AppType appType)
		{
			Source = appType;
			AppInfos = new ObservableCollection<AppInfoView>();

			m_AppInfoSync = new CollectionSyncronizer<AppInfo, AppInfoView>(
				appType.AppInfos,
				AppInfos,
				s =>
				{
					var result = new AppInfoView();
					result.Source = s;
					result.ShowTitle = true;
					return result;
				},
				true);
		}
	}
}
