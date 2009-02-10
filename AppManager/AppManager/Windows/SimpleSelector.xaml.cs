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
using System.Collections;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for SimpleSelector.xaml
	/// </summary>
	public partial class SimpleSelector : Window
	{
		public SimpleSelector(IEnumerable items, string displayPath, string title)
		{
			InitializeComponent();

			Title = title;
			CbxInput.DisplayMemberPath = displayPath;
			CbxInput.ItemsSource = items;

			if (CbxInput.Items.Count > 0)
				CbxInput.SelectedIndex = 0;
		}


		public object SelectedItem
		{
			get
			{
				return CbxInput.SelectedItem;
			}
		}


		private void BtnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			CbxInput.Focus();
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
