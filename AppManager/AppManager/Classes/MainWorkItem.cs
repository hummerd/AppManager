using System;
using System.IO;
using AppManager.Commands;
using AppManager.Settings;
using CommonLib.PInvoke.WinHook;
using WinForms = System.Windows.Forms;


namespace AppManager
{
	public class MainWorkItem
	{
		protected AppGroup		_AppData;
		protected MainWindow		_MainWindow;
		protected App				_Application;
		protected KeyboardHook	_KbrdHook;
		protected WinForms.NotifyIcon _TrayIcon;
		protected AppCommands	_Commands;
		protected AppManagerSettings _Settings;


		public MainWorkItem()
		{
			AMSetttingsFactory.WorkItem = this;
			_Settings = AMSetttingsFactory.DefaultSettingsBag.Settings;
			_Commands = new AppCommands(this);
			_KbrdHook = new KeyboardHook();
			_MainWindow = new MainWindow(this);
			_TrayIcon = new WinForms.NotifyIcon();
			_AppData = new AppGroup();
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
	}
}
