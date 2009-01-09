using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Policy;


namespace UpdateLib.FileDownloader
{
    public class WebFileDownloader : IFileDownloader
    {
        public event EventHandler DownloadFileStarted;
        public event EventHandler DownloadFileCompleted;
        public event EventHandler DownloadFileSetCompleted;
        //public event EventHandler DownloadFileCompleted;


        public void DownloadFileSetAsync(IEnumerable<Uri> fileLocation)
        {

        }

        public void DownloadFileAsync(Uri fileLocation)
        {

        }
    }
}
