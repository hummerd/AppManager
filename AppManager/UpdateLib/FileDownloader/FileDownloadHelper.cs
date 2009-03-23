using System;
using System.IO;
using UpdateLib.UI;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.FileDownloader
{
	public class FileDownloadHelper
	{
		protected IFileDownloader _FileDownloader;


		public FileDownloadHelper(IFileDownloader fileDownloader)
		{
			_FileDownloader = fileDownloader;
		}


		public IUIDownloadProgress UIDownloadProgress
		{ get; set; }
		

		public void DownloadVersion(VersionManifest manifest, Version version, string appName, string tempPath)
		{
			//string tempFolder = Path.Combine(Path.GetTempPath(), appName + "_" + manifest.VersionNumber);
			
			_FileDownloader.DownloadFileSetCompleted += (s, e) => FileDownloadCompleted();
			_FileDownloader.DownloadFileStarted += (s, e) => 
				FileDownloadStarted(e.FilePath, e.ToltalSize, e.DownloadedSize);

			_FileDownloader.DownloadFileSetAsync(manifest.VersionItems, tempPath);
		}


		protected void FileDownloadCompleted()
		{
			UIDownloadProgress.Close();
		}

		protected void FileDownloadStarted(string fileLocation, long total, long progress)
		{
			UIDownloadProgress.SetDownloadProgress(fileLocation, total, progress);
		}
	}
}
