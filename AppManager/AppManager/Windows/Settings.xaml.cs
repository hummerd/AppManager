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
			ChkUseTransparentActivationPanel.IsChecked = workItem.Settings.TransparentActivationPanel;
			ChkShowAppTitles.IsChecked = workItem.Settings.ShowAppTitles;
			ChkUseShortActivationPanel.IsChecked = workItem.Settings.UseShortActivationPanel;
			ChkCeckNewVersionAtStartup.IsChecked = workItem.Settings.CheckNewVersionAtStartUp;
			_ActivationPanelColor = workItem.Settings.ActivationPanelColor;
			ActivationPanelColor.Fill = new SolidColorBrush(workItem.Settings.ActivationPanelColor);
		}


		protected void SetEnabledState()
		{
			bool actPanelEnabled = ChkEnableAcivationPanel.IsChecked ?? false;
			bool actPanelTrans = ChkUseTransparentActivationPanel.IsChecked ?? false;

			ChkUseShortActivationPanel.IsEnabled = actPanelEnabled;
			ChkUseTransparentActivationPanel.IsEnabled = actPanelEnabled;

			ActivationPanelColor.IsEnabled =
			ActivationPanelColorLabel.IsEnabled = !actPanelTrans && actPanelEnabled;
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
				var sett = _Controller.WorkItem.Settings;

				sett.NotifyPropertyChanged = false;
				sett.AlwaysOnTop = ChkAlwaysOnTop.IsChecked ?? false;
				sett.StartMinimized = ChkStartMinimized.IsChecked ?? false;
				sett.EnableActivationPanel = ChkEnableAcivationPanel.IsChecked ?? false;
				sett.UseShortActivationPanel = ChkUseShortActivationPanel.IsChecked ?? false;
				sett.CheckNewVersionAtStartUp = ChkCeckNewVersionAtStartup.IsChecked ?? false;
				sett.TransparentActivationPanel = ChkUseTransparentActivationPanel.IsChecked ?? false;
				sett.ShowAppTitles = ChkShowAppTitles.IsChecked ?? false;
				sett.ActivationPanelColor = _ActivationPanelColor;
				sett.NotifyPropertyChanged = true;

				sett.NotifyAllPropertyChanged();
			}
		}

		private void ChkEnableAcivationPanel_Checked(object sender, RoutedEventArgs e)
		{
			SetEnabledState();
			//ChkUseShortActivationPanel.IsEnabled = true;
			//ActivationPanelColor.IsEnabled = true;
			//ActivationPanelColorLabel.IsEnabled = true;
		}

		private void ChkEnableAcivationPanel_Unchecked(object sender, RoutedEventArgs e)
		{
			SetEnabledState();
			//ChkUseShortActivationPanel.IsEnabled = false;
			//ActivationPanelColor.IsEnabled = false;
			//ActivationPanelColorLabel.IsEnabled = false;
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
