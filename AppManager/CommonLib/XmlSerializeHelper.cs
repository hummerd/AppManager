using System;
using System.IO;
using System.Xml.Serialization;


namespace CommonLib
{
	public static class XmlSerializeHelper
	{
		public static string SerializeItem(object obj)
		{
			if (obj == null)
				return String.Empty;

			string result;
			XmlSerializer xser = new XmlSerializer(obj.GetType());

			using (StringWriter sr = new StringWriter())
			{
				xser.Serialize(sr, obj);
				result = sr.ToString();
			}

			return result;
		}

		public static void SerializeItem(object obj, string path)
		{
			if (obj == null)
				return;

			XmlSerializer xser = new XmlSerializer(obj.GetType());

			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				xser.Serialize(fs, obj);
			}
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

		public static object DeserializeItem(Type dataType, string path)
		{
			XmlSerializer xser = new XmlSerializer(dataType);

			object result;
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				result = xser.Deserialize(fs);
			}

			return result;
		}
	}
}
