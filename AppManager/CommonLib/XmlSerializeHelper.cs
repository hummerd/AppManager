using System;
using System.IO;
using System.Xml.Serialization;


namespace CommonLib
{
	public static class XmlSerializeHelper
	{
		public static string SerializeItem(object obj)
		{
			string result;
			XmlSerializer xser = new XmlSerializer(obj.GetType());

			using (StringWriter sr = new StringWriter())
			{
				xser.Serialize(sr, obj);
				result = sr.ToString();
			}

			return result;
		}

		public static object DeserializeItem(string obj, Type dataType)
		{
			XmlSerializer xser = new XmlSerializer(dataType);

			object result;
			using (TextReader xr = new StringReader(obj))
			{
				result = xser.Deserialize(xr);
			}

			return result;
		}
	}
}
