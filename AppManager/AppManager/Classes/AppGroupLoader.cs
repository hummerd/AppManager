using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace AppManager
{
	public static class AppGroupLoader
	{
		public static AppGroup Load(string xmlPath)
		{
			AppGroup result = new AppGroup();
			using (XmlReader reader = XmlReader.Create(xmlPath))
			{
				reader.Read();
				reader.ReadStartElement("AppGroup");
				reader.ReadStartElement("AppTypes");

				while (reader.IsStartElement())
				{
					result.AppTypes.Add(ReadAppType(reader));
				}

				reader.ReadEndElement();

				reader.ReadStartElement("LastAppInfoID");
				result.LastAppInfoID = reader.ReadContentAsInt();
				reader.ReadEndElement();

				reader.ReadEndElement();
			}

			return result;
		}

		private static AppType ReadAppType(XmlReader reader)
		{
			AppType result = new AppType();

			reader.ReadStartElement("AppType");

			reader.ReadStartElement("AppTypeName");
			result.AppTypeName = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadStartElement("AppInfos");

			while (reader.IsStartElement())
			{
				result.AppInfos.Add(ReadAppInfo(reader));
			}

			reader.ReadEndElement();

			reader.ReadEndElement();
			return result;
		}

		private static AppInfo ReadAppInfo(XmlReader reader)
		{
			AppInfo result = new AppInfo();

			reader.ReadStartElement("AppInfo");

			reader.ReadStartElement("AppInfoID");
			result.AppInfoID = reader.ReadContentAsInt();
			reader.ReadEndElement();

			reader.ReadStartElement("AppName");
			result.AppName = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadStartElement("ExecPath");
			result.ExecPath = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadEndElement();

			return result;
		}
	}
}
