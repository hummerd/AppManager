using System.Windows;
using System.Windows.Input;
using AppManager.Classes;
using CommonLib.Windows;
using System.ComponentModel;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : DialogWindow
	{
		protected SettingsController _Controller;


		public Settings(MainWorkItem workItem)
		{
			InitializeComponent();

			_Controller = new SettingsController(workItem);
			ChkAutoStart.IsChecked = _Controller.IsStartupFileExists();
			ChkAlwaysOnTop.IsChecked = workItem.Settings.AlwaysOnTop;
			ChkStartMinimized.IsChecked = workItem.Settings.StartMinimized;
			ChkEnableAcivationPanel.IsChecked = workItem.Settings.EnableActivationPanel;
			ChkUseShortActivationPanel.IsEnabled = ChkEnableAcivationPanel.IsChecked ?? false;
			ChkUseShortActivationPanel.IsChecked =workItem.Settings.UseShortActivationPanel;
			ChkCeckNewVersionAtStartup.IsChecked = workItem.Settings.CheckNewVersionAtStartUp;
		}


		private void BtnOpenAppDataPath_Click(object sender, RoutedEventArgs e)
		{
			_Controller.ShowAppDataPath();
		}

		private void BtnEditAppData_Click(object sender, RoutedEventArgs e)
		{
			_Controller.EditAppData();
		}

		private void DialogWindow_Closing(object sender, CancelEventArgs e)
		{
			if (DialogResult ?? false)
			{
				_Controller.SetStartUp(ChkAutoStart.IsChecked ?? false);
				_Controller.WorkItem.Settings.AlwaysOnTop = ChkAlwaysOnTop.IsChecked ?? false;
				_Controller.WorkItem.Settings.StartMinimized = ChkStartMinimized.IsChecked ?? false;
				_Controller.WorkItem.Settings.EnableActivationPanel = ChkEnableAcivationPanel.IsChecked ?? false;
				_Controller.WorkItem.Settings.UseShortActivationPanel = ChkUseShortActivationPanel.IsChecked ?? false;
				_Controller.WorkItem.Settings.CheckNewVersionAtStartUp = ChkCeckNewVersionAtStartup.IsChecked ?? false;
			}
		}

		private void ChkEnableAcivationPanel_Checked(object sender, RoutedEventArgs e)
		{
			ChkUseShortActivationPanel.IsEnabled = true;
		}

		private void ChkEnableAcivationPanel_Unchecked(object sender, RoutedEventArgs e)
		{
			ChkUseShortActivationPanel.IsEnabled = false;
		}
	}
}
