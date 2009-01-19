using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using UpdateLib.Install;


namespace Updater
{
	public class MainWorkItem
	{
		public MainWorkItem()
		{

		}

		public void Run()
		{
			try
			{
				var manifest = InstallManifest.LoadFromCurrentDirectory();
				var install = new InstallHelper(manifest);
				install.Install();
			}
			catch(Exception exc)
			{
				MessageBox.Show(exc.ToString());
			}
		}
	}
}
