using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using WinForms = System.Windows.Forms;

namespace AppManager
{
	public class AppManagerController
	{
		protected AppGroup _Data;


		public AppManagerController(AppGroup data)
		{
			_Data = data;
		}
		

		public void AddType()
		{
			_Data.AppTypes.Add(new AppType());
		}

		public void MoveType(AppType appType, bool up)
		{
			if (appType != null)
			{
				int ix = _Data.AppTypes.IndexOf(appType);
				int ix2 = up ? ix - 1 : ix + 1;

				if (ix2 >= 0 && ix2 < _Data.AppTypes.Count)
					_Data.AppTypes.Move(ix, ix2);
			}
		}

		public void RemoveType(AppType appType)
		{
			if (appType != null)
				_Data.AppTypes.Remove(appType);
		}

		public void AddApp(AppType appType)
		{
			if (appType != null)
			{
				_Data.CreateNewAppInfo(appType);
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

		public void RemoveApp(AppType appType, AppInfo appInfo)
		{
			if (appType != null && appInfo != null)
			{
				appType.AppInfos.Remove(appInfo);
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
			WinForms.OpenFileDialog ofd = new WinForms.OpenFileDialog();

			if (ofd.ShowDialog() == WinForms.DialogResult.OK)
				return ofd.FileName;

			return string.Empty;
		}

		public string SelectPath()
		{
			WinForms.FolderBrowserDialog fbd = new WinForms.FolderBrowserDialog();

			if (fbd.ShowDialog() == WinForms.DialogResult.OK)
				return fbd.SelectedPath;

			return string.Empty;
		}

		public List<AppInfoAdapter> Scan(string path)
		{
			List<AppInfoAdapter> result = new List<AppInfoAdapter>();

			if (!Directory.Exists(path))
				return result;

			string[] files = Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories);

			foreach (var file in files)
			{
				AppInfo appi = new AppInfo();
				appi.ExecPath = file;
				appi.SetAutoAppName();

				result.Add(new AppInfoAdapter(appi));
			}

			return result;
		}

		public void AddScned(AppType appType, List<AppInfoAdapter> list)
		{
			foreach (AppInfoAdapter infoAdp in list)
			{
				if (infoAdp.Checked)
					appType.AppInfos.Add(infoAdp.App);
			}
		}


		public class AppInfoAdapter
		{
			protected AppInfo _Source;


			public AppInfoAdapter(AppInfo source)
			{
				_Source = source;
			}


			public AppInfo App { get { return _Source; } }

			public string AppName { get { return _Source.AppName; } set { _Source.AppName = value; } }
			public string ExecPath { get { return _Source.ExecPath; } set { } }
			public BitmapSource AppImage { get { return _Source.AppImage; } }

			public bool Checked { get; set; }
		}
	}
}
