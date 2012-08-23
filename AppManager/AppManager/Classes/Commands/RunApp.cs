using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using AppManager.Entities;
using CommonLib;


namespace AppManager.Commands
{
	public class RunApp : CommandBase
	{
		public RunApp(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		/// <summary>
		/// Starts specified application
		/// </summary>
		/// <param name="parameter">Can be either AppInfo or objetc[] {AppInfo app, string appArgs}</param>
		public override void Execute(object parameter)
		{
			if (parameter == null)
				return;

			var prm = parameter as AppStartParams;
			if (prm.App == null)
				return;

			bool altPressed = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;

			string appPath = Environment.ExpandEnvironmentVariables(prm.App.AppPath);
			bool unc = PathHelper.IsPathUNC(appPath);

			if (unc || PathHelper.FileExists(appPath) || Directory.Exists(appPath))
			{
				string args = String.Empty;
				string runAs = String.Empty;

				try
				{
					using (Process p = new Process())
					{
						p.StartInfo.FileName = prm.App.AppPath;
						p.StartInfo.WorkingDirectory = Path.GetDirectoryName(prm.App.AppPath);

						p.StartInfo.
							Arguments =
							args = String.IsNullOrEmpty(prm.Args) ?
								prm.App.AppArgs : 
								prm.Args;

						if (prm.RunAs && Array.IndexOf(p.StartInfo.Verbs, "runas") >= 0)
						{
							p.StartInfo.
								Verb = 
								runAs = "runas";
						}
							
						p.Start();
					}
				}
				catch
				{ ; }

				prm.App.RunHistory.Add(
					new AppRunInfo
					{
						RunTime = DateTime.Now,
						Areguments = new StartArgs 
						{
							Args = args,
							RunAs = !String.IsNullOrEmpty(runAs),
						}
					});
			}

			_WorkItem.MainWindow.InvalidateVisual();
			if (!altPressed)
				_WorkItem.Commands.Deactivate.Execute(null);
			else
				_WorkItem.Commands.Activate.Execute(null);
		}
	}

	public class AppStartParams : StartArgs
	{
		public AppStartParams(AppInfo app)
		{
			App = app;
			RunAs = false;
		}

		public AppInfo App { get; set; }
	}

	public class StartArgs
	{
		public string Args { get; set; }
		public bool RunAs { get; set; }	
	}
}
