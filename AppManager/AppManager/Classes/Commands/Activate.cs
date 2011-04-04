using CommonLib.PInvoke;
using CommonLib.Application;


namespace AppManager.Commands
{
	public class Activate : CommandBase
	{
		public Activate(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override void Execute(object parameter)
		{
			MainWindow wnd = _WorkItem.MainWindow;
			wnd.Show();
			User32.ActivateWindow(wnd);
			wnd.SetFocus();

			MemoryHelper.Clean();
		}
	}
}
