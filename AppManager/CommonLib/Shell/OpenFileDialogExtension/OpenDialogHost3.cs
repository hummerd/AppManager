using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CommonLib.PInvoke;
using System.ComponentModel;


namespace CommonLib.Shell.OpenFileDialogExtension
{
	public class OpenFileDialogExHost : Form
	{
		protected bool					_WatchForActivate = false;
		protected Control				_Extension;
		protected OpenFileDialogNative	_Handler;


		public OpenFileDialogExHost(Control extension)
		{
			_Extension = extension;
			Text = String.Empty;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(-32000, -32000);
			ShowInTaskbar = false;
		}


		public bool WatchForActivate
		{
			get { return _WatchForActivate; }
			set { _WatchForActivate = value; }
		}


		protected override void OnClosing(CancelEventArgs e)
		{
			_Handler.ReleaseHandle();
			base.OnClosing(e);
		}

		protected override void WndProc(ref Message m)
		{
			if (_WatchForActivate && m.Msg == (int)WindowMessage.WM_ACTIVATE)
			{
				_WatchForActivate = false;
				_Handler = new OpenFileDialogNative(_Extension);
				_Handler.AssignHandle(m.LParam);
			}

			base.WndProc(ref m);
		}
	}
}
