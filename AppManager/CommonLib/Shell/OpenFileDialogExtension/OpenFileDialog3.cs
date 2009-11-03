using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CommonLib.PInvoke;
using System.Drawing;
using System.Runtime.InteropServices;
using CommonLib.Shell.OpenFileDialogExtension;


namespace CommonLib.Shell
{
	public class OpenFileDialogEx
	{
		private string m_Filter = "";
		private string m_DefaultExt = "";

		private string m_FileName = "";

		private Control _ControlHost;


		public string DefaultExt
		{
			get
			{
				return m_DefaultExt;
			}
			set
			{
				m_DefaultExt = value;
			}
		}

		public string Filter
		{
			get
			{
				return m_Filter;
			}
			set
			{
				m_Filter = value;
			}
		}

		public string FileName
		{
			get
			{
				return m_FileName;
			}
			set
			{
				m_FileName = value;
			}
		}


		public DialogResult ShowDialog(IWin32Window owner)
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			_ControlHost = new ListView();

			DialogResult returnDialogResult = DialogResult.Cancel;
			OpenFileDialogExHost mHostForm = new OpenFileDialogExHost(_ControlHost);
			mHostForm.Show(owner);
			User32.SetWindowPos(mHostForm.Handle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.UFLAGSHIDE);
			mHostForm.WatchForActivate = true;

			try
			{
				returnDialogResult = ShowDialog(mHostForm.Handle);
			}
			// Sometimes if you open a animated .gif on the preview and the Form is closed, .Net class throw an exception
			// Lets ignore this exception and keep closing the form.
			catch (Exception) { }

			mHostForm.Close();
			return returnDialogResult;
		}

		public DialogResult ShowDialog(IntPtr hwndOwner)
		{
			OPENFILENAME_3 ofn = new OPENFILENAME_3();

			ofn.lStructSize = Marshal.SizeOf(ofn);
			ofn.lpstrFilter = m_Filter.Replace('|', '\0') + '\0';
			ofn.lpstrFile = m_FileName + new string(' ', 512);
			ofn.nMaxFile = ofn.lpstrFile.Length;
			ofn.lpstrFileTitle = System.IO.Path.GetFileName(m_FileName) + new string(' ', 512);
			ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
			ofn.lpstrTitle = "Save file as";
			ofn.lpstrDefExt = m_DefaultExt;
			ofn.hwndOwner = hwndOwner;
			ofn.Flags =
				OpenFileNameFlags.OFN_ENABLESIZING |
				OpenFileNameFlags.OFN_EXPLORER | 
				OpenFileNameFlags.OFN_PATHMUSTEXIST | 
				OpenFileNameFlags.OFN_NOTESTFILECREATE | 
				OpenFileNameFlags.OFN_ENABLEHOOK | 
				OpenFileNameFlags.OFN_HIDEREADONLY;
			ofn.lpfnHook = new OfnHookProc(HookProc);

			//if we're running on Windows 98/ME then the struct is smaller
			if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				ofn.lStructSize -= 12;
			}

			//show the dialog

			if (!ComDlg32.GetOpenFileName(ref ofn))
			{
				int ret = ComDlg32.CommDlgExtendedError();

				if (ret != 0)
				{
					throw new ApplicationException("Couldn't show file open dialog - " + ret.ToString());
				}

				return DialogResult.Cancel;
			}

			m_FileName = ofn.lpstrFile;
			return DialogResult.OK;
		}


		protected IntPtr HookProc(IntPtr hdlg, UInt16 msg, Int32 wParam, Int32 lParam)
		{
			//var m = System.Windows.Forms.Message.Create(hdlg, (int)msg, (IntPtr)wParam, (IntPtr)lParam);
			//System.Diagnostics.Debug.WriteLine(m.ToString());

			switch ((WindowMessage)msg)
			{
				case WindowMessage.WM_INITDIALOG:

					//we need to centre the dialog
					Rectangle sr = Screen.FromPoint(Cursor.Position).Bounds;
					User32.RECT cr = new User32.RECT();
					IntPtr parent = User32.GetParent(hdlg);
					User32.GetWindowRect(parent, ref cr);

					int x = (sr.Right + sr.Left - (cr.right - cr.left)) / 2;
					int y = (sr.Bottom + sr.Top - (cr.bottom - cr.top)) / 2;

					User32.SetWindowPos(
						parent,
						IntPtr.Zero,
						x,
						y,
						cr.right - cr.left + _ControlHost.Width + 6,
						cr.bottom - cr.top,
						SetWindowPosFlags.SWP_NOZORDER);

					User32.RECT dlgClient = new User32.RECT();
					User32.GetClientRect(parent, ref dlgClient);

					User32.SetParent(_ControlHost.Handle, parent);
					User32.MoveWindow(
						_ControlHost.Handle,
						dlgClient.right - _ControlHost.Width - 6,
						36,
						_ControlHost.Width,
						dlgClient.bottom - 104,
						true
						);
					break;

				case WindowMessage.WM_DESTROY:
					break;

				case WindowMessage.WM_NOTIFY:

					//we need to intercept the CDN_FILEOK message
					//which is sent when the user selects a filename

					NMHDR nmhdr = (NMHDR)Marshal.PtrToStructure(new IntPtr(lParam), typeof(NMHDR));

					if (nmhdr.code == (ushort)CommonDlgNotification.CDN_FILEOK)
					{

					}
					break;

			}

			return IntPtr.Zero;
		}
	}
}
