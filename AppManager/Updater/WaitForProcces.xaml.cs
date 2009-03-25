using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Updater
{
	/// <summary>
	/// Interaction logic for WaitForProcces.xaml
	/// </summary>
	public partial class WaitForProcces : Window
	{
		public WaitForProcces(IEnumerable<string> prs)
		{
			InitializeComponent();

			Title = UIStrings.Str("UPDATER");
			BtnRetry.Content = UIStrings.Str("RETRY");
			BtnCancel.Content = UIStrings.Str("CANCEL");
			LblMessage.Content = UIStrings.Str("WAIT_MSG");

			LBProccesses.ItemsSource = prs;
		}

		private void BtnRetry_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void BtnNo_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
