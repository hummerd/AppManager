using System;
using System.IO;
using AppManager.Commands;
using AppManager.Common;
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


		public MainWorkItem()
		{
			_Commands = new AppCommands(this);
			_KbrdHook = new KeyboardHook();
			_MainWindow = new MainWindow();
			_TrayIcon = new WinForms.NotifyIcon();
			_AppData = new AppGroup();
			//_AppGroup.AppTypes.Add(new AppType());
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


		public AppGroup AppData
		{
			get
			{
				return _AppData;
			}
			set
			{
				_AppData = value;
				_MainWindow.Init(this);
			}
		}

		public string DataPath
		{
			get
			{ 
				string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				path = Path.Combine(path, "AppManager");
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

#if DEBUG
				return Path.Combine(path, "appdata.xml");
#else
				return Path.Combine(path, "appdata.xml");
#endif
			}
		}
	}
}
