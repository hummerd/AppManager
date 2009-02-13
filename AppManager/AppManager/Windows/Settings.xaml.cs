using System.Windows;
using System.Windows.Input;
using AppManager.Classes;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
		protected SettingsController _Controller;


		public Settings(MainWorkItem workItem)
		{
			InitializeComponent();

			_Controller = new SettingsController(workItem);
			ChkAutoStart.IsChecked = _Controller.IsStartupFileExists();
		}


		private void BtnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			_Controller.SetStartUp(ChkAutoStart.IsChecked ?? false);
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void BtnOpenAppDataPath_Click(object sender, RoutedEventArgs e)
		{
			_Controller.ShowAppDataPath();
		}

		private void BtnEditAppData_Click(object sender, RoutedEventArgs e)
		{
			_Controller.EditAppData();
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				DialogResult = true;
			}

			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				DialogResult = false;
			}
		}
	}
}
