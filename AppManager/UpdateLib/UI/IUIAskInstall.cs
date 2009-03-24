using UpdateLib.VersionNumberProvider;


namespace UpdateLib.UI
{
	public interface IUIAskInstall
	{
		bool AskForInstall(VersionInfo versionInfo);
	}
}
