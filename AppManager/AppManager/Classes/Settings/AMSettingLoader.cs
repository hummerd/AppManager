using System.Collections.Generic;
using System.IO;


namespace AppManager.Settings
{
	public class AMSettingLoader : XMLSettingsLoader, ISettingProvider
	{
		public MainWorkItem WorkItem { get; set; }

		#region ISettingProvider Members

		public override Dictionary<string, object> LoadSettings(string path)
		{
			string userSettingsDir = WorkItem.DataDir;
			userSettingsDir = Path.Combine(userSettingsDir, path);
			return base.LoadSettings(userSettingsDir);
		}

		public override void SaveSettings(string path, Dictionary<string, object> settings)
		{
			string userSettingsDir = WorkItem.DataDir;
			userSettingsDir = Path.Combine(userSettingsDir, path);
			base.SaveSettings(userSettingsDir, settings);
		}

		#endregion
	}
}
