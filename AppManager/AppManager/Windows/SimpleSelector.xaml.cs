using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for SimpleSelector.xaml
	/// </summary>
	public partial class SimpleSelector : Window
	{
		protected object _ItemToSelect;


		public SimpleSelector(IEnumerable items, object selectedItem, string displayPath, string title)
		{
			InitializeComponent();

			Title = title;
			CbxInput.DisplayMemberPath = displayPath;
			CbxInput.ItemsSource = items;

			_ItemToSelect = selectedItem;
		}


		public object SelectedItem
		{
			get
			{
				return CbxInput.SelectedItem;
			}
		}

		public string NewName
		{
			get
			{
				return TxtNewTypeName.IsEnabled ? TxtNewTypeName.Text : null;
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
			if (_ItemToSelect != null)
				CbxInput.SelectedItem = _ItemToSelect;
			else if (CbxInput.Items.Count > 0)
				CbxInput.SelectedIndex = 0;

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

		private void RadioNew_Click(object sender, RoutedEventArgs e)
		{
			TxtNewTypeName.IsEnabled = RadioNew.IsChecked ?? false;
			CbxInput.IsEnabled = !(RadioNew.IsChecked ?? false);
		}

		private void RadioExisting_Click(object sender, RoutedEventArgs e)
		{
			TxtNewTypeName.IsEnabled = RadioNew.IsChecked ?? false;
			CbxInput.IsEnabled = !(RadioNew.IsChecked ?? false);
		}
	}
}
