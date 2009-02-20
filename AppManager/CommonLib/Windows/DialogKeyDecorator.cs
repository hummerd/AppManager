using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;


namespace CommonLib.Windows
{
	public class DialogKeyDecorator
	{
		protected Window _Wnd;


		public DialogKeyDecorator(Window wnd, Button btnOk, Button btnCancel, bool isDialog)
		{
			_Wnd = wnd;
			_Wnd.PreviewKeyUp += (s, e) => OnPreviewKeyUp(e);

			if (isDialog)
			{
				if (btnOk != null)
					btnOk.Click += (s, e) => _Wnd.DialogResult = true;

				if (btnCancel != null)
					btnCancel.Click += (s, e) => _Wnd.DialogResult = false;
			}
			else
			{
				if (btnOk != null)
					btnOk.Click += (s, e) => _Wnd.Close();

				if (btnCancel != null)
					btnCancel.Click += (s, e) => _Wnd.Close();
			}
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
