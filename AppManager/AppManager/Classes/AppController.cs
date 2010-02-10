using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using AppManager.Entities;
using AppManager.Windows;
using CommonLib.IO;
using CommonLib.PInvoke;
using CommonLib.UI;
using CommonLib.Windows;


namespace AppManager
{
	public class AppController : ControllerBase
	{
		private static string _WinDir;
		private static string _ComFiles;
		private static string _MicSDK;
		private static string _WinApps;
		private static Dictionary<SearchLocation, string[]> _SearchPaths;


		static AppController()
		{
			_WinDir = Environment.GetEnvironmentVariable("windir").ToLower();
			_ComFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles).ToLower();
			_MicSDK = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			_MicSDK = Path.Combine(_MicSDK, "Microsoft SDKs").ToLower();
			_WinApps = "calc,explorer,wordpad,mspaint,notepad";

			_SearchPaths = new Dictionary<SearchLocation, string[]>(2);

			string quickSearchPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"Microsoft\Internet Explorer\Quick Launch");

			StringBuilder allUsersPrograms = new StringBuilder(300);
			Shell32.SHGetSpecialFolderPath(IntPtr.Zero, allUsersPrograms, Shell32.CSIDL_COMMON_PROGRAMS, false);
			string currentUserPrograms = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

			_SearchPaths.Add(SearchLocation.QuickLaunch, new string[] { quickSearchPath });
			_SearchPaths.Add(SearchLocation.AllProgramsMenu, new string[] { allUsersPrograms.ToString(), currentUserPrograms });
		}


		public AppController(MainWorkItem workItem)
			: base(workItem)
		{

		}


		public void InsertAppType(AppGroup appGroup, AppType addAppType, AppType beforeAppType)
		{
			if (appGroup == null)
				return;

			if (addAppType == null)
				return;

			if (beforeAppType == null)
			{
				appGroup.AppTypes.Insert(0, addAppType);
				return;
			}

			int ix = appGroup.AppTypes.IndexOf(beforeAppType);
			if (ix >= 0)
			{
				foreach (var item in addAppType.AppInfos)
					PrepareItem(item);

				appGroup.AppTypes.Insert(ix, addAppType);
			}
		}

		public void PrepareItem(AppInfo appInfo)
		{
			if (appInfo == null)
				return;

			appInfo.ID = _WorkItem.AppData.LastAppInfoID;
			_WorkItem.AppData.LastAppInfoID += 1;
			_WorkItem.AppData.RequestAppImage(appInfo);
		}

		public void DeleteAppType(AppGroup appGroup, AppType appType, bool silent)
		{
			if (appType == null)
				return;

			if (appGroup == null)
				return;

			if (!silent && !MsgBox.Show(
					_WorkItem.MainWindow,
					Strings.APP_TITLE,
					string.Format(Strings.QUEST_DEL_APP_TYPE, appType.AppTypeName)
					)
				)
				return;

			appGroup.AppTypes.Remove(appType);
		}


		public void DeleteAppInfo(AppType appType, AppInfo appInfo, bool silent)
		{
			if (appInfo == null)
				return;

			if (appType == null)
				return;

			if (!silent && !MsgBox.Show(
					_WorkItem.MainWindow,
					Strings.APP_TITLE,
					string.Format(Strings.QUEST_DEL_APP, appInfo.AppName)
					)
				)
				return;

			//AppType appType = _WorkItem.AppData.FindAppType(appInfo);
			//if (appType != null)
			appType.AppInfos.Remove(appInfo);
		}

		public void SetAppInfoImage(AppInfo appInfo)
		{
			using (var dlg = new OpenIconDlg())
			{
				var wnd = _WorkItem.FindActiveWindow();
				if (dlg.ShowOpenFileDialog(new WpfWin32Window(wnd)) == System.Windows.Forms.DialogResult.OK)
				{
					appInfo.LoadImagePath = dlg.SelectedFile + "," + dlg.SelectedIconIndex;
				}
			}
		}


		public AppInfoCollection FindApps(SearchLocation location, AppGroup appData, bool excludeExisting)
		{
			if (location == SearchLocation.None)
				return new AppInfoCollection();

			var searchPaths = new List<string>();

			if ((location & SearchLocation.QuickLaunch) == SearchLocation.QuickLaunch)
				searchPaths.AddRange(_SearchPaths[SearchLocation.QuickLaunch]);

			if ((location & SearchLocation.AllProgramsMenu) == SearchLocation.AllProgramsMenu)
				searchPaths.AddRange(_SearchPaths[SearchLocation.AllProgramsMenu]);

			return FindApps(
				 searchPaths,
				new List<string>() { "lnk" },
				appData,
				excludeExisting
				);
		}

		public AppInfoCollection FindApps(IEnumerable<string> pathList, IEnumerable<string> extList, AppGroup appData, bool excludeExisting)
		{
			var result = FindApps(
				pathList,
				extList,
				true);

			if (excludeExisting && result != null)
				for (int i = result.Count - 1; i >= 0; i--)
				{
					if (appData.FindAppByExecPath(result[i].ExecPath) != null)
						result.RemoveAt(i);
				}

			return result;
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

			if (extList == null)
				return result;

			if (pathList == null)
				return result;

			var allFiles = new List<string>();

			foreach (var path in pathList)
			{
				if (!Directory.Exists(path))
					continue;

				foreach (var ext in extList)
				{
					string extension = ext;

					if (String.IsNullOrEmpty(extension))
						continue;

					if (extension.StartsWith(".") && extension.Length > 1)
						extension = extension.Substring(1, ext.Length - 1);

					string[] files = Directory.GetFiles(path, "*." + extension, SearchOption.AllDirectories);
					allFiles.AddRange(files);
				}
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

	[Flags]
	public enum SearchLocation
	{
		None = 0x00,
		QuickLaunch = 0x01,
		AllProgramsMenu = 0x02,

		All = QuickLaunch | AllProgramsMenu
	}
}
