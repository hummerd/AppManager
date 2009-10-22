using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AppManager.Properties;
using AppManager.Windows;
using CommonLib;
using CommonLib.Application;
using CommonLib.PInvoke.WinHook;
using CommonLib.Windows;
using WinForms = System.Windows.Forms;


namespace AppManager.Commands
{
	public class StartApp : CommandBase
	{
		protected delegate void ActivateTask();


		protected Mutex				_Mutex;
		protected bool					_FirstStart = false;
		protected SingleInstance	_Single;
		protected bool					_SilentUpdate = true;
		protected DateTime			_LostTime = DateTime.Now;
		protected Window				_WndActivation = null;
		protected DispatcherTimer	_ActivationWndPinger;


		public StartApp(MainWorkItem workItem)
			: base(workItem)
		{ 
			
		}


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			if (!CheckSingleInstance())
				return;

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " Start");

			LoadData();
			_FirstStart = FirstLoad();
			
			_WorkItem.MainWindow.DataContext = _WorkItem;
			_WorkItem.MainWindow.LoadState();
			_WorkItem.MainWindow.Deactivated += (s, e) =>
				_LostTime = DateTime.Now;

			if (!_WorkItem.Settings.StartMinimized)
			{
				_WorkItem.MainWindow.Loaded += (s, e) =>
					EndInit((bool)parameter);
				_WorkItem.MainWindow.Show();
				_WorkItem.MainWindow.SetFocus();
			}
			else
			{
				EndInit((bool)parameter);
			}

			if (_FirstStart)
				_WorkItem.Commands.Help.Execute(false);

