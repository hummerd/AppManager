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
		protected Window _Target;
		protected int	 _Corner;


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
				_Resizer.Cursor = Cursors.SizeNWSE;
			else if (pos.X < _Corner && pos.Y > _Corner && pos.Y < yb) //l
				_Resizer.Cursor = Cursors.SizeWE;
			else if (pos.X < _Corner && pos.Y > yb) //lb
				_Resizer.Cursor = Cursors.SizeNESW;
			else if (pos.X > _Corner && pos.X < xr && pos.Y > yb) //b
				_Resizer.Cursor = Cursors.SizeNS;
			else if (pos.X > xr && pos.Y > yb) //rb
				_Resizer.Cursor = Cursors.SizeNWSE;
			else if (pos.X > xr && pos.Y > _Corner && pos.Y < yb)//r
				_Resizer.Cursor = Cursors.SizeWE;
			else if (pos.X > xr && pos.Y < _Corner)//rt
				_Resizer.Cursor = Cursors.SizeNESW;
			else if (pos.X > _Corner && pos.X < xr && pos.Y < _Corner)//t
				_Resizer.Cursor = Cursors.SizeNS;
		}

		protected override void PrepareResize(Point pos)
		{
			base.PrepareResize(pos);
		}

		protected override void DoResize(Point pos)
		{
			base.DoResize(pos);

			if (_DoResize)
			{
				_Target.Width = pos.X + _AccX;
			}
		}
	}
}
