using System;
using System.Collections.Generic;


namespace Updater
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			MainWorkItem mwi = new MainWorkItem();
			mwi.Run(args);

			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run();
		}
	}
}
