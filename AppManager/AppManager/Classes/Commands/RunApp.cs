using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using CommonLib;


namespace AppManager.Commands
{
	public class RunApp : CommandBase
	{
		public RunApp(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			AppInfo app = parameter as AppInfo;

			if (app == null)
				return;

			bool altPressed = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;

			string appPath = app.AppPath;
			bool unc = PathHelper.IsPathUNC(appPath);

			if (unc || File.Exists(appPath) || Directory.Exists(appPath))
			{
				try
				{
					Process p = new Process();
					p.StartInfo.FileName = app.AppPath;
					p.StartInfo.WorkingDirectory = Path.GetDirectoryName(app.AppPath);
					p.StartInfo.Arguments = app.AppArgs;
					p.Start();
				}
				catch
				{ ; }
			}

			_WorkItem.MainWindow.InvalidateVisual();
			if (!altPressed)
				_WorkItem.Commands.Deactivate.Execute(null);
			else
				_WorkItem.Commands.Activate.Execute(null);
		}
	}
}
