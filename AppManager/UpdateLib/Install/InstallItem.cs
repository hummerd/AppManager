using System;
using System.Collections.Generic;
using System.Text;


namespace UpdateLib.Install
{
    [Serializable]
    public class InstallItemList : List<InstallItem>
    { 
    }

    [Serializable]
	public class InstallItem
	{
		public string SrcPath { get; set; }
		public string DstPath { get; set; }
		public InstallAction InstallAction { get; set; }
	}
}
