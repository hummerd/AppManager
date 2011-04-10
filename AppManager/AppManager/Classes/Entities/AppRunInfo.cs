using System;
using AppManager.Commands;
using AppManager.EntityCollection;


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
