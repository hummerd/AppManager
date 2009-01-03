using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DragDropLib;
using DataObject = System.Windows.DataObject;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ButtonList : ListBox
	{
		protected string	_DraggingElement = null;
		protected bool		_IsDown = false;
		protected Point	_DragStartPoint;


		public event EventHandler<ObjEventArgs> ButtonClicked;


		public ButtonList()
		{
			this.InitializeComponent();

			ItemContainerStyle = new Style();
			ItemContainerStyle.Resources[SystemColors.HighlightBrushKey] = Brushes.Transparent;
			ItemContainerStyle.Resources[SystemColors.ControlBrushKey] = Brushes.Transparent;
		}


		//private void ImageButton_Click(object sender, RoutedEventArgs e)
		//{
		//   ImageButton ib = sender as ImageButton;
		//   if (ButtonClicked != null)
		//      ButtonClicked(this, new ObjEventArgs() { Obj = ib.DataContext });
		//}

		private void ImageButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//ListBox listbox = (ListBox)sender;
			//string s = GetBoundItemFromPoint(listbox, e.GetPosition(listbox));
			if (1 == 1)
			{
				_IsDown = true;
				//SetValue(DraggingElementProperty, s);
				Button btn = sender as Button;
				_DragStartPoint = e.GetPosition(btn);
			}
		}

		private void ImageButton_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (!_IsDown)
				return;

			Button btn = sender as Button;

			if (Math.Abs(e.GetPosition(btn).X - _DragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
				 Math.Abs(e.GetPosition(btn).Y - _DragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				DragStarted(btn);
			}
		}

		private void ImageButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_IsDown = false;
			_DragStartPoint = default(Point);

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


		protected override void OnDragEnter(DragEventArgs e)
		{
			DragSourceHelper.OnDragEnter(e, this);
		}

		protected override void OnDragLeave(DragEventArgs e)
		{
			DragSourceHelper.OnDragLeave(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			DragSourceHelper.OnDragOver(e, this);
		}

		protected override void OnDrop(DragEventArgs e)
		{
			_IsDown = false;
			_DragStartPoint = default(Point);
			DragSourceHelper.OnDrop(e, this);
		}


		private void DragStarted(FrameworkElement element)
		{
			DataObject data = DragSourceHelper.CreateFromElement(element, _DragStartPoint);
			DragDrop.DoDragDrop(element, data, DragDropEffects.Copy);
		}
	}


	public class ObjEventArgs : EventArgs
	{
		public object Obj { get; set; }
	}
}