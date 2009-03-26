using System.IO;
using CommonLib;
using UpdateLib.FileDownloader;
using UpdateLib.VersionInfo;
using System;


namespace UpdateLib.WebUpdate
{
	public class WebVNP : WebFileDownloader, IVersionNumberProvider
	{
		protected MemoryStream _TempStream;


		#region IVersionNumberProvider Members

		public VersionData GetLatestVersionInfo(string location)
		{
			try
			{
				var loc = new Uri(location + "/" + VersionManifest.VersionFileName);
				DownloadFile(loc, null);
				_TempStream.Position = 0;
				StreamReader sr = new StreamReader(_TempStream);
				return XmlSerializeHelper.DeserializeItem(sr.ReadToEnd(), typeof(VersionData)) as VersionData;
			}
			catch
			{ ; }
			finally
			{ _TempStream.Dispose(); }

			return null;
		}

		public VersionManifest GetLatestVersionManifest(string location)
		{
			try
			{
				try
				{
					var loc = new Uri(location + "/" + VersionManifest.VersionManifestFileName);
					DownloadFile(loc, null);
					_TempStream.Position = 0;
					StreamReader sr = new StreamReader(_TempStream);
					return XmlSerializeHelper.DeserializeItem(sr.ReadToEnd(), typeof(VersionManifest)) as VersionManifest;
				}
				catch
				{ ; }
				finally
				{ _TempStream.Dispose(); }

				return null;
			}
			catch
			{ ; }

			return null;
		}

		#endregion

		protected override Stream GetTempStream(string tempPath)
		{
			_TempStream = new MemoryStream();
			return _TempStream;
		}

		protected override void CloseTempStream(Stream tempStream)
		{
			;
		}
	}
}
