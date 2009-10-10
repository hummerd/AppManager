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
			UIElementCollection children = Children;
			RowDefinitionCollection rowDefs = RowDefinitions;
			int chCount = children.Count;
			int rdCount = rowDefs.Count;

			if (rdCount < chCount)
			{
				int d = chCount - rdCount;
				int ix = chCount - d;

				for (int i = 0; i < d; i++)
				{
					rowDefs.Add(new RowDefinition()
						{
							Height = new GridLength(100.0, GridUnitType.Star),
							MinHeight = MinRowHeight
						});
					//SetRow(children[ix], ix);
					//ix++;
				}

				for (int i = 0; i < chCount; i++)
				{
					SetRow(children[i], i);
				}
			}
			else if (rdCount > chCount)
			{
				int chIx = chCount - 1;

				if (chIx < 0)
					rowDefs.Clear();
				else
				{
					for (int i = rdCount - 1; i >= 0; i--)
					{
						if (chIx < 0)
						{
							rowDefs.RemoveAt(i);
							continue;							
						}

						int row = GetRow(children[chIx]);
						if (row != i)
						{
							rowDefs.RemoveAt(i);
							continue;
						}

						chIx--;
					}

					for (int i = 0; i < chCount; i++)
					{
						int row = GetRow(children[i]);
						if (row != i)
							SetRow(children[i], i);
					}
				}
			}
			else
			{
				for (int i = 0; i < chCount; i++)
				{
					int row = GetRow(children[i]);
					if (row != i)
						SetRow(children[i], i);
				}
			}

			return base.MeasureOverride(constraint);
		}
	}
}
