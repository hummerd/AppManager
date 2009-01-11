using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using AppManager.Classes.Common;


namespace AppManager.Settings
{
	public class XMLSettingsLoader : ISettingProvider
	{
		#region ISettingProvider Members

		public virtual Dictionary<string, object> LoadSettings(string path)
		{
			Dictionary<string, object> result;

			if (!File.Exists(path))
				return new Dictionary<string, object>();

			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				if (fileStream.Length == 0)
					result = new Dictionary<string,object>();
				else
				{
					try
					{
						XmlSerializer serializer = new XmlSerializer(
							typeof(DictionarySerializer<string, object>));
						var ds = serializer.Deserialize(fileStream)
							as DictionarySerializer<string, object>;

						if (ds != null)
							result = ds.Source;
						else
							result = new Dictionary<string, object>();
					}
					catch
					{
						result = new Dictionary<string, object>();
					}
				}
			}

			return result;
		}

		public virtual void SaveSettings(string path, Dictionary<string, object> settings)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				XmlSerializer serializer = new XmlSerializer(
					typeof(DictionarySerializer<string, object>));
				serializer.Serialize(fileStream, 
					new DictionarySerializer<string, object>(settings));
			}
		}

		#endregion
	}
}
