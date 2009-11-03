using System;
using System.Windows;
using System.Windows.Interop;


namespace CommonLib.UI
{
	public class WpfWin32Window : System.Windows.Forms.IWin32Window
	{
		protected IntPtr _Handle;


		public WpfWin32Window(Window wnd)
		{
			_Handle = new WindowInteropHelper(wnd).Handle;
		}


		#region IWin32Window Members

		public IntPtr Handle
		{
			get { return _Handle; }
		}

		#endregion
	}
}
