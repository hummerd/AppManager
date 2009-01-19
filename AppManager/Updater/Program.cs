﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Updater
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			MainWorkItem mwi = new MainWorkItem();
			mwi.Run();

			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run();
		}
	}
}