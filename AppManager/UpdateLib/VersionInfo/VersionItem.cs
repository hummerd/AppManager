using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace UpdateLib.VersionInfo
{
	public enum InstallAction
	{
		Copy,
		CopyAndRun,
		Delete
	}
	
	[Serializable]
	public class VersionItemList : List<VersionItem>
	{

	}

	[Serializable]
	public class VersionItem : LocationHash
	{
		public VersionItem()
		{
			VersionNumber = new Version();
		}

		public InstallAction InstallAction
		{ get; set; }
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
		

		public bool NeedCopyItem()
		{
			return
				InstallAction == InstallAction.Copy ||
				InstallAction == InstallAction.CopyAndRun;
		}

		public bool NeedRunItem()
		{
			return
				InstallAction == InstallAction.CopyAndRun;
		}
	}
}
