using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AppManager.Windows;
using CommonLib.Windows;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for Manger.xaml
	/// </summary>
	public partial class WndAppManager : DialogWindow
	{
		protected AppManagerController _Controller;
		protected object _ItemToSelect;
		protected AppTypeCollection _AppTypes;
		protected bool _SearchAppCheck = true;


		public WndAppManager()
		{
			InitializeComponent();
		}


		public void Init(MainWorkItem workItem, AppGroup appGroup, AppInfo appInfo, AppType appType)
		{
			_Controller = new AppManagerController(workItem, appGroup);
			AppTypes.ItemsSource = appGroup.AppTypes;
			_AppTypes = appGroup.AppTypes;
			AppTypeSelector.ItemsSource = appGroup.AppTypes;
			//AppScanType.ItemsSource = appGroup.AppTypes;

			if (appInfo != null)
			{
				AppType selAppType = appGroup.FindAppType(appInfo);
				AppTypes.SelectedItem = selAppType;
				AppTypeSelector.SelectedItem = selAppType;

				_ItemToSelect = appInfo;
				AppList.SelectedItem = appInfo;

				AppTabs.SelectedIndex = 1;
			}

			if (appType != null)
			{
				AppType selAppType = appGroup.AppTypes.FindBySource(appType);
				AppTypes.SelectedItem = selAppType;
				AppTypeSelector.SelectedItem = selAppType;

				_ItemToSelect = appGroup.CreateNewAppInfo(selAppType);
				AppList.SelectedItem = _ItemToSelect;

				AppTabs.SelectedIndex = 1;
			}
		}


		//App type tab events===================================================
		private void BtnAddAppType_Click(object sender, RoutedEventArgs e)
		{
			_Controller.AddType();
			AppTypes.SelectedIndex = AppTypes.Items.Count - 1;
			AppTypeName.Focus();
		}

		private void BtnRemoveAppType_Click(object sender, RoutedEventArgs e)
		{
			if (AppTypes.SelectedItem != null)
				_Controller.RemoveType(AppTypes.SelectedItem as AppType);
		}

		private void BtnAppTypeUp_Click(object sender, RoutedEventArgs e)
		{
			_Controller.MoveType(
				 AppTypes.SelectedItem as AppType,
				 true);
		}

		private void BtnAppTypeDown_Click(object sender, RoutedEventArgs e)
		{
			_Controller.MoveType(
				 AppTypes.SelectedItem as AppType,
				 false);
		}

		//App management tab events===================================================
		private void BtnAddApp_Click(object sender, RoutedEventArgs e)
		{
			AppType appType = AppTypeSelector.SelectedItem as AppType;

			if (appType != null)
			{
				_Controller.AddApp(appType);

				AppList.Focus();
				AppList.SelectedIndex = AppList.Items.Count - 1;
				AppList.ScrollIntoView(AppList.Items[AppList.Items.Count - 1]);
			}

			//ListBoxItem lbi =
			//   (ListBoxItem)(AppList.ItemContainerGenerator.ContainerFromItem(AppList.Items.CurrentItem));

			//// Getting the ContentPresenter of myListBoxItem
			//ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(lbi);

			//// Finding textBlock from the DataTemplate that is set on that ContentPresenter
			//DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
			//Grid myTextBlock = (Grid)myDataTemplate.FindName("GridT", myContentPresenter);
		}

		private void BtnRemoveApp_Click(object sender, RoutedEventArgs e)
		{
			AppType appType = AppTypeSelector.SelectedItem as AppType;
			for (int i = AppList.SelectedItems.Count - 1; i >= 0; i--)
				_Controller.RemoveApp(appType, AppList.SelectedItems[i] as AppInfo);
		}

		private void BtnAppDown_Click(object sender, RoutedEventArgs e)
		{
			_Controller.MoveApp(
				 AppTypeSelector.SelectedItem as AppType,
				 AppList.SelectedItem as AppInfo,
				 false);
		}

		private void BtnAppUp_Click(object sender, RoutedEventArgs e)
		{
			_Controller.MoveApp(
				 AppTypeSelector.SelectedItem as AppType,
				 AppList.SelectedItem as AppInfo,
				 true);
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

		//Search apps tab events===================================================
		private void BtnSearch_Click(object sender, RoutedEventArgs e)
		{
			AppScanList.ItemsSource = _Controller.AdaptTo(
					 _Controller.Scan(TxtFolder.Text), _SearchAppCheck);
		}

		private void BtnAddScan_Click(object sender, RoutedEventArgs e)
		{
			//AppType appType = AppScanType.SelectedItem as AppType;
			if (AppScanList.Items.Count <= 0)
				return;
			
			var ss = new SimpleSelector(
				_AppTypes, 
				AppTypes.SelectedItem, 
				"AppTypeName", 
				Strings.SELECT_APP_GROUP);

			ss.Owner = this;

			if (ss.ShowDialog() ?? false)
			{
				var foundApps = AppScanList.ItemsSource as List<AppManagerController.AppInfoAdapter>;
				if (ss.AutoGroup)
					_Controller.AddScned(foundApps);
				else
					_Controller.AddScned(
						ss.SelectedItem as AppType,
						ss.NewName,
						foundApps);
			}
		}

		private void ScanTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox tb = sender as TextBox;

			DependencyObject dobj = AppScanList.ContainerFromElement(tb);
			AppScanList.SelectedIndex = AppScanList.ItemContainerGenerator.IndexFromContainer(dobj);
		}

		private void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			if (cb != null)
			{
				_SearchAppCheck = cb.IsChecked ?? false;
				_Controller.SelectAllScan(AppScanList.ItemsSource, _SearchAppCheck);
			}
		}

		private void BtnScanQuickLaunch_Click(object sender, RoutedEventArgs e)
		{
			AppScanList.ItemsSource = _Controller.AdaptTo(
					 _Controller.FindAppsInQuickLaunch(), _SearchAppCheck);
		}

		private void BtnScanAllProgs_Click(object sender, RoutedEventArgs e)
		{
			AppScanList.ItemsSource = _Controller.AdaptTo(
				 _Controller.FindAppsInAllProgs(), _SearchAppCheck);
		}

		//Other events===================================================
		private void AppsManagerWindow_Activated(object sender, EventArgs e)
		{
			if (_ItemToSelect != null)
			{
				AppList.ScrollIntoView(_ItemToSelect);
				AppList.Focus();
				var cont = AppList.ItemContainerGenerator.ContainerFromItem(_ItemToSelect) as FrameworkElement;
				cont.Focus();
				_ItemToSelect = null;
			}
		}

		private void AppTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			AppTypeName.Focus();
			AppTypeName.SelectAll();
		}
	}
}