﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace CommonLib.PInvoke
{
	public static class Kernel32
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetLastError();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetCurrentThreadId();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryExFlags dwFlags);


		public static void GropWorkingSet()
		{
			SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
		}

		public static void ZeroMemory(IntPtr ptr, int count)
		{
			for (int i = 0; i < count; i++)
				Marshal.WriteByte((IntPtr)((int)ptr + i), 0);
		}
	}

	public enum LoadLibraryExFlags
	{
		DONT_RESOLVE_DLL_REFERENCES	= 0x00000001,
		LOAD_IGNORE_CODE_AUTHZ_LEVEL	= 0x00000010,
		LOAD_LIBRARY_AS_DATAFILE		= 0x00000002,
		LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE	= 0x00000040,
		LOAD_LIBRARY_AS_IMAGE_RESOURCE		= 0x00000020,
		LOAD_WITH_ALTERED_SEARCH_PATH			= 0x00000008
	}
}
