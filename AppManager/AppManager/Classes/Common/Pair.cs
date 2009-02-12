using System;
using System.Collections.Generic;
using System.Text;


namespace AppManager.Common
{
	public class Pair<TFirst, TSecond>
	{
		public TFirst First { get; set; }
		public TSecond Second { get; set; }
	}
}
