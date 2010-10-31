using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateLib.VersionInfo
{
	[Serializable]
	public class LocationHash
	{
		public string Path
		{ get; set; }

		public string Location
		{ get; set; }

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
