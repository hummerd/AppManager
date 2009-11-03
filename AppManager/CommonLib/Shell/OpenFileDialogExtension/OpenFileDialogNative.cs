using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CommonLib.PInvoke;


namespace CommonLib.Shell
{
	public class OpenFileDialogNative : NativeWindow
	{
		protected Control _Extension;


		public OpenFileDialogNative(Control ctrl)
		{
			_Extension = ctrl;
		}


		protected override void WndProc(ref Message m)
		{
			switch ((WindowMessage)m.Msg)
			{
				case WindowMessage.WM_SIZE:

					var wcr = new User32.RECT();
					User32.GetClientRect(m.HWnd, ref wcr);

					User32.SetWindowPos(
						_Extension.Handle,
						IntPtr.Zero,
						0,
						0,
						_Extension.Width,
						wcr.bottom - 103,
						SetWindowPosFlags.UFLAGSSIZE
						);

					break;
			}

			base.WndProc(ref m);
		}
	}
}
