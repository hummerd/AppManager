using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WinSh = IWshRuntimeLibrary;


namespace AppManager
{
	public class MainWindowController
	{
		private static string _WinDir;
		private static string _ComFiles;
		private static string _MicSDK;
		private static string _WinApps;


		static MainWindowController()
		{
			_WinDir = Environment.GetEnvironmentVariable("windir").ToLower();
			_ComFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles).ToLower();
			_MicSDK = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			_MicSDK = Path.Combine(_MicSDK, "Microsoft SDKs").ToLower();
			_WinApps = "calc,explorer,wordpad,mspaint,notepad";
		}


		protected MainWorkItem _WorkItem;


		public MainWindowController(MainWorkItem workItem)
		{
			_WorkItem = workItem;
		}


		public void AddFiles(AppType type, IEnumerable<string> files)
		{
			AddFiles(type, files, false, true);
		}

		public void AddFiles(AppType type, IEnumerable<string> files, bool onlyPrograms, bool saveData)
		{
			if (files == null)
				return;

			var uniq = new Dictionary<string, object>(100);

			
			WinSh.WshShell shell = new WinSh.WshShellClass();

			foreach (var path in files)
			{
				//Add files
				if (File.Exists(path))
				{
					string ext = System.IO.Path.GetExtension(path).ToLower();
					string fullPath = String.Empty;
					string appPath = String.Empty;
					string appArgs = String.Empty;

					if (ext == ".lnk")
					{
						WinSh.WshShortcut shortcut = shell.CreateShortcut(path) as WinSh.WshShortcut;
						appPath = shortcut.TargetPath;
						appArgs = shortcut.Arguments;

						if (!String.IsNullOrEmpty(appArgs ))
							fullPath = "\"" + appPath + "\"" + " " + appArgs;
						else
							fullPath = appPath;
					}
					else //if (ext == ".exe")
						appPath = fullPath = path;

					if (String.IsNullOrEmpty(appPath))
						continue;

					if (onlyPrograms && !FilterApps(appPath, appArgs))
						continue;

					if (!String.IsNullOrEmpty(fullPath))
					{
						if (!uniq.ContainsKey(fullPath))
						{
							uniq.Add(fullPath, null);
							AppInfo app = _WorkItem.AppData.CreateNewAppInfo(type, fullPath);
						}
					}
				}
				else if (!onlyPrograms)// Add folders
				{
					if (Directory.Exists(path))
					{
						if (!uniq.ContainsKey(path))
						{
							uniq.Add(path, null);
							AppInfo app = _WorkItem.AppData.CreateNewAppInfo(type, path);
						}
					}
				}
			}

			if (saveData)
				_WorkItem.Commands.Save.Execute(null);
		}


		protected bool FilterApps(string appPath, string appArgs)
		{
			string dirName = Path.GetDirectoryName(appPath).ToLower();
			string fileName = Path.GetFileNameWithoutExtension(appPath).ToLower();
			string ext = Path.GetExtension(appPath).ToLower();

			if (ext != ".exe")
				return false;

			if (appPath.Contains("{"))
				return false;

			if (fileName.Contains("setup") ||
				 fileName.Contains("uninst") ||
				 fileName.Contains("unins000") ||
				 fileName.Contains("uinst")
				 )
				return false;

			if ((dirName.Contains(_WinDir) && !_WinApps.Contains(fileName)) ||
				 dirName.Contains(_ComFiles) ||
				 dirName.Contains(_MicSDK)
				)
				return false;

			if (!String.IsNullOrEmpty(appArgs))
				return false;

			//if (appArgs.Contains("debug") ||
			//    appArgs.Contains("install") ||
			//    appArgs.Contains("unistall") ||
			//    appArgs.Contains("recycle")
			//   )
			//   return false;

			return true;
		}
	}
}
