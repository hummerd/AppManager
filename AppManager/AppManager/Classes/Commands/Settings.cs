﻿

namespace AppManager.Commands
{
	public class Settings: CommandBase
	{
		public Settings(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			Windows.Settings wndSett = new Windows.Settings(_WorkItem);
			wndSett.Owner = _WorkItem.MainWindow;
			if (wndSett.ShowDialog() ?? false)
			{ 
				
			}
		}
	}
}
