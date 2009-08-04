using System;


namespace UpdateLib.VersionInfo
{
	public class VersionData
	{
		public VersionData()
		{
			Description = " ";
		}

		public VersionData(Version ver)
		{
			VersionNumber = ver;
			Description = " ";
		}


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
