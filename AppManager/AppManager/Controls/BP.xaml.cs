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
	/// <summary>
	/// Interaction logic for BP.xaml
	/// </summary>
	public partial class BP : UserControl
	{
		public BP()
		{
			InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			
			Size radius = new Size(this.RenderSize.Width / 2, this.RenderSize.Height / 2);

			int s = 6;
			int r = 4;
			int rs = s + r;
			var top = 24 / 2;

			var p = new Pen(Brushes.Blue, 1);

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

			base.OnRender(drawingContext);
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
			// setup the geometry object
			LineGeometry geometry = new LineGeometry(start, end);
			// draw the line
			drawingContext.DrawGeometry(brush, pen, geometry);
		}
	}
}
