using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
	public static class MathHelper
	{
		public static double[] Normilize(double[] array, double to)
		{
			if (array == null)
				return null;

			double max = Max(array);
			for (int i = 0; i < array.Length; i++)
				array[i] = array[i] * to / max;

			return array;
		}

		public static double Max(IEnumerable<double> nums)
		{
			double result = double.MinValue;
			foreach (var item in nums)
				result = Math.Max(result, item);

			return result;
		}
	}
}
