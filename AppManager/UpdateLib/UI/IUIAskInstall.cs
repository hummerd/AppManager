using UpdateLib.VersionInfo;


namespace UpdateLib.UI
{
	public interface IUIAskInstall
	{
		bool AskForInstall(VersionData versionInfo);
	}
}
