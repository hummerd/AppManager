using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CommonLib.PInvoke;


namespace CommonLib.Shell
{
	public class ODHandler : NativeWindow
	{
		protected Control _ctrl;

		public ODHandler(Control ctrl)
		{
			_ctrl = ctrl;
		}

		protected override void WndProc(ref Message m)
		{
			switch ((WindowMessage)m.Msg)
			{
				case WindowMessage.WM_SIZE:

					User32.RECT wcr = new User32.RECT();
					User32.GetClientRect(m.HWnd, ref wcr);

					//hfileView = User32.GetDlgItem(hparent, 0x0461);

					User32.SetWindowPos(
						_ctrl.Handle,
						IntPtr.Zero,
						0,
						0,
						_ctrl.Width,
						wcr.bottom - 103,
						SetWindowPosFlags.UFLAGSSIZE
						);
					//User32.SetWindowPos(mSourceControl.Handle, (IntPtr)ZOrderPos.HWND_BOTTOM, 0, 0, 0, 0, SetWindowPosFlags.UFLAGSZORDER);


					break;
			}
			base.WndProc(ref m);
		}
	}
}
