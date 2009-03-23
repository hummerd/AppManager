using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.FileDownloader
{
	public abstract class FileDownloaderBase : IDisposable
	{
		protected BackgroundWorker _Downloader = new BackgroundWorker();


		public event EventHandler<FileDownloadProgress> DownloadFileStarted;
		public event EventHandler DownloadFileSetCompleted;


		public FileDownloaderBase()
		{
			_Downloader.WorkerReportsProgress = true;
			_Downloader.WorkerSupportsCancellation = true;

			_Downloader.DoWork += (s, e) => DownloadAsync(
				(e.Argument as object[])[0] as IEnumerable<VersionItem>,
				(e.Argument as object[])[1] as string
				);
			_Downloader.RunWorkerCompleted += (s, e) => OnDownloadFileSetCompleted();
			_Downloader.ProgressChanged += (s, e) => OnDownloadFileStarted(
				new FileDownloadProgress()
					{ 
						FilePath = (string)(e.UserState as object[])[1],
						DownloadedSize = (long)(e.UserState as object[])[0],
						ToltalSize = e.ProgressPercentage
					});
		}


		public void DownloadFileSetAsync(IEnumerable<VersionItem> fileLocation, string tempPath)
		{
			_Downloader.RunWorkerAsync(new object[] { fileLocation, tempPath });
		}


		protected bool DownloadAsync(IEnumerable<VersionItem> fileLocation, string tempPath)
		{
			int buffSize = 4096;
			byte[] buff = new byte[buffSize];

			try
			{
				foreach (var item in fileLocation)
				{
					using (var stream = GetFileStream(new Uri(item.Location)))
					using (var tempStream = GetTempStream(Path.Combine(tempPath, item.Location)))
					{
						var fileSize = GetFileSize(new Uri(item.Location));

						int readCount = 0;
						int totalRead = 0;

						_Downloader.ReportProgress(0, new object[] { fileSize, item.Location });

						while ((int)(readCount = stream.Read(buff, 0, buffSize)) > 0)
						{
							if (_Downloader.CancellationPending)
							{
								stream.Close();
								return false;
							}

							totalRead += readCount;
							tempStream.Write(buff, 0, readCount);

							// send progress info
							int progress = (int)((((double)totalRead) / fileSize) * 100);
							_Downloader.ReportProgress(progress, new object[] { fileSize, item.Location });
						}
					}
				}
			}
			catch(Exception)
			{
				return false;	
			}

			return true;
		}

		protected Stream GetTempStream(string tempPath)
		{
			return new FileStream(tempPath, FileMode.Create, FileAccess.Write);
		}

		protected abstract Stream GetFileStream(Uri location);

		protected abstract long GetFileSize(Uri url);


		protected virtual void OnDownloadFileStarted(FileDownloadProgress e)
		{
			if (DownloadFileStarted != null)
				DownloadFileStarted(this, e);
		}

		protected virtual void OnDownloadFileSetCompleted()
		{
			if (DownloadFileSetCompleted != null)
				DownloadFileSetCompleted(this, EventArgs.Empty);
		}

		#region IDisposable Members

		public void Dispose()
		{
			_Downloader.Dispose();
		}

		#endregion
	}


	public class FileDownloadProgress : EventArgs
	{
		public string FilePath { get; set; }
		public long ToltalSize { get; set; }
		public long DownloadedSize { get; set; }
	}
}
