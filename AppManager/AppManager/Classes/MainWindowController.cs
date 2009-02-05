using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WinSh = IWshRuntimeLibrary;


namespace AppManager
{
	public class MainWindowController
	{
		protected MainWorkItem _WorkItem;


		public MainWindowController(MainWorkItem workItem)
		{
			_WorkItem = workItem;
		}


		public void AddFiles(AppType type, IEnumerable<string> files)
		{
			if (files == null)
				return;

			WinSh.WshShell shell = new WinSh.WshShellClass();

			foreach (var path in files)
			{
				//Add files
				if (File.Exists(path))
				{
					string ext = System.IO.Path.GetExtension(path).ToLower();
					string fullPath = String.Empty;

					if (ext == ".lnk")
					{
						WinSh.WshShortcut shortcut = shell.CreateShortcut(path) as WinSh.WshShortcut;

						if (!String.IsNullOrEmpty(shortcut.Arguments))
							fullPath = "\"" + shortcut.TargetPath + "\"" + " " + shortcut.Arguments;
						else
							fullPath = shortcut.TargetPath;
					}
					else //if (ext == ".exe")
						fullPath = path;

					if (!String.IsNullOrEmpty(fullPath))
					{
						AppInfo app = _WorkItem.AppData.CreateNewAppInfo(type, fullPath);
					}
				}
				else // Add folders
				{
					if (Directory.Exists(path))
					{
						AppInfo app = _WorkItem.AppData.CreateNewAppInfo(type, path);
					}
				}
			}

			_WorkItem.Commands.Save.Execute(null);
		}
	}
}
