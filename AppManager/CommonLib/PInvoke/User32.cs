using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Text;


namespace CommonLib.PInvoke
{
	/// <summary>
	/// Functions and delegates used for performing PInvoke for Win32 calls in User32.dll
	/// </summary>
	public class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWINFO
		{
			public UInt32 cbSize;
			public RECT rcWindow;
			public RECT rcClient;
			public UInt32 dwStyle;
			public UInt32 dwExStyle;
			public UInt32 dwWindowStatus;
			public UInt32 cxWindowBorders;
			public UInt32 cyWindowBorders;
			public UInt16 atomWindowType;
			public UInt16 wCreatorVersion;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS
		{
			public IntPtr hwnd;
			public IntPtr hwndAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public uint flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;
		};


		public delegate bool EnumWindowsCallBack(IntPtr hWnd, int lParam);


		[DllImport("user32.Dll")]
		public static extern int GetDlgCtrlID(IntPtr hWndCtl);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int MapWindowPoints(IntPtr hWnd, IntPtr hWndTo, ref POINT pt, int cPoints);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowInfo(IntPtr hwnd, out WINDOWINFO pwi);

		[DllImport("user32.Dll")]
		public static extern void GetWindowText(IntPtr hWnd, StringBuilder param, int length);

		[DllImport("user32.Dll")]
		public static extern void GetClassName(IntPtr hWnd, StringBuilder param, int length);

		[DllImport("user32.Dll")]
		public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsCallBack lpEnumFunc, int lParam); 

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool EndDialog(IntPtr hDlg, int nResult);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll")]
		public static extern RegionType GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

