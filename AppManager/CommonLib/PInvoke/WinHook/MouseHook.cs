using System;
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
			(IntPtr)User32.WindowMessage.WM_LBUTTONDOWN,
			(IntPtr)User32.WindowMessage.WM_MBUTTONDOWN,
			(IntPtr)User32.WindowMessage.WM_RBUTTONDOWN
		};

		protected IntPtr[] _MouseUpEvents = new IntPtr[] { 
			(IntPtr)User32.WindowMessage.WM_LBUTTONUP,
			(IntPtr)User32.WindowMessage.WM_MBUTTONUP,
			(IntPtr)User32.WindowMessage.WM_RBUTTONUP
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
			var hookEA = new MouseHookEventArgs(new Point(param.pt.X, param.pt.Y));

			if ((param.flags & 0x80) == 0)
			{
				if (Array.IndexOf(_MouseDownEvents, wParam) >= 0 && MouseDown != null)
					MouseDown(this, hookEA);

				if (Array.IndexOf(_MouseUpEvents, wParam) >= 0 && MouseUp != null)
					MouseUp(this, hookEA);
			}

			return !hookEA.Handled;
		}
	}

	public class MouseHookEventArgs : EventArgs
	{
		public MouseHookEventArgs(Point position)
		{
			Handled = false;
			Position = position;
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
	}
}
