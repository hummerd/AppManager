using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;


namespace AppManager.Settings
{
	public class AppManagerSettings
	{
		public AppManagerSettings()
		{
			MainFormSett = WndSettings.Empty;
		}

		public double[] MianFormRowHeights { get; set; }
		public WndSettings MainFormSett { get; set; }
	}
}
