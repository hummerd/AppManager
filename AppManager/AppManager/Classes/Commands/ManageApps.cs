using System;
using System.Collections.Generic;
using System.Text;

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

			appManager.Init(
				_WorkItem, 
				mangerData,
				parameter as AppInfo,
				parameter as AppType);

			_WorkItem.Commands.Activate.Execute(null);
			appManager.Owner = _WorkItem.MainWindow;

			if (appManager.ShowDialog() ?? false)
			{
				_WorkItem.AppData.MergeEntity(mangerData);
				_WorkItem.Commands.Save.Execute(null);
			}
		}
	}
}
