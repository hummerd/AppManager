using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;


namespace AppManager.Controls
{
	public class AdjustedWrapPanel : WrapPanel
	{
		protected override Size MeasureOverride(Size constraint)
		{
			base.MeasureOverride(constraint);
			double childHeight = 0.0;
			foreach (UIElement child in InternalChildren)
				childHeight = childHeight < child.DesiredSize.Height ?
					child.DesiredSize.Height :
					childHeight;

			ItemHeight = childHeight;
			ItemWidth = childHeight * 1.63;

			//foreach (UIElement child in InternalChildren)
			//{
			//    child.Measure(new Size(childHeight * 1.63, childHeight));
			//    child.Arrange(new Rect(0, 0, childHeight * 1.63, childHeight));
			//}

			return base.MeasureOverride(constraint);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			//foreach (UIElement child in InternalChildren)
			//{
			//    child.Measure(new Size(childHeight * 1.63, childHeight));
			//    child.Arrange(new Rect(0, 0, childHeight * 1.63, childHeight));
			//}

			//double maxHeight = 0.0;
			//foreach (UIElement child in InternalChildren)
			//    maxHeight = maxHeight < child.DesiredSize.Height ? 
			//        child.DesiredSize.Height :
			//        maxHeight;

			//ItemHeight = maxHeight;
			//ItemWidth = maxHeight * 1.63;

			return base.ArrangeOverride(arrangeBounds); // Returns the final Arranged size
		}
	}
}
