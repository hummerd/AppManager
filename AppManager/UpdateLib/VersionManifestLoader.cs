using System;
using System.IO;
using System.Xml;
using UpdateLib.VersionInfo;


namespace UpdateLib
{
	public static class VersionManifestLoader
	{
		public static VersionManifest Load(TextReader txtReader)
		{
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;
			using (XmlReader reader = XmlReader.Create(txtReader, sett))
			{
				return ReadVersionManifest(reader);
			}
		}

		public static VersionManifest Load(string path)
		{			
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;
			using (XmlReader reader = XmlReader.Create(path, sett))
			{
				return ReadVersionManifest(reader);
			}
		}

		public static VersionData LoadData(TextReader txtReader)
		{
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;
			using (XmlReader reader = XmlReader.Create(txtReader, sett))
			{
				return ReadVersionData(reader);
			}
		}

		public static VersionData LoadData(string path)
		{
			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			sett.IgnoreComments = true;
			using (XmlReader reader = XmlReader.Create(path, sett))
			{
				return ReadVersionData(reader);
			}
		}	


		private static VersionManifest ReadVersionManifest(XmlReader reader)
		{
			VersionManifest result = new VersionManifest();

			reader.ReadStartElement("VersionManifest");

			reader.ReadStartElement("VersionNumberString");
			result.VersionNumberString = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadStartElement("VersionItems");

			//reader.Read();

			if (reader.Name == "VersionItem")
			{
				while (reader.IsStartElement())
				{
					result.VersionItems.Add(ReadVersionItem(reader));
				}
			}

			if (reader.NodeType == XmlNodeType.EndElement)
				reader.ReadEndElement();

			reader.ReadStartElement("UpdateUri");
			result.UpdateUri = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadStartElement("UpdateUriAlt");
			result.UpdateUriAlt = reader.ReadContentAsString();
			reader.ReadEndElement();

			return result;
		}

		private static VersionItem ReadVersionItem(XmlReader reader)
		{
			var result = new VersionItem();

			reader.ReadStartElement("VersionItem");

			reader.ReadStartElement("Location");
			result.Location = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadStartElement("Path");
			if (reader.NodeType == XmlNodeType.Text)
			{
				result.Path = reader.ReadContentAsString();
				reader.ReadEndElement();
			}
			else
				result.Path = String.Empty;

			reader.ReadStartElement("InstallAction");
			result.InstallAction = (InstallAction)Enum.Parse(typeof(InstallAction), reader.ReadContentAsString());
			reader.ReadEndElement();

			reader.ReadStartElement("VersionNumberString");
			result.VersionNumberString = reader.ReadContentAsString();
			reader.ReadEndElement();
			
			reader.ReadStartElement("Base64Hash");
			result.Base64Hash = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadEndElement();

			return result;
		}

		private static VersionData ReadVersionData(XmlReader reader)
		{
			var result = new VersionData();

			reader.ReadStartElement("VersionData");

			reader.ReadStartElement("VersionNumberString");
			result.VersionNumberString = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadStartElement("Description");
			if (reader.NodeType == XmlNodeType.Text)
			{
				result.Description = reader.ReadContentAsString();
				reader.ReadEndElement();
			}

			if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Description")
				reader.ReadEndElement();

			reader.ReadEndElement();

			return result;
		}
	}
}
