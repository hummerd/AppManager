using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace UpdateLib.VersionNumberProvider
{
	[Serializable]
	public class VersionInfo
	{
		public VersionInfo()
		{
		}

		public VersionInfo(Version ver)
		{
			VersionNumber = ver;
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

		public string Description
		{ get; set; }
	}
}
