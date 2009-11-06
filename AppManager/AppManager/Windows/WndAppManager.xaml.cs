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
		protected AppGroup _AppGroup;
		protected AppTypeCollection _AppTypes;
		protected bool _SearchAppCheck = true;


		public WndAppManager()
		{
			InitializeComponent();
		}


		public void Init(MainWorkItem workItem, AppGroup appGroup, AppInfo appInfo, AppType appType)
		{
			_AppGroup = appGroup;
			_Controller = new AppManagerController(workItem);
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


		protected void SelectAppListItem(DependencyObject element)
		{
			DependencyObject dobj = AppList.ContainerFromElement(element);
			AppList.SelectedIndex = AppList.ItemContainerGenerator.IndexFromContainer(dobj);
		}


		//App type tab events===================================================
		private void BtnAddAppType_Click(object sender, RoutedEventArgs e)
		{
			_Controller.AddEmptyAppType(_AppGroup, null);
			AppTypes.SelectedIndex = AppTypes.Items.Count - 1;
			AppTypeName.Focus();
		}

		private void BtnRemoveAppType_Click(object sender, RoutedEventArgs e)
		{
			if (AppTypes.SelectedItem != null)
				_Controller.DeleteAppType(
					_AppGroup, 
					AppTypes.SelectedItem as AppType, 
					true);
		}

		private void BtnAppTypeUp_Click(object sender, RoutedEventArgs e)
		{
			_Controller.MoveType(
				_AppGroup,
				 AppTypes.SelectedItem as AppType,
				 true);
		}

		private void BtnAppTypeDown_Click(object sender, RoutedEventArgs e)
		{
			_Controller.MoveType(
				_AppGroup,
				 AppTypes.SelectedItem as AppType,
				 false);
		}


		//App management tab events===================================================
		private void BtnAddApp_Click(object sender, RoutedEventArgs e)
		{
			AppType appType = AppTypeSelector.SelectedItem as AppType;

			if (appType != null)
			{
				_Controller.AddAppInfo(_AppGroup, appType);

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
				_Controller.DeleteAppInfo(appType, AppList.SelectedItems[i] as AppInfo, true);
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

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			SelectAppListItem(sender as DependencyObject);
		}

		private void BtnFolder_Click(object sender, RoutedEventArgs e)
		{
			TxtFolder.Text = _Controller.SelectPath();
		}

		private void AppItemRemove_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Button btn = sender as Button;
			AppType appType = AppTypeSelector.SelectedItem as AppType;
			_Controller.DeleteAppInfo(appType, btn.DataContext as AppInfo, true);

		}

		private void AppItemIconPathSelect_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Button btn = sender as Button;
			SelectAppListItem(btn);
			_Controller.SetAppInfoImage(btn.DataContext as AppInfo);
		}

		private void AppItemPathSelect_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Button btn = sender as Button;
			SelectAppListItem(btn);
			_Controller.SelectAppPath(btn.DataContext as AppInfo);
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
					_Controller.AddScned(_AppGroup, foundApps);
				else
					_Controller.AddScned(
						_AppGroup,
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

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var wnd = CommonLib.UIHelper.FindAncestorOrSelf<Window>(sender as DependencyObject, null);
		}


		//private void AppScanList_KeyDown(object sender, KeyEventArgs e)
		//{
			//if (e.OriginalSource is TextBox)
			//    return;

			//var apps = AppScanList.ItemsSource as AppManagerController.AppInfoAdapterCollection;
			//if (apps != null)
			//{
			//    var item = apps.FindByNameStart(
			//        e.Key.ToString(), AppScanList.SelectedIndex);

			//    if (item != null)
			//    {
			//        AppScanList.ScrollIntoView(item);
			//        AppScanList.SelectedItem = item;
			//        var cont = AppScanList.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
			//        cont.Focus();
			//    }
			//}

			//e.Handled = true;
		//}
	}
}