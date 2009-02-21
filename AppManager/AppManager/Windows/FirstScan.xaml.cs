using System.Windows;
using CommonLib.Windows;
using CommonLib;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for FirstScan.xaml
	/// </summary>
	public partial class FirstScan : Window
	{
		public FirstScan()
		{
			InitializeComponent();

			new DialogKeyDecorator(this, BtnYes, BtnNo, true);
		}


		public bool AddFromQickStart
		{
			get { return ChkQuickSart.IsChecked ?? false; }
		}

		public bool AddFromAllProgs
		{
			get { return ChkAllProgs.IsChecked ?? false; }
		}


		private void ChkQuickSart_Click(object sender, RoutedEventArgs e)
		{
			BtnYes.IsEnabled = AddFromQickStart || AddFromAllProgs;
		}
	}
}
