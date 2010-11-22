using System.Diagnostics;
using System.Reflection;


namespace CommonLib.PInvoke
{
	public static class ModuleHelper
	{
		public static ProcessModule GetCurrentModule()
		{
			// need instance handle to module to create a system-wide hook
			Module[] list = System.Reflection.Assembly.GetExecutingAssembly().GetModules();
			System.Diagnostics.Debug.Assert(list != null && list.Length > 0);

			var currentProcess = Process.GetCurrentProcess();
			var modules = currentProcess.Modules;
			ProcessModule mod = null;
			foreach (ProcessModule m in modules)
				if (m.ModuleName == list[0].Name)
				{
					mod = m;
					break;
				}

			if (mod == null)
				mod = Process.GetCurrentProcess().MainModule;

			return mod;
		}
	}
}
