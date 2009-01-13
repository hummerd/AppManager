using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.UI
{
	public interface IUIAskDownload
	{
		bool AskDownload(VersionInfo versionInfo, Version version);
	}
}
