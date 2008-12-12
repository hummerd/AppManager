using System;
using System.Collections.Generic;
using System.Text;

namespace AppManager.Classes
{
	class Program
	{
		// Entry point method
		[STAThread]
		public static void Main()
		{
			MainWorkItem mwi = new MainWorkItem();
			mwi.Commands.Start.Execute(null);
		}
	}
}
