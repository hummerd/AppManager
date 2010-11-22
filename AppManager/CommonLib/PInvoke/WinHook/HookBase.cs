using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;


namespace CommonLib.PInvoke.WinHook
{
	public class HookBase<HookParam> : IDisposable
		where HookParam : struct
	{
		protected HookAPI.HookType _HookType;
		protected IntPtr				_HookHandle = IntPtr.Zero;
		protected HookAPI.HookProc _HookFunction = null;


		public HookBase(HookAPI.HookType hookType)
		{
			_HookType = hookType;
			_HookFunction = new HookAPI.HookProc(HookCallback);

			Install();
		}

		~HookBase()
		{
			Dispose(false);
			//Uninstall();
		}


		// hook function called by system
		protected int HookCallback(int code, IntPtr wParam, IntPtr lParam)
		{
			if (code < 0)
				return HookAPI.CallNextHookEx(_HookHandle, code, wParam, lParam);

			HookParam structData = (HookParam)Marshal.PtrToStructure(lParam, typeof(HookParam));
			if (ProcessHook(wParam, structData))
				return HookAPI.CallNextHookEx(_HookHandle, code, wParam, lParam);

			return -1;
		}

		protected virtual bool ProcessHook(IntPtr wParam, HookParam param)
		{
			return true;
		}

		protected void Install()
		{
			// make sure not already installed
			if (_HookHandle != IntPtr.Zero)
				return;

			var mod = ModuleHelper.GetCurrentModule();

			// install system-wide hook
			_HookHandle = HookAPI.SetWindowsHookEx(
				_HookType,
				_HookFunction, 
				mod.BaseAddress, 
				0);
		}

		protected void Uninstall()
		{
			if (_HookHandle != IntPtr.Zero)
			{
				// uninstall system-wide hook
				HookAPI.UnhookWindowsHookEx(_HookHandle);
				_HookHandle = IntPtr.Zero;
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
}
