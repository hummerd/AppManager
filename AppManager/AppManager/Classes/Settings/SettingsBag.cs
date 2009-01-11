using System.Collections.Generic;


namespace AppManager.Settings
{
	public class SettingsBag
	{
		protected Dictionary<string, object> _Bag = new Dictionary<string,object>(1);
		protected ISettingProvider _SettingProvider;


		public SettingsBag(ISettingProvider settingProvider)
		{
			_SettingProvider = settingProvider;
		}


		public void LoadSettings(string path)
		{
			_Bag = _SettingProvider.LoadSettings(path);
		}

		public void SaveSettings(string path)
		{
			_SettingProvider.SaveSettings(path, _Bag);
		}


		public string GetSettingStr(string setting, string defaultVal)
		{
			object val;
			if (_Bag.TryGetValue(setting, out val))
				return (string)val;
			else
				return defaultVal;
		}

		public int GetSettingInt(string setting, int defaultVal)
		{
			object val;
			if (_Bag.TryGetValue(setting, out val))
				return (int)val;
			else
				return defaultVal;
		}

		public T GetSettingCmplx<T>(string setting, T defaultVal)
		{
			object val;
			if (_Bag.TryGetValue(setting, out val))
				return (T)val;
			else
				return defaultVal;
		}


		public void SetSetting(string setting, object val)
		{
			if (_Bag.ContainsKey(setting))
				_Bag[setting] = val;
			else
				_Bag.Add(setting, val);
		}
	}
}
