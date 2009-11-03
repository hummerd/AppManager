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
	public class OpenFileDialog3
	{
		private string m_Filter = "";
		private string m_DefaultExt = "";

		private string m_FileName = "";

		private Screen  m_ActiveScreen;
		private Control _ControlHost;


		private IntPtr HookProc(IntPtr hdlg, UInt16 msg, Int32 wParam, Int32 lParam)
		{
			IntPtr hfileView;

			var m = System.Windows.Forms.Message.Create(hdlg, (int)msg, (IntPtr)wParam, (IntPtr)lParam);
			System.Diagnostics.Debug.WriteLine(m.ToString());

			switch ((WindowMessage)msg)
			{
				//case WindowMessage.WM_SIZE:

				//    User32.RECT wcr = new User32.RECT();
				//    IntPtr hparent = User32.GetParent(hdlg);
				//    User32.GetClientRect(hparent, ref wcr);

				//    hfileView = User32.GetDlgItem(hparent, 0x0461);

				//    User32.SetWindowPos(
				//        _ctrl.Handle,
				//        IntPtr.Zero,
				//        0,
				//        0,
				//        100,
				//        100,
				//        SetWindowPosFlags.UFLAGSSIZE
				//        );
				//    //User32.SetWindowPos(mSourceControl.Handle, (IntPtr)ZOrderPos.HWND_BOTTOM, 0, 0, 0, 0, SetWindowPosFlags.UFLAGSZORDER);
					

				//    break;
				case WindowMessage.WM_WINDOWPOSCHANGED:
					
					break;

				case WindowMessage.WM_INITDIALOG:

					//we need to centre the dialog
					Rectangle sr = m_ActiveScreen.Bounds;
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

					//we need to find the label to position our new label under

					//IntPtr fileTypeWindow = User32.GetDlgItem(parent, 0x441);
					//IntPtr fileView = IntPtr.Zero; // (int)FindWindowEx((IntPtr)parent, IntPtr.Zero, "SHELLDLL_DefView", null); //GetDlgItem(parent, 0x01);

					//User32.RECT aboveRect = new User32.RECT();
					//User32.GetWindowRect(fileView, ref aboveRect);

					//now convert the label's screen co-ordinates to client co-ordinates
					//User32.POINT point = new User32.POINT();
					//point.X = aboveRect.right;
					//point.Y = aboveRect.top;

					//User32.ScreenToClient(parent, ref point);

					//create the label
					//int labelHandle = CreateWindowEx(0, "", "SysListView32", 
					//    WS_VISIBLE | WS_CHILD | WS_TABSTOP, 
					//    point.X + 400, 
					//    12, 
					//    200, 100, 
					//    parent, 0, 0, 0);
					//SetWindowText(labelHandle, "&Encoding:");

					User32.RECT dlgClient = new User32.RECT();
					User32.GetClientRect(parent, ref dlgClient);

					//ListView ctrl = new ListView();
					//_ctrl = ctrl;
					//ctrl.Left = point.X + 6;
					//ctrl.Top = point.Y;
					//ctrl.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

					User32.SetParent(_ControlHost.Handle, parent);

					User32.MoveWindow(
						_ControlHost.Handle,
						dlgClient.right - _ControlHost.Width - 6,
						36,
						_ControlHost.Width,
						dlgClient.bottom - 104,
						true
						);

					//var c = Control.FromHandle((IntPtr)labelHandle);

					//TextBox tb = new TextBox();
					//tb.Multiline = true;
					//tb.Dock = DockStyle.Fill;



					//int fontHandle=SendMessage(fileTypeWindow, WM_GETFONT, 0, 0);
					//SendMessage(labelHandle, WM_SETFONT, fontHandle, 0);

					//we now need to find the combo-box to position the new combo-box under

					//IntPtr fileComboWindow = User32.GetDlgItem(parent, 0x470);
					//aboveRect = new User32.RECT();
					//User32.GetWindowRect(fileComboWindow, ref aboveRect);

					//point = new User32.POINT();
					//point.X = aboveRect.left;
					//point.Y = aboveRect.bottom;
					//User32.ScreenToClient(parent, ref point);

					//User32.POINT rightPoint = new User32.POINT();
					//rightPoint.X = aboveRect.right;
					//rightPoint.Y = aboveRect.top;

					//User32.ScreenToClient(parent, ref rightPoint);

					//we create the new combobox

					//int comboHandle = CreateWindowEx(0, "ComboBox", "mycombobox", WS_VISIBLE | WS_CHILD | CBS_HASSTRINGS | CBS_DROPDOWNLIST | WS_TABSTOP, point.X, point.Y + 8, rightPoint.X - point.X, 100, parent, 0, 0, 0);
					//SendMessage(comboHandle, WM_SETFONT, fontHandle, 0);

					break;
				case WindowMessage.WM_DESTROY:
					//destroy the handles we have created
					//if (m_ComboHandle != 0)
					//{
					//    User32.DestroyWindow(m_ComboHandle);
					//}

					//if (m_LabelHandle != 0)
					//{
					//    User32.DestroyWindow(m_LabelHandle);
					//}
					break;
				case WindowMessage.WM_NOTIFY:

					//we need to intercept the CDN_FILEOK message
					//which is sent when the user selects a filename

					NMHDR nmhdr = (NMHDR)Marshal.PtrToStructure(new IntPtr(lParam), typeof(NMHDR));

					if (nmhdr.code == (ushort)CommonDlgNotification.CDN_FILEOK)
					{
						//a file has been selected
						//we need to get the encoding

						//m_EncodingType = (EncodingType)User32.SendMessage(m_ComboHandle, CB_GETCURSEL, 0, 0);
					}
					break;

			}
			return IntPtr.Zero;
		}

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

		public DialogResult ShowDialog()
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			_ControlHost = new ListView();

			DialogResult returnDialogResult = DialogResult.Cancel;
			OpenDialogHost3 mHostForm = new OpenDialogHost3(_ControlHost);
			mHostForm.Show();
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
			//Application.EnableVisualStyles();
			//set up the struct and populate it

			OPENFILENAME_3 ofn = new OPENFILENAME_3();

			ofn.lStructSize = Marshal.SizeOf(ofn);
			ofn.lpstrFilter = m_Filter.Replace('|', '\0') + '\0';

			ofn.lpstrFile = m_FileName + new string(' ', 512);
			ofn.nMaxFile = ofn.lpstrFile.Length;
			ofn.lpstrFileTitle = System.IO.Path.GetFileName(m_FileName) + new string(' ', 512);
			ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
			ofn.lpstrTitle = "Save file as";
			ofn.lpstrDefExt = m_DefaultExt;

			//position the dialog above the active window
			ofn.hwndOwner = hwndOwner;

			//we need to find out the active screen so the dialog box is
			//centred on the correct display

			m_ActiveScreen = Screen.PrimaryScreen; //FromControl(Form.ActiveForm);

			//set up some sensible flags
			ofn.Flags =
				OpenFileNameFlags.OFN_ENABLESIZING |
				OpenFileNameFlags.OFN_EXPLORER | 
				OpenFileNameFlags.OFN_PATHMUSTEXIST | 
				OpenFileNameFlags.OFN_NOTESTFILECREATE | 
				OpenFileNameFlags.OFN_ENABLEHOOK | 
				OpenFileNameFlags.OFN_HIDEREADONLY;

			//this is where the hook is set. Note that we can use a C# delegate in place of a C function pointer
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
	}
}