		[DllImport("user32.dll")]
		public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn,	bool bRedraw);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(ref POINT lpPoint);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr PostMessage(IntPtr hwnd, WindowMessage msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr FindWindow(string className, string windowName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, int f);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int l, int t, int cx, int cy, SetWindowPosFlags flag);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool AttachThreadInput(int t, int tt, bool b);

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr GetDlgItem(IntPtr hWndDlg, Int16 Id);

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, StringBuilder buffer);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetWindowRect(IntPtr hWnd, ref RECT rc);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetClientRect(IntPtr hWnd, ref RECT rc);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool repaint);


		public static RECT ScreenToClient(IntPtr hwnd, RECT screenRect)
		{
			var result = new RECT();

			POINT lt = new POINT();

			lt.X = screenRect.left;
			lt.Y = screenRect.top;
			ScreenToClient(hwnd, ref lt);
			result.left = lt.X;
			result.top = lt.Y;

			lt.X = screenRect.right;
			lt.Y = screenRect.bottom;
			ScreenToClient(hwnd, ref lt);
			result.right = lt.X;
			result.bottom = lt.Y;

			return result;
		}

		public static Point GetCursorPos(Visual relativeTo)
		{
			User32.POINT p = new User32.POINT();
			User32.GetCursorPos(ref p);
			return relativeTo.PointFromScreen(new Point(p.X, p.Y));
		}

		public static void ActivateWindow(Window wnd)
		{
			AttachThreadInput();

			var win = new System.Windows.Interop.WindowInteropHelper(wnd);
			BringWindowToTop(win.Handle);
			SetWindowPos(win.Handle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.UFLAGSZORDER);
			SetForegroundWindow(win.Handle);
		}

		public static void AttachThreadInput()
		{
			var hwndFrgnd = User32.GetForegroundWindow();
			var idThreadAttachTo = hwndFrgnd.ToInt64() > 0 ? User32.GetWindowThreadProcessId(hwndFrgnd, 0) : 0;
			if (idThreadAttachTo > 0)
			{
				bool r = User32.AttachThreadInput(
					Kernel32.GetCurrentThreadId(),
					idThreadAttachTo, true);
			}
		}
	}

	#region User32 Enums

	public enum ImeNotify
	{
		IMN_CLOSESTATUSWINDOW = 0x0001,
		IMN_OPENSTATUSWINDOW = 0x0002,
		IMN_CHANGECANDIDATE = 0x0003,
		IMN_CLOSECANDIDATE = 0x0004,
		IMN_OPENCANDIDATE = 0x0005,
		IMN_SETCONVERSIONMODE = 0x0006,
		IMN_SETSENTENCEMODE = 0x0007,
		IMN_SETOPENSTATUS = 0x0008,
		IMN_SETCANDIDATEPOS = 0x0009,
		IMN_SETCOMPOSITIONFONT = 0x000A,
		IMN_SETCOMPOSITIONWINDOW = 0x000B,
		IMN_SETSTATUSWINDOWPOS = 0x000C,
		IMN_GUIDELINE = 0x000D,
		IMN_PRIVATE = 0x000E
	}

	public enum ZOrderPos
	{
		HWND_TOP = 0,
		HWND_BOTTOM = 1,
		HWND_TOPMOST = -1,
		HWND_NOTOPMOST = -2
	}

	/// <summary>
	/// Specifies keys that must be pressed in combination with another key.
	/// </summary>
	[Flags]
	public enum KeyModifier
	{
		/// <summary>
		/// Either ALT key must be held down.
		/// </summary>
		MOD_ALT = 0x0001,
		/// <summary>
		/// Either CTRL key must be held down.
		/// </summary>
		MOD_CONTROL = 0x0002,
		/// <summary>
		/// Either SHIFT key must be held down.
		/// </summary>
		MOD_SHIFT = 0x0004,
		/// <summary>
		/// Either WINDOWS key was held down. These keys are labeled with the Microsoft Windows logo.
		/// </summary>
		MOD_WIN = 0x0008
	}

	/// <summary>
	/// Enumeration of the region types
	/// </summary>
	public enum RegionType
	{
		/// <summary>
		/// Error creating region
		/// </summary>
		ERROR = 0,
		/// <summary>
		/// An empty region
		/// </summary>
		NULLREGION = 1,
		/// <summary>
		/// A Simple region
		/// </summary>
		SIMPLEREGION = 2,
		/// <summary>
		/// A Complex region
		/// </summary>
		COMPLEXREGION = 3
	}

	/// <summary>
	/// GetWindow enumeration types.  Specifies the relationship between the specified window and the window whose handle is to be retrieved. 
	/// </summary>
	public enum GetWindowCmd : uint
	{
		/// <summary>
		/// The retrieved handle identifies the window of the same type that is highest in the Z order. If the specified window is a topmost window, the handle identifies the topmost window that is highest in the Z order. If the specified window is a top-level window, the handle identifies the top-level window that is highest in the Z order. If the specified window is a child window, the handle identifies the sibling window that is highest in the Z order.
		/// </summary>
		GW_HWNDFIRST = 0,
		/// <summary>
		/// The retrieved handle identifies the window of the same type that is lowest in the Z order. If the specified window is a topmost window, the handle identifies the topmost window that is lowest in the Z order. If the specified window is a top-level window, the handle identifies the top-level window that is lowest in the Z order. If the specified window is a child window, the handle identifies the sibling window that is lowest in the Z order.
		/// </summary>
		GW_HWNDLAST = 1,
		/// <summary>
		/// The retrieved handle identifies the window below the specified window in the Z order. If the specified window is a topmost window, the handle identifies the topmost window below the specified window. If the specified window is a top-level window, the handle identifies the top-level window below the specified window. If the specified window is a child window, the handle identifies the sibling window below the specified window.
		/// </summary>
		GW_HWNDNEXT = 2,
		/// <summary>
		/// The retrieved handle identifies the window above the specified window in the Z order. If the specified window is a topmost window, the handle identifies the topmost window above the specified window. If the specified window is a top-level window, the handle identifies the top-level window above the specified window. If the specified window is a child window, the handle identifies the sibling window above the specified window.
		/// </summary>
		GW_HWNDPREV = 3,
		/// <summary>
		/// The retrieved handle identifies the specified window's owner window, if any. For more information, see Owned Windows. 
		/// </summary>
		GW_OWNER = 4,
		/// <summary>
		/// The retrieved handle identifies the child window at the top of the Z order, if the specified window is a parent window; otherwise, the retrieved handle is NULL. The function examines only child windows of the specified window. It does not examine descendant windows.
		/// </summary>
		GW_CHILD = 5,
		/// <summary>
		/// Windows 2000/XP: The retrieved handle identifies the enabled popup window owned by the specified window (the search uses the first such window found using GW_HWNDNEXT); otherwise, if there are no enabled popup windows, the retrieved handle is that of the specified window. 
		/// </summary>
		GW_ENABLEDPOPUP = 6
	}

	/// <summary>
	/// Specifies how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values. 
	/// </summary>
	public enum ShowWindowCmd : int
	{
		/// <summary>
		/// Hides the window and activates another window.
		/// </summary>
		SW_HIDE = 0,
		/// <summary>
		/// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
		/// </summary>
		SW_SHOWNORMAL = 1,
		/// <summary>
		/// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
		/// </summary>
		SW_NORMAL = 1,
		/// <summary>
		/// Activates the window and displays it as a minimized window.
		/// </summary>
		SW_SHOWMINIMIZED = 2,
		/// <summary>
		/// Activates the window and displays it as a maximized window.
		/// </summary>
		SW_SHOWMAXIMIZED = 3,
		/// <summary>
		/// Maximizes the specified window.
		/// </summary>
		SW_MAXIMIZE = 3,
		/// <summary>
		/// Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except the window is not actived.
		/// </summary>
		SW_SHOWNOACTIVATE = 4,
		/// <summary>
		/// Activates the window and displays it in its current size and position.
		/// </summary>
		SW_SHOW = 5,
		/// <summary>
		/// Minimizes the specified window and activates the next top-level window in the Z order.
		/// </summary>
		SW_MINIMIZE = 6,
		/// <summary>
		/// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
		/// </summary>
		SW_SHOWMINNOACTIVE = 7,
		/// <summary>
		/// Displays the window in its current size and position. This value is similar to SW_SHOW, except the window is not activated.
		/// </summary>
		SW_SHOWNA = 8,
		/// <summary>
		/// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
		/// </summary>
		SW_RESTORE = 9,
		/// <summary>
		/// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
		/// </summary>
		SW_SHOWDEFAULT = 10,
		/// <summary>
		/// Windows 2000/XP: Minimizes a window, even if the thread that owns the window is hung. This flag should only be used when minimizing windows from a different thread.
		/// </summary>
		SW_FORCEMINIMIZE = 11
	};

	/// <summary>
	/// Define for the blend function (currently only one valid)
	/// </summary>
	public enum BlendFunctions : byte
	{
		/// <summary>
		/// The source bitmap is placed over the destination bitmap based on the alpha values of the source pixels.
		/// </summary>
		AC_SRC_OVER = 0x00
	}

	/// <summary>
	/// Define for the alpha format (currently only one valid)
	/// </summary>
	public enum AlphaFormat : byte
	{
		/// <summary>
		/// This flag is set when the bitmap has an Alpha channel (that is, per-pixel alpha). Note that the APIs use premultiplied alpha, which means that the red, green and blue channel values in the bitmap must be premultiplied with the alpha channel value. For example, if the alpha channel value is x, the red, green and blue channels must be multiplied by x and divided by 0xff prior to the call.
		/// </summary>
		AC_SRC_ALPHA = 0x01
	}

	/// <summary>
	/// 
	/// </summary>
	public enum SetWindowPosFlags : int
	{
		SWP_NOSIZE          = 0x0001,
		SWP_NOMOVE          = 0x0002,
		SWP_NOZORDER        = 0x0004,
		SWP_NOREDRAW        = 0x0008,
		SWP_NOACTIVATE      = 0x0010,
		SWP_FRAMECHANGED    = 0x0020,
		SWP_SHOWWINDOW      = 0x0040,
		SWP_HIDEWINDOW      = 0x0080,
		SWP_NOCOPYBITS      = 0x0100,
		SWP_NOOWNERZORDER   = 0x0200, 
		SWP_NOSENDCHANGING  = 0x0400,
		SWP_DRAWFRAME       = 0x0020,
		SWP_NOREPOSITION    = 0x0200,
		SWP_DEFERERASE      = 0x2000,
		SWP_ASYNCWINDOWPOS  = 0x4000,

		UFLAGSHIDE = 
            SetWindowPosFlags.SWP_NOACTIVATE |
			SetWindowPosFlags.SWP_NOOWNERZORDER |
			SetWindowPosFlags.SWP_NOMOVE |
            SetWindowPosFlags.SWP_NOSIZE | 
            SetWindowPosFlags.SWP_HIDEWINDOW,
		UFLAGSSIZE =
			SetWindowPosFlags.SWP_NOACTIVATE |
			SetWindowPosFlags.SWP_NOOWNERZORDER |
			SetWindowPosFlags.SWP_NOMOVE,
		UFLAGSSIZEEX =
			SetWindowPosFlags.SWP_NOACTIVATE |
			SetWindowPosFlags.SWP_NOOWNERZORDER |
			SetWindowPosFlags.SWP_NOMOVE |
			SetWindowPosFlags.SWP_ASYNCWINDOWPOS |
			SetWindowPosFlags.SWP_DEFERERASE,
		UFLAGSMOVE =
			SetWindowPosFlags.SWP_NOACTIVATE |
			SetWindowPosFlags.SWP_NOOWNERZORDER |
			SetWindowPosFlags.SWP_NOSIZE,
		UFLAGSZORDER =
			SetWindowPosFlags.SWP_NOACTIVATE |
			SetWindowPosFlags.SWP_NOMOVE |
			SetWindowPosFlags.SWP_NOSIZE
	}

	/// <summary>
	/// WM_ window messages. Add as needed.
	/// </summary>
	public enum WindowMessage : int
	{
		WM_NULL                   = 0x0000,
		WM_CREATE                 = 0x0001,
		WM_DESTROY                = 0x0002,
		WM_MOVE                   = 0x0003,
		WM_SIZE                   = 0x0005,
		WM_ACTIVATE               = 0x0006,
		WM_SETFOCUS               = 0x0007,
		WM_KILLFOCUS              = 0x0008,
		WM_ENABLE                 = 0x000A,
		WM_SETREDRAW              = 0x000B,
		WM_SETTEXT                = 0x000C,
		WM_GETTEXT                = 0x000D,
		WM_GETTEXTLENGTH          = 0x000E,
		WM_PAINT                  = 0x000F,
		WM_CLOSE                  = 0x0010,
		WM_QUERYENDSESSION        = 0x0011,
		WM_QUIT                   = 0x0012,
		WM_QUERYOPEN              = 0x0013,
		WM_ERASEBKGND             = 0x0014,
		WM_SYSCOLORCHANGE         = 0x0015,
		WM_ENDSESSION             = 0x0016,
		WM_SHOWWINDOW             = 0x0018,
		WM_CTLCOLOR               = 0x0019,
		WM_WININICHANGE           = 0x001A,
		WM_SETTINGCHANGE          = 0x001A,
		WM_DEVMODECHANGE          = 0x001B,
		WM_ACTIVATEAPP            = 0x001C,
		WM_FONTCHANGE             = 0x001D,
		WM_TIMECHANGE             = 0x001E,
		WM_CANCELMODE             = 0x001F,
		WM_SETCURSOR              = 0x0020,
		WM_MOUSEACTIVATE          = 0x0021,
		WM_CHILDACTIVATE          = 0x0022,
		WM_QUEUESYNC              = 0x0023,
		WM_GETMINMAXINFO          = 0x0024,
		WM_PAINTICON              = 0x0026,
		WM_ICONERASEBKGND         = 0x0027,
		WM_NEXTDLGCTL             = 0x0028,
		WM_SPOOLERSTATUS          = 0x002A,
		WM_DRAWITEM               = 0x002B,
		WM_MEASUREITEM            = 0x002C,
		WM_DELETEITEM             = 0x002D,
		WM_VKEYTOITEM             = 0x002E,
		WM_CHARTOITEM             = 0x002F,
		WM_SETFONT                = 0x0030,
		WM_GETFONT                = 0x0031,
		WM_SETHOTKEY              = 0x0032,
		WM_GETHOTKEY              = 0x0033,
		WM_QUERYDRAGICON          = 0x0037,
		WM_COMPAREITEM            = 0x0039,
		WM_GETOBJECT              = 0x003D,
		WM_COMPACTING             = 0x0041,
		WM_COMMNOTIFY             = 0x0044 ,
		WM_WINDOWPOSCHANGING      = 0x0046,
		WM_WINDOWPOSCHANGED       = 0x0047,
		WM_POWER                  = 0x0048,
		WM_COPYDATA               = 0x004A,
		WM_CANCELJOURNAL          = 0x004B,
		WM_NOTIFY                 = 0x004E,
		WM_INPUTLANGCHANGEREQUEST = 0x0050,
		WM_INPUTLANGCHANGE        = 0x0051,
		WM_TCARD                  = 0x0052,
		WM_HELP                   = 0x0053,
		WM_USERCHANGED            = 0x0054,
		WM_NOTIFYFORMAT           = 0x0055,
		WM_CONTEXTMENU            = 0x007B,
		WM_STYLECHANGING          = 0x007C,
		WM_STYLECHANGED           = 0x007D,
		WM_DISPLAYCHANGE          = 0x007E,
		WM_GETICON                = 0x007F,
		WM_SETICON                = 0x0080,
		WM_NCCREATE               = 0x0081,
		WM_NCDESTROY              = 0x0082,
		WM_NCCALCSIZE             = 0x0083,
		WM_NCHITTEST              = 0x0084,
		WM_NCPAINT                = 0x0085,
		WM_NCACTIVATE             = 0x0086,
		WM_GETDLGCODE             = 0x0087,
		WM_SYNCPAINT              = 0x0088,
		WM_NCMOUSEMOVE            = 0x00A0,
		WM_NCLBUTTONDOWN          = 0x00A1,
		WM_NCLBUTTONUP            = 0x00A2,
		WM_NCLBUTTONDBLCLK        = 0x00A3,
		WM_NCRBUTTONDOWN          = 0x00A4,
		WM_NCRBUTTONUP            = 0x00A5,
		WM_NCRBUTTONDBLCLK        = 0x00A6,
		WM_NCMBUTTONDOWN          = 0x00A7,
		WM_NCMBUTTONUP            = 0x00A8,
		WM_NCMBUTTONDBLCLK        = 0x00A9,
		WM_NCXBUTTONDOWN		  = 0x00AB,
		WM_NCXBUTTONUP			  = 0x00AC,
		WM_NCXBUTTONDBLCLK		  = 0x00AD,
		WM_KEYDOWN                = 0x0100,
		WM_KEYUP                  = 0x0101,
		WM_CHAR                   = 0x0102,
		WM_DEADCHAR               = 0x0103,
		WM_SYSKEYDOWN             = 0x0104,
		WM_SYSKEYUP               = 0x0105,
		WM_SYSCHAR                = 0x0106,
		WM_SYSDEADCHAR            = 0x0107,
		WM_KEYLAST                = 0x0108,
		WM_IME_STARTCOMPOSITION   = 0x010D,
		WM_IME_ENDCOMPOSITION     = 0x010E,
		WM_IME_COMPOSITION        = 0x010F,
		WM_IME_KEYLAST            = 0x010F,
		WM_INITDIALOG             = 0x0110,
		WM_COMMAND                = 0x0111,
		WM_SYSCOMMAND             = 0x0112,
		WM_TIMER                  = 0x0113,
		WM_HSCROLL                = 0x0114,
		WM_VSCROLL                = 0x0115,
		WM_INITMENU               = 0x0116,
		WM_INITMENUPOPUP          = 0x0117,
		WM_MENUSELECT             = 0x011F,
		WM_MENUCHAR               = 0x0120,
		WM_ENTERIDLE              = 0x0121,
		WM_MENURBUTTONUP          = 0x0122,
		WM_MENUDRAG               = 0x0123,
		WM_MENUGETOBJECT          = 0x0124,
		WM_UNINITMENUPOPUP        = 0x0125,
		WM_MENUCOMMAND            = 0x0126,
		WM_CTLCOLORMSGBOX         = 0x0132,
		WM_CTLCOLOREDIT           = 0x0133,
		WM_CTLCOLORLISTBOX        = 0x0134,
		WM_CTLCOLORBTN            = 0x0135,
		WM_CTLCOLORDLG            = 0x0136,
		WM_CTLCOLORSCROLLBAR      = 0x0137,
		WM_CTLCOLORSTATIC         = 0x0138,
		WM_MOUSEMOVE              = 0x0200,
		WM_LBUTTONDOWN            = 0x0201,
		WM_LBUTTONUP              = 0x0202,
		WM_LBUTTONDBLCLK          = 0x0203,
		WM_RBUTTONDOWN            = 0x0204,
		WM_RBUTTONUP              = 0x0205,
		WM_RBUTTONDBLCLK          = 0x0206,
		WM_MBUTTONDOWN            = 0x0207,
		WM_MBUTTONUP              = 0x0208,
		WM_MBUTTONDBLCLK          = 0x0209,
		WM_MOUSEWHEEL             = 0x020A,
		WM_XBUTTONDOWN			  = 0x020B,
		WM_XBUTTONUP			  = 0x020C,
		WM_XBUTTONDBLCLK		  = 0x020D,
		WM_PARENTNOTIFY           = 0x0210,
		WM_ENTERMENULOOP          = 0x0211,
		WM_EXITMENULOOP           = 0x0212,
		WM_NEXTMENU               = 0x0213,
		WM_SIZING                 = 0x0214,
		WM_CAPTURECHANGED         = 0x0215,
		WM_MOVING                 = 0x0216,
		WM_DEVICECHANGE           = 0x0219,
		WM_MDICREATE              = 0x0220,
		WM_MDIDESTROY             = 0x0221,
		WM_MDIACTIVATE            = 0x0222,
		WM_MDIRESTORE             = 0x0223,
		WM_MDINEXT                = 0x0224,
		WM_MDIMAXIMIZE            = 0x0225,
		WM_MDITILE                = 0x0226,
		WM_MDICASCADE             = 0x0227,
		WM_MDIICONARRANGE         = 0x0228,
		WM_MDIGETACTIVE           = 0x0229,
		WM_MDISETMENU             = 0x0230,
		WM_ENTERSIZEMOVE          = 0x0231,
		WM_EXITSIZEMOVE           = 0x0232,
		WM_DROPFILES              = 0x0233,
		WM_MDIREFRESHMENU         = 0x0234,
		WM_IME_SETCONTEXT         = 0x0281,
		WM_IME_NOTIFY             = 0x0282,
		WM_IME_CONTROL            = 0x0283,
		WM_IME_COMPOSITIONFULL    = 0x0284,
		WM_IME_SELECT             = 0x0285,
		WM_IME_CHAR               = 0x0286,
		WM_IME_REQUEST            = 0x0288,
		WM_IME_KEYDOWN            = 0x0290,
		WM_IME_KEYUP              = 0x0291,
		WM_MOUSEHOVER             = 0x02A1,
		WM_MOUSELEAVE             = 0x02A3,
		WM_CUT                    = 0x0300,
		WM_COPY                   = 0x0301,
		WM_PASTE                  = 0x0302,
		WM_CLEAR                  = 0x0303,
		WM_UNDO                   = 0x0304,
		WM_RENDERFORMAT           = 0x0305,
		WM_RENDERALLFORMATS       = 0x0306,
		WM_DESTROYCLIPBOARD       = 0x0307,
		WM_DRAWCLIPBOARD          = 0x0308,
		WM_PAINTCLIPBOARD         = 0x0309,
		WM_VSCROLLCLIPBOARD       = 0x030A,
		WM_SIZECLIPBOARD          = 0x030B,
		WM_ASKCBFORMATNAME        = 0x030C,
		WM_CHANGECBCHAIN          = 0x030D,
		WM_HSCROLLCLIPBOARD       = 0x030E,
		WM_QUERYNEWPALETTE        = 0x030F,
		WM_PALETTEISCHANGING      = 0x0310,
		WM_PALETTECHANGED         = 0x0311,
		WM_HOTKEY                 = 0x0312,
		WM_PRINT                  = 0x0317,
		WM_PRINTCLIENT            = 0x0318,
		WM_THEME_CHANGED          = 0x031A,
		WM_HANDHELDFIRST          = 0x0358,
		WM_HANDHELDLAST           = 0x035F,
		WM_AFXFIRST               = 0x0360,
		WM_AFXLAST                = 0x037F,
		WM_PENWINFIRST            = 0x0380,
		WM_PENWINLAST             = 0x038F,
		WM_APP                    = 0x8000,
		WM_USER                   = 0x0400,
		WM_REFLECT                = WM_USER + 0x1c00

		//WM_NOTIFY = 0x004E,
		//WM_SYSCOMMAND = 274,
		//WM_ACTIVATE = 0x0006,
		//WM_ACTIVATEAPP = 0x001C,
		//WM_CHANGEUISTATE = 0x0127,

		//WM_KEYFIRST = 0x0100,
		//WM_KEYDOWN = 0x0100,
		//WM_KEYUP = 0x0101,
		//WM_CHAR = 0x0102,
		//WM_DEADCHAR = 0x0103,
		//WM_SYSKEYDOWN = 0x0104,
		//WM_SYSKEYUP = 0x0105,
		//WM_KEYLAST = 0x108,

		//WM_MOUSEFIRST = 0x200,
		//WM_MOUSEMOVE = 0x200,
		//WM_LBUTTONDOWN = 0x201,
		//WM_LBUTTONUP = 0x202,
		//WM_LBUTTONDBLCLK = 0x203,
		//WM_RBUTTONDOWN = 0x204,
		//WM_RBUTTONUP = 0x205,
		//WM_RBUTTONDBLCLK = 0x206,
		//WM_MBUTTONDOWN = 0x207,
		//WM_MBUTTONUP = 0x208,
		//WM_MBUTTONDBLCLK = 0x209,
		//WM_MOUSEWHEEL = 0x20A,
		//WM_MOUSEHWHEEL = 0x20E,

		//WM_SIZE = 0x0005,
		//WM_INITDIALOG = 0x0110,

		//WM_USER = 0x400,
		//WM_CLOSE =  0x10,
		//WM_DESTROY = 0x02
	}

	/// <summary>
	/// WM_SYSCOMMAND WPARAMs
	/// </summary>
	public enum SysCommand : int
	{
		SC_FIRST = 0xF000,

		/// <summary>
		/// Sizes the window.
		/// </summary>
		SC_SIZE = SC_FIRST,

		/// <summary>
		/// Moves the window.
		/// </summary>
		SC_MOVE = SC_FIRST + 0x10,

		/// <summary>
		/// Minimizes the window.
		/// </summary>
		SC_MINIMIZE = SC_FIRST + 0x20,

		/// <summary>
		/// Maximizes the window.
		/// </summary>
		SC_MAXIMIZE = SC_FIRST + 0x30,

		/// <summary>
		/// Moves to the next window.
		/// </summary>
		SC_NEXTWINDOW = SC_FIRST + 0x40,

		/// <summary>
		/// Moves to the previous window.
		/// </summary>
		SC_PREVWINDOW = SC_FIRST + 0x50,

		/// <summary>
		/// Closes the window.
		/// </summary>
		SC_CLOSE = SC_FIRST + 0x60,

		/// <summary>
		/// Scrolls vertically.
		/// </summary>
		SC_VSCROLL = SC_FIRST + 0x70,

		/// <summary>
		/// Scrolls horizontally.
		/// </summary>
		SC_HSCROLL = SC_FIRST + 0x80,

		/// <summary>
		/// Retrieves the window menu as a result of a mouse click.
		/// </summary>
		SC_MOUSEMENU = SC_FIRST + 0x90,

		/// <summary>
		/// Retrieves the window menu as a result of a keystroke. For more information, see the Remarks section.
		/// </summary>
		SC_KEYMENU = SC_FIRST + 0x100,

		SC_ARRANGE = SC_FIRST + 0x110,

		/// <summary>
		/// Restores the window to its normal position and size.
		/// </summary>
		SC_RESTORE = SC_FIRST + 0x120,

		/// <summary>
		/// Activates the Start menu.
		/// </summary>
		SC_TASKLIST = SC_FIRST + 0x130,

		/// <summary>
		/// Executes the screen saver application specified in the [boot] section of the System.ini file.
		/// </summary>
		SC_SCREENSAVE = SC_FIRST + 0x140,

		/// <summary>
		/// Activates the window associated with the application-specified hot key. The lParam parameter identifies the window to activate.
		/// </summary>
		SC_HOTKEY = SC_FIRST + 0x150,
	}

	/// <summary>
	/// Sizing action used with the SC_SIZE wParam
	/// </summary>
	public enum SCSizingAction : int
	{
		West = 0x0001,
		East = 0x0002,
		North = 0x0003,
		NorthWest = 0x0004,
		NorthEast = 0x0005,
		South = 0x0006,
		SouthWest = 0x0007,
		SouthEast = 0x0008,
	}

	[Flags]
	public enum WindowStyles : uint
	{
		WS_OVERLAPPED = 0x0000,
		WS_POPUP = 0x80000000,
		WS_CHILD = 0x40000000,
		WS_MINIMIZE = 0x20000000,
		WS_VISIBLE = 0x10000000,
		WS_DISABLED = 0x8000000,
		WS_CLIPSIBLINGS = 0x4000000,
		WS_CLIPCHILDREN = 0x2000000,
		WS_MAXIMIZE = 0x1000000,
		WS_BORDER = 0x800000,
		WS_DLGFRAME = 0x400000,
		WS_VSCROLL = 0x200000,
		WS_HSCROLL = 0x100000,
		WS_SYSMENU = 0x80000,
		WS_THICKFRAME = 0x40000,
		WS_GROUP = 0x20000,
		WS_TABSTOP = 0x10000,
		WS_MINIMIZEBOX = 0x20000,
		WS_MAXIMIZEBOX = 0x10000,
		WS_CAPTION = WS_BORDER | WS_DLGFRAME,
		WS_TILED = WS_OVERLAPPED,
		WS_ICONIC = WS_MINIMIZE,
		WS_SIZEBOX = WS_THICKFRAME,
		WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
		WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
		WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
		WS_CHILDWINDOW = WS_CHILD
	}

	#endregion
}