using System;
using System.Runtime.InteropServices;
using System.Windows;


namespace CommonLib.PInvoke
{
	/// <summary>
	/// Functions and delegates used for performing PInvoke for Win32 calls in User32.dll
	/// </summary>
	public class User32
	{
		public struct POINT
		{
			public Int32 X;
			public Int32 Y;
		}
		

		/// <summary>
		/// See MSDN documentation for the Win32 function GetDC.
		/// </summary>
		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		/// <summary>
		/// See MSDN documentation for the win32 function ReleaseDC.
		/// </summary>
		[DllImport("User32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		/// <summary>
		/// See MSDN documentation for the Win32 function GetWindowRgn.
		/// </summary>
		[DllImport("user32.dll")]
		public static extern RegionType GetWindowRgn(
			IntPtr hWnd,
			IntPtr hRgn);

		/// <summary>
		/// See MSDN documentation for the Win32 function SetWindowRgn.
		/// </summary>
		[DllImport("user32.dll")]
		public static extern int SetWindowRgn(
			IntPtr hWnd,
			IntPtr hRgn,
			bool bRedraw);

		/// <summary>
		/// See MSDN documentation for the Win32 function SendMessage
		/// </summary>
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
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int l, int t, int r, int b, int flag);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool AttachThreadInput(int t, int tt, bool b);


		public static void ActivateWindow(Window wnd)
		{
			AttachThreadInput();

			var win = new System.Windows.Interop.WindowInteropHelper(wnd);
			BringWindowToTop(win.Handle);
			SetWindowPos(win.Handle, IntPtr.Zero, 0, 0, 0, 0, 0x0001 | 0x0002);
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
		

		#region User32 Enums
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
		public enum SetWindowPosFlags
		{
			/// <summary>
			/// 
			/// </summary>
			SWP_NOSIZE = 0x0001,
			/// <summary>
			/// 
			/// </summary>
			SWP_NOMOVE = 0x0002,
			/// <summary>
			/// 
			/// </summary>
			SWP_NOZORDER = 0x0004,
			/// <summary>
			/// 
			/// </summary>
			SWP_NOREDRAW = 0x0008,
			/// <summary>
			/// 
			/// </summary>
			SWP_NOACTIVATE = 0x0010,
			/// <summary>
			/// 
			/// </summary>
			SWP_FRAMECHANGED = 0x0020,
			/// <summary>
			/// 
			/// </summary>
			SWP_SHOWWINDOW = 0x0040,
			/// <summary>
			/// 
			/// </summary>
			SWP_HIDEWINDOW = 0x0080,
			/// <summary>
			/// 
			/// </summary>
			SWP_NOCOPYBITS = 0x0100,
			/// <summary>
			/// 
			/// </summary>
			SWP_NOOWNERZORDER = 0x0200,
			/// <summary>
			/// 
			/// </summary>
			SWP_NOSENDCHANGING = 0x0400
		}

		/// <summary>
		/// WM_ window messages. Add as needed.
		/// </summary>
		public enum WindowMessage : int
		{
			WM_SYSCOMMAND = 274,
			WM_ACTIVATE	= 0x0006,
			WM_ACTIVATEAPP	= 0x001C,
			WM_CHANGEUISTATE = 0x0127,

			WM_KEYFIRST = 0x0100,
			WM_KEYDOWN  = 0x0100,
			WM_KEYUP    = 0x0101,
			WM_CHAR     = 0x0102,
			WM_DEADCHAR = 0x0103,
			WM_SYSKEYDOWN = 0x0104,
			WM_SYSKEYUP = 0x0105,
			WM_KEYLAST	= 0x108,

			WM_MOUSEFIRST = 0x200,
			WM_MOUSEMOVE = 0x200,
			WM_LBUTTONDOWN = 0x201,
			WM_LBUTTONUP = 0x202,
			WM_LBUTTONDBLCLK = 0x203,
			WM_RBUTTONDOWN = 0x204,
			WM_RBUTTONUP = 0x205,
			WM_RBUTTONDBLCLK = 0x206,
			WM_MBUTTONDOWN = 0x207,
			WM_MBUTTONUP = 0x208,
			WM_MBUTTONDBLCLK = 0x209,
			WM_MOUSEWHEEL = 0x20A,
			WM_MOUSEHWHEEL = 0x20E
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

		#endregion
	}
}