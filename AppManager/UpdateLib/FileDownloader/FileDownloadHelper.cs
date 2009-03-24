using System;
using System.IO;
using UpdateLib.UI;
using UpdateLib.VersionNumberProvider;
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


		public void DownloadVersion(VersionManifest manifest, VersionInfo versionInfo, Version latestVersion, string appName, string tempPath)
		{
			//string tempFolder = Path.Combine(Path.GetTempPath(), appName + "_" + manifest.VersionNumber);

			_FileDownloader.DownloadFileSetCompleted += (s, e) => FileDownloadCompleted(
				new VersionDownloadInfo() 
				{
					Succeded = e.Value,
					DownloadedVersionManifest = manifest,
					DownloadedVersionInfo = versionInfo,
					LatestVersion = latestVersion,
					AppName = appName,
					TempPath = tempPath
				});

			_FileDownloader.DownloadFileStarted += (s, e) => FileDownloadStarted(
				e.FilePath, e.ToltalSize, e.DownloadedSize);

			if (_UIDownloadProgress != null)
			{
				_UIDownloadProgress.Show();
				_UIDownloadProgress.SetDownloadInfo(manifest);
			}

			_FileDownloader.DownloadFileSetAsync(manifest.VersionItems, tempPath, true);
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
		public VersionInfo DownloadedVersionInfo
		{ get; set; }
		public Version LatestVersion
		{ get; set; }
		public string AppName
		{ get; set; }
		public string TempPath
		{ get; set; }
		public VersionManifest LatestVersionManifest
		{ get; set; }
	}
}
