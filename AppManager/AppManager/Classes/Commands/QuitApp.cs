

namespace AppManager.Commands
{
	public class QuitApp : CommandBase
	{
		public QuitApp(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			_WorkItem.TrayIcon.Dispose();
			_WorkItem.KbrdHook.Dispose();

			_WorkItem.Commands.Save.Execute(null);

			App.Current.Shutdown();
		}
	}
}
