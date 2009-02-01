using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateLib.VersionNumberProvider
{
	[Serializable]
	public class VersionManifest
	{
        public VersionItemList VersionItems { get; set; }
	}
}
