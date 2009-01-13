using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.UI;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.FileDownloader
{
	public class FileDownloadHelper
	{
		protected IFileDownloader _FileDownloader;


		public FileDownloadHelper(IFileDownloader fileDownloader)
		{
			_FileDownloader = fileDownloader;
		}


		public IUIDownloadProgress UIDownloadProgress { get; set; }


		public void DownloadVersion(VersionManifest manifest, Version version)
		{ 
			
		}
	}
}
