using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CommonLib.PInvoke;


namespace CommonLib.Shell.OpenFileDialogExtension
{
	public class OpenFileDialogParentHook : NativeWindow
	{
		protected bool _WatchForActivate = false;
		protected Control _Extension;
		protected OpenFileDialogHook _Handler;
		protected IntPtr _DialogHandle;


		public OpenFileDialogParentHook(Control extension)
		{
			_Extension = extension;
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


		public override void ReleaseHandle()
		{
			if (_Handler != null)
				_Handler.ReleaseHandle();

			base.ReleaseHandle();
		}


		protected override void WndProc(ref Message m)
		{
			if (_WatchForActivate && m.Msg == (int)WindowMessage.WM_ACTIVATE)
			{
				_WatchForActivate = false;
				_Handler = new OpenFileDialogHook(_Extension);
				_DialogHandle = m.LParam;
				_Handler.AssignHandle(_DialogHandle);
			}

			base.WndProc(ref m);
		}
	}
}
