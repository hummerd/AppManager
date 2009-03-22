using System;
using System.Runtime.InteropServices;


namespace CommonLib.PInvoke.WinHook
{
	public static class HookAPI
	{
		public enum HookType : int
		{
			WH_JOURNALRECORD = 0,
			WH_JOURNALPLAYBACK = 1,
			WH_KEYBOARD = 2,
			WH_GETMESSAGE = 3,
			WH_CALLWNDPROC = 4,
			WH_CBT = 5,
			WH_SYSMSGFILTER = 6,
			WH_MOUSE = 7,
			WH_HARDWARE = 8,
			WH_DEBUG = 9,
			WH_SHELL = 10,
			WH_FOREGROUNDIDLE = 11,
			WH_CALLWNDPROCRET = 12,
			WH_KEYBOARD_LL = 13,
			WH_MOUSE_LL = 14
		}


		// hook method called by system
		public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);


		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowsHookEx(
			HookType code, HookProc func, IntPtr instance, int threadID);

		[DllImport("user32.dll")]
		public static extern int UnhookWindowsHookEx(
			IntPtr hook);

		[DllImport("user32.dll")]
		public static extern int CallNextHookEx(
			IntPtr hook, int code, IntPtr wParam, IntPtr lParam);
	}
}
