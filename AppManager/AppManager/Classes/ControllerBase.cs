using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using CommonLib.PInvoke;
using CommonLib.IO;


namespace AppManager
{
	public class ControllerBase
	{
		protected MainWorkItem _WorkItem;


		public ControllerBase(MainWorkItem workItem)
		{
			_WorkItem = workItem;
		}


		public MainWorkItem WorkItem
		{ 
			get
			{
				return _WorkItem;
			}
		}
	}
}
