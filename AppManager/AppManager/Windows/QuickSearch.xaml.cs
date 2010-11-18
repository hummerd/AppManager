using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CommonLib.PInvoke;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for QuickSearch.xaml
	/// </summary>
	public partial class QuickSearch : Window
	{
		public event EventHandler SerachStringChanged;
		public event EventHandler ItemSelected;


		protected DispatcherTimer _SearchTimer = new DispatcherTimer();


		public QuickSearch()
		{
			InitializeComponent();

			_SearchTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
			_SearchTimer.Tick += (s, e) => 
			{ 
				_SearchTimer.Stop(); 
				OnSearchStringChanged(); 
			};
		}


		public string SearchString
		{
			get
			{ 
				return TxtSearch.Text;  
			}
			set
			{
				TxtSearch.Text = value;
			}
		}

		public IEnumerable FoundItems
		{
			get
			{
				return LstApp.ItemsSource;
			}
			set
			{
				LstApp.ItemsSource = value;
				if (LstApp.Items.Count > 0)
					LstApp.SelectedIndex = 0;
			}
		}

		public object SelectedItem
		{
			get
			{
				return LstApp.SelectedItem;
			}
		}

		
		protected void OnSearchStringChanged()
		{
			if (SerachStringChanged != null)
				SerachStringChanged(this, EventArgs.Empty);
		}

		protected void OnItemSelected()
		{
			if (ItemSelected != null)
				ItemSelected(this, EventArgs.Empty);
		}

		protected void SelectItem(bool first)
		{ 
			if (LstApp.Items.Count > 0)
			{
				LstApp.SelectedIndex = first && LstApp.Items.Count > 1 ? 1 : LstApp.Items.Count - 1;
				LstApp.ScrollIntoView(LstApp.SelectedItem);

				var c = LstApp.ItemContainerGenerator.ContainerFromItem(LstApp.SelectedItem) as ListBoxItem;
				c.IsSelected = true;
				c.Focus();
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


		private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			_SearchTimer.Start();
			//OnSearchStringChanged();
		}

		private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Down || e.Key == Key.Up)
			{
				SelectItem(e.Key != Key.Up);
				e.Handled = true;
			}
		}

		private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (e.OriginalSource != TxtSearch)
			{
				if ((char.IsLetterOrDigit(e.Text[0]) || char.IsWhiteSpace(e.Text[0])) &&
					 e.Text != "\r")
				{
					TxtSearch.Text = TxtSearch.Text + e.Text;
					TxtSearch.Focus();
					TxtSearch.SelectionStart = TxtSearch.Text.Length;
					e.Handled = true;
				}
			}
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			try
			{
				Close();
			}
			catch
			{ ; }
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				OnItemSelected();
			}

			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				Close();
			}
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			MainContent.InvalidateVisual();

			TxtSearch.Focus();
			TxtSearch.CaretIndex = TxtSearch.Text.Length;
		}

		private void LstApp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			OnItemSelected();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			_SearchTimer.Stop(); 
		}
	}
}
