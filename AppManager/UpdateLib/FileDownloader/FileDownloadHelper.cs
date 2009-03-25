using System;
using System.IO;
using UpdateLib.UI;
using UpdateLib.VersionInfo;
using CommonLib;


namespace UpdateLib.FileDownloader
{
	public class FileDownloadHelper
	{
		public event EventHandler<VersionDownloadInfo> DownloadCompleted;


		protected IFileDownloader _FileDownloader;
		protected IUIDownloadProgress _UIDownloadProgress;


		public FileDownloadHelper(IFileDownloader fileDownloader, IUIDownloadProgress downloadProgress)
		{
			_FileDownloader = fileDownloader;
			_UIDownloadProgress = downloadProgress;
		}


		public void DownloadVersion(VersionDownloadInfo downloadInfo)
		{
			_FileDownloader.DownloadFileSetCompleted += delegate(object s, ValueEventArgs<bool> e)
				{
					downloadInfo.Succeded = e.Value;
					FileDownloadCompleted(downloadInfo);
				};

			_FileDownloader.DownloadFileStarted += (s, e) => FileDownloadStarted(
				e.FilePath, e.ToltalSize, e.DownloadedSize);

			if (_UIDownloadProgress != null)
			{
				_UIDownloadProgress.Show();
				_UIDownloadProgress.SetDownloadInfo(downloadInfo.DownloadedVersionManifest);
			}

			_FileDownloader.DownloadFileSetAsync(
				downloadInfo.DownloadedVersionManifest.VersionItems, 
				downloadInfo.TempPath,
				true);
		}


		protected void FileDownloadCompleted(VersionDownloadInfo e)
		{
			if (_UIDownloadProgress != null)
				_UIDownloadProgress.Close();

			OnDownloadCompleted(e);
		}

		protected void FileDownloadStarted(string fileLocation, long total, long progress)
		{
			if (_UIDownloadProgress != null)
				_UIDownloadProgress.SetDownloadProgress(fileLocation, total, progress);
		}


		protected virtual void OnDownloadCompleted(VersionDownloadInfo e)
		{
			if (DownloadCompleted != null)
				DownloadCompleted(this, e);
		}
	}

	public class VersionDownloadInfo : EventArgs
	{
		public bool Succeded
		{ get; set; }
		public VersionManifest DownloadedVersionManifest
		{ get; set; }
		public VersionManifest LatestVersionManifest
		{ get; set; }
		public VersionData LatestVersionInfo
		{ get; set; }
		public VersionManifest CurrentVersionManifest
		{ get; set; }
		public string AppName
		{ get; set; }
		public string AppPath
		{ get; set; }
		public string[] ExecutePaths
		{ get; set; }
		public string[] LockProcesses
		{ get; set; }
		public string TempPath
		{ get; set; }
	}
}
