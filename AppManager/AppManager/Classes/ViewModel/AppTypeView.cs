using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using AppManager.Entities;
using CommonLib;
using System.ComponentModel;


namespace AppManager.Classes.ViewModel
{
	public class AppTypeView : ViewBase<AppType>
	{
		public event PropertyChangedEventHandler PropertyChanged;


		protected CollectionSyncronizer<AppInfo, AppInfoView> m_AppInfoSync;


		public AppTypeView(MainWorkItem workItem)
			: base(workItem)
		{ ; }

		public ObservableCollection<AppInfoView> AppInfos
		{ get; set; }


		public void Init(AppType appType)
		{
			Source = appType;
			AppInfos = new ObservableCollection<AppInfoView>();

			if (m_AppInfoSync != null)
			{
				m_AppInfoSync.TargetUpdated -= OnTargetUpdated;
			}

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

            m_AppInfoSync.TargetUpdated += OnTargetUpdated;
		}

		public void SetAppTitleView()
		{
            foreach (var av in AppInfos)
				av.ShowTitle = m_WorkItem.Settings.ShowAppTitles;
		}


        protected void OnTargetUpdated(object sender, CollectionEventArgs<AppInfoView> ea)
        {
            SetAppTitleView();
        }
	}
}
