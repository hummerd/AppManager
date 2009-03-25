using System;
using System.Collections.Generic;
using System.Windows;


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
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			var app = new Application();
			app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			app.Startup += new StartupEventHandler(Current_Startup);
			app.Run();
		}

		static void Current_Startup(object sender, StartupEventArgs e)
		{
			MainWorkItem mwi = new MainWorkItem();
			mwi.Run();			
		}
	}
}
