using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateLib.FileDownloader
{
    public interface IFileDownloader
    {
		 event EventHandler DownloadFileStarted;
		 event EventHandler DownloadFileCompleted;
		 event EventHandler DownloadFileSetCompleted;


		 void DownloadFileSetAsync(IEnumerable<Uri> fileLocation);
		 void DownloadFileAsync(Uri fileLocation);
    }
}
