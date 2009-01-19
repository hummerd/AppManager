using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.Install
{
	public class InstallHelper
	{
		public InstallHelper()
		{

		}


		public void InstallVersion(VersionManifest verManifest, Version lastVersion)
		{
			//TODO Unzip

			InstallManifest install = new InstallManifest(verManifest);
			install.Save(String.Empty);
		}
	}
}
