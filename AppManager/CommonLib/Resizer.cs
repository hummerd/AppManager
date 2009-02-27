using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;


namespace CommonLib
{
	public class Resizer
	{
		protected Grid					_Target;
		protected FrameworkElement _Resizer;
		protected Point				_InGridPos;
		protected bool					_DoResize = false;


		public Resizer(FrameworkElement resizer)
		{
			_Resizer = resizer;
			_Target = UIHelper.FindAncestorOrSelf<Grid>(_Resizer);

			resizer.MouseDown += new MouseButtonEventHandler(resizer_MouseDown);
			resizer.MouseUp += new MouseButtonEventHandler(resizer_MouseUp);
			resizer.MouseMove += new MouseEventHandler(resizer_MouseMove);
		}


		private void resizer_MouseMove(object sender, MouseEventArgs e)
		{
			if (_DoResize)
			{ 
				
			}
		}
		
		private void resizer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			_Resizer.CaptureMouse();
			_InGridPos = e.GetPosition(_Target);
			_DoResize = true;
		}

		private void resizer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			_Resizer.ReleaseMouseCapture();
			_DoResize = false;
		}
	}
}
