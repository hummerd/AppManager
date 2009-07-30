using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AppManager.DragDrop;
using CommonLib;
using CommonLib.UI;
using CommonLib.PInvoke;


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

			_DragHelper = new ButtonListDrag(this, typeof(AppInfo));
			_DragHelper.DragOver += (s, e) => OnDragHelperDragOver(e);
			_DragHelper.DragLeave += (s, e) => ResetLastMove(e, false);
			_DragHelper.DragDroped += (s, e) => ResetLastMove(e, true);
			_DragHelper.NeedTargetObject += (s, e) => e.Value = GetLastItems();
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
			if (!_LastRect.IsEmpty && _LastRect.Contains(pt))
				return;

			var input = InputHitTest(pt) as FrameworkElement;
			if (input != null)
			{
				var ib = UIHelper.FindAncestorOrSelf<ImageButton>(input, null);
				if (ib != null && _LastMoved != ib)
				{
					ResetLastMove(e, true);
					_LastMoved = ib;
					_LastRect = new Rect(ib.TranslatePoint(new Point(0, 0), this), ib.RenderSize);
					Debug.WriteLine("rect set " + _LastRect);
					new MoveAnimation(ib);
				}
				else if (ib == null)
					ResetLastMove(e, true);
			}
		}

		protected void ResetLastMove(DragEventArgs e, bool anyWay)
		{
			Debug.WriteLine("reset " + e.Source);
			Debug.WriteLine("reset " + e.OriginalSource);

			if (e.GetPosition((IInputElement)e.OriginalSource) == new Point(0, 0))
				anyWay = true;

			Point pt = User32.GetCursorPos(this);
			var wnd = UIHelper.FindAncestorOrSelf<Window>(this, null);
			var wndi = wnd.InputHitTest(User32.GetCursorPos(wnd));
			var input = InputHitTest(pt) as FrameworkElement;

			if (wndi == input && !anyWay && !_LastRect.IsEmpty && _LastRect.Contains(pt))
				return;
			
			if (_LastMoved != null)
				_LastMoved.RenderTransform.BeginAnimation(TranslateTransform.XProperty, null);

			_LastRect = Rect.Empty;
			_LastMoved = null;
		}

		protected object GetLastItems()
		{
			return _LastMoved;
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