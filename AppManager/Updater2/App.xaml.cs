using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using Updater;


namespace Updater2
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			MainWorkItem mwi = new MainWorkItem();
			mwi.Run();	
		}
	}
}
