using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;


namespace CommonLib.IO
{
	public class GZipCompression
	{
		public static void CompressFile(string path, string outPath)
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

		public static void DecompressFile(string path, string outPath)
		{
			if (!File.Exists(path))
				return;

			if (File.Exists(outPath))
				File.Delete(outPath);

			using (FileStream compressedFile = new FileStream(path, FileMode.Open, FileAccess.Read))
			using (GZipStream decompressedzipStream = new GZipStream(compressedFile, CompressionMode.Decompress, false))
			using (FileStream decompressedFile = new FileStream(outPath, FileMode.Create, FileAccess.Write))
			{
				int buffSize = 4096;
				byte[] buff = new byte[buffSize];

				int readCount = 0;
				while ((int)(readCount = decompressedzipStream.Read(buff, 0, buffSize)) > 0)
					decompressedFile.Write(buff, 0, readCount);
			}
		}
	}
}
