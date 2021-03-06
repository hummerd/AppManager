﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using CommonLib.Application;
using WinForms = System.Windows.Forms;


namespace AppManager.Classes
{
	public class SettingsController : ControllerBase
	{
		protected AutoStart _AutoStart = new AutoStart("AppManager");


		public SettingsController(MainWorkItem workItem)
			: base(workItem)
		{
		}


		public bool SelectColor(out Color color)
		{
			color = Colors.AliceBlue;

			using (WinForms.ColorDialog cd = new WinForms.ColorDialog())
			{
				if (cd.ShowDialog() == WinForms.DialogResult.OK)
				{
					color = Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);
					return true;
				}
			}

			return false;
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
			if (!File.Exists(_WorkItem.DataPath))
				return;

			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = _WorkItem.DataPath,
				UseShellExecute = true
			};
			Process.Start(psi);
		}

		public bool IsStartupFileExists()
		{
			return _AutoStart.IsAutoStartSet();
		}

		public void SetStartUp(bool startUp)
		{
			//clean for old method
			if (File.Exists(GetStartUpFile()))
				File.Delete(GetStartUpFile());

			_AutoStart.SetStartUp(startUp);
		}


		//public bool IsStartupFileExists()
		//{
		//   return File.Exists(GetStartUpFile());
		//}

		//public void SetStartUp(bool startUp)
		//{
		//   string path = GetStartUpFile();
		//   bool scExsist = File.Exists(path);

		//   if (startUp)
		//   {
		//      WinSh.WshShellClass shell = new WinSh.WshShellClass();
		//      WinSh.IWshShortcut lnk;

		//      lnk = (WinSh.IWshShortcut)shell.CreateShortcut(path);
		//      lnk.TargetPath = Assembly.GetExecutingAssembly().Location;
		//      lnk.Save();
		//   }
		//   else
		//   {
		//      if (scExsist)
		//         File.Delete(path);
		//   }
		//}

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
