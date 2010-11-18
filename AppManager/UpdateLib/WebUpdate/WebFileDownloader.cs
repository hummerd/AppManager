using System;
using System.IO;
using System.Net;
using UpdateLib.FileDownloader;


namespace UpdateLib.WebUpdate
{
	public class WebFileDownloader : FileDownloaderBase
	{
		protected WebResponse	_DownloadResponse;


		protected override Stream GetFileStream(Uri location)
		{
			_DownloadResponse = GetRequest(location).GetResponse();
			return _DownloadResponse.GetResponseStream();
		}

		protected override long GetFileSize(Uri location)
		{
			long size = -1;
			using (WebResponse response = GetRequest(location).GetResponse())
			{
				size = response.ContentLength;
			}

			return size < 0 ? 1024 * 1024 : size;
		}


		protected WebRequest GetRequest(Uri url)
		{
			WebRequest request = WebRequest.Create(url);

			HttpWebRequest webRequest = request as HttpWebRequest;
			if (webRequest != null)
			{
				webRequest.Timeout = 10000;
				//request.Credentials = _Credentials;
				request.PreAuthenticate = true;
			}

			return request;
		}
	}
}
