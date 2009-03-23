using System;
using System.Collections.Generic;
using System.Text;
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
		public string Location { get; set; }
		public string Path { get; set; }
		public InstallAction InstallAction { get; set; }
	}
}
