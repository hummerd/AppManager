using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;


namespace CommonLib.UI
{
	public class BorderResizer : ResizerBase
	{
		protected Window _Target;


		public BorderResizer(FrameworkElement resizer)
			: base(resizer)
		{
			_Target = UIHelper.FindLogicalAncestorOrSelf<Window>(_Resizer, null);
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
