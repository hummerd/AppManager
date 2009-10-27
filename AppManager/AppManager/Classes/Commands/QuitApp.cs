using System;
using CommonLib.Windows;


namespace AppManager.Commands
{
	public class QuitApp : CommandBase
	{
		public QuitApp(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override void Execute(object parameter)
		{
			try
			{
				_WorkItem.TrayIcon.Dispose();
				_WorkItem.KbrdHook.Dispose();

				_WorkItem.Commands.Save.Execute(null);
			}
			catch(Exception exc)
			{
				ErrorBox.Show(Strings.APP_TITLE, exc);
			}
			
			App.Current.Shutdown();
		}
	}
}
