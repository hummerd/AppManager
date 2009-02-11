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


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			WndAppManager appManager = new WndAppManager();
			AppGroup mangerData = _WorkItem.AppData.CloneEntity();
			appManager.Init(_WorkItem, mangerData, parameter as AppInfo);
			appManager.Owner = _WorkItem.MainWindow;

			if (appManager.ShowDialog() ?? false)
			{
				_WorkItem.AppData.MergeEntity(mangerData);
				_WorkItem.MainWindow.Init(false);
				_WorkItem.Commands.Save.Execute(null);
			}
		}
	}
}
