using System.Windows;
using CommonLib.Windows;


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

			BtnYes.Content = CommonLib.Strings.YES;
			BtnNo.Content = CommonLib.Strings.NO;

			LblHeader.Content = Strings.QUEST_FIRST_SCAN;
			ChkAllProgs.Content = Strings.ALL_PROGS;
			ChkQuickSart.Content = Strings.QUICK_START;

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
