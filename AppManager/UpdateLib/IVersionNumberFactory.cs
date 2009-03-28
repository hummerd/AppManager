using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionInfo;


namespace UpdateLib
{
	public interface IVersionNumberFactory
	{
		IVersionNumberProvider GetVNP(string uriScheme);
	}
}
