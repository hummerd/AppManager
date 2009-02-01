using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateLib.VersionNumberProvider
{
    public class VersionItemList : List<VersionItem>
    { 
    
    }

	[Serializable]
	public class VersionItem
	{
		public Uri Location { get; set; }
        public string Path { get; set; }
		public int InstallAction { get; set; }
	}
}
