using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionNumberProvider;
using System.Reflection;
using System.IO;
using System.Diagnostics;


namespace UpdateLib.Install
{
	public class InstallHelper
	{
		public InstallHelper()
		{

		}


		public void InstallVersion(string tempDir, VersionManifest verManifest, Version lastVersion)
		{
			//TODO Unzip

			InstallManifest install = new InstallManifest(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                tempDir,
                verManifest);

            string installerTemp = tempDir.TrimEnd('\\') + "_inst";
            if (!Directory.Exists(installerTemp))
                Directory.CreateDirectory(installerTemp);

            install.Save(installerTemp);

            string updaterPath = Assembly.GetExecutingAssembly().Location;
            File.Copy(updaterPath, Path.Combine(installerTemp, Path.GetFileName(updaterPath)));

            string installerPath = Path.Combine(installerTemp, "Updater.exe");
            File.WriteAllBytes(updaterPath, Resource.Updater);

            Process.Start(installerPath, "-pid" + Process.GetCurrentProcess().Id);
		}
	}
}
