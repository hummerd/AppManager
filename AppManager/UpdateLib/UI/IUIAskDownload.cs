using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.UI
{
	public interface IUIAskDownload
	{
		bool AskForDownload(VersionInfo versionInfo);
	}
}
