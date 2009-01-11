using System.Collections.Generic;


namespace AppManager.Settings
{
	public interface ISettingProvider
	{
		Dictionary<string, object> LoadSettings(string path);
		void SaveSettings(string path, Dictionary<string, object> settings);
	}
}
