using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using AppManager.Properties;
using AppManager.Windows;
using CommonLib;
using CommonLib.PInvoke;
using CommonLib.Windows;
using WinForms = System.Windows.Forms;


namespace AppManager.Commands
{
	public class StartApp : CommandBase
	{
		protected Mutex _Mutex;
		protected bool _FirstStart = false;
		protected SingleInstance _Single;


		public StartApp(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			AppDomain.CurrentDomain.UnhandledException += UnhandledException;

			_Single = new SingleInstance(10251);
			if (!_Single.FirstInstance)
				return;
						
			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " Start");
			
			App app = new App();
			app.InitializeComponent();

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " InitializeComponent");

			app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			app.SessionEnding += App_SessionEnding;

			LoadData();
			_FirstStart = FirstLoad();
			_WorkItem.AppData.StartLoadImages();

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " LoadData");

			//_WorkItem.MainWindow.Init(first);
			//System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " Init");

			KeyboardHook kbrdHook = _WorkItem.KbrdHook;
			kbrdHook.KeyDown += KbrdHook_KeyDown;

			WinForms.NotifyIcon tray = _WorkItem.TrayIcon;
			tray.Icon = Resources.leftarrow;
			tray.MouseUp += TrayIcon_MouseUp;
			tray.Visible = true;
			tray.ContextMenuStrip = CreateTrayMenu();

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " NotifyIcon");

			Assembly.Load("DragDropLib");

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " DragDropLib");

			_WorkItem.MainWindow.DataContext = _WorkItem;
			_WorkItem.MainWindow.LoadState();

			if (!_WorkItem.Settings.StartMinimized)
				_WorkItem.MainWindow.Show();

			OnMainWindowLoaded();

			app.MainWindow = _WorkItem.MainWindow;
			app.Run();
		}


		protected void OnMainWindowLoaded()
		{
			//_WorkItem.MainWindow.LoadState();
			///_WorkItem.MainWindow.Init(_FirstStart);

			//_WorkItem.MainWindow.ContentPanel.InvalidateVisual();
			//System.Threading.Thread.Sleep(500);
			//_WorkItem.MainWindow.ContentPanel.InvalidateVisual();

			if (_FirstStart)
				_WorkItem.Commands.Help.Execute(false);
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
							var quickAppType = new AppType(apps) { AppTypeName = Strings.QUICK_LAUNCH };
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
				CommStr.ABOUT, _WorkItem.Commands.Help, true));

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

			try
			{
				XmlSerializer xser = new XmlSerializer(_WorkItem.AppData.GetType());

				using (XmlReader xr = XmlReader.Create(_WorkItem.DataPath))
				{
					apps = xser.Deserialize(xr) as AppGroup;
				}
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
		
		protected void ChangeActiveState(bool focus)
		{
			if (_WorkItem.MainWindow.IsVisible && (focus || _WorkItem.MainWindow.IsKeyboardFocusWithin))
				_WorkItem.Commands.Deactivate.Execute(null);
			else
				_WorkItem.Commands.Activate.Execute(null);
		}

		
		private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject != null)
			{
				var exc = e.ExceptionObject as Exception;
				if (exc != null)
					ErrorBox.Show(Strings.ERROR, exc.Message, exc.ToString());
				else
					ErrorBox.Show(Strings.ERROR, Strings.ERROR_OCCUR, String.Empty);
			}
		}

		private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
		{
			_WorkItem.Commands.Quit.Execute(null);
		}

		private void TrayIcon_MouseUp(object sender, WinForms.MouseEventArgs e)
		{
			if (e.Button == WinForms.MouseButtons.Left)
			{
				//WinForms.Application.DoEvents();
				ChangeActiveState(true);
			}
		}

		private void KbrdHook_KeyDown(object sender, HookEventArgs e)
		{
			if (e.Alt && e.Key == System.Windows.Forms.Keys.Oemtilde)
			{
				ChangeActiveState(false);
				e.Handled = true;
			}
		}
	}
}
