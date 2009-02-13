using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WinSh = IWshRuntimeLibrary;


namespace AppManager.Classes
{
	public class SettingsController
	{
		protected MainWorkItem _WorkItem;


		public SettingsController(MainWorkItem workItem)
		{
			_WorkItem = workItem;
		}


		public void ShowAppDataPath()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = _WorkItem.DataDir,
				UseShellExecute = true
			};
			Process.Start(psi);
		}

		public void EditAppData()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = _WorkItem.DataPath,
				UseShellExecute = true
			};
			Process.Start(psi);
		}

		public bool IsStartupFileExists()
		{
			return File.Exists(GetStartUpFile());
		}

		public void SetStartUp(bool startUp)
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
		
		public string GetStartUpPath()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
		}

		public string GetStartUpFile()
		{
			string path = GetStartUpPath();
			return Path.Combine(path, "AppManager.lnk");
		}
	}
}
