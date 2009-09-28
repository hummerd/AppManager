using System;
using System.Xml;
using System.Text;
using System.IO;
using DragDropLib;


namespace AppManager
{
	public class AppGroupLoader : IObjectSerializer
	{
		protected static AppGroupLoader _Instance;

		public static AppGroupLoader Default
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new AppGroupLoader();
				}

				return _Instance;
			}
		}


		public static void Save(string xmlPath, AppGroup appGroup)
		{
			XmlWriterSettings sett = new XmlWriterSettings();
			sett.Indent = true;
			using (XmlWriter writer = XmlWriter.Create(xmlPath, sett))
			{
				//writer.Settings.NewLineHandling = NewLineHandling.

				writer.WriteStartElement("AppGroup");

				writer.WriteStartAttribute("Name");
				writer.WriteValue(appGroup.AppGroupName ?? String.Empty);
				writer.WriteEndAttribute();

				writer.WriteStartAttribute("LastAppInfoID");
				writer.WriteValue(appGroup.LastAppInfoID);
				writer.WriteEndAttribute();

				foreach (var item in appGroup.AppTypes)
				{
					WriteAppType(writer, item);
				}
		
				writer.WriteEndElement();
			}
		}

		public static AppGroup Load(string xmlPath)
		{
			AppGroup result = new AppGroup();
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;
			using (XmlReader reader = XmlReader.Create(xmlPath, sett))
			{
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

		public static AppGroup Load2(string xmlPath)
		{
			AppGroup result = new AppGroup();
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;
			using (XmlReader reader = XmlReader.Create(xmlPath, sett))
			{
				reader.ReadToFollowing("AppGroup");
								
				reader.MoveToAttribute("Name");
				result.AppGroupName = reader.ReadContentAsString();

				reader.MoveToAttribute("LastAppInfoID");
				result.LastAppInfoID = reader.ReadContentAsInt();

				reader.Read();

				if (reader.Name == "AppType")
				{
					while (reader.IsStartElement())
					{
						result.AppTypes.Add(ReadAppType2(reader));
					}
				}

				if (reader.NodeType == XmlNodeType.EndElement)
					reader.ReadEndElement();
			}

			return result;
		}

		public static string SaveAppType(AppType appType)
		{
			XmlWriterSettings sett = new XmlWriterSettings();
			sett.Indent = true;
			StringBuilder result = new StringBuilder(500);
			using (XmlWriter writer = XmlWriter.Create(result, sett))
			{
				WriteAppType(writer, appType);
			}

			return result.ToString();
		}

		public static AppType LoadAppType(string xml)
		{
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;
			using (XmlReader reader = XmlReader.Create(new StringReader(xml), sett))
			{
				reader.ReadToFollowing("AppType");
				return ReadAppType2(reader);
			}
		}

		public static string SaveAppInfo(AppInfo appInfo)
		{
			XmlWriterSettings sett = new XmlWriterSettings();
			sett.Indent = true;
			StringBuilder result = new StringBuilder(500);
			using (XmlWriter writer = XmlWriter.Create(result, sett))
			{
				WriteAppInfo(writer, appInfo);
			}

			return result.ToString();
		}

		public static AppInfo LoadAppInfo(string xml)
		{
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;
			using (XmlReader reader = XmlReader.Create(new StringReader(xml), sett))
			{
				reader.ReadToFollowing("AppInfo");
				return ReadAppInfo2(reader);
			}
		}


		private static void WriteAppType(XmlWriter writer, AppType appType)
		{
			writer.WriteStartElement("AppType");

			writer.WriteStartAttribute("Name");
			writer.WriteValue(appType.AppTypeName ?? String.Empty);
			writer.WriteEndAttribute();

			foreach (var item in appType.AppInfos)
			{
				WriteAppInfo(writer, item);
			}

			writer.WriteEndElement();
		}

		private static void WriteAppInfo(XmlWriter writer, AppInfo appInfo)
		{
			writer.WriteStartElement("AppInfo");

			writer.WriteStartAttribute("ID");
			writer.WriteValue(appInfo.ID);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("Name");
			writer.WriteValue(appInfo.AppName ?? String.Empty);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("ExecPath");
			writer.WriteValue(appInfo.ExecPath ?? String.Empty);
			writer.WriteEndAttribute();

			if (appInfo.HasImagePath)
			{
				writer.WriteStartAttribute("ImagePath");
				writer.WriteValue(appInfo.ImagePath);
				writer.WriteEndAttribute();
			}

			writer.WriteEndElement();
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
			result.ID = reader.ReadContentAsInt();
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

		private static AppType ReadAppType2(XmlReader reader)
		{
			AppType result = new AppType();

			//reader.ReadStartElement("AppType");

			reader.MoveToAttribute("Name");
			result.AppTypeName = reader.ReadContentAsString();

			reader.Read();

			if (reader.Name == "AppInfo")
			{
				while (reader.IsStartElement())
				{
					result.AppInfos.Add(ReadAppInfo2(reader));
				}
			}

			if (reader.NodeType == XmlNodeType.EndElement)
				reader.ReadEndElement();

			return result;
		}

		private static AppInfo ReadAppInfo2(XmlReader reader)
		{
			AppInfo result = new AppInfo();

			//reader.ReadStartElement("AppInfo");

			reader.MoveToAttribute("ID");
			result.ID = reader.ReadContentAsInt();

			reader.MoveToAttribute("Name");
			result.AppName = reader.ReadContentAsString();

			reader.MoveToAttribute("ExecPath");
			result.ExecPath = reader.ReadContentAsString();

			reader.MoveToAttribute("ImagePath");
			if (reader.Name == "ImagePath")
				result.ImagePath = reader.ReadContentAsString();

			reader.Read();

			return result;
		}


		#region IObjectSerializer Members

		public object Serialize(object obj)
		{
			if (obj.GetType() == typeof(AppInfo))
				return SaveAppInfo(obj as AppInfo);
			else if (obj.GetType() == typeof(AppType))
				return SaveAppType(obj as AppType);
			else
				throw new NotImplementedException();
		}

		public object Deserialize(object data, Type objType)
		{
			if (objType == typeof(AppInfo))
				return LoadAppInfo(data.ToString());
			else if (objType == typeof(AppType))
				return LoadAppType(data.ToString());
			else
				throw new NotImplementedException();
		}

		#endregion
	}
}
