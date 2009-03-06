using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AppManager.Commands;
using AppManager.Settings;
using CommonLib;
using CommonLib.PInvoke;
using DragDropLib;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		protected MainWindowController	_Controller;
		protected ItemsControl				_FocusElement;
		protected FileDropHandler			_FileDrop = new FileDropHandler();
		protected SimpleDragDataHandler	_AppTypeDrop;
		protected SimpleDragDataHandler	_AppDrop;


		public MainWindow(MainWorkItem workItem)
		{
			InitializeComponent();

			//ContentPanel.Children.Clear();
			//ContentPanel.RowDefinitions.Clear();

			_Controller = new MainWindowController(workItem);
			InitCommands(_Controller.WorkItem.Commands);

			_AppDrop = new SimpleDragDataHandler(
				ButtonListDrag.DragDataFormat, typeof(AppInfo));
			_AppDrop.ObjectDroped += (s, e) =>
				_Controller.AddApp(
					(s as FrameworkElement).DataContext as AppType,
					e.DropObject as AppInfo);
			

			_AppTypeDrop = new SimpleDragDataHandler(
				AppTypeDrag.DragDataFormat, typeof(AppType));

			_AppTypeDrop.ObjectDroped += (s, e) => 
				_Controller.InsertAppType(
					e.DropObject as AppType, 
					(s as FrameworkElement).DataContext as AppType);


			_FileDrop.AddFiles += (s, e) => 
				OnDropFiles(s as FrameworkElement, e);

			workItem.Settings.PropertyChanged += Settings_PropertyChanged;
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
					f.Focus();
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
			groupContent.DragHelper.DragHandlers.Add(_FileDrop);
			groupContent.DragHelper.DragHandlers.Add(_AppTypeDrop);

			groupContent.DragHelper.DragStart += (s, e) => OnDragStarted();
			groupContent.DragHelper.DragEnd += (s, e) => OnDragEnded();
			groupContent.DragHelper.PrepareItem += (s, e) => _Controller.PrepareItem(e.Value as AppInfo);

			groupContent.ButtonClicked += (s, e) => _Controller.WorkItem.Commands.RunApp.Execute(e.Value);

			groupContent.CommonMenu = CreateAppTypeContextMenu();
			groupContent.EditMenu = CreateAppContextMenu();

			groupContent.ContextMenuOpening += (s, e) =>
				e.Handled = OnAppListContextMenuOpening(s as ButtonList, e.OriginalSource as FrameworkElement);
		}

		protected void InitAppTypeGroupBox(GroupBox group)
		{
			group.ContextMenu = CreateAppTypeContextMenu();
			group.ContextMenuOpening += (s, e) =>
				e.Handled = OnAppTypeContextMenuOpening(s as FrameworkElement);

			var drag = new AppTypeDrag(group);
			(drag.DragHandlers[0] as SimpleDragDataHandler).ObjectDroped +=
				(s, e) => _Controller.InsertAppType(e.DropObject as AppType, (s as FrameworkElement).DataContext as AppType);

			drag.DragHandlers.Add(_FileDrop);
			drag.DragHandlers.Add(_AppDrop);

			drag.DragStart += (s, e) => OnDragStarted();
			drag.DragEnd += (s, e) => OnDragEnded();
			drag.DragEnd += (s, e) => OnAppTypeDragEnded(e.DropEffects, e.DropObject as AppType);
		}

		//protected ButtonList CreateButtonList(int rowi, AppType appType)
		//{
		//   //ButtonList groupContent = new ButtonList()
		//   //{
		//   //   TabIndex = rowi,
		//   //   AllowDrop = true,
		//   //   SnapsToDevicePixels = true
		//   //};
			
		//   //groupContent.CommonMenu = CreateAppTypeContextMenu();
		//   //groupContent.EditMenu = CreateAppContextMenu();
		//   //groupContent.ContextMenuOpening += (s, e) =>
		//   //   e.Handled = OnAppListContextMenuOpening(s as ButtonList, e.OriginalSource as FrameworkElement);

		//   //groupContent.SetBinding(ButtonList.ItemsSourceProperty, "AppInfos");
		//   //groupContent.DataContext = appType;

		//   //return groupContent;

		//   return null;
		//}

		protected ContextMenu CreateAppContextMenu()
		{
			var menu = MenuHelper.CopyMenu(App.Current.Resources["ItemMenu"] as ContextMenu);

			((MenuItem)menu.Items[0]).Click += (s, ea) =>
				_Controller.EditItem((s as FrameworkElement).DataContext as AppInfo);

			((MenuItem)menu.Items[1]).Click += (s, ea) =>
				_Controller.RenameItem((s as FrameworkElement).DataContext as AppInfo);

			((MenuItem)menu.Items[2]).Click += (s, ea) =>
				_Controller.DeleteItem((s as FrameworkElement).DataContext as AppInfo);

			((MenuItem)menu.Items[3]).Click += (s, ea) =>
				_Controller.GoToAppFolder((s as FrameworkElement).DataContext as AppInfo);

			return menu;
		}

		protected ContextMenu CreateAppTypeContextMenu()
		{
			var menu = MenuHelper.CopyMenu(App.Current.Resources["AppTypeMenu"] as ContextMenu);

			((MenuItem)menu.Items[0]).Click += (s, ea) =>
				_Controller.AddNewApp((s as FrameworkElement).DataContext as AppType);

			((MenuItem)menu.Items[1]).Click += (s, ea) =>
				_Controller.DeleteAppType((s as FrameworkElement).DataContext as AppType);

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

				((MenuItem)menu.Items[0]).Header = String.Format(Strings.MNU_EDIT, item.DataContext);
				((MenuItem)menu.Items[1]).Header = String.Format(Strings.MNU_RENAME, item.DataContext);
				((MenuItem)menu.Items[2]).Header = String.Format(Strings.MNU_DELETE, item.DataContext);
				((MenuItem)menu.Items[3]).Header = String.Format(Strings.MNU_GOTO, item.DataContext);

				menu.Placement = PlacementMode.Right;
				menu.PlacementTarget = item;
				menu.IsOpen = true;
			}
			else
			{
				var menu = buttonList.CommonMenu;
				menu.DataContext = buttonList.DataContext;

				((MenuItem)menu.Items[0]).Header = Strings.MNU_ADD_APP;
				((MenuItem)menu.Items[1]).Header = String.Format(Strings.MNU_DELETE_TYPE, buttonList.DataContext);

				//menu.Placement = PlacementMode.;
				//menu.PlacementTarget = item;
				menu.IsOpen = true;
			}

			return true;
		}

		//protected GroupBox CreateGroupBox(object content, AppType appType)
		//{
		//   GroupBox group = new GroupBox()
		//   {
		//      Margin = new Thickness(7.0),
		//      SnapsToDevicePixels = true,
		//      Content = content,
		//      Foreground = Brushes.Blue,
		//      Style = Resources["CustomGB"] as Style,
		//      AllowDrop = true
		//   };

		//   group.ContextMenu = CreateAppTypeContextMenu();
		//   group.ContextMenuOpening += (s, e) => 
		//      e.Handled = OnAppTypeContextMenuOpening(s as FrameworkElement);
		//   group.SetBinding(GroupBox.HeaderProperty, "AppTypeName");
		//   group.DataContext = appType;

		//   var drag = new AppTypeDrag(group);
		//   (drag.DragHandlers[0] as SimpleDragDataHandler).ObjectDroped +=
		//      (s, e) => _Controller.InsertAppType(e.DropObject as AppType, (s as FrameworkElement).DataContext as AppType);
		//   drag.DragHandlers.Add(_FileDrop);
		//   drag.DragStart += (s, e) => OnDragStarted();
		//   drag.DragEnd += (s, e) => OnDragEnded();
		//   drag.DragEnd += (s, e) => OnAppTypeDragEnded(e.DropEffects, e.DropObject as AppType);

		//   return group;
		//}

		protected bool OnAppTypeContextMenuOpening(FrameworkElement sender)
		{
			if (sender == null)
				return false;

			var menu = sender.ContextMenu;
			menu.DataContext = sender.DataContext;

			((MenuItem)menu.Items[0]).Header = Strings.MNU_ADD_APP;
			((MenuItem)menu.Items[1]).Header = String.Format(Strings.MNU_DELETE_TYPE, sender.DataContext);

			return false;
		}
		
		//protected GridSplitter CreateGridSplitter(int rowi)
		//{
		//   var split = new GridSplitter()
		//   {
		//      ResizeDirection = GridResizeDirection.Rows,
		//      Height = 3,
		//      VerticalAlignment = VerticalAlignment.Top,
		//      HorizontalAlignment = HorizontalAlignment.Stretch,
		//      ShowsPreview = true,
		//      Background = Brushes.Transparent
		//   };

		//   split.DragCompleted += (s, e) => SaveRowHeight();
		//   Grid.SetRow(split, rowi - 1);

		//   return split;
		//}

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

		protected void OnDropFiles(FrameworkElement target, ValueEventArgs<string[]> e)
		{
			if (target == null)
				return;

			var appType = target.DataContext as AppType;
				
			if (appType == null)
				return;

			_Controller.AddFiles(appType, e.Value);
		}

		protected void OnDragStarted()
		{
			var anim = Resources["TrashMarkShow"] as Storyboard;
			anim.Begin();
		}

		protected void OnDragEnded()
		{
			var anim = Resources["TrashMarkHide"] as Storyboard;
			anim.Begin();
		}

		protected void OnAppTypeDragEnded(DragDropEffects effects, AppType appType)
		{
			if ((effects & DragDropEffects.Move) == DragDropEffects.Move)
				_Controller.RemoveAppType(appType);
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
			var win = new System.Windows.Interop.WindowInteropHelper(this);
			User32.SetWindowRgn(win.Handle, titleArea, true);
		}


		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "AlwaysOnTop")
				Topmost = _Controller.WorkItem.Settings.AlwaysOnTop;
		}

		private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
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
					User32.WindowMessage.WM_SYSCOMMAND,
					(IntPtr)((int)(User32.SysCommand.SC_SIZE) + (int)User32.SCSizingAction.SouthEast),
					IntPtr.Zero);
			}
		}

		private void TrashMark_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(ButtonListDrag.DragDataFormat))
				e.Effects = DragDropEffects.Move;

			if (e.Data.GetDataPresent(AppTypeDrag.DragDataFormat))
				e.Effects = DragDropEffects.Move;
		}

		private void TrashMark_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(ButtonListDrag.DragDataFormat))
				e.Effects = DragDropEffects.Move;

			if (e.Data.GetDataPresent(AppTypeDrag.DragDataFormat))
				e.Effects = DragDropEffects.Move;
		}

		private void ContentPanel_DragOver(object sender, DragEventArgs e)
		{
			if (AppTypeContent.Items.Count <= 0)
				_Controller.CreateDefaultType();

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
					_Controller.FindApp(e.Text);
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
			new Resizer(sender as Control, "ContentGrid", Brushes.Gray);
		}

		private void GroupBox_Initialized(object sender, EventArgs e)
		{
			InitAppTypeGroupBox(sender as GroupBox);
		}

		private void ButtonList_Loaded(object sender, RoutedEventArgs e)
		{
			InitButtonList(sender as ButtonList);
		}
	}

	public class TrashMarkAlighner : IValueConverter
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
