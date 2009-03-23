using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UpdateLib.VersionNumberProvider;


namespace UpdateLib.Install
{
	[Serializable]
	public class InstallManifest
	{
		public const string FileName = "instman.xml";

		public static InstallManifest LoadFromCurrentDirectory()
		{
			string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			path = Path.Combine(path, InstallManifest.FileName);

			InstallManifest result = null;
			XmlSerializer ser = new XmlSerializer(typeof(InstallManifest));

			using (FileStream fs = new FileStream(path, FileMode.Open))
				result = ser.Deserialize(fs) as InstallManifest;

			return result;
		}


		public InstallManifest()
		{

		}

		public InstallManifest(string appDir, string tempDir, VersionManifest versionManifest)
		{
			InstallItems = new InstallItemList();

			foreach (var item in versionManifest.VersionItems)
			{
				InstallItem ii = new InstallItem()
				{
					SrcPath = Path.Combine(tempDir, item.Path),
					DstPath = Path.Combine(appDir, item.Path)
				};

				InstallItems.Add(ii);
			}
		}


		public InstallItemList InstallItems { get; set; }


		public void Save(string path)
		{
			XmlSerializer ser = new XmlSerializer(typeof(InstallManifest));

			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
				ser.Serialize(fs, this);
		}
	}
}
