using System;
using System.Collections.Generic;
using System.ComponentModel;
using UpdateLib.FileDownloader;
using System.IO;


namespace UpdateLib.ShareUpdate
{
	public class ShareFileDownloader : FileDownloaderBase
	{
		protected override Stream GetFileStream(Uri location)
		{
			return new FileStream(location.LocalPath, FileMode.Open, FileAccess.Read);
		}

		protected override long GetFileSize(Uri location)
		{
			return new FileInfo(location.LocalPath).Length;
		}
	}
}
