using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CommonLib.PInvoke;


namespace CommonLib.Shell.OpenFileDialogExtension
{
	public class OpenFileDialogExHost : Form
	{
		protected bool					_WatchForActivate = false;
		protected Control				_Extension;
		protected OpenFileDialogNative	_Handler;
		protected IntPtr				_DialogHandle;


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

		public IntPtr DialogHandle
		{
			get { return _DialogHandle; }
			set { _DialogHandle = value; }
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
				_DialogHandle = m.LParam;
				_Handler.AssignHandle(_DialogHandle);
			}

			base.WndProc(ref m);
		}
	}
}
