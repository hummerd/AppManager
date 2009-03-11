using System.Runtime.InteropServices;


namespace CommonLib.PInvoke
{
	class Kernel32
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetCurrentThreadId();
	}
}
