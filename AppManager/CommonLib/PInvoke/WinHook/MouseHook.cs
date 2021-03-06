﻿using System;
using System.Runtime.InteropServices;
using System.Windows;


namespace CommonLib.PInvoke.WinHook
{
	public class MouseHook : HookBase<MouseHook.MSLLHOOKSTRUCT>
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct MSLLHOOKSTRUCT
		{
			public User32.POINT pt;
			public UInt32 mouseData;
			public UInt32 flags;
			public UInt32 time;
			public IntPtr extraInfo;
		}

		protected IntPtr[] _MouseDownEvents = new IntPtr[] { 
			(IntPtr)WindowMessage.WM_LBUTTONDOWN,
			(IntPtr)WindowMessage.WM_MBUTTONDOWN,
			(IntPtr)WindowMessage.WM_RBUTTONDOWN
		};

		protected IntPtr[] _MouseUpEvents = new IntPtr[] { 
			(IntPtr)WindowMessage.WM_LBUTTONUP,
			(IntPtr)WindowMessage.WM_MBUTTONUP,
			(IntPtr)WindowMessage.WM_RBUTTONUP
		};

		protected IntPtr[] _MouseLeftEvents = new IntPtr[] { 
			(IntPtr)WindowMessage.WM_LBUTTONDOWN,
			(IntPtr)WindowMessage.WM_LBUTTONUP
		};

		protected IntPtr[] _MouseRightEvents = new IntPtr[] { 
			(IntPtr)WindowMessage.WM_RBUTTONDOWN,
			(IntPtr)WindowMessage.WM_RBUTTONUP
		};


		// events
		public event EventHandler<MouseHookEventArgs> MouseDown;
		public event EventHandler<MouseHookEventArgs> MouseUp;


		public MouseHook()
			: base(HookAPI.HookType.WH_MOUSE_LL)
		{
		}


		protected override bool ProcessHook(IntPtr wParam, MouseHook.MSLLHOOKSTRUCT param)
		{
			bool handled = false;

			if ((param.flags & 0x80) == 0)
			{
				if (MouseDown != null && Array.IndexOf(_MouseDownEvents, wParam) >= 0)
				{
					var hookEA = new MouseHookEventArgs(
						 new Point(param.pt.X, param.pt.Y),
						Array.IndexOf(_MouseLeftEvents, wParam) >= 0,
						Array.IndexOf(_MouseRightEvents, wParam) >= 0);

					MouseDown(this, hookEA);

					handled = hookEA.Handled;
				}

				if (MouseUp != null && Array.IndexOf(_MouseUpEvents, wParam) >= 0)
				{
					var hookEA = new MouseHookEventArgs(
						 new Point(param.pt.X, param.pt.Y),
						Array.IndexOf(_MouseLeftEvents, wParam) >= 0,
						Array.IndexOf(_MouseRightEvents, wParam) >= 0);

					MouseUp(this, hookEA);

					handled = hookEA.Handled;
				}
			}

			return !handled;
		}
	}

	public class MouseHookEventArgs : EventArgs
	{
		public MouseHookEventArgs(Point position, bool leftButton, bool rightButton)
		{
			Handled = false;
			Position = position;
            LeftButton = leftButton;
            RightButton = rightButton;
		}


		public bool Handled
		{ 
			get; 
			set; 
		}

		public Point Position
		{
			get;
			set;
		}

        public bool LeftButton
        { get; set; }

        public bool RightButton
        { get; set; }
	}
}
