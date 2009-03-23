using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace UpdateLib.VersionNumberProvider
{
	[Serializable]
	public class VersionManifest
	{
		public const string VersionFileName = "AppVersion.xml";
		public const string VersionManifestFileName = "VersionManifest.xml";


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
	}
}
