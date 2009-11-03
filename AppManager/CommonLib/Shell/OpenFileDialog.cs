using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using CommonLib.PInvoke;
using System.Drawing;


namespace CommonLib.Shell
{
	/// <summary>
	/// The extensible OpenFileDialog
	/// </summary>
	public class CustomOpenFileDialog : IDisposable
	{
		// The maximum number of characters permitted in a path
		private const int _MAX_PATH = 260;

		// The "control ID" of the content window inside the OpenFileDialog
		// See the accompanying article to learn how I discovered it
		private const int _CONTENT_PANEL_ID = 0x0461;

		// A constant that determines the spacing between panels inside the OpenFileDialog
		private const int _PANEL_GAP_FACTOR = 6;

		/// <summary>
		/// Clients can implement handlers of this type to catch "selection changed" events
		/// </summary>
		public delegate void SelectionChangedHandler(string path);

		/// <summary>
		/// This event is fired whenever the user selects an item in the dialog
		/// </summary>
		public event SelectionChangedHandler SelectionChanged;

		// unmanaged memory buffers to hold the file name (with and without full path)
		private IntPtr _fileNameBuffer;
		private IntPtr _fileTitleBuffer;

		// the OPENFILENAME structure, used to control the appearance and behaviour of the OpenFileDialog
		private OpenFileName _ofn;
		private IntPtr _ptrOfn;
		private IntPtr _hwndDialog;
		private IntPtr _hwndParent;

		// user-supplied control that gets placed inside the OpenFileDialog
		private System.Windows.Forms.Control _userControl;

		// unmanaged memory buffer that holds the Win32 dialog template
		private IntPtr _ipTemplate;


		/// <summary>
		/// Sets up the data structures necessary to display the OpenFileDialog
		/// </summary>
		/// <param name="defaultExtension">The file extension to use if the user doesn't specify one (no "." required)</param>
		/// <param name="fileName">You can specify a filename to appear in the dialog, although the user can change it</param>
		/// <param name="filter">See the documentation for the OPENFILENAME structure for a description of filter strings</param>
		/// <param name="userPanel">Any Windows Forms control, it will be placed inside the OpenFileDialog</param>
		public CustomOpenFileDialog(string defaultExtension, string fileName, string filter, System.Windows.Forms.Control userControl)
		{
			// Need two buffers in unmanaged memory to hold the filename
			// Note: the multiplication by 2 is to allow for Unicode (16-bit) characters
			_fileNameBuffer = Marshal.AllocCoTaskMem(2 * _MAX_PATH + 2);
			_fileTitleBuffer = Marshal.AllocCoTaskMem(2 * _MAX_PATH);

			// Zero these two buffers
			byte[] zeroBuffer = new byte[2 * (_MAX_PATH + 1)];
			for (int i = 0; i < 2 * (_MAX_PATH + 1); i++) zeroBuffer[i] = 0;
			Marshal.Copy(zeroBuffer, 0, _fileNameBuffer, 2 * _MAX_PATH);
			Marshal.Copy(zeroBuffer, 0, _fileTitleBuffer, 2 * _MAX_PATH);

			// Create an in-memory Win32 dialog template; this will be a "child" window inside the FileOpenDialog
			// We have no use for this child window, except that its presence allows us to capture events when
			// the user interacts with the FileOpenDialog
			_ipTemplate = BuildDialogTemplate();

			// Populate the OPENFILENAME structure
			// The flags specified are the minimal set to get the appearance and behaviour we need
			_ofn.lStructSize = Marshal.SizeOf(_ofn);
			_ofn.lpstrFile = _fileNameBuffer;
			_ofn.nMaxFile = _MAX_PATH;
			_ofn.lpstrDefExt = Marshal.StringToCoTaskMemUni(defaultExtension);
			_ofn.lpstrFileTitle = _fileTitleBuffer;
			_ofn.nMaxFileTitle = _MAX_PATH;
			_ofn.lpstrFilter = Marshal.StringToCoTaskMemUni(filter);
			_ofn.Flags =
				OpenFileNameFlags.OFN_ENABLEHOOK |
				OpenFileNameFlags.OFN_ENABLETEMPLATEHANDLE |
				OpenFileNameFlags.OFN_ENABLESIZING |
				OpenFileNameFlags.OFN_HIDEREADONLY |
				OpenFileNameFlags.OFN_EXPLORER;
			_ofn.hInstance = _ipTemplate;
			_ofn.lpfnHook = new OfnHookProc(MyHookProc);

			// copy initial file name into unmanaged memory buffer
			UnicodeEncoding ue = new UnicodeEncoding();
			byte[] fileNameBytes = ue.GetBytes(fileName);
			Marshal.Copy(fileNameBytes, 0, _fileNameBuffer, fileNameBytes.Length);

			_ptrOfn = Marshal.AllocHGlobal(Marshal.SizeOf(_ofn));
			Marshal.StructureToPtr(_ofn, _ptrOfn, false);

			// keep a reference to the user-supplied control
			_userControl = userControl;
		}

