using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using AppManager.Properties;
using AppManager.Windows;
using CommonLib;
using CommonLib.Application;
using CommonLib.PInvoke.WinHook;
using CommonLib.Windows;
using UpdateLib;
using WinForms = System.Windows.Forms;


namespace AppManager.Commands
{
	public class StartApp : CommandBase
	{
		//protected Mutex				_Mutex;
		protected bool					_FirstStart = false;
		protected SingleInstance	_Single;


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
			ThreadPool.QueueUserWorkItem(InitDrag);

			_Single = new SingleInstance(10251, true);
			if (!_Single.FirstInstance)
				return;

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " Start");

			LoadData();
			_FirstStart = FirstLoad();
			
			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " LoadData");

			_WorkItem.KbrdHook.KeyDown += KbrdHook_KeyDown;
#if RELEASE
			//_WorkItem.MsHook.MouseUp += MsHook_MouseUp;
#endif
			WinForms.NotifyIcon tray = _WorkItem.TrayIcon;
			tray.Icon = Resources.leftarrow;
			tray.MouseUp += TrayIcon_MouseUp;
			tray.Visible = true;
			tray.Text = Strings.APP_TITLE;
			tray.ContextMenuStrip = CreateTrayMenu();

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " NotifyIcon");

			_WorkItem.AppData.StartLoadImages();
			_WorkItem.MainWindow.DataContext = _WorkItem;
			_WorkItem.MainWindow.LoadState();

			_WorkItem.Updater.UpdateAppAsync(
				"AppManager",
				Strings.APP_TITLE,
				_WorkItem.AppPath,
				new string[] { Assembly.GetExecutingAssembly().Location },
				new string[] { Process.GetCurrentProcess().ProcessName }
				);

			if (!_WorkItem.Settings.StartMinimized)
			{
				_WorkItem.MainWindow.Show();
				_WorkItem.MainWindow.SetFocus();
			}

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

			try
			{
				apps = XmlSerializeHelper.DeserializeItem(
					_WorkItem.AppData.GetType(),
					_WorkItem.DataPath) as AppGroup;
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

		protected void InitDrag(object state)
		{
			try
			{
				DragDropLib.DragHelperBase.InitDragHelper();
			}
			catch
			{ ; }
		}


		private void TrayIcon_MouseUp(object sender, WinForms.MouseEventArgs e)
		{
			if (e.Button == WinForms.MouseButtons.Left)
			{
				//WinForms.Application.DoEvents();
				ChangeActiveState(true);
			}
		}

		private void KbrdHook_KeyDown(object sender, KbrdHookEventArgs e)
		{
			if (e.Alt && e.Key == System.Windows.Forms.Keys.Oemtilde)
			{
				ChangeActiveState(false);
				e.Handled = true;
			}
		}

		private void MsHook_MouseUp(object sender, MouseHookEventArgs e)
		{
			if (e.LeftButton && e.Position.X <= 0.1)
			{
				ChangeActiveState(true);
				//e.Handled = true;
			}
		}
	}
}
