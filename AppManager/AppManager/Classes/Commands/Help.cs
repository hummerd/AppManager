using System;
using System.Collections.Generic;
using System.Text;
using AppManager.Windows;


namespace AppManager.Commands
{
	public class Help : CommandBase
	{
		protected HelpBox _HelpWnd;


		public Help(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			if (_HelpWnd != null)
			{
				_HelpWnd.Activate();
				return;
			}

			_HelpWnd = new HelpBox(_WorkItem, (bool)parameter);
			_HelpWnd.Owner = _WorkItem.MainWindow;
			_HelpWnd.Closed += HelpWnd_Closed;
			_HelpWnd.Show();
		}


		private void HelpWnd_Closed(object sender, EventArgs e)
		{
			_HelpWnd = null;
		}
	}
}
