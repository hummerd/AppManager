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
		protected Window	_Wnd;
		protected bool		_IsDialog;


		public DialogKeyDecorator(Window wnd, Button btnOk, Button btnCancel, bool isDialog)
		{
			_IsDialog = isDialog;
			_Wnd = wnd;
			_Wnd.PreviewKeyUp += (s, e) => OnPreviewKeyUp(e);

			if (_IsDialog)
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
			if (e.Key == Key.Enter || e.Key == Key.Escape)
			{
				e.Handled = true;
				if (_IsDialog)
					_Wnd.DialogResult = e.Key == Key.Enter;
				else
					_Wnd.Close();
			}
		}
	}
}
