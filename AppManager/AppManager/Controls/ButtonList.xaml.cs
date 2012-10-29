using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using AppManager.DragDrop;
using AppManager.Entities;
using CommonLib;
using CommonLib.PInvoke;
using CommonLib.UI;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Controls.Primitives;
using AppManager.Classes.ViewModel;
using System.Windows.Threading;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ButtonList : ListBox
	{
		public event EventHandler<ValueEventArgs<object>> ButtonClicked;

		protected ButtonListDrag<AppInfo> _DragHelper;

		protected ContextMenu _EditMenu;
		protected ContextMenu _CommonMenu;
		protected FrameworkElement _LastMoved = null;
		protected Rect _LastRect;
		protected bool _IsAppTitleVisible;
        protected DispatcherTimer _HoverTimer;
        protected DateTime _LastDragOver;


		public ButtonList()
		{
			IsSetUp = false;

			this.InitializeComponent();

			_DragHelper = new ButtonListDrag<AppInfo>(this);
			_DragHelper.DragOver += (s, e) => OnDragHelperDragOver(e);
			//_DragHelper.DragLeave += (s, e) => ResetLastMove(e, false);
			_DragHelper.DragDroped += (s, e) => ResetLastMove();
			_DragHelper.NeedTargetObject += (s, e) => e.Value = GetLastItems();

            _HoverTimer = new DispatcherTimer();
            _HoverTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _HoverTimer.Tick += (s, e) => CheckDrag();
		}


		public bool IsSetUp
		{ get; set; }

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

		public ButtonListDrag<AppInfo> DragHelper
		{
			get
			{
				return _DragHelper;
			}
		}


        protected void CheckDrag()
        {
            var pt = User32.GetCursorPos(this);
            //System.Diagnostics.Debug.WriteLine("timer " + pt);
            bool ended = (DateTime.Now - _LastDragOver).TotalMilliseconds > 400;
            
            if (!_LastRect.IsEmpty && _LastRect.Contains(pt) && !ended)
                return;
                        
			var input = InputHitTest(pt) as FrameworkElement;
            if (input == null || ended)
            {
                ResetLastMove();
                _HoverTimer.Stop();
            }
            else
            {
                var ib = UIHelper.FindAncestorOrSelf<AppButton>(input, null);
                if (ib == null)
                {
                    ResetLastMove();
                    _HoverTimer.Stop();
                }
            }
        }

		protected void OnDragHelperDragOver(DragEventArgs e)
		{
            _LastDragOver = DateTime.Now;
			var pt = e.GetPosition(this);
            //System.Diagnostics.Debug.WriteLine("rect set " + pt);

			if (!_LastRect.IsEmpty && _LastRect.Contains(pt))
				return;

			var input = InputHitTest(pt) as FrameworkElement;
			if (input != null)
			{
				var ib = UIHelper.FindAncestorOrSelf<AppButton>(input, null);
				if (ib != null && _LastMoved != ib)
				{
					ResetLastMove();
					_LastMoved = ib;
					_LastRect = new Rect(ib.TranslatePoint(new Point(0, 0), this), ib.RenderSize);
					//System.Diagnostics.Debug.WriteLine("rect set " + _LastRect);
					new MoveAnimation(ib);
                    _HoverTimer.Start();
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
			AppButton ib = sender as AppButton;
			if (ButtonClicked != null)
				ButtonClicked(this, new ValueEventArgs<object>(ib.DataContext));
		}
	}
}