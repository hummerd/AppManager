using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;


namespace CommonLib.IO
{
	public class FileHash
	{
		public static string GetBase64FileHash(string path)
		{
			return Convert.ToBase64String(GetFileHash(path));
		}

		public static byte[] GetFileHash(string path)
		{
			MD5 md5Hasher = MD5.Create();
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				return md5Hasher.ComputeHash(fs);
		}
	}
}
