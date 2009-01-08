using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DragDropLib;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ButtonList : ListBox
	{
		public event EventHandler<ObjEventArgs> ButtonClicked;


		protected DragHelper _DragHelper;
		

		public ButtonList()
		{
			this.InitializeComponent();

			ItemContainerStyle = new Style();
			ItemContainerStyle.Resources[SystemColors.HighlightBrushKey] = Brushes.Transparent;
			ItemContainerStyle.Resources[SystemColors.ControlBrushKey] = Brushes.Transparent;

			_DragHelper = new DragHelper(this, "ButtonListDataFormat", typeof(AppInfo));
		}


		private void ImageButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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