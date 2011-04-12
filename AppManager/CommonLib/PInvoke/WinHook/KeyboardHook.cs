using System;
using WinForms = System.Windows.Forms;


namespace CommonLib.PInvoke.WinHook
{
	public class KeyboardHook : HookBase<KeyboardHook.KBDLLHOOKSTRUCT>
	{
		public struct KBDLLHOOKSTRUCT
		{
			public UInt32 vkCode;
			public UInt32 scanCode;
			public UInt32 flags;
			public UInt32 time;
			public IntPtr extraInfo;
		}

		protected IntPtr[] _KbrdDownEvents = new IntPtr[] { 
			(IntPtr)WindowMessage.WM_KEYDOWN,
			(IntPtr)WindowMessage.WM_SYSKEYDOWN
		};

		protected IntPtr[] _KbrdUpEvents = new IntPtr[] { 
			(IntPtr)WindowMessage.WM_KEYUP,
			(IntPtr)WindowMessage.WM_SYSKEYUP
		};


		// events
		public event KbrdHookEventHandler KeyDown;
		public event KbrdHookEventHandler KeyUp;


		public KeyboardHook()
			: base(HookAPI.HookType.WH_KEYBOARD_LL)
		{
		}


		protected override bool ProcessHook(IntPtr wParam, KeyboardHook.KBDLLHOOKSTRUCT param)
		{
			var handled = base.ProcessHook(wParam, param);

			KbrdHookEventArgs hookEA = new KbrdHookEventArgs(param.vkCode);
			if ((param.flags & 0x80) == 0)
			{
				// KeyUp event
				if (Array.IndexOf(_KbrdUpEvents, wParam) >= 0 && KeyUp != null)
					KeyUp(this, hookEA);

				// KeyDown event
				if (Array.IndexOf(_KbrdDownEvents, wParam) >= 0 && KeyDown != null)
					KeyDown(this, hookEA);
			}

			handled = handled && !hookEA.Handled;
			return handled;
		}
	}

	[Serializable]
	public delegate void KbrdHookEventHandler(object sender, KbrdHookEventArgs e);

	public struct KbrdHookEventArgs
	{
		// using Windows.Forms.Keys instead of Input.Key since the Forms.Keys maps
		// to the Win32 KBDLLHOOKSTRUCT virtual key member, where Input.Key does not
		public WinForms.Keys Key;
		public bool Alt;
		public bool Control;
		public bool Shift;
		public bool Handled;

		public KbrdHookEventArgs(UInt32 keyCode)
		{
			// detect what modifier keys are pressed, using 
			// Windows.Forms.Control.ModifierKeys instead of Keyboard.Modifiers
			// since Keyboard.Modifiers does not correctly get the state of the 
			// modifier keys when the application does not have focus
			Key = (WinForms.Keys)keyCode;
			Alt = (WinForms.Control.ModifierKeys & WinForms.Keys.Alt) != 0;
			Control = (WinForms.Control.ModifierKeys & WinForms.Keys.Control) != 0;
			Shift = (WinForms.Control.ModifierKeys & WinForms.Keys.Shift) != 0;
			Handled = false;
		}
	}
}
