using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace AppManager.Controls
{
	public class LayoutPanel : Panel
	{
		public LayoutPanel()
		{
		}


		protected override Size MeasureOverride(Size availableSize)
		{
			//return base.MeasureOverride(availableSize);

			Size childSize = availableSize;

			foreach (UIElement child in Children)
				child.Measure(childSize);

			if (double.IsPositiveInfinity(availableSize.Height))
				availableSize.Height = GetDesiredHeight(availableSize.Width);

			//return base.MeasureOverride(availableSize);
			return availableSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			Point childPos = new Point(0, 0);

			for (int i = 0; i < Children.Count; i++)
			{
				UIElement child = Children[i];
				child.Arrange(new Rect(childPos.X, childPos.Y, child.DesiredSize.Width, child.DesiredSize.Height));

				if (i + 1 < Children.Count)
				{
					UIElement nextChild = Children[i + 1];

					if (childPos.X + child.DesiredSize.Width + nextChild.DesiredSize.Width > finalSize.Width)
					{
						childPos.Y = childPos.Y + child.DesiredSize.Height;
						childPos.X = 0.0;
					}
					else
						childPos.X = childPos.X + child.DesiredSize.Width;
				}
			}

			return base.ArrangeOverride(finalSize);
		}

		protected double GetDesiredHeight(double avialableWidth)
		{
			Point childPos = new Point(0, 0);

			for (int i = 0; i < Children.Count; i++)
			{
				UIElement child = Children[i];
				//child.Arrange(new Rect(childPos.X, childPos.Y, child.DesiredSize.Width, child.DesiredSize.Height));

				if (i + 1 < Children.Count)
				{
					UIElement nextChild = Children[i + 1];

					if (childPos.X + child.DesiredSize.Width + nextChild.DesiredSize.Width > avialableWidth)
					{
						childPos.Y = childPos.Y + child.DesiredSize.Height;
						childPos.X = 0.0;
					}
					else
						childPos.X = childPos.X + child.DesiredSize.Width;
				}
			}

			double lastHeight = 0.0;
			if (Children.Count > 0)
				lastHeight = Children[Children.Count - 1].DesiredSize.Height;

			return childPos.Y + lastHeight;
		}
	}
}
