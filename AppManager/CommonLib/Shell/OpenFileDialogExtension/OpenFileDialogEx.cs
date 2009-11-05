using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CommonLib.PInvoke;


namespace CommonLib.Shell.OpenFileDialogExtension
{
	/// <summary>
	/// Class that provides open file dialog extension
	/// </summary>
	/// <remarks>
	/// To apply visual styles to dialog add manifest with referense to Microsoft.Windows.Common-Controls 6.0.0.0 into your app.
	/// </remarks>
	public class OpenFileDialogEx
	{
		public event EventHandler SelectionChanged;

		protected string _Filter = String.Empty;
		protected string _DefaultExt = String.Empty;
		protected string _FileName = String.Empty;
		protected string _Title = String.Empty;
		protected Control _ControlHost;
		protected OpenFileDialogParentHook _HostForm;


		public string DefaultExt
		{
			get
			{
				return _DefaultExt;
			}
			set
			{
				_DefaultExt = value;
			}
		}

		public string Filter
		{
			get
			{
				return _Filter;
			}
			set
			{
				_Filter = value;
			}
		}

		public string FileName
		{
			get
			{
				return _FileName;
			}
			set
			{
				_FileName = value;
			}
		}

		public string Title
		{
			get
			{
				return _Title;
			}
			set
			{
				_Title = value;
			}
		}


		public DialogResult ShowDialog(Control extension, IWin32Window owner)
		{
			DialogResult returnDialogResult = DialogResult.Cancel;
			_ControlHost = extension;

			Form dialogHook = null;
			_HostForm = new OpenFileDialogParentHook(_ControlHost);
			if (owner != null)
			{
				_HostForm.AssignHandle(owner.Handle);
			}
			else
			{
				dialogHook = new Form();
				dialogHook.Text = String.Empty;
				dialogHook.StartPosition = FormStartPosition.Manual;
				dialogHook.Location = new Point(-32000, -32000);
				dialogHook.ShowInTaskbar = false;
				dialogHook.Show(owner);

				_HostForm.AssignHandle(dialogHook.Handle);
				User32.SetWindowPos(dialogHook.Handle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.UFLAGSHIDE);
			}

			_HostForm.WatchForActivate = true;

			try
			{
				returnDialogResult = ShowDialog(_HostForm.Handle);
			}
			finally
			{
				_HostForm.ReleaseHandle();

				if (dialogHook != null)
					dialogHook.Close();
			}

			return returnDialogResult;
		}

		public void CloseDialog(bool ok)
		{
			User32.SendMessage(
				_HostForm.DialogHandle,
				WindowMessage.WM_COMMAND,
				ok ? (IntPtr)1 : IntPtr.Zero,
				IntPtr.Zero
				);
		}


		protected DialogResult ShowDialog(IntPtr hwndOwner)
		{
			OPENFILENAME ofn = new OPENFILENAME();

			ofn.lStructSize = Marshal.SizeOf(ofn);
			ofn.lpstrFilter = _Filter.Replace('|', '\0') + '\0' + '\0';
			ofn.lpstrFile = _FileName + new string(' ', 520);
			ofn.nMaxFile = ofn.lpstrFile.Length;
			ofn.lpstrFileTitle = System.IO.Path.GetFileName(_FileName) + new string(' ', 512);
			ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
			ofn.lpstrTitle = _Title;
			ofn.lpstrDefExt = _DefaultExt;
			ofn.hwndOwner = hwndOwner;
			ofn.Flags =
				OpenFileNameFlags.OFN_ENABLEHOOK |
				OpenFileNameFlags.OFN_ENABLESIZING |
				OpenFileNameFlags.OFN_HIDEREADONLY |
				OpenFileNameFlags.OFN_EXPLORER;
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

			_FileName = ofn.lpstrFile;
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

					int x = (sr.Right + sr.Left - (cr.right - cr.left + _ControlHost.Width + 6)) / 2;
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
					else if (nmhdr.code == (ushort)CommonDlgNotification.CDN_SELCHANGE)
					{ 
						IntPtr hWndParent = User32.GetParent(hdlg);
						StringBuilder pathBuffer = new StringBuilder(260);
						UInt32 ret = User32.SendMessage(hWndParent, (uint)DialogChangeProperties.CDM_GETFILEPATH, 260, pathBuffer);
						_FileName = pathBuffer.ToString();

						if (SelectionChanged != null)
							SelectionChanged(this, EventArgs.Empty);
					}
					break;
			}

			return IntPtr.Zero;
		}
	}
}
