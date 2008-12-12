using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using WinForms = System.Windows.Forms;
using AppManager.Properties;


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
			App app = new App();
			app.InitializeComponent();
			app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			app.SessionEnding += App_SessionEnding;

			LoadData();
			ConfigureMenu();

			KeyboardHook kbrdHook = _WorkItem.KbrdHook;
			kbrdHook.KeyDown += KbrdHook_KeyDown;

			WinForms.NotifyIcon tray = _WorkItem.TrayIcon;
			tray.Icon = Resources.Window1;
			tray.MouseUp += TrayIcon_MouseUp;
			tray.Visible = true;

			_WorkItem.MainWindow.Show();
			app.Run();
		}
		

		protected void ConfigureMenu()
		{
		  //  <ContextMenu x:Key="TrayMenu">
		  //    <MenuItem Name="Show" Header="Показать Стартер"/>
		  //    <MenuItem Header="Скрыть Стартер"/>
		  //    <Separator/>
		  //    <MenuItem Header="Насройки"/>
		  //    <MenuItem Header="Управление приложениями"/>
		  //	  <MenuItem Header="Закрыть Стартер"/>
		  //</ContextMenu>

			ContextMenu mnu = App.Current.Resources["TrayMenu"] as ContextMenu;
			((MenuItem)mnu.Items[0]).Command = _WorkItem.Commands.Activate;
			((MenuItem)mnu.Items[1]).Command = _WorkItem.Commands.Deactivate;
			((MenuItem)mnu.Items[3]).Command = _WorkItem.Commands.Settings;
			((MenuItem)mnu.Items[4]).Command = _WorkItem.Commands.ManageApps;
			((MenuItem)mnu.Items[5]).Command = _WorkItem.Commands.Quit;
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
				_WorkItem.AppData = apps;
			else
			{
				_WorkItem.AppData = new AppGroup();
				_WorkItem.AppData.AppTypes.Add(new AppType());
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
				_WorkItem.Commands.Activate.Execute(null);
			}
			else
			{
				ContextMenu mnu = App.Current.Resources["TrayMenu"] as ContextMenu;
				mnu.IsOpen = true;
			}
		}

		private void KbrdHook_KeyDown(object sender, HookEventArgs e)
		{
			if (e.Alt && e.Key == System.Windows.Forms.Keys.Oemtilde)
			{
				_WorkItem.Commands.Activate.Execute(null);
			}
		}
	}
}
