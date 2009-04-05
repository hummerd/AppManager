using System;


namespace UpdateLib.VersionInfo
{
	public interface IVersionNumberProvider
	{
		VersionData GetLatestVersionInfo(Uri location);
		VersionManifest GetLatestVersionManifest(Uri location);
	}
}
