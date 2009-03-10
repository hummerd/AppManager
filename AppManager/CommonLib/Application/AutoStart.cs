using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Reflection;


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
			else
				key.DeleteValue(_AppName);

			key.Close();
		}


		protected RegistryKey GetStartUpKey()
		{
			return Registry.CurrentUser.CreateSubKey(_CuRun);
		}
	}
}
