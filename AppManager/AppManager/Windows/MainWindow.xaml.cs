using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AppManager.Classes.ViewModel;
using AppManager.Commands;
using AppManager.Controls;
using AppManager.DragDrop;
using AppManager.Entities;
using AppManager.Settings;
using CommonLib;
using CommonLib.PInvoke;
using CommonLib.UI;
using DragDropLib;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		protected System.Windows.Interop.WindowInteropHelper _NativeWindow;
		protected MainWindowController	_Controller;
		protected ItemsControl			_FocusElement;
		protected FileAppDropHandler	_FileDrop = new FileAppDropHandler();
		protected SimpleDragDataHandler	_AppTypeDrop;
		protected SimpleDragDataHandler	_AppDrop;
		protected AppGroup				_AppData;
		protected bool					_IsAppTitleVisible;


		public MainWindow(MainWorkItem workItem)
		{
			InitializeComponent();

			_FocusElement = AppTypeContent;
			_NativeWindow = new System.Windows.Interop.WindowInteropHelper(this);
			//ContentPanel.Children.Clear();
			//ContentPanel.RowDefinitions.Clear();

			_Controller = new MainWindowController(workItem);
			InitCommands(_Controller.WorkItem.Commands);

			_AppDrop = new SimpleDragDataHandler(
				ButtonListDrag<AppInfo>.DragDataFormat, typeof(AppInfo), AppGroupLoader.Default);
			_AppDrop.ObjectDroped += (s, e) =>
				_Controller.AddApp(
					((s as FrameworkElement).DataContext as AppTypeView).Source,
					e.DropObject as AppInfo);
			

			_AppTypeDrop = new SimpleDragDataHandler(
				AppTypeDrag.DragDataFormat, typeof(AppType), AppGroupLoader.Default);

			_AppTypeDrop.ObjectDroped += (s, e) => 
				_Controller.InsertAppType(
					_AppData,
					e.DropObject as AppType, 
					((s as FrameworkElement).DataContext as AppTypeView).Source);

			_FileDrop.AddFiles += (s, e) => 
				OnDropFiles(s as FrameworkElement, e);

			workItem.Settings.PropertyChanged += Settings_PropertyChanged;

			new BorderResizer(MainContentBorder, 8);
		}


		public void InitData(MainWorkItem workItem)
		{
			_AppData = workItem.AppData;
			DataContext = workItem;
		}

		public void SaveState()
		{
			//AppManager.Properties.Settings.Default.MWindowWidth = ActualWidth;
			//AppManager.Properties.Settings.Default.MWindowHeight = ActualHeight;

			WndSettingsAdapter<AppManagerSettings>.Instance.SaveControlSettings(
				this,
				"MainFormSett",
				AMSetttingsFactory.DefaultSettingsBag
				);

			SaveRowHeight();
		}

		public void LoadState()
		{
			WndSettingsAdapter<AppManagerSettings>.Instance.SetControlSettings(
				this,
				"MainFormSett",
				AMSetttingsFactory.DefaultSettingsBag,
				false
				);

			Topmost = _Controller.WorkItem.Settings.AlwaysOnTop;

			//LoadRowHeight();
			//ContentPanel.InvalidateVisual();
			//UpdateLayout();
			//InvalidateVisual();
		}

		public void SetFocus()
		{
			if (_FocusElement != null && _FocusElement.Items.Count > 0)
			{
				var f = _FocusElement.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
				if (f != null)
				{
					var b = UIHelper.FindVisualChild<ListBoxItem>(f, null);
					if (b != null)
						b.Focus();
				}
			}
		}


		protected void InitCommands(AppCommands commands)
		{
			var mwi = DataContext as MainWorkItem;

			InputBindings.Add(
				new InputBinding(
					commands.Deactivate, 
					new KeyGesture(Key.Escape)
				));

			InputBindings.Add(
				new InputBinding(
					commands.Help,
					new KeyGesture(Key.F1)
				) { CommandParameter = true }
				);

			//InputBindings[0].Command = 

			//ButtonExit.Command = commands.Deactivate;
			//ButtonHelp.Command = commands.Help;
			//ButtonHelp.CommandParameter = true;

			//BtnManageApps.Command = commands.ManageApps;
			//BtnConfigure.Command = commands.Settings;

			//InputBindings[0].Command = commands.Deactivate;
			//InputBindings[1].Command = commands.Help;
			//InputBindings[1].CommandParameter = false;
		}

		protected void InitButtonList(ButtonList groupContent)
		{
			if (groupContent.IsSetUp)
				return;

			groupContent.DragHelper.DragHandlers.Add(_FileDrop);
			groupContent.DragHelper.DragHandlers.Add(_AppTypeDrop);

			groupContent.DragHelper.DragStart += (s, e) => OnDragStarted();
			groupContent.DragHelper.DragEnd += (s, e) => OnDragEnded();
			groupContent.DragHelper.PrepareItem += (s, e) => e.Value = new AppInfoView {
               Source = _Controller.PrepareItem(e.Value as AppInfo),
               ShowTitle = _Controller.WorkItem.Settings.ShowAppTitles,
            };

			groupContent.ButtonClicked += (s, e) => 
				_Controller.WorkItem.Commands.RunApp.Execute(
					new AppStartParams((e.Value as AppInfoView).Source));

			groupContent.CommonMenu = CreateAppTypeContextMenu();
			groupContent.EditMenu = CreateAppContextMenu();

			groupContent.ContextMenuOpening += (s, e) =>
				e.Handled = OnAppListContextMenuOpening(s as ButtonList, e.OriginalSource as FrameworkElement);

			groupContent.IsSetUp = true;
		}

		protected void InitAppTypeGroupBox(ContentControl group)
		{
			if (group == null)
				return;

			group.ContextMenu = CreateAppTypeContextMenu();
			group.ContextMenuOpening += (s, e) =>
				e.Handled = OnAppTypeContextMenuOpening(s as FrameworkElement);

			var drag = new AppTypeDrag(group);
			(drag.DragHandlers[0] as SimpleDragDataHandler).ObjectDroped += (s, e) => 
				_Controller.InsertAppType(
					_AppData, 
					e.DropObject as AppType, 
					((s as FrameworkElement).DataContext as AppTypeView).Source);

			drag.DragHandlers.Add(_AppDrop);
			drag.DragHandlers.Add(_FileDrop);
			
			drag.DragStart += (s, e) => OnDragStarted();
			drag.DragEnd += (s, e) => OnDragEnded();
			drag.DragEnd += (s, e) =>
				OnAppTypeDragEnded(e.DropEffects, e.DropObject as AppType);
		}

		protected ContextMenu CreateAppContextMenu()
		{
			var menu = MenuHelper.CopyMenu(App.Current.Resources["ItemMenu"] as ContextMenu);

			if (Environment.OSVersion.Version.Major < 6)// before Vista
				((MenuItem)menu.Items[1]).Icon = null;


			((MenuItem)menu.Items[0]).Click += (s, ea) =>
				_Controller.WorkItem.Commands.RunApp.Execute(
					new AppStartParams(((s as FrameworkElement).DataContext as AppInfoView).Source)
					);

			((MenuItem)menu.Items[1]).Click += (s, ea) =>
				_Controller.WorkItem.Commands.RunApp.Execute(
					new AppStartParams(((s as FrameworkElement).DataContext as AppInfoView).Source)
						{ RunAs = true }
					);

			((MenuItem)menu.Items[2]).Click += (s, ea) =>
				_Controller.RunAppWithArgs(((s as FrameworkElement).DataContext as AppInfoView).Source);

			((MenuItem)menu.Items[4]).Click += (s, ea) =>
                _Controller.EditItem(((s as FrameworkElement).DataContext as AppInfoView).Source);

			((MenuItem)menu.Items[5]).Click += (s, ea) =>
                _Controller.SetAppInfoImage(((s as FrameworkElement).DataContext as AppInfoView).Source);

			((MenuItem)menu.Items[6]).Click += (s, ea) =>
                _Controller.RefreshItemImage(((s as FrameworkElement).DataContext as AppInfoView).Source);

			((MenuItem)menu.Items[7]).Click += (s, ea) =>
                _Controller.RenameItem(((s as FrameworkElement).DataContext as AppInfoView).Source);

			((MenuItem)menu.Items[8]).Click += (s, ea) =>
				{
					var f = ((s as MenuItem).Parent as ContextMenu).PlacementTarget;
					var gb = UIHelper.FindAncestorOrSelf<BorderedPanel>(f, "AppTypeGroup");
					var at = (gb.DataContext as AppTypeView).Source;
                    _Controller.DeleteAppInfo(at, ((s as FrameworkElement).DataContext as AppInfoView).Source, false);
				};

			((MenuItem)menu.Items[9]).Click += (s, ea) =>
                _Controller.GoToAppFolder(((s as FrameworkElement).DataContext as AppInfoView).Source);

			return menu;
		}

		protected ContextMenu CreateAppTypeContextMenu()
		{
			var menu = MenuHelper.CopyMenu(App.Current.Resources["AppTypeMenu"] as ContextMenu);

			((MenuItem)menu.Items[0]).Click += (s, ea) =>
				_Controller.AddNewApp(((s as FrameworkElement).DataContext as AppTypeView).Source);

			((MenuItem)menu.Items[1]).Click += (s, ea) =>
				_Controller.AddAppType(_AppData, ((s as FrameworkElement).DataContext as AppTypeView).Source);

			((MenuItem)menu.Items[2]).Click += (s, ea) =>
				_Controller.RenameAppType(((s as FrameworkElement).DataContext as AppTypeView).Source);

			((MenuItem)menu.Items[3]).Click += (s, ea) =>
				_Controller.DeleteAppType(_AppData, ((s as FrameworkElement).DataContext as AppTypeView).Source, false);

			return menu;
		}

		protected bool OnAppListContextMenuOpening(ButtonList buttonList, FrameworkElement item)
		{
			if (item == null)
				return false;

			if (buttonList == null)
				return false;

			item = buttonList.ContainerFromElement(item) as FrameworkElement;
			if (item != null)
			{
				var menu = buttonList.EditMenu;
				menu.DataContext = item.DataContext;

				((MenuItem)menu.Items[0]).Header = String.Format(
					Strings.MNU_RUN, ((AppInfoView)item.DataContext).Source);

				if (Environment.OSVersion.Version.Major < 6)// before Vista
					((MenuItem)menu.Items[1]).Header = Strings.MNU_RUNAS;
				else
					((MenuItem)menu.Items[1]).Header = Strings.MNU_RUNAS_ADMIN;

				((MenuItem)menu.Items[2]).Header = Strings.MNU_RUN_WITH_ARGS;

				((MenuItem)menu.Items[4]).Header = Strings.MNU_EDIT;
				((MenuItem)menu.Items[5]).Header = Strings.MNU_CHANGE_ICON;
				((MenuItem)menu.Items[6]).Header = Strings.MNU_REFRESH;
				((MenuItem)menu.Items[7]).Header = Strings.MNU_RENAME;
				((MenuItem)menu.Items[8]).Header = Strings.MNU_DELETE;
				((MenuItem)menu.Items[9]).Header = Strings.MNU_GOTO;

				menu.Placement = PlacementMode.Right;
				menu.PlacementTarget = item;
				menu.IsOpen = true;
			}
			else
			{
				var menu = buttonList.CommonMenu;
				PrepareAppTypeContextMenu(menu, buttonList.DataContext);
				menu.IsOpen = true;
			}

			return true;
		}

		protected bool OnAppTypeContextMenuOpening(FrameworkElement sender)
		{
			if (sender == null)
				return false;

			PrepareAppTypeContextMenu(sender.ContextMenu, sender.DataContext);
			return false;
		}

		protected void PrepareAppTypeContextMenu(ContextMenu menu, object dataContext)
		{
            menu.DataContext = dataContext;
            var appType = ((AppTypeView)dataContext).Source;

            ((MenuItem)menu.Items[2]).Header = String.Format(Strings.MNU_RENAME_APP_TYPE, appType);
            ((MenuItem)menu.Items[3]).Header = String.Format(Strings.MNU_DELETE_TYPE, appType);
		}

		protected void SaveRowHeight()
		{
			var contentGrid = UIHelper.FindVisualChild<Grid>(AppTypeContent, "ContentGrid");

			if (contentGrid == null)
				return;

			double[] h = new double[contentGrid.RowDefinitions.Count];
			int i = 0;
			foreach (var item in contentGrid.RowDefinitions)
				h[i++] = item.Height.Value;

			AMSetttingsFactory.DefaultSettingsBag.Settings.MianFormRowHeights = h;
		}

		protected void LoadRowHeight()
		{
			double[] h = AMSetttingsFactory.DefaultSettingsBag.Settings.MianFormRowHeights;

			if (h == null)
				return;

			var contentGrid = UIHelper.FindVisualChild<Grid>(AppTypeContent, "ContentGrid");
			int i = 0;
			foreach (var item in h)
			{
				if (i >= contentGrid.RowDefinitions.Count)
					break;

				contentGrid.RowDefinitions[i].Height = new GridLength(
					item, GridUnitType.Star);

				i++;
			}
		}

		protected void OnDropFiles(FrameworkElement target, FileDropEventArgs e)
		{
			if (target == null)
				return;

			//File dropped on app, so start it with such argument
			if ((e.KeyState & DragDropKeyStates.AltKey) == DragDropKeyStates.AltKey)
			{
				var input = (target as ItemsControl).InputHitTest(e.DropPoint) as FrameworkElement;
				if (input != null)
				{
					var appInfo = input.DataContext as AppInfoView;
					if (appInfo != null)
						_Controller.WorkItem.Commands.RunApp.Execute(
							new AppStartParams(appInfo.Source) { Args = "\"" + e.Files[0] + "\"" }
							);
				}
			}
			else // add new application
			{
			
				var appType = target.DataContext as AppTypeView;
				if (appType == null)
					return;

				_Controller.AddFiles(appType.Source, e.Files);
			}
		}

		protected void OnDragStarted()
		{
			var anim = Resources["TrashMarkShow"] as Storyboard;
			anim.Begin(TrashMark, HandoffBehavior.Compose, false);
		}

		protected void OnDragEnded()
		{
			var anim = Resources["TrashMarkHide"] as Storyboard;
			anim.Begin(TrashMark, HandoffBehavior.Compose, false);
			//anim.Begin();
		}

		protected void OnAppTypeDragEnded(DragDropEffects effects, AppType appType)
		{
			if ((effects & DragDropEffects.Move) == DragDropEffects.Move)
				_Controller.DeleteAppType(_AppData, appType, true);
		}


		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			//
			// In WPF, using layered windows to make non-rectangular windows incurs a huge performance hit
			// in rendering because the rendering system has to get a device context from GDI, which is
			// currently only supported in software.
			//
			// To avoid this, we use interop to get access to old Win32 functions to programmatically describe
			// what the shape of our window will be. We do this whenever the size changes to make sure the window's
			// shape remains the same.
			//

			// Note: ScaleUI is a class Mike Malinak wrote to account for different DPI settings. I will elaborate
			// more on WPF and DPI in a future blog post

			// First, we create the rounded portion of the title bar area with a call to CreateRoundRectRgn
			IntPtr titleArea = GDI32.CreateRoundRectRgn(
				0,
				0,
				(int)ScaleUI.UpScaleX(sizeInfo.NewSize.Width),
				(int)ScaleUI.UpScaleY(MainContentBorder.ActualHeight),
				12,
				12);

			//IntPtr clientArea = GDI32.CreateRoundRectRgn(
			//   0,
			//   0,
			//   (int)ScaleUI.UpScaleX(sizeInfo.NewSize.Width),
			//   (int)ScaleUI.UpScaleY(MainContentBorder.ActualHeight) + 10,
			//   10,
			//   10);
			//// Next, we create a rectangle for the client area
			//IntPtr clientArea = GDI32.CreateRectRgn(
			//   0, 
			//   (int)ScaleUI.UpScaleY(MainContentBorder.ActualHeight),
			//   (int)ScaleUI.UpScaleX(sizeInfo.NewSize.Width),
			//   (int)ScaleUI.UpScaleY(sizeInfo.NewSize.Height));

			//// After the regions have been created, you have to combine them
			//IntPtr windowArea = GDI32.CreateRectRgn(0, 0, 0, 0);
			//GDI32.CombineRgn(windowArea, titleArea, clientArea, GDI32.CombineRgnStyles.RGN_OR);

			// Last, SetWindowRgn tells GDI what the windows final shape will look like
			User32.SetWindowRgn(_NativeWindow.Handle, titleArea, true);
		}


		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "AlwaysOnTop" ||
                e.PropertyName == "All")
				Topmost = _Controller.WorkItem.Settings.AlwaysOnTop;
		}

		private void CaptionGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
			e.Handled = true;
		}

		private void Resizer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var win = new System.Windows.Interop.WindowInteropHelper(this);

				// This is the familiar SendMessage function. This enables us to resize the window when the user depresses the ResizeGrip control
				// If you use Spy to snoop the messages, it closely matches sizing messages sent when regular windows are resized.
				User32.SendMessage(
					win.Handle,
					WindowMessage.WM_SYSCOMMAND,
					(IntPtr)((int)(SysCommand.SC_SIZE) + (int)SCSizingAction.SouthEast),
					IntPtr.Zero);
			}
		}

		private void TrashMark_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(ButtonListDrag<AppInfo>.DragDataFormat))
				e.Effects = DragDropEffects.Move;

			if (e.Data.GetDataPresent(AppTypeDrag.DragDataFormat))
				e.Effects = DragDropEffects.Move;
		}

		private void TrashMark_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(ButtonListDrag<AppInfo>.DragDataFormat) ||
				 e.Data.GetDataPresent(AppTypeDrag.DragDataFormat))
				e.Effects = DragDropEffects.Move;
			else
				e.Effects = DragDropEffects.None;
		}

		private void ContentPanel_DragOver(object sender, DragEventArgs e)
		{
			if (AppTypeContent.Items.Count <= 0)
				_Controller.AddAppType(_AppData, null);

			e.Effects = DragDropEffects.None;
			e.Handled = true;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LoadRowHeight();

			AppTypeContent.ContextMenu = MenuHelper.CopyMenu(App.Current.Resources["AddAppTypeMenu"] as ContextMenu);
			((MenuItem)AppTypeContent.ContextMenu.Items[0]).Click += (s, ea) =>
				_Controller.AddAppType(_AppData, null);

			//ContentPanel.InvalidateVisual();
			//UpdateLayout();
			//InvalidateVisual();
		}

		private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (!String.IsNullOrEmpty(e.Text))
				if (char.IsLetterOrDigit(e.Text[0]) || char.IsWhiteSpace(e.Text[0]))
				{
					e.Handled = true;
					_Controller.FindApp(_AppData, e.Text);
				}
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			CaptionBorder.Background = (Brush)Resources["ActiveCaptionBrush"];
			//InvalidateVisual();
			//ContentPanel.InvalidateVisual();
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			CaptionBorder.Background = (Brush)Resources["InactiveCaptionBrush"];
		}

		private void Resizer_Initialized(object sender, EventArgs e)
		{
			new GridRowResizer(sender as Control, "ContentGrid", Brushes.Gray);
		}

		private void GroupBox_Initialized(object sender, EventArgs e)
		{
			InitAppTypeGroupBox(sender as ContentControl);
		}

		private void ButtonList_Loaded(object sender, RoutedEventArgs e)
		{
			InitButtonList(sender as ButtonList);
		}
	}

	public class TrashMarkAligner : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (double)value - 64.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
