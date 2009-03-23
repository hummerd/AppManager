using System;
using System.Collections.Generic;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.FileDownloader
{
    public interface IFileDownloader
    {
		 event EventHandler<FileDownloadProgress> DownloadFileStarted;
		 //event EventHandler DownloadFileCompleted;
		 event EventHandler DownloadFileSetCompleted;


		 void DownloadFileSetAsync(IEnumerable<VersionItem> fileLocation, string tempPath);
		 //void DownloadFileAsync(Uri fileLocation);
    }
}
