using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UpdateLib.Install;


namespace UpdateLib.VersionNumberProvider
{
	[Serializable]
	public class VersionItemList : List<VersionItem>
	{

	}

	[Serializable]
	public class VersionItem
	{
		public VersionItem()
		{
			VersionNumber = new Version();
		}

		public string Location { get; set; }
		public string Path { get; set; }
		public InstallAction InstallAction { get; set; }
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
		public string Base64Hash
		{
			get;
			set;
		}

		public string GetItemFullPath()
		{
			var location = new Uri(Location);
			return System.IO.Path.Combine(Path, location.Segments[location.Segments.Length - 1]);
		}
	}
}
