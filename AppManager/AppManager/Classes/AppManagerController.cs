using System;
using System.Collections.Generic;
using AppManager.Entities;
using WinForms = System.Windows.Forms;


namespace AppManager
{
	public class AppManagerController : AppController
	{
		public AppManagerController(MainWorkItem workItem)
			: base(workItem)
		{
			//_Data = data;
		}


		public void AddToBin(DeletedAppCollection recycleBin, IEnumerable<AppInfo> apps)
		{
			foreach (var app in apps)
			{
				recycleBin.AddApp(null, app, false);
			}
		}

		public void DeleteFromBin(DeletedAppCollection recycleBin, DeletedAppCollection deletedApp)
		{
			for (int i = 0; i < deletedApp.Count; i++)
			{
				recycleBin.Remove(deletedApp[i] as DeletedApp);
			}
		}

		public void RestoreApp(AppGroup appGroup, DeletedAppCollection deletedApp, DeletedAppCollection recycleBin, AppType restore, string newAppTypeName)
		{
			if (deletedApp == null)
				return;

			if (newAppTypeName != null)
			{
				restore = new AppType() { AppTypeName = newAppTypeName };
				appGroup.AppTypes.Add(restore);
			}

			for (int i = 0; i < deletedApp.Count; i++)
			{
				var item = deletedApp[i] as DeletedApp;
				var at = appGroup.FindAppType((item.DeletedFrom ?? restore).AppTypeName);

				var ai = appGroup.CreateNewAppInfo(
					at,
					item.App.AppName,
					item.App.ExecPath,
					item.App.ImagePath);

				recycleBin.Remove(item);
			}
		}

		public void AddEmptyAppType(AppGroup appGroup, AppType beforeAppType)
		{
			InsertAppType(
				appGroup,
				new AppType() { AppTypeName = Strings.APPLICATIONS },
				beforeAppType);
		}

		public void MoveType(AppGroup appGroup, AppType appType, bool up)
		{
			if (appGroup == null)
				return;

			if (appType == null)
				return;

			int ix = appGroup.AppTypes.IndexOf(appType);
			int ix2 = up ? ix - 1 : ix + 1;

			if (ix2 >= 0 && ix2 < appGroup.AppTypes.Count)
				appGroup.AppTypes.Move(ix, ix2);
		}

		public void AddAppInfo(AppGroup appGroup, AppType appType)
		{
			if (appType != null)
			{
				appGroup.CreateNewAppInfo(appType);
			}
		}

		public void MoveApp(AppType appType, AppInfo appInfo, bool up)
		{
			if (appType != null && appInfo != null)
			{
				int ix = appType.AppInfos.IndexOf(appInfo);
				int ix2 = up ? ix - 1 : ix + 1;

				if (ix2 >= 0 && ix2 < appType.AppInfos.Count)
					appType.AppInfos.Move(ix, ix2);
			}
		}

		public void SelectAppPath(AppInfo appInfo)
		{
			if (appInfo == null)
				return;

			string path = SelectFile();
			if (!String.IsNullOrEmpty(path))
			{
				appInfo.ExecPath = path;
				appInfo.SetAutoAppName();
			}
		}

		public string SelectFile()
		{
			using (WinForms.OpenFileDialog ofd = new WinForms.OpenFileDialog())
			{
				ofd.Title = Strings.SELECT_APP;
				ofd.Filter = Strings.APP_FILTER;

				if (ofd.ShowDialog() == WinForms.DialogResult.OK)
					return ofd.FileName;
			}

			return string.Empty;
		}

		public string SelectPath()
		{
			WinForms.FolderBrowserDialog fbd = new WinForms.FolderBrowserDialog();

			if (fbd.ShowDialog() == WinForms.DialogResult.OK)
				return fbd.SelectedPath;

			return string.Empty;
		}

		public void AddScned(AppGroup appGroup, AppType appType, string newAppTypeName, IEnumerable<AppInfo> list)
		{
			if (list == null)
				return;

			if (appType == null && newAppTypeName == null)
				return;

			if (newAppTypeName != null)
			{
				appType = new AppType() { AppTypeName = newAppTypeName };
				appGroup.AppTypes.Add(appType);
			}

			appType.AppInfos.AddRange(list);
		}

		public void AddScned(AppGroup appGroup, IEnumerable<AppInfo> list)
		{
			if (list == null)
				return;

			var appType = new AppType() { AppTypeName = appGroup.GetDefaultTypeName() };
			appGroup.AppTypes.Add(appType);
			appType.AppInfos.AddRange(list);
			appGroup.GroupByFolders(appType);
		}
	}
}
