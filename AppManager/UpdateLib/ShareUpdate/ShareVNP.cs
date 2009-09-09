using System;
using UpdateLib.VersionInfo;


namespace UpdateLib.ShareUpdate
{
	public class ShareVNP : IVersionNumberProvider
	{
		#region IVersionNumberProvider Members

		public VersionData GetLatestVersionInfo(Uri location)
		{
			return VersionManifestLoader.LoadData(location.LocalPath);
		}

		public VersionManifest GetLatestVersionManifest(Uri location)
		{
			try
			{
				//return XmlSerializeHelper.DeserializeItem(typeof(VersionManifest),
				//   location.LocalPath) as VersionManifest;

				return VersionManifestLoader.Load(location.LocalPath);
			}
			catch
			{ ; }

			return null;
		}

		#endregion
	}
}
