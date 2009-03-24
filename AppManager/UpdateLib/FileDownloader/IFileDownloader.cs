using System;
using System.Collections.Generic;
using CommonLib;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.FileDownloader
{
    public interface IFileDownloader
    {
		 event EventHandler<FileDownloadProgress> DownloadFileStarted;
		 event EventHandler<ValueEventArgs<bool>> DownloadFileSetCompleted;

		 void DownloadFileSetAsync(IEnumerable<VersionItem> fileLocation, string tempPath, bool waitFor);
    }
}
