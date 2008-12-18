using System;
using System.Collections.Generic;
using System.Text;

namespace AppManager.Commands
{
	public class Deactivate : CommandBase
	{
		public Deactivate(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			_WorkItem.MainWindow.Hide();

			//GC.Collect();
			//GC.WaitForPendingFinalizers();
			//GC.Collect();
			//GC.WaitForPendingFinalizers();
		}
	}
}
