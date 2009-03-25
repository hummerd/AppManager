using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionInfo;


namespace UpdateLib.UI
{
	public interface IUIAskDownload
	{
		bool AskForDownload(string appName, VersionData versionInfo);
	}
}
