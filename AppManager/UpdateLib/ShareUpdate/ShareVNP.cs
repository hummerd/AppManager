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
			return XmlSerializeHelper.DeserializeItem(typeof(VersionInfo), 
				Path.Combine(location, VersionManifest.VersionFileName)) as VersionInfo;
		}

		public VersionManifest GetLatestVersionManifest(string location)
		{
			return XmlSerializeHelper.DeserializeItem(typeof(VersionManifest),
				Path.Combine(location, VersionManifest.VersionManifestFileName)) as VersionManifest;
		}

		#endregion
	}
}
