

namespace AppManager.Settings
{
	public interface ISettingProvider<TSettings>
	{
		TSettings LoadSettings(string path);
		void SaveSettings(string path, TSettings settings);
	}
}
