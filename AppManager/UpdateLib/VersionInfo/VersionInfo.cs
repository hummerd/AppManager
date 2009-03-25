using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace UpdateLib.VersionInfo
{
	[Serializable]
	public class VersionData
	{
		public VersionData()
		{
		}

		public VersionData(Version ver)
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
