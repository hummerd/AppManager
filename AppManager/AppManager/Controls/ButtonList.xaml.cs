using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppManager.DragDrop;
using CommonLib;
using CommonLib.UI;
using System.Windows.Media;
using System.Diagnostics;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ButtonList : ListBox
	{
		public event EventHandler<ValueEventArgs<object>> ButtonClicked;
		
		protected ButtonListDrag _DragHelper;

		protected ContextMenu _EditMenu;
		protected ContextMenu _CommonMenu;
		protected FrameworkElement _LastMoved = null;
		protected Rect _LastRect;


		public ButtonList()
		{
			this.InitializeComponent();

			//ItemContainerStyle = new Style();
			//ItemContainerStyle.Resources[SystemColors.HighlightBrushKey] = Brushes.Transparent;
			//ItemContainerStyle.Resources[SystemColors.ControlBrushKey] = Brushes.Transparent;

			_DragHelper = new ButtonListDrag(this, typeof(AppInfo));
			_DragHelper.DragOver += (s, e) => OnDragHelperDragOver(e);
			_DragHelper.DragLeave += (s, e) => ResetLastMove();
		}


		public ContextMenu EditMenu
		{
			get { return _EditMenu; }
			set { _EditMenu = value; }
		}

		public ContextMenu CommonMenu
		{
			get { return _CommonMenu; }
			set { _CommonMenu = value; }
		}

		public ButtonListDrag DragHelper
		{
			get
			{
				return _DragHelper;
			}
		}


		protected void OnDragHelperDragOver(DragEventArgs e)
		{
			Point pt = e.GetPosition(this);
			if (!_LastRect.IsEmpty)
			{
				if (_LastRect.Contains(pt))
				{
					Debug.WriteLine("nc");
					return;
				}
				//_LastMoved.TranslatePoint(new Point(0, 0), this);
			}

			Debug.WriteLine("passed " + pt);
			Debug.WriteLine("passed rect " + _LastRect);
			Debug.WriteLine("passed");

			var input = InputHitTest(pt) as FrameworkElement;
			if (input != null)
			{
				var ib = UIHelper.FindAncestorOrSelf<ImageButton>(input, null);
				if (ib != null && _LastMoved != ib)
				{
					ResetLastMove();
					_LastMoved = ib;
					_LastRect = new Rect(ib.TranslatePoint(new Point(0, 0), this), ib.RenderSize);
					Debug.WriteLine("rect set " + _LastRect);
					new MoveAnimation(ib);
				}
				else if (ib == null)
					ResetLastMove();
			}
		}

		protected void ResetLastMove()
		{
			if (_LastMoved != null)
				_LastMoved.RenderTransform.BeginAnimation(TranslateTransform.XProperty, null);

			_LastRect = Rect.Empty;
			_LastMoved = null;
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
					ButtonClicked(this, new ValueEventArgs<object>(lbi.DataContext));
			}
		}

		private void ImageButton_Click(object sender, RoutedEventArgs e)
		{
			ImageButton ib = sender as ImageButton;
			if (ButtonClicked != null)
				ButtonClicked(this, new ValueEventArgs<object>(ib.DataContext));
		}
	}
}