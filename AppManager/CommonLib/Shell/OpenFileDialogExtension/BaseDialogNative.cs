using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CommonLib.PInvoke;
using System.Drawing;


namespace CommonLib.Shell.OpenFileDialogExtension
{
	public class BaseDialogNative : NativeWindow, IDisposable
	{
		public delegate void PathChangedHandler(BaseDialogNative sender, string filePath);


		public event PathChangedHandler FileNameChanged;
		public event PathChangedHandler FolderNameChanged;


		protected IntPtr mHandle;
		protected Size mAddOnSize;

		
		public BaseDialogNative(IntPtr handle, Size addonSize)
		{
			mHandle = handle;
			mAddOnSize = addonSize;
			AssignHandle(handle);
		}
		

		public void Dispose()
		{
			ReleaseHandle();
		}
		
		
		protected override void WndProc(ref Message m)
		{
			switch ((WindowMessage)m.Msg)
			{
				case WindowMessage.WM_NOTIFY:
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
				case WindowMessage.WM_INITDIALOG:
					
					Rectangle sr = Screen.PrimaryScreen.Bounds;
					User32.RECT cr = new User32.RECT();
					IntPtr parent = User32.GetParent(m.HWnd);
					User32.GetWindowRect(parent, ref cr);

					int x = (sr.Right + sr.Left - (cr.right - cr.left)) / 2;
					int y = (sr.Bottom + sr.Top - (cr.bottom - cr.top)) / 2;

					User32.SetWindowPos(
						parent,
						IntPtr.Zero,
						x,
						y,
						cr.right - cr.left + mAddOnSize.Width + 6,
						cr.bottom - cr.top,
						SetWindowPosFlags.SWP_NOZORDER);

					break;
			}
			base.WndProc(ref m);
		}
	}
}
