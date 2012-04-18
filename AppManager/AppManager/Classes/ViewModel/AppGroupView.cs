using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using AppManager.Entities;
using System.Collections.Specialized;
using CommonLib;

namespace AppManager.Classes.ViewModel
{
	public class AppGroupView : ISourceReference<AppGroup>
	{
		protected CollectionSyncronizer<AppType, AppTypeView> m_AppTypeSync;


		public AppGroup Source
		{
			get;
			set;
		}

		public ObservableCollection<AppTypeView> AppTypes { get; set; }


		public void Init(AppGroup group)
		{
			Source = group;
			AppTypes = new ObservableCollection<AppTypeView>();

			m_AppTypeSync = new CollectionSyncronizer<AppType, AppTypeView>(
				group.AppTypes,
				AppTypes,
				s => 
				{
					var result = new AppTypeView();
					result.Source = s;
					result.Init(s);
					return result;
				},
				true);
		}

		public void SetAppTitleView(bool visible)
		{
			foreach (var av in EnumAppViews())
				av.ShowTitle = visible;
		}


		protected IEnumerable<AppInfoView> EnumAppViews()
		{
			foreach (var atv in AppTypes)
				foreach (var av in atv.AppInfos)
					yield return av;
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
