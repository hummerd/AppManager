using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AppManager.Entities;
using AppManager.Properties;
using AppManager.Windows;
using CommonLib;
using CommonLib.Application;
using CommonLib.PInvoke;
using CommonLib.PInvoke.WinHook;
using WinForms = System.Windows.Forms;


namespace AppManager.Commands
{
	public class StartApp : CommandBase
	{
		protected delegate void ActivateTask();

		//protected Mutex			_Mutex;
		protected bool				_FirstStart = false;
		protected SingleInstance2	_Single;
		protected bool				_SilentUpdate = true;
		protected DateTime			_LostTime = DateTime.Now;
		protected Window			_WndActivation = null;
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
			{
				App.Current.Shutdown();
				return;
			}

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " Start");

			LoadData();
			_FirstStart = FirstLoad();
			
			_WorkItem.MainWindow.InitData(_WorkItem);
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

			_WorkItem.Settings.PropertyChanged += (s, e) => 
				OnSettingsChanged(e.PropertyName);
		}


		protected void EndInit(bool noUpdate)
		{
			ThreadPool.QueueUserWorkItem(InitDrag);

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " LoadData");

			_WorkItem.KbrdHook.KeyDown += KbrdHook_KeyDown;
			CreateTrayIcon();

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " NotifyIcon");

			if (!noUpdate && _WorkItem.Settings.CheckNewVersionAtStartUp)
				_WorkItem.Commands.CheckVersion.Execute(true);

			_WorkItem.AppData.NeedAppImage += (s, e) =>
				_WorkItem.ImageLoader.RequestImage(s as AppInfo);
			_WorkItem.RecycleBin.RegisterSource(_WorkItem.AppData, true);
			_WorkItem.ImageLoader.StartLoad();
			_WorkItem.AppData.ReInitImages();

			CreateActivationPanelWatcher();
			CreateActivationPanel();

			MemoryHelper.Clean();
		}

		protected bool CheckSingleInstance()
		{
			_Single = new SingleInstance2("AppManagerSingleInstanceObj", delegate()
			{
				ActivateTask act = delegate() { _WorkItem.Commands.Activate.Execute(null); };
				DispatcherHelper.Invoke(act);
			});

			return _Single.FirstInstance;

			//bool first;
			//MutexSecurity msec = new MutexSecurity();

			//using mutex prevent us from immediate loading remoting infrastructure
			//if we start first instance of program
			//_Mutex = new Mutex(true, "AppManagerSingleInstance", out first);

			//if (!first)
			//    return InitFirstInstance();
			//else
			//    ThreadPool.QueueUserWorkItem((o) => InitFirstInstance());

			//return true;
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
					var ctrl = new AppController(_WorkItem);

					var sl = SearchLocation.None;
					if (askScan.AddFromAllProgs)
						sl |= SearchLocation.AllProgramsMenu;

					if (askScan.AddFromQickStart)
						sl |= SearchLocation.QuickLaunch;

					var apps = ctrl.FindApps(sl, null, null, false, false);
					_WorkItem.AppData.AppTypes[0].AppInfos.AddRange(apps);
					_WorkItem.AppData.GroupByFolders(_WorkItem.AppData.AppTypes[0]);


					//if (askScan.AddFromAllProgs)
					//{
					//    var apps = ctrl.FindAppsInAllProgs(null, false);
					//    _WorkItem.AppData.AppTypes[0].AppInfos.AddRange(apps);
					//    _WorkItem.AppData.GroupByFolders(_WorkItem.AppData.AppTypes[0]);
					//}

					//if (askScan.AddFromQickStart)
					//{
					//    var apps = ctrl.FindAppsInQuickLaunch();
					//    if (apps.Count > 0)
					//    {
					//        var quickAppType = new AppType() { AppTypeName = Strings.QUICK_LAUNCH };
					//        quickAppType.AppInfos.AddRange(apps);
					//        _WorkItem.AppData.AppTypes.Insert(0, quickAppType);
					//    }
					//}

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
			//mnu.RenderMode = WinForms.ToolStripRenderMode.Professional;

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
				if (e.PropertyName == "AlwaysOnTop" || e.PropertyName == "All")
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
				apps = AppGroupLoader.Load2(
					_WorkItem.DataPath,
					_WorkItem.StatPath);

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

			_WorkItem.RecycleBin = AppGroupLoader.LoadRecycleBin(_WorkItem.RecycleBinPath);
			
			if (apps != null && 
				apps.AppTypes != null && 
				apps.AppTypes.Count > 0)
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

		protected void OnSettingsChanged(string settName)
		{
			if (settName == "EnableActivationPanel" || 
				settName == "UseShortActivationPanel" ||
				settName == "ActivationPanelColor" ||
				settName == "TransparentActivationPanel" ||
				settName == "All"
				)
				CreateActivationPanel();
		}

		protected void CreateActivationPanelWatcher()
		{
			_ActivationWndPinger = new DispatcherTimer();
			_ActivationWndPinger.Interval = new TimeSpan(0, 0, 5);
			_ActivationWndPinger.Tick += delegate
			{
				if (_WndActivation != null)
				{
					_WndActivation.Show();
					_WndActivation.Topmost = false;
					_WndActivation.Topmost = true;
				}
			};
			_ActivationWndPinger.Start();
		}

		protected void CreateActivationPanel()
		{
			if (!_WorkItem.Settings.EnableActivationPanel)
			{
				if (_WndActivation != null)
					_WndActivation.Hide();

				return;
			}

			if (_WndActivation != null)
				_WndActivation.Close();

			//if (_WndActivation == null)
			//{
				_WndActivation = new Window();

				//_WndActivation.AllowsTransparency = true;
				//wndActivate.Background = System.Windows.Media.Brushes.Transparent;
				//_WndActivation.Opacity = 0.05;
				//_WndActivation.Background = System.Windows.Media.Brushes.CadetBlue;
				_WndActivation.WindowStyle = WindowStyle.None;
				_WndActivation.ResizeMode = ResizeMode.NoResize;
				_WndActivation.Topmost = true;
				_WndActivation.ShowInTaskbar = false;
				_WndActivation.Left = 0;
				_WndActivation.Top = 0;
				_WndActivation.Width = 1;
				_WndActivation.MouseDown += (s, e) =>
					{
						if (e.ChangedButton == MouseButton.Left)
							ChangeActiveState();
						else
							_WorkItem.TrayIcon.ContextMenuStrip.Show(0, 0);
					};
				_WndActivation.Closed += (s, e) => _WndActivation = null;
			//}

			if (_WorkItem.Settings.TransparentActivationPanel)
			{
				_WndActivation.AllowsTransparency = true;
				_WndActivation.Opacity = 0.05;
			}
			else
			{
				_WndActivation.AllowsTransparency = false;
				_WndActivation.Background = new SolidColorBrush(_WorkItem.Settings.ActivationPanelColor);
			}

			_WndActivation.Height = _WorkItem.Settings.UseShortActivationPanel ?
				16 : System.Windows.Forms.SystemInformation.WorkingArea.Height;
			_WndActivation.Show();
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
				ThreadPool.QueueUserWorkItem(o => DispatcherHelper.Invoke(new SimpleMathod(ChangeActiveState)));
				//ChangeActiveState();
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
