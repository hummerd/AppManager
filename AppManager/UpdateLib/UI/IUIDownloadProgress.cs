using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.UI
{
	public interface IUIDownloadProgress
	{
		void SetDownloadInfo(VersionManifest manifest, Version version);
		void SetDownloadProgress(string location, long total, long progress);
		void Close();
	}
}
