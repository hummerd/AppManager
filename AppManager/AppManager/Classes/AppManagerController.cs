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
