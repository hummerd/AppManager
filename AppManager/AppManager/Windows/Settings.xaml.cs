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
				_Controller.SetStartUp(ChkAutoStart.IsChecked ?? false);
		}
	}
}
