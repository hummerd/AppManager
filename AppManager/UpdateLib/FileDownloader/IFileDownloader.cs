using System;
using System.Collections.Generic;
using CommonLib;
using UpdateLib.VersionInfo;


namespace UpdateLib.FileDownloader
{
    public interface IFileDownloader
    {
		 event EventHandler<FileDownloadProgress> DownloadFileStarted;
		 event EventHandler<ValueEventArgs<bool>> DownloadFileSetCompleted;

		 bool Cancel
		 { get; set; }

		 bool DownloadFileSet(IEnumerable<VersionItem> fileLocation, string tempPath);
		 void DownloadFileSetAsync(IEnumerable<VersionItem> fileLocation, string tempPath, bool waitFor);
    }
}
