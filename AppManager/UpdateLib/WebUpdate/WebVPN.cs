using System.IO;
using CommonLib;
using UpdateLib.FileDownloader;
using UpdateLib.VersionInfo;


namespace UpdateLib.WebUpdate
{
	public class WebVPN : WebFileDownloader, IVersionNumberProvider
	{
		#region IVersionNumberProvider Members

		public VersionData GetLatestVersionInfo(string location)
		{
			try
			{
				return XmlSerializeHelper.DeserializeItem(typeof(VersionData),
					Path.Combine(location, VersionManifest.VersionFileName)) as VersionData;
			}
			catch
			{ ; }

			return null;
		}

		public VersionManifest GetLatestVersionManifest(string location)
		{
			try
			{
				return XmlSerializeHelper.DeserializeItem(typeof(VersionManifest),
					Path.Combine(location, VersionManifest.VersionManifestFileName)) as VersionManifest;
			}
			catch
			{ ; }

			return null;
		}

		#endregion
	}
}
