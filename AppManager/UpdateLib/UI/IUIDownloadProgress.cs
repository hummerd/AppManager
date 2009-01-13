using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.UI
{
	public interface IUIDownloadProgress
	{
		void SetDownloadInfo(VersionManifest manifest, Version version);
		void SetDownloadProgress(int major, int minor);
		void Close();
	}
}
