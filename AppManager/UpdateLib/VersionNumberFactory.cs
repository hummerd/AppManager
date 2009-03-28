using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionInfo;


namespace UpdateLib
{
	public class VersionNumberFactory : IVersionNumberFactory
	{
		#region IVersionNumberFactory Members

		public IVersionNumberProvider GetVNP(string uriScheme)
		{
			if (uriScheme == Uri.UriSchemeFile)
				return new ShareUpdate.ShareVNP();
			else if (uriScheme == Uri.UriSchemeHttp)
				return new WebUpdate.WebVNP();

			return null;
		}

		#endregion
	}
}
