using System;
using System.Collections.Generic;
using System.Text;


namespace UpdateLib
{
	public class UpdateException : Exception
	{
		public UpdateException()
		{

		}

		public UpdateException(string msg)
			: base(msg)
		{

		}
	}
}
