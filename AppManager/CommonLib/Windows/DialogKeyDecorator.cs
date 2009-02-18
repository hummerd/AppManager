using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace CommonLib.Windows
{
	public class DialogKeyDecorator
	{
		protected Window _Wnd;


		public DialogKeyDecorator(Window wnd)
		{
			_Wnd = wnd;
			_Wnd.PreviewKeyUp += (s, e) => OnPreviewKeyUp(e);
		}


		protected void OnPreviewKeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				_Wnd.DialogResult = true;
			}

			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				_Wnd.DialogResult = false;
			}
		}
	}
}
