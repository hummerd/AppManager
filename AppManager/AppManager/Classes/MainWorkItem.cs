using System;
using System.IO;
using System.Reflection;
using System.Windows;
using AppManager.Commands;
using AppManager.Entities;
using AppManager.Settings;
using CommonLib.PInvoke.WinHook;
using WinForms = System.Windows.Forms;


namespace AppManager
{
	public class MainWorkItem
	{
		public event EventHandler UpdateCompleted;


		protected AsyncImageLoader _ImageLoader;
		protected AppGroup _AppData;
		protected DeletedAppCollection _RecycleBin;
		protected MainWindow _MainWindow;
		protected App _Application;
		protected KeyboardHook _KbrdHook;
		protected MouseHook _MsHook;
		protected WinForms.NotifyIcon _TrayIcon;
		protected AppCommands _Commands;
		protected AppManagerSettings _Settings;
		

		public MainWorkItem()
		{
			AMSetttingsFactory.WorkItem = this;
			_Settings = AMSetttingsFactory.DefaultSettingsBag.Settings;
			_Commands = new AppCommands(this);
			_KbrdHook = new KeyboardHook();

//#if RELEASE
			//_MsHook = new MouseHook();
//#endif
			_ImageLoader = new AsyncImageLoader();
			_MainWindow = new MainWindow(this);
			_TrayIcon = new WinForms.NotifyIcon();
			_AppData = new AppGroup();
			_RecycleBin = new DeletedAppCollection();
			UpdateRunning = false;
		}


		public bool UpdateRunning
		{ get; set; }

		public AsyncImageLoader ImageLoader
		{
			get
			{
				return _ImageLoader;
			}
			set
			{
				_ImageLoader = value;
			}
		}

		public AppCommands Commands
		{
			get
			{
				return _Commands;
			}
		}

		public MainWindow MainWindow
		{
			get
			{
				return _MainWindow;
			}
		}

		public KeyboardHook KbrdHook
		{
			get
			{
				return _KbrdHook;
			}
		}

		public MouseHook MsHook
		{
			get
			{
				return _MsHook;
			}
		}

		public WinForms.NotifyIcon TrayIcon
		{
			get
			{
				return _TrayIcon;
			}
		}

		public AppManagerSettings Settings
		{
			get
			{
				return _Settings;
			}
		}

		public string AppPath
		{
			get
			{
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}

		
		public AppGroup AppData
		{
			get
			{
				return _AppData;
			}
			set
			{
				_AppData = value;
			}
		}

		public DeletedAppCollection RecycleBin
		{
			get
			{
				return _RecycleBin;
			}
			set
			{
				_RecycleBin = value;
			}
		}

		public string DataDir
		{
			get
			{
				string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				path = Path.Combine(path, "AppManager");
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				return path;
			}
		}

		public string DataPath
		{
			get
			{
#if DEBUG
				return Path.Combine(DataDir, "appdata.xml");
#else
				return Path.Combine(DataDir, "appdata.xml");
#endif
			}
		}

		public string StatPath
		{
			get
			{
#if DEBUG
				return Path.Combine(DataDir, "appstat.xml");
#else
				return Path.Combine(DataDir, "appstat.xml");
#endif
			}
		}

		public string RecycleBinPath
		{
			get
			{
#if DEBUG
				return Path.Combine(DataDir, "recyclebin.xml");
#else
				return Path.Combine(DataDir, "recyclebin.xml");
#endif
			}
		}


		public void NotifyUpdateCompleted()
		{
			if (UpdateCompleted != null)
				UpdateCompleted(this, EventArgs.Empty);
		}

		public Window FindActiveWindow()
		{
			Window result = null;
			foreach (Window item in App.Current.Windows)
			{
				if (item.IsActive)
					result = item;
			}

			return result == null ? MainWindow : result;
		}
	}
}