			_WorkItem.Settings.PropertyChanged += (s, e) => OnSettingsChanged(e.PropertyName);
		}


		protected void EndInit(bool noUpdate)
		{
			ThreadPool.QueueUserWorkItem(InitDrag);

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " LoadData");

			_WorkItem.KbrdHook.KeyDown += KbrdHook_KeyDown;
			CreateTrayIcon();

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " NotifyIcon");

			SetupUpdater(noUpdate);

			_WorkItem.AppData.NeedAppImage += (s, e) => 
				_WorkItem.ImageLoader.RequestImage(s as AppInfo);
			_WorkItem.ImageLoader.StartLoad();
			_WorkItem.AppData.ReInitImages();

			CreateActivationPanel();
		}

		protected bool CheckSingleInstance()
		{
			bool first;
			MutexSecurity msec = new MutexSecurity();

			//using mutex prevent us from immediate loading remoting infrastructure
			//if we start first instance of program
			_Mutex = new Mutex(true, "AppManagerSingleInstance", out first);

			if (!first)
				return InitFirstInstance();
			else
				ThreadPool.QueueUserWorkItem((o) => InitFirstInstance());

			return true;
		}

		protected bool InitFirstInstance()
		{
			//Since this method can be executed in thread pool 
			//we should not allow unhandled exceptions here
			try
			{
				_Single = new SingleInstance(10251, true, delegate()
				{
					ActivateTask act = delegate() { _WorkItem.Commands.Activate.Execute(null); };
					DispatcherHelper.Invoke(act);
				});

				if (!_Single.FirstInstance)
				{
					ActivateTask act = delegate() { App.Current.Shutdown(); };
					DispatcherHelper.Invoke(act);
					return false;
				}
			}
			catch (Exception ex)
			{
				// Dispatch the exception back to the main ui thread and reraise it
				DispatcherHelper.PassExceptionOnUIThread(ex);
			}

			return true;
		}

		protected bool FirstLoad()
		{
			if (_WorkItem.AppData.AppTypes.Count == 1 &&
				 _WorkItem.AppData.AppTypes[0].AppInfos.Count == 0)
			{
				FirstScan askScan = new FirstScan();
				askScan.Title = Strings.APP_TITLE;

				bool doScan = askScan.ShowDialog() ?? false;

				if (doScan)
				{
					var ctrl = new ControllerBase(_WorkItem);

					if (askScan.AddFromAllProgs)
					{
						var apps = ctrl.FindAppsInAllProgs();
						_WorkItem.AppData.AppTypes[0].AppInfos.AddRange(apps);
						_WorkItem.AppData.GroupByFolders(_WorkItem.AppData.AppTypes[0]);
					}

					if (askScan.AddFromQickStart)
					{
						var apps = ctrl.FindAppsInQuickLaunch();
						if (apps.Count > 0)
						{
							var quickAppType = new AppType() { AppTypeName = Strings.QUICK_LAUNCH };
							quickAppType.AppInfos.AddRange(apps);
							_WorkItem.AppData.AppTypes.Insert(0, quickAppType);
						}
					}

					_WorkItem.Commands.Save.Execute(null);
				}

				return true;
			}
			else
				return false;
		}

		protected void CreateTrayIcon()
		{
			WinForms.NotifyIcon tray = _WorkItem.TrayIcon;
			tray.Icon = Resources.leftarrow;
			tray.MouseUp += TrayIcon_MouseUp;
			tray.Visible = true;
			tray.Text = Strings.APP_TITLE;
			tray.ContextMenuStrip = CreateTrayMenu();
		}

		protected WinForms.ContextMenuStrip CreateTrayMenu()
		{
			var mnu = new WinForms.ContextMenuStrip();

			mnu.Items.Add(
				WinFrmMenuAdapter.CreateMenuItem(
					Strings.MNU_SHOW, _WorkItem.Commands.Activate));
			mnu.Items.Add(
				WinFrmMenuAdapter.CreateMenuItem(
					Strings.MNU_HIDE, _WorkItem.Commands.Deactivate));

			mnu.Items.Add( //-------
				new WinForms.ToolStripSeparator());

			mnu.Items.Add(
				WinFrmMenuAdapter.CreateMenuItem(
					Strings.MNU_SETTINGS, _WorkItem.Commands.Settings));

			var tsmi = new System.Windows.Forms.ToolStripMenuItem(Strings.ALWAYS_ON_TOP);
			tsmi.Checked = _WorkItem.Settings.AlwaysOnTop;

			tsmi.Click += (s, e) =>
				_WorkItem.Settings.AlwaysOnTop = !(s as System.Windows.Forms.ToolStripMenuItem).Checked;

			_WorkItem.Settings.PropertyChanged += delegate(object s, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "AlwaysOnTop")
					tsmi.Checked = _WorkItem.Settings.AlwaysOnTop;
			};

			mnu.Items.Add(tsmi);

			mnu.Items.Add(
				WinFrmMenuAdapter.CreateMenuItem(
					Strings.MNU_MANAGEAPP, _WorkItem.Commands.ManageApps));

			mnu.Items.Add(
				WinFrmMenuAdapter.CreateMenuItem(
				CommStr.ABOUT + "...", _WorkItem.Commands.Help, true));

			mnu.Items.Add( //-------
				new WinForms.ToolStripSeparator());

			mnu.Items.Add(
				WinFrmMenuAdapter.CreateMenuItem(
					Strings.MNU_CLOSE, _WorkItem.Commands.Quit));

			return mnu;
		}

		protected void LoadData()
		{
			AppGroup apps = null;
			bool readNew = false;
			try
			{
				apps = AppGroupLoader.Load2(_WorkItem.DataPath);
				readNew = true;
				//apps = XmlSerializeHelper.DeserializeItem(
				//   _WorkItem.AppData.GetType(),
				//   _WorkItem.DataPath) as AppGroup;
			}
			catch
			{ ; }

			if (!readNew)
			try
			{
				apps = AppGroupLoader.Load(_WorkItem.DataPath);
			}
			catch
			{ ; }


			if (apps != null)
			{
				_WorkItem.AppData = apps;
				_WorkItem.AppData.CorrectAppInfoID();
			}
			else
			{
				_WorkItem.AppData = new AppGroup();
				_WorkItem.AppData.AppTypes.Add(new AppType() { AppTypeName = Strings.APPLICATIONS });
			}
		}

		protected void SetupUpdater(bool noupdate)
		{
			_WorkItem.Updater.UpdateCompleted += (s, e) => OnUpdateCompleted(e.SuccessfulCheck, e.HasNewVersion, e.Message);
			_WorkItem.Updater.NeedCloseApp += (s, e) => _WorkItem.Commands.Quit.Execute(null);

			if (!noupdate)
				_WorkItem.Updater.UpdateAppAsync(
					"AppManager",
					Strings.APP_TITLE,
					_WorkItem.AppPath,
					new string[] { Assembly.GetExecutingAssembly().Location },
					new string[] { Process.GetCurrentProcess().ProcessName },
					"http://dimanick.ru/MySoft/AppManager/Update"
					);
		}

		protected void ChangeActiveState()
		{
			var lostDelta = DateTime.Now - _LostTime;
			bool deactivate = lostDelta.TotalMilliseconds < 300;

			if (_WorkItem.MainWindow.IsVisible && (deactivate || _WorkItem.MainWindow.IsKeyboardFocusWithin))
				_WorkItem.Commands.Deactivate.Execute(null);
			else
				_WorkItem.Commands.Activate.Execute(null);
		}

		protected void InitDrag(object state)
		{
			try
			{
				DragDropLib.DragHelperBase.InitDragHelper();
			}
			catch
			{ ; }
		}

		protected void OnUpdateCompleted(bool successfulCheck, bool hasNewVersion, string message)
		{
			if (_SilentUpdate)
			{
				_SilentUpdate = false;
				return;
			}

			var wnd = FindActiveWindow();

			if (!successfulCheck)
			{
				ErrorBox.Show(
					Strings.APP_TITLE,
					Strings.UPDATE_CHECK_FAILED,
					message
					);

				return;
			}

			if (!hasNewVersion)
			{
				MsgBox.Show(
					wnd, 
					Strings.APP_TITLE, 
					String.Format(Strings.NO_NEW_VERSION, Strings.APP_TITLE),
					false);
			}
		}

		protected void OnSettingsChanged(string settName)
		{
			if (settName == "EnableActivationPanel" || settName == "UseShortActivationPanel")
				CreateActivationPanel();
		}

		protected void CreateActivationPanel()
		{
			if (!_WorkItem.Settings.EnableActivationPanel)
			{
				if (_WndActivation != null)
					_WndActivation.Hide();

				return;
			}

			if (_WndActivation == null)
			{
				_WndActivation = new Window();
				_WndActivation.AllowsTransparency = true;
				//wndActivate.Background = System.Windows.Media.Brushes.Transparent;
				_WndActivation.Opacity = 0.05;
				_WndActivation.WindowStyle = WindowStyle.None;
				_WndActivation.ResizeMode = ResizeMode.NoResize;
				_WndActivation.Topmost = true;
				_WndActivation.ShowInTaskbar = false;
				_WndActivation.Left = 0;
				_WndActivation.Top = 0;
				_WndActivation.Width = 1;
				_WndActivation.MouseDown += (s, e) => ChangeActiveState();

				_ActivationWndPinger = new DispatcherTimer();
				_ActivationWndPinger.Interval = new TimeSpan(0, 0, 5);
				_ActivationWndPinger.Tick += delegate 
					{
						_WndActivation.Show();
						_WndActivation.Topmost = false;
						_WndActivation.Topmost = true;
					};
				_ActivationWndPinger.Start();
			}

			_WndActivation.Height = _WorkItem.Settings.UseShortActivationPanel ?
				24 : System.Windows.Forms.SystemInformation.WorkingArea.Height;
			_WndActivation.Show();
		}

		protected Window FindActiveWindow()
		{
			Window result = null;
			foreach (Window item in App.Current.Windows)
			{
				if (item.IsActive)
					result = item;
			}

			return result == null ? _WorkItem.MainWindow : result;
		}


		private void TrayIcon_MouseUp(object sender, WinForms.MouseEventArgs e)
		{
			if (e.Button == WinForms.MouseButtons.Left)
			{
				ChangeActiveState();
			}
		}

		private void KbrdHook_KeyDown(object sender, KbrdHookEventArgs e)
		{
			if (e.Alt && e.Key == System.Windows.Forms.Keys.Oemtilde)
			{
				ChangeActiveState();
				e.Handled = true;
			}
		}

		private void MsHook_MouseUp(object sender, MouseHookEventArgs e)
		{
			if (e.LeftButton && e.Position.X <= 0.1)
			{
				ChangeActiveState();
			}
		}
	}
}
