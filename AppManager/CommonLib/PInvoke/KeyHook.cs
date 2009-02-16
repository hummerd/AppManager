﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using WinForms = System.Windows.Forms;


namespace CommonLib.PInvoke
{
	public class KeyboardHook : IDisposable
	{
		#region pinvoke details

		private enum HookType : int
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

		public struct KBDLLHOOKSTRUCT
		{
			public UInt32 vkCode;
			public UInt32 scanCode;
			public UInt32 flags;
			public UInt32 time;
			public IntPtr extraInfo;
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowsHookEx(
			 HookType code, HookProc func, IntPtr instance, int threadID);

		[DllImport("user32.dll")]
		private static extern int UnhookWindowsHookEx(IntPtr hook);

		[DllImport("user32.dll")]
		private static extern int CallNextHookEx(
			 IntPtr hook, int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		#endregion

		HookType _hookType = HookType.WH_KEYBOARD_LL;
		IntPtr _hookHandle = IntPtr.Zero;
		HookProc _hookFunction = null;

		// hook method called by system
		private delegate int HookProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		// events
		public delegate void HookEventHandler(object sender, HookEventArgs e);
		public event HookEventHandler KeyDown;
		public event HookEventHandler KeyUp;

		public KeyboardHook()
		{
			_hookFunction = new HookProc(HookCallback);
			Install();
		}

		~KeyboardHook()
		{
			Dispose(false);
			//Uninstall();
		}

		// hook function called by system
		private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
		{
			if (code < 0)
				return CallNextHookEx(_hookHandle, code, wParam, ref lParam);

			var hookEA = new HookEventArgs(lParam.vkCode);
			if ((lParam.flags & 0x80) == 0)
			{
				// KeyUp event
				if (this.KeyUp != null)
					this.KeyUp(this, hookEA);

				// KeyDown event
				if (this.KeyDown != null)
					this.KeyDown(this, hookEA);
			}

			if (!hookEA.Handled)
				return CallNextHookEx(_hookHandle, code, wParam, ref lParam);

			return -1;
		}

		private void Install()
		{
			// make sure not already installed
			if (_hookHandle != IntPtr.Zero)
				return;

			// need instance handle to module to create a system-wide hook
			Module[] list = System.Reflection.Assembly.GetExecutingAssembly().GetModules();
			System.Diagnostics.Debug.Assert(list != null && list.Length > 0);

			// install system-wide hook
			_hookHandle = SetWindowsHookEx(_hookType,
				 _hookFunction, Marshal.GetHINSTANCE(list[0]), 0);
		}

		private void Uninstall()
		{
			if (_hookHandle != IntPtr.Zero)
			{
				// uninstall system-wide hook
				UnhookWindowsHookEx(_hookHandle);
				_hookHandle = IntPtr.Zero;
			}
		}

		#region IDisposable Members

		//Implement IDisposable.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Uninstall();
		}
		#endregion
	}

	public class HookEventArgs : EventArgs
	{
		// using Windows.Forms.Keys instead of Input.Key since the Forms.Keys maps
		// to the Win32 KBDLLHOOKSTRUCT virtual key member, where Input.Key does not
		public WinForms.Keys Key;
		public bool Alt;
		public bool Control;
		public bool Shift;
		public bool Handled = false;

		public HookEventArgs(UInt32 keyCode)
		{
			// detect what modifier keys are pressed, using 
			// Windows.Forms.Control.ModifierKeys instead of Keyboard.Modifiers
			// since Keyboard.Modifiers does not correctly get the state of the 
			// modifier keys when the application does not have focus
			this.Key = (WinForms.Keys)keyCode;
			this.Alt = (WinForms.Control.ModifierKeys & WinForms.Keys.Alt) != 0;
			this.Control = (WinForms.Control.ModifierKeys & WinForms.Keys.Control) != 0;
			this.Shift = (WinForms.Control.ModifierKeys & WinForms.Keys.Shift) != 0;
		}
	}
}
