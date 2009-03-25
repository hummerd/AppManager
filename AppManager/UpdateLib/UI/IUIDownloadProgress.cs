using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionInfo;


namespace UpdateLib.UI
{
	public interface IUIDownloadProgress
	{
		void Show();
		void SetDownloadInfo(VersionManifest manifest);
		void SetDownloadProgress(string location, long total, long progress);
		void Close();
	}
}
