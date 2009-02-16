using System;


namespace CommonLib
{
	public class ValueEventArgs<T> : EventArgs
	{
		public ValueEventArgs(T value)
		{
			Value = value;
		}


		public T Value { get; set; }
	}
}
