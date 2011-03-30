using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppManager.Controls
{
	public class BorderedPanel : UserControl
	{
		protected FrameworkElement m_Header;


		public BorderedPanel()
		{
		}


		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			m_Header = (FrameworkElement)GetTemplateChild("HeaderText");
		}


		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			int s = 1;
			int r = 4;
			int rs = s + r;
			var top = m_Header.RenderSize.Height / 2;
			var radius = new Size(r, r);

			var b = new SolidColorBrush(Color.FromArgb(0xFF, 0xDF, 0xCB, 0x9A));
			var p = new Pen(b, 1.0);

			// left - top
			DrawArc(drawingContext, null, p,
				new Point(s, r + top),
				new Point(rs, top),
				radius);

			// right top
			DrawArc(drawingContext, null, p,
				new Point(RenderSize.Width - rs, top),
				new Point(RenderSize.Width - s, r + top),
				radius);

			// right bottom
			DrawArc(drawingContext, null, p,
				new Point(RenderSize.Width - s, RenderSize.Height - rs),
				new Point(RenderSize.Width - rs, RenderSize.Height - s),
				radius);

			// bottom left
			DrawArc(drawingContext, null, p,
				new Point(rs, RenderSize.Height - s),
				new Point(s, RenderSize.Height - rs),
				radius);

			//top left
			DrawLine(drawingContext, null, p,
				new Point(rs, top),
				new Point(8, top)
				);
			//top right
			DrawLine(drawingContext, null, p,
				new Point(8 + m_Header.RenderSize.Width, top),
				new Point(RenderSize.Width - rs, top)
				);

			//left
			DrawLine(drawingContext, null, p,
				new Point(s, r + top),
				new Point(s, RenderSize.Height - rs)
				);
			//right
			DrawLine(drawingContext, null, p,
				new Point(RenderSize.Width - s, r + top),
				new Point(RenderSize.Width - s, RenderSize.Height - rs)
				);
			//bottom
			DrawLine(
				drawingContext, null, p,
				new Point(RenderSize.Width - rs, RenderSize.Height - s),
				new Point(rs, RenderSize.Height - s)
				);
			
		}

		protected void DrawArc(DrawingContext drawingContext, Brush brush,
			Pen pen, Point start, Point end, Size radius)
		{
			// setup the geometry object
			PathGeometry geometry = new PathGeometry();
			PathFigure figure = new PathFigure();
			geometry.Figures.Add(figure);
			figure.StartPoint = start;

			// add the arc to the geometry
			figure.Segments.Add(new ArcSegment(end, radius,
				0, false, SweepDirection.Clockwise, true));

			// draw the arc
			drawingContext.DrawGeometry(brush, pen, geometry);
		}

		protected void DrawLine(DrawingContext drawingContext, Brush brush,
			Pen pen, Point start, Point end)
		{

			double halfPenWidth = pen.Thickness / 2;

			// Create a guidelines set
			GuidelineSet guidelines = new GuidelineSet();
			guidelines.GuidelinesX.Add(start.X + halfPenWidth);
			guidelines.GuidelinesX.Add(end.X + halfPenWidth);
			guidelines.GuidelinesY.Add(start.Y + halfPenWidth);
			guidelines.GuidelinesY.Add(end.Y + halfPenWidth);

			// setup the geometry object
			LineGeometry geometry = new LineGeometry(start, end);
			// draw the line
			drawingContext.PushGuidelineSet(guidelines);
			drawingContext.DrawGeometry(brush, pen, geometry);
			drawingContext.Pop();
		}
	}
}
