using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using WinForms = System.Windows.Forms;


namespace AppManager
{
	public class AppManagerController : AppController
	{
		//protected AppGroup _Data;


		public AppManagerController(MainWorkItem workItem)
			: base(workItem)
		{
			//_Data = data;
		}


		//public void AddType()
		//{
		//    _Data.AppTypes.Add(new AppType());
		//}

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

		//public void RemoveType(AppType appType)
		//{
		//    if (appType != null)
		//        _Data.AppTypes.Remove(appType);
		//}

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

		//public void RemoveApp(AppType appType, AppInfo appInfo)
		//{
		//    if (appType != null && appInfo != null)
		//    {
		//        appType.AppInfos.Remove(appInfo);
		//    }
		//}

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

		public AppInfoCollection Scan(string path)
		{
			var result = new AppInfoCollection();

			if (!Directory.Exists(path))
				return result;

			return FindApps(
				new List<string>() { path },
				new List<string>() { "lnk", "exe", "jar" },
				false);
		}

		public void AddScned(AppGroup appGroup, AppType appType, string newAppTypeName, List<AppInfoAdapter> list)
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

			foreach (AppInfoAdapter infoAdp in list)
			{
				if (infoAdp.Checked)
					appType.AppInfos.Add(infoAdp.App);
			}
		}

		public void AddScned(AppGroup appGroup, List<AppInfoAdapter> list)
		{
			if (list == null)
				return;

			var appType = new AppType() { AppTypeName = appGroup.GetDefaultTypeName() };
			appGroup.AppTypes.Add(appType);

			foreach (AppInfoAdapter infoAdp in list)
			{
				if (infoAdp.Checked)
					appType.AppInfos.Add(infoAdp.App);
			}

			appGroup.GroupByFolders(appType);
		}

		public void SelectAllScan(IEnumerable appAdps, bool check)
		{
			var apps = appAdps as List<AppInfoAdapter>;
			if (apps != null)
				foreach (AppInfoAdapter infoAdp in apps)
					infoAdp.Checked = check;
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
		{
			public AppInfoAdapter FindByNameStart(string start, int greaterThen)
			{
				if (greaterThen >= Count - 1)
					greaterThen = 0;
				else
					greaterThen++;

				//look in tale
				for (int i = greaterThen; i < Count; i++)
				{
					if (this[i].AppName.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))
						return this[i];
				}

				//if not found in tale look in head
				for (int i = 0; i < greaterThen; i++)
				{
					if (this[i].AppName.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))
						return this[i];
				}

				return null;
			}
		}

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

			public override string ToString()
			{
				return _Source.ToString();
			}
		}
	}
}
