using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace UpdateLib.VersionInfo
{
	[Serializable]
	public class LocationHash
	{
		[XmlElement(Order = 10)]
		public string Location
		{ get; set; }

		[XmlElement(Order=20)]
		public string Path
		{ get; set; }

		[XmlElement(Order=30)]
		public string Base64Hash
		{ get; set; }


		public string GetItemFullPath()
		{
			if (Location == null)
				return Path;

			var location = new Uri(Location);
			return System.IO.Path.Combine(Path, location.Segments[location.Segments.Length - 1]);
		}

		public string GetUnzipItemFullPath()
		{
			var itemFullPath = GetItemFullPath();

			if (itemFullPath.EndsWith(".gzip", StringComparison.InvariantCultureIgnoreCase))
				return itemFullPath.Substring(
					0,
					itemFullPath.Length - System.IO.Path.GetExtension(itemFullPath).Length);
			else
				return itemFullPath;
		}
	}
}
