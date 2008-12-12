using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using WinSh = IWshRuntimeLibrary;


namespace AppManager.Commands
{
	public class Settings: CommandBase
	{
		public Settings(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			Windows.Settings wndSett = new Windows.Settings();
			wndSett.ChkAutoStart.IsChecked = File.Exists(GetStartUpFile());

			if (wndSett.ShowDialog() ?? false)
			{
				SetStartUp(wndSett.ChkAutoStart.IsChecked ?? false);
			}
		}


		protected void SetStartUp(bool startUp)
		{
			string path = GetStartUpFile();
			bool scExsist = File.Exists(path);

			if (startUp)
			{
				WinSh.WshShellClass shell = new WinSh.WshShellClass();
				WinSh.IWshShortcut lnk;

				lnk = (WinSh.IWshShortcut)shell.CreateShortcut(path);
				lnk.TargetPath = Assembly.GetExecutingAssembly().Location;
				lnk.Save();
			}
			else
			{
				if (scExsist)
					File.Delete(path);
			}
		}

		protected string GetStartUpFile()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			return Path.Combine(path, "AppManager.lnk");
		}
	}
}
