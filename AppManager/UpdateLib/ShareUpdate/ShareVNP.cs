using System.IO;
using CommonLib;
using UpdateLib.VersionInfo;
using System;


namespace UpdateLib.ShareUpdate
{
	public class ShareVNP : IVersionNumberProvider
	{
		#region IVersionNumberProvider Members

		public VersionData GetLatestVersionInfo(Uri location)
		{
			try
			{
				//return XmlSerializeHelper.DeserializeItem(typeof(VersionData),
				//   location.LocalPath) as VersionData;

				return VersionManifestLoader.LoadData(location.LocalPath);
			}
			catch
			{ ; }

			return null;
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
