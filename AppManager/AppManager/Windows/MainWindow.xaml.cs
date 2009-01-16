using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AppManager.Common;
using AppManager.Settings;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		protected MainWindowController _Controller;


		public MainWindow()
		{
			InitializeComponent();
		}


		public void Init(MainWorkItem workItem)
		{
			_Controller = new MainWindowController(workItem);

			ButtonExit.Command = workItem.Commands.Deactivate;
			BtnManageApps.Command = workItem.Commands.ManageApps;
			BtnConfigure.Command = workItem.Commands.Settings;

			InputBindings[0].Command = workItem.Commands.Deactivate;

			ContentPanel.Children.Clear();
			ContentPanel.RowDefinitions.Clear();

			int rowi = 0;
			foreach (var appType in workItem.AppData.AppTypes)
			{
				RowDefinition row = new RowDefinition()
					{ Height = new GridLength(100.0, GridUnitType.Star) };
				ContentPanel.RowDefinitions.Add(row);
				
				ButtonList groupContent = CreateButtonList(workItem, rowi, appType);
				GroupBox group = CreateGroupBox(groupContent, appType);

				ContentPanel.Children.Add(group);
				Grid.SetRow(group, rowi++);

				if (rowi > 1)
				{
					GridSplitter split = new GridSplitter();
					split.ResizeDirection = GridResizeDirection.Rows;
					split.Height = 3;
					split.VerticalAlignment = VerticalAlignment.Top;
					split.HorizontalAlignment = HorizontalAlignment.Stretch;
					split.ShowsPreview = true;
					split.Background = Brushes.Transparent;
					split.DragCompleted += (s, e) => SaveRowHeight();
					Grid.SetRow(split, rowi - 1);
					ContentPanel.Children.Add(split);
				}
			}

			LoadRowHeight();
			//UpdateLayout();
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

			LoadRowHeight();
			UpdateLayout();
		}


		protected ButtonList CreateButtonList(MainWorkItem workItem, int rowi, AppType appType)
		{
			ButtonList groupContent = new ButtonList()
			{
				TabIndex = rowi,
				AllowDrop = true,
				SnapsToDevicePixels = true
			};

			groupContent.Drop += (s, e) => OnDropFiles(s as ButtonList, e);
			groupContent.ButtonClicked += (s, e) => workItem.Commands.RunApp.Execute(e.Obj);

			groupContent.SetBinding(ButtonList.ItemsSourceProperty, "AppInfos");
			groupContent.DataContext = appType;

			return groupContent;
		}

		protected GroupBox CreateGroupBox(object content, AppType appType)
		{
			GroupBox group = new GroupBox()
			{
				Margin = new Thickness(7.0),
				SnapsToDevicePixels = true,
				Content = content
			};

			group.SetBinding(GroupBox.HeaderProperty, "AppTypeName");
			group.DataContext = appType;
			
			return group;
		}

		protected void SaveRowHeight()
		{
			double[] h = new double[ContentPanel.RowDefinitions.Count];
			int i = 0;
			foreach (var item in ContentPanel.RowDefinitions)
				h[i++] = item.Height.Value;

			AMSetttingsFactory.DefaultSettingsBag.Settings.MianFormRowHeights = h;
		}

		protected void LoadRowHeight()
		{
			double[] h = AMSetttingsFactory.DefaultSettingsBag.Settings.MianFormRowHeights;

			if (h == null)
				return;

			int i = 0;
			foreach (var item in h)
			{
				if (i >= ContentPanel.RowDefinitions.Count)
					break;

				ContentPanel.RowDefinitions[i].Height = new GridLength(
					item, GridUnitType.Star);

				i++;
			}
		}

		protected void OnDropFiles(ButtonList buttonList, DragEventArgs e)
		{ 
			if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
			{
				string[] files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
				_Controller.AddFiles(buttonList.DataContext as AppType, files);

				e.Handled = true;
			}			
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

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateLayout();
			InvalidateVisual();
		}
	}
}
