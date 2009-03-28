using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.FileDownloader;


namespace UpdateLib
{
	public class FileDownloaderFactory : IFileDownloaderFactory
	{
		#region IFileDownloaderFactory Members

		public IFileDownloader GetFileDownloader(string uriScheme)
		{
			if (uriScheme == Uri.UriSchemeFile)
				return new ShareUpdate.ShareFileDownloader();
			else if (uriScheme == Uri.UriSchemeHttp)
				return new WebUpdate.WebFileDownloader();

			return null;
		}

		#endregion
	}
}
