using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CommonLib.PInvoke;
using System.Runtime.InteropServices;


namespace CommonLib.Shell.OpenFileDialogExtension
{
	public class BaseDialogNative : NativeWindow, IDisposable
	{
		public delegate void PathChangedHandler(BaseDialogNative sender, string filePath);


		public event PathChangedHandler FileNameChanged;
		public event PathChangedHandler FolderNameChanged;


		protected IntPtr mHandle;

		
		public BaseDialogNative(IntPtr handle)
		{
			mHandle = handle;
			AssignHandle(handle);
		}
		

		public void Dispose()
		{
			ReleaseHandle();
		}
		
		
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case (int)WindowMessage.WM_NOTIFY:
					OFNOTIFY ofNotify = (OFNOTIFY)Marshal.PtrToStructure(m.LParam, typeof(OFNOTIFY));
					if (ofNotify.hdr.code == (uint)CommonDlgNotification.CDN_SELCHANGE)
					{
						StringBuilder filePath = new StringBuilder(256);
						User32.SendMessage(User32.GetParent(mHandle), (int)DialogChangeProperties.CDM_GETFILEPATH, (int)256, filePath);
						if (FileNameChanged != null)
							FileNameChanged(this, filePath.ToString());
					}
					else if (ofNotify.hdr.code == (uint)CommonDlgNotification.CDN_FOLDERCHANGE)
					{
						StringBuilder folderPath = new StringBuilder(256);
						User32.SendMessage(User32.GetParent(mHandle), (int)DialogChangeProperties.CDM_GETFOLDERPATH, (int)256, folderPath);
						if (FolderNameChanged != null)
							FolderNameChanged(this, folderPath.ToString());
					}
					break;
			}
			base.WndProc(ref m);
		}
	}
}
