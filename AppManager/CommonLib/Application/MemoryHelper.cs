using System;
using CommonLib.PInvoke;

namespace CommonLib.Application
{
	public static class MemoryHelper
	{
		public static void Clean()
		{
			Collect();
			Kernel32.GropWorkingSet();
		}

		public static void Collect()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			GC.WaitForPendingFinalizers();			
		}
	}
}
