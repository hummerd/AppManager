using System.Collections.Generic;
using System.IO;


namespace AppManager.Settings
{
	public class AMSettingLoader : FastSettingsLoader //XMLSettingsLoader<AppManagerSettings>
	{
		public MainWorkItem WorkItem { get; set; }

		#region ISettingProvider Members

		public override AppManagerSettings LoadSettings(string path)
		{
			string userSettingsDir = WorkItem.DataDir;
			userSettingsDir = Path.Combine(userSettingsDir, path);
			return base.LoadSettings(userSettingsDir);
		}

		public override void SaveSettings(string path, AppManagerSettings settings)
		{
			string userSettingsDir = WorkItem.DataDir;
			userSettingsDir = Path.Combine(userSettingsDir, path);
			base.SaveSettings(userSettingsDir, settings);
		}

		#endregion
	}
}
