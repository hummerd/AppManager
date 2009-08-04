using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using CommonLib;


namespace UpdateLib.VersionInfo
{
	public class VersionManifest
	{
		public const string VersionFileName = "AppVersion.{0}.xml";
		public const string VersionManifestFileName = "VersionManifest.xml";
		public const string DownloadedVersionManifestFileName = "DownloadedVersionManifest.xml";


		public VersionManifest()
		{
			VersionItems = new VersionItemList();
			VersionNumber = new Version();
		}


		public Version VersionNumber
		{ get; set; }

		public string VersionNumberString
		{
			get
			{
				return VersionNumber.ToString();
			}
			set
			{
				VersionNumber = new Version(value);
			}
		}

		public VersionItemList VersionItems
		{ get; set; }

		public string UpdateUri
		{ get; set; }

		public string UpdateUriAlt
		{ get; set; }


		public VersionManifest GetUpdateManifest(VersionManifest currentVersionManifest)
		{
			VersionManifest result = new VersionManifest();
			result.VersionNumberString = VersionNumberString;

			foreach (var item in VersionItems)
			{
				var existing = currentVersionManifest.VersionItems.Find(
					vi => String.Equals(item.GetItemFullPath(), vi.GetItemFullPath()));

				if (existing == null || item.VersionNumber > existing.VersionNumber)
					result.VersionItems.Add(item);
			}

			return result;
		}

		public Uri GetUpdateUriLocal()
		{
			return GetLocalUri(UpdateUri);
		}

		public Uri GetUpdateUriAltLocal()
		{
			return GetLocalUri(UpdateUriAlt);
		}

		public Uri GetManifestUri(bool alt)
		{
			return new Uri(PathHelper.ConcatUri(
				alt ? UpdateUriAlt : UpdateUri,
				VersionManifestFileName
				));
		}


		protected Uri GetLocalUri(string uri)
		{
			if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
				return null;

			return new Uri(PathHelper.ConcatUri(
				uri, 
				String.Format(VersionFileName, CultureInfo.CurrentCulture.Parent.Name)
				));
		}
	}
}
