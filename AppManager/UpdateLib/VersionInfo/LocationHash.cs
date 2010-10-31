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
	}
}
