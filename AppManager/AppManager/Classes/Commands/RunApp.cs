using System;
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

		/// <summary>
		/// Starts specified application
		/// </summary>
		/// <param name="parameter">Can be either AppInfo or objetc[] {AppInfo app, string appArgs}</param>
		public override void Execute(object parameter)
		{
			if (parameter == null)
				return;

			string appArgs = String.Empty;
			AppInfo app = null;

			object[] prms = parameter as object[];

			if (prms != null && prms.Length >= 2)
			{
				app = prms[0] as AppInfo;
				appArgs = prms[1] as string;
			}
			else
				app = parameter as AppInfo;

			if (app == null)
				return;

			bool altPressed = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;

			string appPath = app.AppPath;
			bool unc = PathHelper.IsPathUNC(appPath);

			if (unc || File.Exists(appPath) || Directory.Exists(appPath))
			{
				try
				{
					using (Process p = new Process())
					{
						p.StartInfo.FileName = app.AppPath;
						p.StartInfo.WorkingDirectory = Path.GetDirectoryName(app.AppPath);
						p.StartInfo.Arguments = String.IsNullOrEmpty(appArgs) ? app.AppArgs : appArgs;
						p.Start();
					}
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
