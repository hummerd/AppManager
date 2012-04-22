using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using AppManager.Entities;
using System.Collections.Specialized;
using CommonLib;

namespace AppManager.Classes.ViewModel
{
    public class AppGroupView : ViewBase<AppGroup>
	{
		protected CollectionSyncronizer<AppType, AppTypeView> m_AppTypeSync;


        public AppGroupView(MainWorkItem workItem)
            : base(workItem)
        { ; }

		public ObservableCollection<AppTypeView> AppTypes { get; set; }


		public void Init(AppGroup group)
		{
			Source = group;
			AppTypes = new ObservableCollection<AppTypeView>();

            if (m_AppTypeSync != null)
            {
                m_AppTypeSync.TargetUpdated -= OnTargetUpdated;
            }
            
			m_AppTypeSync = new CollectionSyncronizer<AppType, AppTypeView>(
				group.AppTypes,
				AppTypes,
				s => 
				{
					var result = new AppTypeView(m_WorkItem);
					result.Source = s;
					result.Init(s);
					return result;
				},
				true);

            m_AppTypeSync.TargetUpdated += OnTargetUpdated;
		}

		public void SetAppTitleView()
		{
            foreach (var at in AppTypes)
                at.SetAppTitleView();
		}

        protected void OnTargetUpdated(object sender, CollectionEventArgs<AppTypeView> ea)
        {
            SetAppTitleView();
        }

		protected ObservableCollection<AppInfoView> GetAppInfoView(IEnumerable<AppInfo> appInfos)
		{
			var result = new ObservableCollection<AppInfoView>();

			foreach (var item in appInfos)
			{
				result.Add(new AppInfoView
				{
					Source = item,
					ShowTitle = true
				});
			}

			return result;
		}
	}
}
