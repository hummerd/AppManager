using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;


namespace CommonLib.Shell
{
	public class ShFileInfo
	{
		private const uint SHGFI_ICON = 0x100;
		private const uint SHGFI_LARGEICON = 0x0; // 'Large icon
		private const uint SHGFI_SMALLICON = 0x1; // 'Small icon

		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);


		public static Icon ExtractIcon(string path, bool largeIcon)
		{
			SHFILEINFO fi = new SHFILEINFO();
			uint flags = SHGFI_ICON | (largeIcon ? SHGFI_LARGEICON : SHGFI_SMALLICON);
			var ptr = SHGetFileInfo(path, 0, ref fi, (uint)Marshal.SizeOf(fi), flags);

			if (IntPtr.Zero == fi.hIcon)
				return null;
			else
				return Icon.FromHandle(fi.hIcon);
		}
	}
}
