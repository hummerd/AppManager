using System;
using System.Collections.Generic;
using System.IO;
using CommonLib;
using UpdateLib.VersionInfo;


namespace UpdateLib.FileDownloader
{
	public abstract class FileDownloaderBase : IFileDownloader, IDisposable
	{
		public event EventHandler<FileDownloadProgress> DownloadFileStarted;
		public event EventHandler<ValueEventArgs<bool>> DownloadFileSetCompleted;


		//protected BackgroundWorker _Downloader = new BackgroundWorker();
		protected volatile bool _Cancel;


		public FileDownloaderBase()
		{
			//_Downloader.WorkerReportsProgress = true;
			//_Downloader.WorkerSupportsCancellation = true;

			//_Downloader.DoWork += (s, e) => e.Result = DownloadAsync(
			//   (e.Argument as object[])[0] as IEnumerable<VersionItem>,
			//   (e.Argument as object[])[1] as string
			//   );
			//_Downloader.RunWorkerCompleted += (s, e) => OnDownloadFileSetCompleted((bool)e.Result);
			//_Downloader.ProgressChanged += (s, e) => OnDownloadFileStarted(
			//   new FileDownloadProgress()
			//      { 
			//         FilePath = (string)(e.UserState as object[])[1],
			//         DownloadedSize = e.ProgressPercentage,
			//         ToltalSize = (long)(e.UserState as object[])[0]
			//      });
		}


		public bool Cancel
		{ 
			get { return _Cancel; }
			set { _Cancel = value; } 
		}


		public void DownloadFileSetAsync(IEnumerable<VersionItem> fileLocation, string tempPath, bool waitFor)
		{
			//_Downloader.RunWorkerAsync(new object[] { fileLocation, tempPath });
			//if (waitFor)
			//   while (_Downloader.IsBusy)
			//      System.Windows.Forms.Application.DoEvents();
			//      //DispatcherHelper.DoEvents();
		}

		public bool DownloadFileSet(IEnumerable<VersionItem> fileLocation, string tempPath)
		{
			int buffSize = 4096;
			byte[] buff = new byte[buffSize];

			try
			{
				foreach (var item in fileLocation)
				{
					var location = new Uri(item.Location);
					var tempDir = Path.Combine(tempPath, item.Path);
					var tempFile = Path.Combine(tempPath, item.GetItemFullPath());

					if (!Directory.Exists(tempDir))
						Directory.CreateDirectory(tempDir);

					DownloadFile(location, tempFile);
				}
			}
			catch(Exception)
			{
				return false;	
			}

			return true;
		}


		protected bool DownloadFile(Uri fileLocation, string tempFile)
		{
			int buffSize = 4096;
			byte[] buff = new byte[buffSize];

			Stream downloadStream = null;
			Stream tempStream = null;

			try
			{
				downloadStream = GetFileStream(fileLocation);
				tempStream = GetTempStream(tempFile);

				var fileSize = GetFileSize(fileLocation);

				int readCount = 0;
				int totalRead = 0;

				OnDownloadFileStarted(new FileDownloadProgress()
				{
					FilePath = fileLocation.AbsoluteUri,
					DownloadedSize = 0,
					ToltalSize = fileSize
				});

				while ((int)(readCount = downloadStream.Read(buff, 0, buffSize)) > 0)
				{
					if (_Cancel)
						return false;

					totalRead += readCount;
					tempStream.Write(buff, 0, readCount);

					// send progress info
					int progress = (int)((((double)totalRead) / fileSize) * 100);
					OnDownloadFileStarted(new FileDownloadProgress()
					{
						FilePath = fileLocation.AbsoluteUri,
						DownloadedSize = progress,
						ToltalSize = fileSize
					});
				}
			}
			catch (Exception)
			{
				return false;
			}
			finally
			{
				CloseDownloadStream(downloadStream);
				CloseTempStream(tempStream);
			}

			return true;
		}

		protected virtual Stream GetTempStream(string tempPath)
		{
			return new FileStream(tempPath, FileMode.Create, FileAccess.Write);
		}

		protected abstract Stream GetFileStream(Uri location);

		protected abstract long GetFileSize(Uri url);

		protected virtual void CloseDownloadStream(Stream downloadStream)
		{
			if (downloadStream != null)
				downloadStream.Dispose();
		}

		protected virtual void CloseTempStream(Stream tempStream)
		{
			if (tempStream != null)
				tempStream.Dispose();
		}


		protected virtual void OnDownloadFileStarted(FileDownloadProgress e)
		{
			if (DownloadFileStarted != null)
				DownloadFileStarted(this, e);
		}

		protected virtual void OnDownloadFileSetCompleted(bool succeded)
		{
			if (DownloadFileSetCompleted != null)
				DownloadFileSetCompleted(this, new ValueEventArgs<bool>(succeded));
		}

		#region IDisposable Members

		public void Dispose()
		{
			//_Downloader.Dispose();
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
