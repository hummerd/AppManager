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

namespace AppManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ButtonList : ListBox
	{
		public event EventHandler<ObjEventArgs> ButtonClicked;


		public ButtonList()
		{
			this.InitializeComponent();

			ItemContainerStyle = new Style();
			ItemContainerStyle.Resources[SystemColors.HighlightBrushKey] = Brushes.Transparent;
			ItemContainerStyle.Resources[SystemColors.ControlBrushKey] = Brushes.Transparent;
		}


		private void ImageButton_Click(object sender, RoutedEventArgs e)
		{
			ImageButton ib = sender as ImageButton;
			if (ButtonClicked != null)
				ButtonClicked(this, new ObjEventArgs() { Obj = ib.DataContext });
		}

		private void ButtonList_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (ButtonClicked != null &&
				 (e.Key == Key.Enter ||
				  e.Key == Key.Space)
				)
			{
				var lbi = Keyboard.FocusedElement as ListBoxItem;
				if (lbi != null && lbi.DataContext != null)
					ButtonClicked(this, new ObjEventArgs() { Obj = lbi.DataContext });
			}	
		}
	}


	public class ObjEventArgs : EventArgs
	{
		public object Obj { get; set; }
	}
}