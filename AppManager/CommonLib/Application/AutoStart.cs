using System;
using System.Reflection;
using Microsoft.Win32;


namespace CommonLib.Application
{
	public class AutoStart
	{
		protected string _AppName;
		protected string _CuRun;


		public AutoStart(string appName)
		{
			_AppName = appName;
			_CuRun = @"Software\Microsoft\Windows\CurrentVersion\Run\";
		}


		public bool IsAutoStartSet()
		{
			var key = GetStartUpKey();
			object res = key.GetValue(_AppName);
			key.Close();

			return res != null;
		}

		public void SetStartUp(bool startUp)
		{
			var key = GetStartUpKey();

			if (startUp)
			{
				key.SetValue(_AppName, Assembly.GetEntryAssembly().Location, RegistryValueKind.String);
			}
			else if (Array.IndexOf(key.GetValueNames(), _AppName) >= 0)
				key.DeleteValue(_AppName);

			key.Close();
		}


		protected RegistryKey GetStartUpKey()
		{
			return Registry.CurrentUser.CreateSubKey(_CuRun);
		}
	}
}
