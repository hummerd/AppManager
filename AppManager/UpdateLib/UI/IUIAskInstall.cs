using UpdateLib.VersionInfo;


namespace UpdateLib.UI
{
	public interface IUIAskInstall
	{
		bool AskForInstall(string appName, VersionData versionInfo);
	}
}
