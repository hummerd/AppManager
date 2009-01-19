using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateLib.Install
{
	public class InstallItem
	{
		public string SrcPath { get; set; }
		public string DstPath { get; set; }
		public InstallAction InstallAction { get; set; }
	}
}
