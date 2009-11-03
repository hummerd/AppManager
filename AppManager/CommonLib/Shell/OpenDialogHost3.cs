using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CommonLib.PInvoke;
using System.ComponentModel;


namespace CommonLib.Shell.OpenFileDialogExtension
{
	public class OpenDialogHost3 : Form
	{
		private OpenDialogNative mNativeDialog = null;
		private OpenFileDialogEx mFileDialogEx = null;
		private bool mWatchForActivate = false;
		private IntPtr mOpenDialogHandle = IntPtr.Zero;
		private Control _Control;
		private ODHandler _Handler;

		public OpenDialogHost3(Control control)
		{
			_Control = control;
			this.Text = String.Empty;
			this.StartPosition = FormStartPosition.Manual;
			this.Location = new Point(-32000, -32000);
			this.ShowInTaskbar = false;
		}


		public IntPtr OpenDialogHandle
		{
			get
			{
				return mOpenDialogHandle;
			}
		}

		public bool WatchForActivate
		{
			get { return mWatchForActivate; }
			set { mWatchForActivate = value; }
		}


		protected override void OnClosing(CancelEventArgs e)
		{
			if (mNativeDialog != null)
				mNativeDialog.Dispose();

			_Handler.ReleaseHandle();

			base.OnClosing(e);
		}

		protected override void WndProc(ref Message m)
		{
			if (mWatchForActivate && m.Msg == (int)WindowMessage.WM_ACTIVATE)
			{
				mWatchForActivate = false;
				mOpenDialogHandle = m.LParam;
				_Handler = new ODHandler(_Control);
				_Handler.AssignHandle(mOpenDialogHandle);
			}

			base.WndProc(ref m);
		}
	}
}
