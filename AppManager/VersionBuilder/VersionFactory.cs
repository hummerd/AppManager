using System;
using System.IO;
using System.IO.Compression;
using CommonLib;
using UpdateLib.Install;
using UpdateLib.VersionNumberProvider;


namespace VersionBuilder
{
	public class VersionFactory
	{
		public void CreateVersion(string dir, Version version, string location)
		{
			if (!Directory.Exists(dir))
				return;

			string versionDir = dir + "_" + version;

			if (Directory.Exists(versionDir))
				Directory.Delete(versionDir, true);
			
			var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
			var verManifest = new VersionManifest() 
				{ VersionNumber = version};

			foreach (var item in files)
			{
				string newPath = item.Replace(dir, versionDir) + ".gzip";
				string newDir = Path.GetDirectoryName(newPath);
				if (!Directory.Exists(newDir))
					Directory.CreateDirectory(newDir);

				var itemPath = newDir.Substring(versionDir.Length, newDir.Length - versionDir.Length);
				if (String.IsNullOrEmpty(itemPath))
					itemPath = "\\";

				verManifest.VersionItems.Add(new VersionItem()
					{
						InstallAction = InstallAction.Copy,
						Location = location + newPath.Replace(versionDir, String.Empty),
						Path = itemPath
					});
				
				CompressFile(item, newPath);
			}

			XmlSerializeHelper.SerializeItem(
				verManifest, 
				Path.Combine(versionDir, VersionManifest.VersionManifestFileName));

			XmlSerializeHelper.SerializeItem(
				new VersionInfo(version),
				Path.Combine(versionDir, VersionManifest.VersionFileName));
		}
		

		protected void CompressFile(string path, string outPath)
		{
			if (!File.Exists(path))
				return;

			var buff = File.ReadAllBytes(path);

			if (File.Exists(outPath))
				File.Delete(outPath);

			using (FileStream compressedFile = new FileStream(outPath, FileMode.CreateNew, FileAccess.Write))
			using (GZipStream compressedzipStream = new GZipStream(compressedFile, CompressionMode.Compress, false))
			{
				compressedzipStream.Write(buff, 0, buff.Length);
			}
		}
	}
}
