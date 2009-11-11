using System;
using System.Collections.Generic;
using System.Text;


namespace UpdateLib.VersionInfo
{
	public class VersionSource
	{
		public VersionSource(VersionData versionData, Uri sourceUri, IVersionNumberProvider vnp)
		{
			VersionData = versionData;
			SourceUri = sourceUri;
			VNP = vnp;
		}


		public VersionData VersionData
		{ get; set; }

		public Uri SourceUri 
		{ get; set; }

		public IVersionNumberProvider VNP
		{ get; set; }
	}
}
