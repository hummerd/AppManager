using System.Windows;
using System.Windows.Controls;


namespace AppManager.Controls
{
	public class VerticalGrid : Grid
	{
		protected int _SpltterCount = 0;


		public VerticalGrid()
		{
		}


		protected override Size MeasureOverride(Size constraint)
		{
			int ch = Children.Count - _SpltterCount;
			if (RowDefinitions.Count < ch)
			{
				int d = Children.Count - RowDefinitions.Count;
				int ix = Children.Count - d;

				for (int i = 0; i < d; i++)
				{
					RowDefinitions.Add(new RowDefinition());

					SetRow(Children[ix], ix);

					var gs = new GridSplitter()
					{
						ResizeDirection = GridResizeDirection.Rows,
						Height = 3,
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						ShowsPreview = true,
						Background = System.Windows.Media.Brushes.Transparent
					};

					Children.Add(gs);
					_SpltterCount++;
					SetRow(gs, ix);

					ix++;
				}
			}

			return base.MeasureOverride(constraint);
		}
	}
}
