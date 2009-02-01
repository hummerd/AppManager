using System;
using System.Diagnostics;
using System.Windows.Forms;
using UpdateLib.Install;


namespace Updater
{
	public class MainWorkItem
	{
		public MainWorkItem()
		{

		}

		public void Run(string[] args)
		{
			try
			{
                var p = Process.GetProcessById(0);
                p.WaitForExit();

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
