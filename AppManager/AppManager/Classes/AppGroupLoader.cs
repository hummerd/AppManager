using System;
using System.IO;
using System.Text;
using System.Xml;
using AppManager.Entities;
using DragDropLib;
using System.Globalization;
using System.Collections.Generic;


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
			var result = new AppGroup();
			var sett = new XmlReaderSettings();
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

		public static AppGroup Load2(string appDataPath, string appStatPath)
		{
			var result = new AppGroup();
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;

			if (!File.Exists(appDataPath))
				return result;

			using (XmlReader reader = XmlReader.Create(appDataPath, sett))
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

			if (!File.Exists(appStatPath))
				return result;

			var appStat = new Dictionary<int, AppRunInfoCollection>(result.AppTypes.Count * 10);
			using (XmlReader reader = XmlReader.Create(appStatPath, sett))
			{
				reader.ReadToFollowing("AppStat");
				reader.Read();

				if (reader.Name == "AppInfo")
				{
					while (reader.IsStartElement())
					{
						if (reader.Name == "AppInfo")
							ReadAppInfo(reader, appStat);
					}
				}

				if (reader.NodeType == XmlNodeType.EndElement)
					reader.ReadEndElement();
			}

			foreach (var appType in result.AppTypes)
				foreach (var app in appType.AppInfos)
				{
					AppRunInfoCollection stat;
					if (appStat.TryGetValue(app.ID, out stat))
						app.RunHistory.AddRange(stat);
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

		public static void SaveRecycleBin(string xmlPath, DeletedAppCollection recycleBin)
		{
			XmlWriterSettings sett = new XmlWriterSettings();
			sett.Indent = true;

			using (XmlWriter writer = XmlWriter.Create(xmlPath, sett))
			{
				writer.WriteStartElement("RecycleBin");

				foreach (var item in recycleBin)
				{
					WriteDeletedApp(writer, item);
				}

				writer.WriteEndElement();
			}
		}

		public static void SaveAppStat(string xmlPath, AppGroup appData)
		{
			XmlWriterSettings sett = new XmlWriterSettings();
			sett.Indent = true;

			using (XmlWriter writer = XmlWriter.Create(xmlPath, sett))
			{
				writer.WriteStartElement("AppStat");

				foreach (var appType in appData.AppTypes)
					foreach (var app in appType.AppInfos)
					{
						WriteAppStat(writer, app);
					}

				writer.WriteEndElement();
			}
		}

		public static DeletedAppCollection LoadRecycleBin(string xmlPath)
		{
			var result = new DeletedAppCollection();
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;

			if (!File.Exists(xmlPath))
				return result;

			using (XmlReader reader = XmlReader.Create(xmlPath, sett))
			{
				reader.ReadStartElement("RecycleBin");

				while (reader.IsStartElement())
				{
					reader.ReadStartElement("DeletedApp");

					var appInfo = ReadAppInfo2(reader);
					reader.Read();

					AppType appType = null;
					if (reader.IsStartElement() && reader.Name == "AppType")
						appType = ReadAppType2(reader);

					result.Add(new DeletedApp
					{
						App = appInfo,
						DeletedFrom = appType
					});

					reader.ReadEndElement();
				}
			}

			return result;
		}

		
		private static void WriteDeletedApp(XmlWriter writer, DeletedApp delApp)
		{
			writer.WriteStartElement("DeletedApp");

			WriteAppInfo(writer, delApp.App);
			if (delApp.DeletedFrom != null)
				WriteAppType(writer, delApp.DeletedFrom);

			writer.WriteEndElement();
		}

		private static void WriteAppStat(XmlWriter writer, AppInfo appInfo)
		{
			writer.WriteStartElement("AppInfo");

			writer.WriteStartAttribute("ID");
			writer.WriteValue(appInfo.ID);
			writer.WriteEndAttribute();

			foreach (var item in appInfo.RunHistory)
			{
				WriteRunEvent(writer, item);
			}

			writer.WriteEndElement();
		}

		private static void WriteRunEvent(XmlWriter writer, AppRunInfo run)
		{
			writer.WriteStartElement("Run");

			writer.WriteStartAttribute("Time");
			writer.WriteValue(run.RunTime.ToString(CultureInfo.InvariantCulture));
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("Args");
			writer.WriteValue(run.Areguments.Args);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("RunAs");
			writer.WriteValue(run.Areguments.RunAs);
			writer.WriteEndAttribute();

			writer.WriteEndElement();
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
				writer.WriteValue(appInfo.LoadImagePath);
				writer.WriteEndAttribute();
			}

			writer.WriteEndElement();
		}


		private static void ReadAppInfo(XmlReader reader, Dictionary<int, AppRunInfoCollection> appStat)
		{
			//We are at AppInfo
			reader.MoveToAttribute("ID");
			var appId = reader.ReadContentAsInt();

			reader.Read();

			if (reader.Name == "Run")
			{
				var runHistory = new AppRunInfoCollection();
				appStat[appId] = runHistory;

				while (reader.IsStartElement())
				{
					runHistory.Add(ReadRunEvent(reader));
					reader.Read();
				}
			}

			if (reader.Name == "AppInfo" && reader.NodeType == XmlNodeType.EndElement)
				reader.ReadEndElement();
		}

		private static AppRunInfo ReadRunEvent(XmlReader reader)
		{
			var result = new AppRunInfo();
			result.Areguments = new Commands.StartArgs();

			reader.MoveToNextAttribute();
			result.RunTime = DateTime.Parse(reader.ReadContentAsString(), CultureInfo.InvariantCulture);

			reader.MoveToNextAttribute();
			result.Areguments.Args = reader.ReadContentAsString();

			reader.MoveToNextAttribute();
			result.Areguments.RunAs = reader.ReadContentAsBoolean();

			return result;
		}

		private static AppType ReadAppType(XmlReader reader)
		{
			var result = new AppType();

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

			//We are at AppType
			reader.MoveToNextAttribute();
			result.AppTypeName = reader.ReadContentAsString();

			reader.Read();

			if (reader.Name == "AppInfo")
			{
				while (reader.IsStartElement())
				{
					result.AppInfos.Add(ReadAppInfo2(reader));
					reader.Read();
				}
			}

			if (reader.Name == "AppType" && reader.NodeType == XmlNodeType.EndElement)
				reader.ReadEndElement();

			return result;
		}

		private static AppInfo ReadAppInfo2(XmlReader reader)
		{
			var result = new AppInfo();

			reader.MoveToNextAttribute();
			result.ID = reader.ReadContentAsInt();

			reader.MoveToNextAttribute();
			result.AppName = reader.ReadContentAsString();

			reader.MoveToNextAttribute();
			result.ExecPath = reader.ReadContentAsString();

			reader.MoveToNextAttribute();
			if (reader.Name == "ImagePath")
				result.LoadImagePath = reader.ReadContentAsString();

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
