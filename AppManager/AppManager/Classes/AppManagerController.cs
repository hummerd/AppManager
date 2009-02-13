using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using WinForms = System.Windows.Forms;


namespace AppManager
{
	public class AppManagerController : ControllerBase
	{
		protected AppGroup _Data;


		public AppManagerController(MainWorkItem workItem, AppGroup data)
			: base(workItem)
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

		public AppInfoCollection Scan(string path)
		{
			var result = new AppInfoCollection();

			if (!Directory.Exists(path))
				return result;

			return FindApps(
				path,
				new List<string>() { "lnk", "exe", "jar" },
				false);
		}

		public void AddScned(AppType appType, string newAppTypeName, List<AppInfoAdapter> list)
		{
			if (list == null)
				return;

			if (appType == null && newAppTypeName == null)
				return;

			if (newAppTypeName != null)
			{
				appType = new AppType() { AppTypeName = newAppTypeName };
				_Data.AppTypes.Add(appType);
			}

			foreach (AppInfoAdapter infoAdp in list)
			{
				if (infoAdp.Checked)
					appType.AppInfos.Add(infoAdp.App);
			}
		}

		public void SelectAllScan(IEnumerable appAdps, bool check)
		{
			var apps = appAdps as List<AppInfoAdapter>;
			if (apps != null)
				foreach (AppInfoAdapter infoAdp in apps)
					infoAdp.Checked = check;
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

		public AppInfoAdapterCollection AdaptTo(AppInfoCollection apps, bool check)
		{
			if (apps == null)
				return new AppInfoAdapterCollection();

			var result = new AppInfoAdapterCollection();

			foreach (var app in apps)
				result.Add(new AppInfoAdapter(app) { Checked = check });

			return result;
		}


		public class AppInfoAdapterCollection : List<AppInfoAdapter>
		{ }

		public class AppInfoAdapter : INotifyPropertyChanged
		{
			protected AppInfo _Source;
			protected bool _Checked = true;


			public AppInfoAdapter(AppInfo source)
			{
				_Source = source;
				_Source.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
			}


			public AppInfo App { get { return _Source; } }

			public string AppName { get { return _Source.AppName; } set { _Source.AppName = value; } }
			public string ExecPath { get { return _Source.ExecPath; } set { } }
			public BitmapSource AppImage { get { return _Source.AppImage; } }

			public bool Checked
			{
				get { return _Checked; }
				set { _Checked = value; OnPropertyChanged("Checked"); }
			}

			#region INotifyPropertyChanged Members

			public event PropertyChangedEventHandler PropertyChanged;

			protected void OnPropertyChanged(string propName)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}

			#endregion
		}
	}
}
