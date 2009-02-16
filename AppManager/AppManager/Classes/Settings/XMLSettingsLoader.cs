using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace AppManager.Settings
{
	public class XMLSettingsLoader<TSettings> : ISettingProvider<TSettings>
		where TSettings : class, new()
	{
		#region ISettingProvider Members

		public virtual TSettings LoadSettings(string path)
		{
			TSettings result;

			if (!File.Exists(path))
				return new TSettings();

			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				if (fileStream.Length == 0)
					result = new TSettings();
				else
				{
					try
					{
						XmlSerializer serializer = new XmlSerializer(typeof(TSettings));
						var ds = serializer.Deserialize(fileStream) as TSettings;

						if (ds != null)
							result = ds;
						else
							result = new TSettings();
					}
					catch
					{
						result = new TSettings();
					}
				}
			}

			return result;
		}

		public virtual void SaveSettings(string path, TSettings settings)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(TSettings));
				serializer.Serialize(fileStream, settings);
			}
		}

		#endregion
	}
}
