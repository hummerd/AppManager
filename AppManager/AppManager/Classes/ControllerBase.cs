using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using CommonLib.PInvoke;
using CommonLib.IO;


namespace AppManager
{
	public class ControllerBase
	{
		private static string _WinDir;
		private static string _ComFiles;
		private static string _MicSDK;
		private static string _WinApps;

		static ControllerBase()
		{
			_WinDir = Environment.GetEnvironmentVariable("windir").ToLower();
			_ComFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles).ToLower();
			_MicSDK = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			_MicSDK = Path.Combine(_MicSDK, "Microsoft SDKs").ToLower();
			_WinApps = "calc,explorer,wordpad,mspaint,notepad";
		}


		protected MainWorkItem _WorkItem;


		public ControllerBase(MainWorkItem workItem)
		{
			_WorkItem = workItem;
		}


		public MainWorkItem WorkItem
		{ 
			get
			{
				return _WorkItem;
			}
		}


		public AppInfoCollection FindAppsInQuickLaunch()
		{
			string dirPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"Microsoft\Internet Explorer\Quick Launch");

			return FindApps(
				new List<string>() { dirPath },
				new List<string>() { "lnk" },
				true);
		}

		public AppInfoCollection FindAppsInAllProgs()
		{
			StringBuilder allPrograms = new StringBuilder(300);
			Shell32.SHGetSpecialFolderPath(IntPtr.Zero, allPrograms, Shell32.CSIDL_COMMON_PROGRAMS, false);

			string path = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

			return FindApps(
				new List<string>() { allPrograms.ToString(), path },
				new List<string>() { "lnk" }, 
				true);
		}
		
		public AppInfoCollection FindApps(
			IEnumerable<string> files)
		{
			return FindApps(files, false);
		}

		public AppInfoCollection FindApps(
			IEnumerable<string> pathList, 
			IEnumerable<string> extList, 
			bool onlyPrograms)
		{
			var result = new AppInfoCollection();

			if (pathList == null)
				return result;

			foreach (var path in pathList)
				result.AddRange(FindApps(path, extList, onlyPrograms));

			return result;
		}

		public AppInfoCollection FindApps(
			string path,
			IEnumerable<string> extList,
			bool onlyPrograms)
		{
			var result = new AppInfoCollection();

			if (extList == null)
				return result;

			if (String.IsNullOrEmpty(path))
				return result;

			if (!Directory.Exists(path))
				return result;


			var allFiles = new List<string>();

			foreach (string ext in extList)
			{
				string extension = ext;

				if (String.IsNullOrEmpty(extension))
					continue;

				if (extension.StartsWith(".") && extension.Length > 1)
					extension = extension.Substring(1, ext.Length - 1);

				string[] files = Directory.GetFiles(path, "*." + extension, SearchOption.AllDirectories);
				allFiles.AddRange(files);
			}

			return FindApps(allFiles, onlyPrograms);
		}


		protected AppInfoCollection FindApps(IEnumerable<string> files, bool onlyPrograms)
		{
			var result = new AppInfoCollection();

			if (files == null)
				return result;

			var uniq = new Dictionary<string, object>(100);


			//WinSh.WshShell shell = new WinSh.WshShellClass();

			foreach (var path in files)
			{
				//Add files
				if (File.Exists(path))
				{
					string ext = System.IO.Path.GetExtension(path).ToLower();
					string fullPath = String.Empty;
					string appPath = String.Empty;
					string appArgs = String.Empty;
					string imagePath = String.Empty;

					if (ext == ".lnk")
					{
						//WinSh.WshShortcut shortcut = shell.CreateShortcut(path) as WinSh.WshShortcut;
						var shortcut = LnkHelper.OpenLnk(path);

						appPath = shortcut.TargetPath;
						appArgs = shortcut.Arguments;
						var icoLocation = shortcut.IconLocation;

						if (appArgs == null)
							appArgs = String.Empty;

						if (appPath.Contains("{"))
							appPath = MsiShortcutParser.ParseShortcut(path);
						else if (!icoLocation.Contains("{"))
							imagePath = icoLocation;

						if (!String.IsNullOrEmpty(appArgs))
							fullPath = "\"" + appPath + "\"" + " " + appArgs;
						else
							fullPath = appPath;
						
						if (!String.IsNullOrEmpty(imagePath))
						{
							var imageLocation = imagePath.Split(',');
							if (imageLocation.Length > 0)
								imagePath = imageLocation[0].Trim();
							else
								imagePath = String.Empty;
						}
					}
					else //if (ext == ".exe")
						appPath = fullPath = path;

					if (String.IsNullOrEmpty(appPath))
						continue;

					if (onlyPrograms && !FilterApps(appPath, appArgs))
						continue;

					if (!String.IsNullOrEmpty(fullPath))
					{
						if (!uniq.ContainsKey(fullPath.ToLower()))
						{
							string appName = Path.GetFileNameWithoutExtension(path);
							uniq.Add(fullPath.ToLower(), null);
							result.Add(_WorkItem.AppData.CreateNewAppInfo(null, appName, fullPath, imagePath));
						}
					}
				}
				else if (!onlyPrograms)// Add folders
				{
					if (Directory.Exists(path))
					{
						if (!uniq.ContainsKey(path.ToLower()))
						{
							uniq.Add(path.ToLower(), null);
							result.Add(_WorkItem.AppData.CreateNewAppInfo(null, path));
						}
					}
				}
			}

			return result;
		}

		protected bool FilterApps(string appPath, string appArgs)
		{
			try
			{
				string dirName = Path.GetDirectoryName(appPath);
				if (String.IsNullOrEmpty(dirName))
					dirName = Path.GetPathRoot(appPath);

                dirName = dirName.ToLower();
				string fileName = Path.GetFileNameWithoutExtension(appPath).ToLower();
				string ext = Path.GetExtension(appPath).ToLower();

				if (!File.Exists(appPath))
					return false;

				if (ext != ".exe")
					return false;

				//if (appPath.Contains("{"))
				//   return false;

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
			catch (ArgumentException) //probably wrong apps
			{
				//MessageBox.Show(appPath + " " + appArgs);
				return false;
			}
			catch
			{
				MessageBox.Show(appPath + " " + appArgs);
				return false;
			}
		}
	}
}
