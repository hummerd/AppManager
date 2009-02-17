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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;


namespace CommonLib.Windows
{
	/// <summary>
	/// Interaction logic for DialogWindowSurface.xaml
	/// </summary>
	[ContentProperty("ContentPanel")]
	public partial class DialogWindowSurface : UserControl
	{
		protected Window _HostWnd;


		public DialogWindowSurface()
		{
			InitializeComponent();

			_HostWnd = Parent as Window;
			if (_HostWnd != null)
				_HostWnd.PreviewKeyUp += Window_PreviewKeyUp;
		}


		public object ContentPanel
		{
			get { return SurfaceContent.Content; }
			set { SurfaceContent.Content = value; }
		}


		private void BtnOk_Click(object sender, RoutedEventArgs e)
		{
			_HostWnd.DialogResult = true;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			_HostWnd.DialogResult = true;
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				_HostWnd.DialogResult = true;
			}

			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				_HostWnd.DialogResult = false;
			}
		}
	}
}
