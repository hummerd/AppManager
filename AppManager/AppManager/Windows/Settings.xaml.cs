using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AppManager.Classes;
using CommonLib.Windows;
using System;
using System.Collections.Generic;
using System.Windows.Controls;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : DialogWindow
	{
		protected SettingsController _Controller;
		protected Color _ActivationPanelColor;
        protected readonly Dictionary<FrameworkElement, string> _HelpContent; 


		public Settings(MainWorkItem workItem)
		{
			InitializeComponent();

            var periods = new[] { 1, 3, 6, 12 };
            cmbStatPeriod.ItemsSource = periods;
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
            cmbStatPeriod.SelectedItem = workItem.Settings.StatisticPeriod;

            SetEnabledState();

            SetHelpText(ChkAutoStart, Strings.HLP_AUTO_START);
            SetHelpText(ChkAlwaysOnTop, Strings.HLP_ALWAYS_ON_TOP);
            SetHelpText(ChkStartMinimized, Strings.HLP_START_MINIMIZED);
            SetHelpText(ChkEnableAcivationPanel, Strings.HLP_ENABLE_ACT_PAN);
            SetHelpText(ChkUseTransparentActivationPanel, Strings.HLP_TRANS_ACT_PANEL);
            SetHelpText(ChkUseShortActivationPanel, Strings.HLP_SHORT_ACT_PANEL);
            SetHelpText(ActivationPanelColor, Strings.HLP_ACT_PANEL_COLOR);
            SetHelpText(ActivationPanelColorLabel, Strings.HLP_ACT_PANEL_COLOR);
            SetHelpText(ChkShowAppTitles, Strings.HLP_SHOW_TITLES);
            SetHelpText(ChkCeckNewVersionAtStartup, Strings.HLP_CHECK_NEW_AT_START);
            SetHelpText(cmbStatPeriod, Strings.HLP_KEEP_STAT);
            SetHelpText(cmbStatPeriodLabel, Strings.HLP_KEEP_STAT);
		}

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            SizeToContent = System.Windows.SizeToContent.Manual;
        }

        protected void SetHelpText(FrameworkElement element, string helpText)
        {
            element.MouseEnter += (e, a) => txtHelp.Text = helpText;
            element.MouseLeave += (e, a) => txtHelp.Text = String.Empty;
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
                sett.StatisticPeriod = (int)cmbStatPeriod.SelectedItem;
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
