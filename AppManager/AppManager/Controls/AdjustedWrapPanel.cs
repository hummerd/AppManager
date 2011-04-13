using System.Windows;
using System.Windows.Controls;


namespace AppManager.Controls
{
	public class AdjustedWrapPanel : Panel
	{
		protected override Size MeasureOverride(Size constraint)
		{
			foreach (UIElement child in InternalChildren)
			{
				child.Measure(constraint);
			}

			var maxHeight = GetMaxHeight();
			var maxWidth = maxHeight * 1.63;

			double y = 0.0;
			double x = 0.0;
			foreach (UIElement child in InternalChildren)
			{
				x += maxWidth;
				if (x > constraint.Width)
				{
					x = 0;
					y += maxHeight;
				}
			}

			y += maxHeight;

			return new Size(constraint.Width, y);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var maxHeight = GetMaxHeight();
			var maxWidth = maxHeight * 1.63;

			var point = new Point();
			foreach (UIElement child in InternalChildren)
			{
				((ListBoxItem)child).HorizontalContentAlignment = HorizontalAlignment.Stretch;
				((ListBoxItem)child).VerticalContentAlignment = VerticalAlignment.Stretch;

				child.Arrange(new Rect(point, new Size(maxWidth, maxHeight)));
				point = new Point(point.X + maxWidth, point.Y);
				if (point.X + maxWidth > arrangeBounds.Width)
				{
					point.X = 0;
					point.Y += maxHeight;
				}
			}
			
			return base.ArrangeOverride(arrangeBounds); // Returns the final Arranged size
		}


		protected double GetMaxHeight()
		{
			double maxHeight = 0.0;
			foreach (UIElement child in InternalChildren)
				maxHeight = maxHeight < child.DesiredSize.Height ?
					child.DesiredSize.Height :
					maxHeight;

			return maxHeight;
		}
	}
}
