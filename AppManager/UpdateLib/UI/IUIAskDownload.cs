using UpdateLib.VersionInfo;


namespace UpdateLib.UI
{
	public interface IUIAskDownload
	{
		bool AskForDownload(string appName, VersionData versionInfo, string sourceUri);
	}
}
