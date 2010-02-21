using System;
using System.Collections.Generic;
using AppManager.Entities;
using CommonLib.PInvoke;


namespace AppManager.Commands
{
	public class ManageApps : CommandBase
	{
		public ManageApps(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override void Execute(object parameter)
		{
			WndAppManager appManager = new WndAppManager();
			AppGroup mangerData = _WorkItem.AppData.CloneEntity();
			mangerData.NeedAppImage += (s, e) => 
				_WorkItem.ImageLoader.RequestImage(s as AppInfo);
			mangerData.ReInitImages();
			var deletedData = new DeletedAppCollection((IEnumerable<DeletedApp>)_WorkItem.RecycleBin.Copy());
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
				deletedData);

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

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
