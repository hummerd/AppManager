﻿using System;
using System.IO;
using System.Net;


namespace UpdateLib.FileDownloader
{
	public class WebFileDownloader : FileDownloaderBase
	{
		protected ICredentials	_Credentials;
		protected bool				_PreAuthenticate;
		protected IWebProxy		_Proxy;
		protected string			_UserAgent = null;
		protected WebResponse _DownloadResponse;


		public WebFileDownloader()
		{
			_Credentials = null;
			_PreAuthenticate = false;
			_Proxy = WebRequest.DefaultWebProxy;
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

			request.Proxy = _Proxy;
			return request;
		}
	}
}
