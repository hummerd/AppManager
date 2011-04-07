using System;
using System.Collections.Generic;
using System.Text;
using AppManager.EntityCollection;
using AppManager.Commands;


namespace AppManager.Entities
{
	public class AppRunInfo : EntityBase<AppRunInfo>
	{
		public DateTime RunTime { get; set; }
		public StartArgs Areguments { get; set; }
	}

	public class AppRunInfoCollection : EntityCollection<AppRunInfo>
	{

	}
}