		/// <summary>
		/// The finalizer will release the unmanaged memory, if I should forget to call Dispose
		/// </summary>
		~CustomOpenFileDialog()
		{
			Dispose(false);
		}


		/// <summary>
		/// Display the OpenFileDialog and allow user interaction
		/// </summary>
		/// <returns>true if the user clicked OK, false if they clicked cancel (or close)</returns>
		public bool Show()
		{
			return ComDlg32.GetOpenFileName(_ptrOfn);
		}

		public void Close(bool ok)
		{
			User32.EndDialog(_hwndParent, ok ? 1 : 0);
		}


		/// <summary>
		/// Builds an in-memory Win32 dialog template.  See documentation for DLGTEMPLATE.
		/// </summary>
		/// <returns>a pointer to an unmanaged memory buffer containing the dialog template</returns>
		private IntPtr BuildDialogTemplate()
		{
			// We must place this child window inside the standard FileOpenDialog in order to get any
			// notifications sent to our hook procedure.  Also, this child window must contain at least
			// one control.  We make no direct use of the child window, or its control.

			// Set up the contents of the DLGTEMPLATE
			DLGTEMPLATE template = new DLGTEMPLATE();

			// Allocate some unmanaged memory for the template structure, and copy it in
			IntPtr ipTemplate = Marshal.AllocCoTaskMem(Marshal.SizeOf(template));
			Marshal.StructureToPtr(template, ipTemplate, true);
			return ipTemplate;
		}

		/// <summary>
		/// The hook procedure for window messages generated by the FileOpenDialog
		/// </summary>
		/// <param name="hWnd">the handle of the window at which this message is targeted</param>
		/// <param name="msg">the message identifier</param>
		/// <param name="wParam">message-specific parameter data</param>
		/// <param name="lParam">mess-specific parameter data</param>
		/// <returns></returns>
		public IntPtr MyHookProc(IntPtr hWnd, UInt16 msg, Int32 wParam, Int32 lParam)
		{
			if (hWnd == IntPtr.Zero)
				return IntPtr.Zero;

			_hwndDialog = hWnd;

			//var m = System.Windows.Forms.Message.Create(hWnd, (int)msg, (IntPtr)wParam, (IntPtr)lParam);
			//System.Diagnostics.Debug.WriteLine(m.ToString());

			//IntPtr hWndParent2 = User32.GetParent(hWnd);

			// Behaviour is dependant on the message received
			switch ((WindowMessage)msg)
			{
				// We're not interested in every possible message; just return a NULL for those we don't care about
				default:
					{
						return IntPtr.Zero;
					}

				// WM_INITDIALOG - at this point the OpenFileDialog exists, so we pull the user-supplied control
				// into the FileOpenDialog now, using the SetParent API.
				case WindowMessage.WM_INITDIALOG:
					{
						IntPtr hWndParent = User32.GetParent(hWnd);
						IntPtr p = User32.GetDlgItem(hWnd, 0x46E);

						_hwndParent = hWndParent;
						User32.SetParent(_userControl.Handle, p);
						//FindAndResizePanels(hWnd);
						return IntPtr.Zero;
					}

				// WM_SIZE - the OpenFileDialog has been resized, so we'll resize the content and user-supplied
				// panel to fit nicely
				case WindowMessage.WM_SIZE:
					{
						//FindAndResizePanels(hWnd);
						User32.MoveWindow(_userControl.Handle,
							600, 600, 100, 100, true);
						return IntPtr.Zero;
					}

				// WM_NOTIFY - we're only interested in the CDN_SELCHANGE notification message:
				// we grab the currently-selected filename and fire our event
				case WindowMessage.WM_NOTIFY:
					{
						IntPtr ipNotify = new IntPtr(lParam);
						OFNOTIFY ofNot = (OFNOTIFY)Marshal.PtrToStructure(ipNotify, typeof(OFNOTIFY));
						CommonDlgNotification code = (CommonDlgNotification)ofNot.hdr.code;
						if (code == CommonDlgNotification.CDN_SELCHANGE)
						{
							// This is the first time we can rely on the presence of the content panel
							// Resize the content and user-supplied panels to fit nicely
							//FindAndResizePanels( hWnd );

							// get the newly-selected path
							IntPtr hWndParent = User32.GetParent(hWnd);
							StringBuilder pathBuffer = new StringBuilder(_MAX_PATH);
							UInt32 ret = User32.SendMessage(hWndParent, (uint)DialogChangeProperties.CDM_GETFILEPATH, _MAX_PATH, pathBuffer);
							string path = pathBuffer.ToString();

							// copy the string into the path buffer
							UnicodeEncoding ue = new UnicodeEncoding();
							byte[] pathBytes = ue.GetBytes(path);
							Marshal.Copy(pathBytes, 0, _fileNameBuffer, pathBytes.Length);
							Marshal.WriteInt16(new IntPtr((long)_fileNameBuffer + pathBytes.Length), 0, 0);

							// fire selection-changed event
							if (SelectionChanged != null)
								SelectionChanged(path);
						}
						else if (code == CommonDlgNotification.CDN_INITDONE)
						{
							//FindAndResizePanels(hWnd);
						}
						//else if (code == CommonDlgNotification.CDN_FILEOK)
						//{
						//    //int dr = User32.GetWindowLong(_hwndDialog, 0);
						//    //User32.SetWindowLong(_hwndDialog, 0, 1);
						//    //return (IntPtr)1;
						//}

						return IntPtr.Zero;
					}
				case WindowMessage.WM_SHOWWINDOW:
					{
						//FindAndResizePanels(hWnd);
						return IntPtr.Zero;
					}
			}
		}

