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
using WinForms = System.Windows.Forms;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for Manger.xaml
	/// </summary>
	public partial class WndAppManager : Window
	{
		protected AppManagerController _Controller = new AppManagerController();
		protected AppGroup _Data;


		public WndAppManager()
		{
			this.InitializeComponent();
		}

		public void Init(AppGroup appGroup)
		{
			_Data = appGroup;
			AppTypes.ItemsSource = _Data.AppTypes;
			AppTypeSelector.ItemsSource = _Data.AppTypes;
			AppScanType.ItemsSource = _Data.AppTypes;
		}


		private void BtnOK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void BtnAddAppType_Click(object sender, RoutedEventArgs e)
		{
			_Data.AppTypes.Add(new AppType());
			AppTypes.SelectedIndex = AppTypes.Items.Count - 1;
		}

		private void BtnRemoveAppType_Click(object sender, RoutedEventArgs e)
		{
			if (AppTypes.SelectedItem != null)
				_Data.AppTypes.Remove(AppTypes.SelectedItem as AppType);
		}

		private void BtnAddApp_Click(object sender, RoutedEventArgs e)
		{
			AppType appType = AppTypeSelector.SelectedItem as AppType;
			
			if (appType != null)
			{
				appType.AppInfos.Add(
					new AppInfo() 
					{ 
						AppName = AppInfo.DefaultAppName,
					});
			}

			//ListBoxItem lbi =
			//   (ListBoxItem)(AppList.ItemContainerGenerator.ContainerFromItem(AppList.Items.CurrentItem));

			//// Getting the ContentPresenter of myListBoxItem
			//ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(lbi);

			//// Finding textBlock from the DataTemplate that is set on that ContentPresenter
			//DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
			//Grid myTextBlock = (Grid)myDataTemplate.FindName("GridT", myContentPresenter);
		}

		private childItem FindVisualChild<childItem>(DependencyObject obj)
			 where childItem : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is childItem)
					return (childItem)child;
				else
				{
					childItem childOfChild = FindVisualChild<childItem>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		private void BtnRemoveApp_Click(object sender, RoutedEventArgs e)
		{
			AppType appType = AppTypeSelector.SelectedItem as AppType;
			AppInfo appInfo = AppList.SelectedItem as AppInfo;

			if (appType != null && appInfo != null)
			{
				appType.AppInfos.Remove(appInfo);
			}
		}

		private void AppPathSelect_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			_Controller.SelectAppPath(btn.DataContext as AppInfo);
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox tb = sender as TextBox;
			
			DependencyObject dobj = AppList.ContainerFromElement(tb);
			AppList.SelectedIndex = AppList.ItemContainerGenerator.IndexFromContainer(dobj);
		}

		private void BtnFolder_Click(object sender, RoutedEventArgs e)
		{
			TxtFolder.Text = _Controller.SelectPath();
		}

		private void BtnSearch_Click(object sender, RoutedEventArgs e)
		{
			AppScanList.ItemsSource = _Controller.Scan(TxtFolder.Text);
		}

		private void BtnAddScan_Click(object sender, RoutedEventArgs e)
		{
			AppType appType = AppScanType.SelectedItem as AppType;
			_Controller.AddScned(
				appType, 
				AppScanList.ItemsSource as List<AppManager.AppManagerController.AppInfoAdapter>);
		}

		private void ScanTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox tb = sender as TextBox;

			DependencyObject dobj = AppScanList.ContainerFromElement(tb);
			AppScanList.SelectedIndex = AppScanList.ItemContainerGenerator.IndexFromContainer(dobj);
		}
	}
}