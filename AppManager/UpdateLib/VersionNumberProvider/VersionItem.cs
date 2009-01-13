using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateLib.VersionNumberProvider
{
	[Serializable]
	public class VersionItem
	{
		public Uri Location { get; set; }
		public int InstallAction { get; set; }
	}
}
