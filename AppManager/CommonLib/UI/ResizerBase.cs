using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;


namespace CommonLib.UI
{
	public class ResizerBase
	{
		protected FrameworkElement	_Resizer;
		protected IInputElement		_Relative;
		protected bool				_DoResize = false;

		protected double _AccY;
		protected double _AccX;

		protected Cursor _OriginalCursor;


		public ResizerBase(FrameworkElement resizer)
		{
			_Resizer = resizer;
			_Relative = resizer;

			resizer.MouseEnter += new MouseEventHandler(resizer_MouseEnter);
			resizer.MouseDown += new MouseButtonEventHandler(resizer_MouseDown);
			resizer.MouseUp += new MouseButtonEventHandler(resizer_MouseUp);
			resizer.MouseMove += new MouseEventHandler(resizer_MouseMove);
			resizer.MouseLeave += new MouseEventHandler(resizer_MouseLeave);
		}
		

		protected virtual void PrepareResize(Point pos)
		{
			_Resizer.CaptureMouse();
			_DoResize = true;
			_AccY = _Resizer.ActualHeight - pos.Y;
			_AccX = _Resizer.ActualWidth - pos.X;
		}

		protected virtual void PrepareCursor(Point pos)
		{

		}

		protected virtual void DoResize(Point pos)
		{
			
		}

		protected virtual void EndResize()
		{
			_Resizer.ReleaseMouseCapture();
			_DoResize = false;
		}


		private void resizer_MouseLeave(object sender, MouseEventArgs e)
		{
			_Resizer.Cursor = _OriginalCursor;
		}

		private void resizer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Source == _Resizer)
				PrepareResize(e.GetPosition(_Resizer));
		}

		private void resizer_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Source == _Resizer)
			{
				PrepareCursor(e.GetPosition(_Relative));
			}
			if (_DoResize && e.Source == _Resizer)
			{
				DoResize(e.GetPosition(_Relative));
				e.Handled = true;
			}
		}
		
		private void resizer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			EndResize();
		}

		private void resizer_MouseEnter(object sender, MouseEventArgs e)
		{
			_OriginalCursor = _Resizer.Cursor;
		}
	}
}
