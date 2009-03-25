

namespace UpdateLib.VersionInfo
{
	public interface IVersionNumberProvider
	{
		VersionData GetLatestVersionInfo(string location);
		VersionManifest GetLatestVersionManifest(string location);
	}
}
