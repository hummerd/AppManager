using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppManager.Entities;
using AppManager.Windows;
using CommonLib.Windows;
using System.Windows.Data;


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
		protected DeletedAppCollection _DeletedApps;


		public WndAppManager()
		{
			InitializeComponent();
		}


		public void Init(MainWorkItem workItem, AppGroup appGroup, AppInfo appInfo, AppType appType, DeletedAppCollection deletedApps)
		{
			_AppGroup = appGroup;
			_Controller = new AppManagerController(workItem);
			AppTypes.ItemsSource = appGroup.AppTypes;
			_AppTypes = appGroup.AppTypes;
			AppTypeSelector.ItemsSource = appGroup.AppTypes;
			DeletedAppList.ItemsSource = deletedApps;
			_DeletedApps = deletedApps;
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

		protected void RemoveSelectedApps()
		{
			AppType appType = AppTypeSelector.SelectedItem as AppType;
			for (int i = AppList.SelectedItems.Count - 1; i >= 0; i--)
				_Controller.DeleteAppInfo(appType, AppList.SelectedItems[i] as AppInfo, true);			
		}

		protected void RemoveSelectedAppType()
		{
			if (AppTypes.SelectedItem != null)
				_Controller.DeleteAppType(
					_AppGroup,
					AppTypes.SelectedItem as AppType,
					true);
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
			RemoveSelectedAppType();
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

		private void AppTypes_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Delete && e.OriginalSource.GetType() == typeof(ListBoxItem))
			{
				RemoveSelectedAppType();
			}
		}

		private void TbAppTypeInfo_Initialized(object sender, EventArgs e)
		{
			var tb = sender as TextBlock;
			tb.FontSize -= 1;
		}

		private void TbAppTypeName_Initialized(object sender, EventArgs e)
		{
			var tb = sender as TextBlock;
			tb.FontSize += 1;
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
			RemoveSelectedApps();
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

		private void AppList_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete && e.OriginalSource.GetType() == typeof(ListBoxItem))
			{
				RemoveSelectedApps();
			}
		}


		//Search apps tab events===================================================
		private void BtnFolder_Click(object sender, RoutedEventArgs e)
		{
			var path = _Controller.SelectPath();
			if (!String.IsNullOrEmpty(path))
				TxtFolder.Text = path;
		}

		private void BtnSearch_Click(object sender, RoutedEventArgs e)
		{
			AppScanList.ItemsSource = _Controller.FindApps(
				new List<string>() { TxtFolder.Text },
				new List<string>() { "lnk", "exe", "jar" },
				_AppGroup,
				_DeletedApps,
				ChkExcludeExisting.IsChecked ?? false,
				ChkExcludeRecycleBin.IsChecked ?? false);

			AppScanList.Focus();
			AppScanList.SelectAll();
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
				Strings.SELECT_APP_GROUP,
				true);

			ss.Owner = this;

			if (ss.ShowDialog() ?? false)
			{
				var selectedApps = GetSelected();

				if (ss.AutoGroup)
					_Controller.AddScned(_AppGroup, selectedApps);
				else
					_Controller.AddScned(
						_AppGroup,
						ss.SelectedItem as AppType,
						ss.NewName,
						selectedApps);
			}
		}

		private void ScanTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox tb = sender as TextBox;

			DependencyObject dobj = AppScanList.ContainerFromElement(tb);
			AppScanList.SelectedIndex = AppScanList.ItemContainerGenerator.IndexFromContainer(dobj);
		}

		private void BtnScanQuickLaunch_Click(object sender, RoutedEventArgs e)
		{
			AppScanList.ItemsSource = _Controller.FindApps(
				SearchLocation.QuickLaunch,
				_AppGroup,
				_DeletedApps,
				ChkExcludeExisting.IsChecked ?? false,
				ChkExcludeRecycleBin.IsChecked?? false);

			AppScanList.Focus();
			AppScanList.SelectAll();
		}

		private void BtnScanAllProgs_Click(object sender, RoutedEventArgs e)
		{
			AppScanList.ItemsSource = _Controller.FindApps(
				SearchLocation.AllProgramsMenu, 
				_AppGroup, 
				_DeletedApps,
				ChkExcludeExisting.IsChecked ?? false,
				ChkExcludeRecycleBin.IsChecked ?? false);

			AppScanList.Focus();
			AppScanList.SelectAll();
		}

		private void BtnToRecycleBin_Click(object sender, RoutedEventArgs e)
		{
			var selected = GetSelected();

			if (selected.Count > 0)
			{
				_Controller.AddToBin(
					_DeletedApps,
					selected,
					AppScanList.ItemsSource as AppInfoCollection);

				AppTabs.SelectedItem = TabRecycleBin;
			}
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

		private void AppsManagerWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var szInf = new Size(double.PositiveInfinity, double.PositiveInfinity);
			BtnScanAllProgs.Measure(szInf);
			BtnScanQuickLaunch.Measure(szInf);

			double nw = Math.Max(
				BtnScanAllProgs.DesiredSize.Width,
				BtnScanQuickLaunch.DesiredSize.Width);

			BtnScanAllProgs.Width = nw;
			BtnScanQuickLaunch.Width = nw;
		}

		private void BtnResore_Click(object sender, RoutedEventArgs e)
		{
			var delApp = DeletedAppList.SelectedItems;

			AppType appType = null;
			bool hasNoType = false;
			foreach (DeletedApp da in delApp)
			{
				if (da.DeletedFrom == null)
				{
					hasNoType = true;
					break;
				}
			}

			if (hasNoType)
			{
				var ss = new SimpleSelector(
					_AppTypes,
					AppTypes.SelectedItem,
					"AppTypeName",
					Strings.SELECT_APP_GROUP,
					false);

				ss.Owner = this;
				if (ss.ShowDialog() ?? false)
				{
					_Controller.RestoreApp(
						_AppGroup,
						new DeletedAppCollection(delApp),
						_DeletedApps,
						appType,
						ss.NewName
						);
				}

				return;
			}

			_Controller.RestoreApp(
				_AppGroup,
				new DeletedAppCollection(delApp),
				_DeletedApps,
				null,
				null
				);
		}

		private void BtnDeleteFromBin_Click(object sender, RoutedEventArgs e)
		{
			_Controller.DeleteFromBin(
				_DeletedApps,
				 new DeletedAppCollection(DeletedAppList.SelectedItems)
				);
		}


		protected AppInfoCollection GetSelected()
		{
			return new AppInfoCollection(
				AppScanList.SelectedItems);
		}
	}

	public class DeletedAppInfoConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var ai = value as DeletedApp;
			if (ai == null)
				return string.Empty;

			return string.Format(
				Strings.DELETED_FROM,
				(ai.DeletedFrom == null ? Strings.NOT_SPECIFIED : ai.DeletedFrom.AppTypeName)
				);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