		/// <summary>
		/// Layout the content of the OpenFileDialog, according to the overall size of the dialog
		/// </summary>
		/// <param name="hWnd">handle of window that received the WM_SIZE message</param>
		private void FindAndResizePanels(IntPtr hWnd)
		{
			// The FileOpenDialog is actually of the parent of the specified window
			IntPtr hWndParent = User32.GetParent(hWnd);

			// The "content" window is the one that displays the filenames, tiles, etc.
			// The _CONTENT_PANEL_ID is a magic number - see the accompanying text to learn
			// how I discovered it.
			IntPtr hWndContent = User32.GetDlgItem(hWndParent, _CONTENT_PANEL_ID);
			int err = Kernel32.GetLastError();

			Rectangle rcClient = new Rectangle(0, 0, 0, 0);
			//Rectangle rcContent = new Rectangle( 0, 0, 0, 0 );

			// Get client rectangle of dialog
			CommonLib.PInvoke.User32.RECT rcTemp = new CommonLib.PInvoke.User32.RECT();
			User32.GetClientRect(hWndParent, ref rcTemp);
			rcClient.X = rcTemp.left;
			rcClient.Y = rcTemp.top;
			rcClient.Width = rcTemp.right - rcTemp.left;
			rcClient.Height = rcTemp.bottom - rcTemp.top;

			// The content window may not be present when the dialog first appears
			if (hWndContent != IntPtr.Zero)
			{
				// Find the dimensions of the content panel
				CommonLib.PInvoke.User32.RECT rcContent = new CommonLib.PInvoke.User32.RECT();
				User32.GetWindowRect(hWndContent, ref rcContent);

				// Translate these dimensions into the dialog's coordinate system
				CommonLib.PInvoke.User32.POINT topLeft;
				topLeft.X = rcContent.left;
				topLeft.Y = rcContent.top;
				User32.ScreenToClient(hWndParent, ref topLeft);

				CommonLib.PInvoke.User32.POINT bottomRight;
				bottomRight.X = rcContent.right;
				bottomRight.Y = rcContent.bottom;
				User32.ScreenToClient(hWndParent, ref bottomRight);

				// Shrink content panel's width
				bottomRight.X = rcClient.Width - _userControl.Width - 2 * _PANEL_GAP_FACTOR;

				rcContent.left = topLeft.X;
				rcContent.right = bottomRight.X;
				rcContent.top = topLeft.Y;
				rcContent.bottom = bottomRight.Y;

				//rcContent.right = rcContent.right - _userControl.Width + _PANEL_GAP_FACTOR;

				User32.MoveWindow(hWndContent,
					rcContent.left,
					rcContent.top,
					rcContent.right - rcContent.left,
					rcContent.bottom - rcContent.top,
					true);

				// Position the user-supplied control alongside the content panel
				Rectangle rcUser = new Rectangle(
					rcContent.right + _PANEL_GAP_FACTOR,
					rcContent.top,
					_userControl.Width,
					//rcClient.Right - rcContent.right - (3 * _PANEL_GAP_FACTOR),
					rcContent.bottom - rcContent.top);

				User32.MoveWindow(_userControl.Handle,
					rcUser.X, rcUser.Y, rcUser.Width, rcUser.Height, true);
			}
		}

		/// <summary>
		/// returns the path currently selected by the user inside the OpenFileDialog
		/// </summary>
		public string SelectedPath
		{
			get
			{
				return Marshal.PtrToStringUni(_fileNameBuffer);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Free any unamanged memory used by this instance of OpenFileDialog
		/// </summary>
		/// <param name="disposing">true if called by Dispose, false otherwise</param>
		public void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}

			Marshal.FreeCoTaskMem(_fileNameBuffer);
			Marshal.FreeCoTaskMem(_fileTitleBuffer);
			Marshal.FreeCoTaskMem(_ipTemplate);
			Marshal.FreeHGlobal(_ptrOfn);
		}

		#endregion
	}
}
