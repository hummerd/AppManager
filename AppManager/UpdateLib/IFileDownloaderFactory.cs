using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.FileDownloader;


namespace UpdateLib
{
	public interface IFileDownloaderFactory
	{
		IFileDownloader GetFileDownloader(string uriScheme);
	}
}
