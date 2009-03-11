using CommonLib.PInvoke;


namespace AppManager.Commands
{
	public class Activate : CommandBase
	{
		public Activate(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			MainWindow wnd = _WorkItem.MainWindow;
			wnd.Show();
			User32.ActivateWindow(wnd);
			wnd.SetFocus();
		}
	}
}
