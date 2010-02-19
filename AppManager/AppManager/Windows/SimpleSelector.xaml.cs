using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using CommonLib.Windows;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for SimpleSelector.xaml
	/// </summary>
	public partial class SimpleSelector : DialogWindow
	{
		protected object _ItemToSelect;


		public SimpleSelector(IEnumerable items, object selectedItem, string displayPath, string title, bool autoVisible)
		{
			InitializeComponent();

			Title = title;
			CbxInput.DisplayMemberPath = displayPath;
			CbxInput.ItemsSource = items;
			RadioAutoGroup.Visibility =
			LblAutoGroup.Visibility = autoVisible ? Visibility.Visible : Visibility.Collapsed;

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

		public bool AutoGroup
		{
			get
			{
				return RadioAutoGroup.IsChecked ?? false;
			}
		}


		protected void SetEnableState()
		{
			TxtNewTypeName.IsEnabled = RadioNew.IsChecked ?? false;
			CbxInput.IsEnabled = RadioExisting.IsChecked ?? false;
		}


		private void Window_Activated(object sender, EventArgs e)
		{
			if (_ItemToSelect != null)
				CbxInput.SelectedItem = _ItemToSelect;
			else if (CbxInput.Items.Count > 0)
				CbxInput.SelectedIndex = 0;

			CbxInput.Focus();
		}

		private void RadioNew_Click(object sender, RoutedEventArgs e)
		{
			SetEnableState();
			TxtNewTypeName.Focus();
		}

		private void RadioExisting_Click(object sender, RoutedEventArgs e)
		{
			SetEnableState();
			CbxInput.Focus();
		}

		private void RadioAutoGroup_Click(object sender, RoutedEventArgs e)
		{
			SetEnableState();
		}
	}
}
