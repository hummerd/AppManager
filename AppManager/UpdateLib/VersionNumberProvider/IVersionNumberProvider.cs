using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateLib.VersionNumberProvider
{
	public interface IVersionNumberProvider
	{
		Version GetLatestVersion();
		VersionInfo GetLatestVersionInfo();
		VersionManifest GetLatestVersionManifest();
	}
}
