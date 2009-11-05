using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AppManager.Classes;
using CommonLib.Windows;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : DialogWindow
	{
		protected SettingsController _Controller;
		protected Color _ActivationPanelColor;


		public Settings(MainWorkItem workItem)
		{
			InitializeComponent();

			_Controller = new SettingsController(workItem);
			ChkAutoStart.IsChecked = _Controller.IsStartupFileExists();
			ChkAlwaysOnTop.IsChecked = workItem.Settings.AlwaysOnTop;
			ChkStartMinimized.IsChecked = workItem.Settings.StartMinimized;
			ChkEnableAcivationPanel.IsChecked = workItem.Settings.EnableActivationPanel;
			ChkUseShortActivationPanel.IsEnabled = ChkEnableAcivationPanel.IsChecked ?? false;
			ChkUseShortActivationPanel.IsChecked = workItem.Settings.UseShortActivationPanel;
			ChkCeckNewVersionAtStartup.IsChecked = workItem.Settings.CheckNewVersionAtStartUp;
			_ActivationPanelColor = workItem.Settings.ActivationPanelColor;
			ActivationPanelColor.Fill = new SolidColorBrush(workItem.Settings.ActivationPanelColor);
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
				_Controller.WorkItem.Settings.ActivationPanelColor = _ActivationPanelColor;
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

		private void ActivationPanelColor_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Color clr;
			if (_Controller.SelectColor(out clr))
			{
				ActivationPanelColor.Fill = new SolidColorBrush(clr);
				_ActivationPanelColor = clr;
			}
		}
	}
}
