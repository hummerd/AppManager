using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;


namespace CommonLib.UI
{
	public class BorderResizer : ResizerBase
	{
		[Flags]
		protected enum ResizeDirect
		{ 
			Unknown = 0,
			Left = 1,
			Top = 2,
			Right = 4,
			Bottom = 8
		}


		protected Window			_Target;
		protected int				_Corner;
		protected ResizeDirect	_ResizeDir = ResizeDirect.Unknown;


		public BorderResizer(FrameworkElement resizer, int corner)
			: base(resizer)
		{
			_Target = UIHelper.FindLogicalAncestorOrSelf<Window>(_Resizer, null);
			_Corner = corner;
		}


		protected override void PrepareCursor(Point pos)
		{
			double xr = _Resizer.ActualWidth - _Corner;
			double yb = _Resizer.ActualHeight - _Corner;

			if (pos.X < _Corner && pos.Y < _Corner) //lt
			{
				_Resizer.Cursor = Cursors.SizeNWSE;
			}
			else if (pos.X < _Corner && pos.Y > _Corner && pos.Y < yb) //l
			{
				_Resizer.Cursor = Cursors.SizeWE;
			}
			else if (pos.X < _Corner && pos.Y > yb) //lb
			{
				_Resizer.Cursor = Cursors.SizeNESW;
			}
			else if (pos.X > _Corner && pos.X < xr && pos.Y > yb) //b
			{
				_Resizer.Cursor = Cursors.SizeNS;
			}
			else if (pos.X > xr && pos.Y > yb) //rb
			{
				_Resizer.Cursor = Cursors.SizeNWSE;
			}
			else if (pos.X > xr && pos.Y > _Corner && pos.Y < yb)//r
			{
				_Resizer.Cursor = Cursors.SizeWE;
			}
			else if (pos.X > xr && pos.Y < _Corner)//rt
			{
				_Resizer.Cursor = Cursors.SizeNESW;
			}
			else if (pos.X > _Corner && pos.X < xr && pos.Y < _Corner)//t
			{
				_Resizer.Cursor = Cursors.SizeNS;
			}
		}

		protected override void PrepareResize(Point pos)
		{
			base.PrepareResize(pos);

			double xr = _Resizer.ActualWidth - _Corner;
			double yb = _Resizer.ActualHeight - _Corner;

			if (pos.X < _Corner && pos.Y < _Corner) //lt
			{
				_ResizeDir = ResizeDirect.Left | ResizeDirect.Top;
			}
			else if (pos.X < _Corner && pos.Y > _Corner && pos.Y < yb) //l
			{
				_ResizeDir = ResizeDirect.Left;
			}
			else if (pos.X < _Corner && pos.Y > yb) //lb
			{
				_ResizeDir = ResizeDirect.Left | ResizeDirect.Bottom;
			}
			else if (pos.X > _Corner && pos.X < xr && pos.Y > yb) //b
			{
				_ResizeDir = ResizeDirect.Bottom;
			}
			else if (pos.X > xr && pos.Y > yb) //rb
			{
				_ResizeDir = ResizeDirect.Right | ResizeDirect.Bottom;
			}
			else if (pos.X > xr && pos.Y > _Corner && pos.Y < yb)//r
			{
				_ResizeDir = ResizeDirect.Right;
			}
			else if (pos.X > xr && pos.Y < _Corner)//rt
			{
				_ResizeDir = ResizeDirect.Right | ResizeDirect.Top;
			}
			else if (pos.X > _Corner && pos.X < xr && pos.Y < _Corner)//t
			{
				_ResizeDir = ResizeDirect.Top;
			}
		}

		protected override void DoResize(Point pos)
		{
			base.DoResize(pos);

			if (_DoResize)
			{
				if ((_ResizeDir & ResizeDirect.Right) == ResizeDirect.Right)
					_Target.Width = pos.X + _AccX;
				else if ((_ResizeDir & ResizeDirect.Left) == ResizeDirect.Left)
				{
					var scPos = _Resizer.PointToScreen(pos);	
					var r = _Target.Left + _Target.ActualWidth;

					if (r - _Target.Left >= _Target.MinWidth && r - _Target.Left <= _Target.MaxWidth)
					{
						_Target.Left = scPos.X - _StartX;
						_Target.Width = r - _Target.Left;
					}
				}

				if ((_ResizeDir & ResizeDirect.Bottom) == ResizeDirect.Bottom)
					_Target.Height = pos.Y + _AccY;
				else if ((_ResizeDir & ResizeDirect.Top) == ResizeDirect.Top)
				{
					var scPos = _Resizer.PointToScreen(pos);
					var b = _Target.Top + _Target.ActualHeight;

					if (b - _Target.Top >= _Target.MinHeight && b - _Target.Top <= _Target.MaxHeight)
					{
						_Target.Top = scPos.Y - _StartY;
						_Target.Height = b - _Target.Top;
					}
				}
			}
		}
	}
}
