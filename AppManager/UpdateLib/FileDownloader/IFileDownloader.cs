using System;
using System.Collections.Generic;
using CommonLib;
using UpdateLib.VersionInfo;


namespace UpdateLib.FileDownloader
{
    public interface IFileDownloader
    {
		 event EventHandler<FileDownloadProgress> DownloadFileStarted;
		 //event EventHandler<ValueEventArgs<bool>> DownloadFileSetCompleted;

		 bool Cancel
		 { get; set; }

		 void DownloadFileSet(IEnumerable<LocationHash> fileLocation, string tempPath);
		 void DownloadFileSetAsync(IEnumerable<LocationHash> fileLocation, string tempPath, bool waitFor);
    }
}
