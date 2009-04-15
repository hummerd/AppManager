using System;
using System.IO;
using System.Net;
using UpdateLib.FileDownloader;


namespace UpdateLib.WebUpdate
{
	public class WebFileDownloader : FileDownloaderBase
	{
		protected ICredentials	_Credentials;
		protected bool				_PreAuthenticate;
		protected string			_UserAgent = null;
		protected WebResponse	_DownloadResponse;


		public WebFileDownloader()
		{
			_Credentials = null;
			_PreAuthenticate = true;

			//WebProxy proxyObject = new WebProxy(WebProxy.GetDefaultProxy().Address);
			//proxyObject.UseDefaultCredentials = true;

			//_Proxy = proxyObject;
			_UserAgent = null;
		}


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
				request.Credentials = _Credentials;
				request.PreAuthenticate = _PreAuthenticate;

				// The default for the HttpWebRequest is for this value to
				// be null, so only do this if the UserAgent value is not
				// null or empty.
				if (!String.IsNullOrEmpty(_UserAgent))
				{
					webRequest.UserAgent = _UserAgent;
				}
			}

			var prx = new WebProxy(WebRequest.DefaultWebProxy.GetProxy(url));
			prx.UseDefaultCredentials = true;
			request.Proxy = prx;
			return request;
		}
	}
}
