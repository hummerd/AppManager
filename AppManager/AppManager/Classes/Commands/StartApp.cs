using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using AppManager.Common;
using AppManager.Properties;
using AppManager.Settings;
using WinForms = System.Windows.Forms;


namespace AppManager.Commands
{
	public class StartApp : CommandBase
	{
		public StartApp(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			AMSetttingsFactory.WorkItem = _WorkItem;

			App app = new App();
			app.InitializeComponent();
			app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			app.SessionEnding += App_SessionEnding;

			LoadData();
			bool first = FirstLoad();
			_WorkItem.AppData.StartLoadImages();

			_WorkItem.MainWindow.Init(first);

			KeyboardHook kbrdHook = _WorkItem.KbrdHook;
			kbrdHook.KeyDown += KbrdHook_KeyDown;

			WinForms.NotifyIcon tray = _WorkItem.TrayIcon;
			tray.Icon = Resources.leftarrow;
			tray.MouseUp += TrayIcon_MouseUp;
			tray.Visible = true;
			tray.ContextMenuStrip = CreateTrayMenu();
			
			Assembly.Load("DragDropLib");

			app.Startup += delegate(object sender, StartupEventArgs e)
			   { _WorkItem.MainWindow.LoadState(); };
			app.Run(_WorkItem.MainWindow);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject != null)
				MessageBox.Show(e.ExceptionObject.ToString());
		}


		protected bool FirstLoad()
		{
			if (_WorkItem.AppData.AppTypes.Count == 1 &&
				 _WorkItem.AppData.AppTypes[0].AppInfos.Count == 0)
			{
				if (MessageBox.Show(
					_WorkItem.MainWindow,
					Strings.ADD_PROGS_QUEST,
					Strings.APP_TITLE,
					MessageBoxButton.YesNo) != MessageBoxResult.Yes)
					return false;

				var ctrl = new ControllerBase(_WorkItem);
				var apps = ctrl.FindAppsInAllProgs();

				_WorkItem.AppData.AppTypes[0].AppInfos.AddRange(apps);
				_WorkItem.AppData.GroupByFolders();
				_WorkItem.Commands.Save.Execute(null);
				
				//StringBuilder allPrograms = new StringBuilder(300);
				//Shell32.SHGetSpecialFolderPath(IntPtr.Zero, allPrograms, Shell32.CSIDL_COMMON_PROGRAMS, false);
				//string[] auLinks = Directory.GetFiles(allPrograms.ToString(), "*.lnk", SearchOption.AllDirectories);

				//string path = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
				//string[] uLinks = Directory.GetFiles(path, "*.lnk", SearchOption.AllDirectories);

				//var links = new List<string>(auLinks.Length + uLinks.Length);
				//links.AddRange(auLinks);
				//links.AddRange(uLinks);

				//var ctrl = new ControllerBase(_WorkItem);
				//ctrl.AddFiles(_WorkItem.AppData.AppTypes[0], links, true, false);
				//_WorkItem.AppData.GroupByFolders();
				//_WorkItem.Commands.Save.Execute(null);

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
					Strings.SETTINGS, _WorkItem.Commands.Settings));
			mnu.Items.Add(
				WinFrmMenuAdapter.CreateMenuItem(
					Strings.MNU_MANAGEAPP, _WorkItem.Commands.ManageApps));
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
		
		protected void ChangeActiveState()
		{
			if (_WorkItem.MainWindow.IsVisible)
				_WorkItem.Commands.Deactivate.Execute(null);
			else
				_WorkItem.Commands.Activate.Execute(null);
		}


		private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
		{
			_WorkItem.Commands.Quit.Execute(null);
		}

		private void TrayIcon_MouseUp(object sender, WinForms.MouseEventArgs e)
		{
			if (e.Button == WinForms.MouseButtons.Left)
			{
				WinForms.Application.DoEvents();
				ChangeActiveState();
			}
			else
			{
				//System.Windows.Forms.ContextMenuStrip mnu = new System.Windows.Forms.ContextMenuStrip();
				//mnu.Items.Add("asdasd");
				//mnu.Items.Add("zxcxc");
				//mnu.Show(sender as System.Windows.Forms.Control, e.X, e.Y);

				//ContextMenu mnu = App.Current.Resources["TrayMenu"] as ContextMenu;
				//mnu.IsOpen = true;
				//mnu.Mo
			}
		}

		private void KbrdHook_KeyDown(object sender, HookEventArgs e)
		{
			if (e.Alt && e.Key == System.Windows.Forms.Keys.Oemtilde)
			{
				ChangeActiveState();
				e.Handled = true;
			}
		}
	}
}
