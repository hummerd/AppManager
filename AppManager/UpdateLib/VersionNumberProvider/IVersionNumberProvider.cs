

namespace UpdateLib.VersionNumberProvider
{
	public interface IVersionNumberProvider
	{
		VersionInfo GetLatestVersionInfo(string location);
		VersionManifest GetLatestVersionManifest(string location);
	}
}
