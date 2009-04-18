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
			if (_DoResize)
				return;

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
				{
					if (pos.X >= _AccX)
						_Target.Width = pos.X + _AccX;
				}
				else if ((_ResizeDir & ResizeDirect.Left) == ResizeDirect.Left)
				{
					var scPos = _Resizer.PointToScreen(pos);
					var right = _Target.Left + _Target.ActualWidth;
					var newLeft = scPos.X - _StartX;
					var newWidth = right - newLeft;

					if (newWidth <= _Target.MinWidth)
					{
						newWidth = _Target.MinWidth;
						newLeft = right - newWidth;
					}

					if (newWidth >= _Target.MaxWidth)
					{
						newWidth = _Target.MaxWidth;
						newLeft = right - newWidth;
					}

					_Target.Left = newLeft;
					_Target.Width = newWidth;
				}

				if ((_ResizeDir & ResizeDirect.Bottom) == ResizeDirect.Bottom)
				{
					if (pos.Y > _AccY)
						_Target.Height = pos.Y + _AccY;
				}
				else if ((_ResizeDir & ResizeDirect.Top) == ResizeDirect.Top)
				{
					var scPos = _Resizer.PointToScreen(pos);
					var bottom = _Target.Top + _Target.ActualHeight;
					var newTop = scPos.Y - _StartY;
					var newHeight = bottom - newTop;

					if (newHeight <= _Target.MinHeight)
					{
						newHeight = _Target.MinHeight;
						newTop = bottom - newHeight;
					}

					if (newHeight >= _Target.MaxHeight)
					{
						newHeight = _Target.MaxHeight;
						newTop = bottom - newHeight;
					}

					_Target.Top = newTop;
					_Target.Height = newHeight;
				}
			}
		}
	}
}
