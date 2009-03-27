using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace UpdateLib.VersionInfo
{
	[Serializable]
	public class VersionManifest
	{
		public const string VersionFileName = "AppVersion.xml";
		public const string VersionManifestFileName = "VersionManifest.xml";
		public const string DownloadedVersionManifestFileName = "DownloadedVersionManifest.xml";


		public VersionManifest()
		{
			VersionItems = new VersionItemList();
			VersionNumber = new Version();
		}


		[XmlIgnore]
		public Version VersionNumber
		{ get; set; }

		public string VersionNumberString
		{
			get
			{
				return VersionNumber.ToString();
			}
			set
			{
				VersionNumber = new Version(value);
			}
		}

		public VersionItemList VersionItems
		{ get; set; }

		public string UpdateUri
		{ get; set; }

		public string UpdateUriAlt
		{ get; set; }


		public VersionManifest GetUpdateManifest(VersionManifest currentVersionManifest)
		{
			VersionManifest result = new VersionManifest();
			result.VersionNumberString = VersionNumberString;

			foreach (var item in VersionItems)
			{
				var existing = currentVersionManifest.VersionItems.Find(
					vi => String.Equals(item.GetItemFullPath(), vi.GetItemFullPath()));

				if (existing == null || item.VersionNumber > existing.VersionNumber)
					result.VersionItems.Add(item);
			}

			return result;
		}
	}
}
