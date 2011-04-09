using System;
using System.Collections.Generic;
using AppManager.Entities;
using CommonLib.PInvoke;
using CommonLib.Application;
using AppManager.Classes.ViewModel;


namespace AppManager.Commands
{
	public class ManageApps : CommandBase
	{
		public ManageApps(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override void Execute(object parameter)
		{
			ShowWndManager(parameter);
			MemoryHelper.Clean();
		}


		protected void ShowWndManager(object parameter)
		{
			var appManager = new WndAppManager();
			var mangerData = _WorkItem.AppData.CloneEntity();
			mangerData.NeedAppImage += (s, e) =>
				_WorkItem.ImageLoader.RequestImage(s as AppInfo);
			mangerData.ReInitImages();
			var appStat = new AppStatCollection(mangerData);
			var deletedData = new DeletedAppCollection(_WorkItem.RecycleBin.Copy());
			deletedData.RegisterSource(mangerData, false);
			foreach (DeletedApp item in deletedData)
			{
				_WorkItem.ImageLoader.RequestImage(item.App);
			}

			appManager.Init(
				_WorkItem,
				mangerData,
				parameter as AppInfo,
				parameter as AppType,
				deletedData,
				appStat
				);

			_WorkItem.Commands.Activate.Execute(null);
			appManager.Owner = _WorkItem.MainWindow;

			if (appManager.ShowDialog() ?? false)
			{
				_WorkItem.AppData.MergeEntity(mangerData);
				_WorkItem.RecycleBin.MergeCollection(deletedData);
				_WorkItem.Commands.Save.Execute(null);
			}

			foreach (DeletedApp item in deletedData)
			{
				item.App.AppImage = null;
			}
		}
	}
}
