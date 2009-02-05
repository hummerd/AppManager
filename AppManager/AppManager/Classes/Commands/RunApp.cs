using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;


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
			
			if (File.Exists(app.AppPath) || Directory.Exists(app.AppPath))
			{
				Process p = new Process();
				p.StartInfo.FileName = app.AppPath;
				p.StartInfo.WorkingDirectory = Path.GetDirectoryName(app.AppPath);
				p.StartInfo.Arguments = app.AppArgs;
				p.Start();
			}

			_WorkItem.MainWindow.InvalidateVisual();
			_WorkItem.Commands.Deactivate.Execute(null);
		}
	}
}
