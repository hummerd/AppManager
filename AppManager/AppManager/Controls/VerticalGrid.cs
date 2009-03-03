using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;


namespace AppManager.Controls
{
	public class VerticalGrid : Grid
	{
		//protected int _SpltterCount = 0;


		public VerticalGrid()
		{
		}


		public double MinRowHeight
		{ get; set; }


		protected override Size MeasureOverride(Size constraint)
		{
			int ch = Children.Count;
			if (RowDefinitions.Count < ch)
			{
				int d = Children.Count - RowDefinitions.Count;
				int ix = Children.Count - d;

				for (int i = 0; i < d; i++)
				{
					RowDefinitions.Add(new RowDefinition()
						{
							Height = new GridLength(100.0, GridUnitType.Star),
							MinHeight = MinRowHeight
						});
					SetRow(Children[ix], ix);
					ix++;
				}
			}
			else
			{
				int childCount = Children.Count;
				var distRows = new Dictionary<int, object>();

				for (int i = 0; i < childCount; i++)
				{
					int row = GetRow(Children[i]);
					if (distRows.ContainsKey(row))
						SetRow(Children[i], i);
					else
						distRows.Add(row, null);
				}
			}

			return base.MeasureOverride(constraint);
		}
	}
}
