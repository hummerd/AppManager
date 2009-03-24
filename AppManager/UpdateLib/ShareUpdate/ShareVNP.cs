using System.IO;
using CommonLib;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.ShareUpdate
{
	public class ShareVNP : IVersionNumberProvider
	{
		#region IVersionNumberProvider Members

		public VersionInfo GetLatestVersionInfo(string location)
		{
			try
			{
				return XmlSerializeHelper.DeserializeItem(typeof(VersionInfo),
					Path.Combine(location, VersionManifest.VersionFileName)) as VersionInfo;
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
