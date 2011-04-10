using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace AppManager.Controls
{
	public class BorderedPanel : UserControl
	{
		public static readonly DependencyProperty FrameBrushProperty =
			DependencyProperty.Register("FrameBrush", typeof(Brush), typeof(BorderedPanel), new UIPropertyMetadata(Brushes.Transparent));

		public static readonly DependencyProperty TitleTextProperty =
			DependencyProperty.Register("TitleText", typeof(string), typeof(BorderedPanel), new UIPropertyMetadata(String.Empty));


		protected ContentControl m_Header;
		protected object m_HeaderContent;
		protected Pen m_FramePen;


		public BorderedPanel()
		{
		}


		public Brush FrameBrush
		{
			get { return (Brush)GetValue(FrameBrushProperty); }
			set { SetValue(FrameBrushProperty, value); }
		}

		public string TitleText
		{
			get { return (string)GetValue(TitleTextProperty); }
			set { SetValue(TitleTextProperty, value); }
		}


		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == FrameBrushProperty)
			{
				m_FramePen = new Pen(FrameBrush, 1.0);
			}
			else if (e.Property == TitleTextProperty)
			{
				m_HeaderContent = e.NewValue;
				if (m_Header != null)
					m_Header.Content = m_HeaderContent;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			m_Header = (ContentControl)GetTemplateChild("Header");
			m_Header.Content = m_HeaderContent;
		}


		protected override Size MeasureOverride(Size constraint)
		{
			var r = base.MeasureOverride(constraint);
			return r;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var r = base.ArrangeOverride(arrangeBounds);
			return r;
		}


		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			
			int s = 1;
			int r = 4;
			int rs = s + r;
			var top = m_Header.RenderSize.Height / 2;
			var radius = new Size(r, r);
			

			// left - top
			DrawArc(drawingContext, null, m_FramePen,
				new Point(s, r + top),
				new Point(rs, top),
				radius);

			// right top
			DrawArc(drawingContext, null, m_FramePen,
				new Point(RenderSize.Width - rs, top),
				new Point(RenderSize.Width - s, r + top),
				radius);

			// right bottom
			DrawArc(drawingContext, null, m_FramePen,
				new Point(RenderSize.Width - s, RenderSize.Height - rs),
				new Point(RenderSize.Width - rs, RenderSize.Height - s),
				radius);

			// bottom left
			DrawArc(drawingContext, null, m_FramePen,
				new Point(rs, RenderSize.Height - s),
				new Point(s, RenderSize.Height - rs),
				radius);

			//top left
			DrawLine(drawingContext, null, m_FramePen,
				new Point(rs, top),
				new Point(8, top)
				);
			//top right
			DrawLine(drawingContext, null, m_FramePen,
				new Point(8 + m_Header.RenderSize.Width, top),
				new Point(RenderSize.Width - rs, top)
				);

			//left
			DrawLine(drawingContext, null, m_FramePen,
				new Point(s, r + top),
				new Point(s, RenderSize.Height - rs)
				);
			//right
			DrawLine(drawingContext, null, m_FramePen,
				new Point(RenderSize.Width - s, r + top),
				new Point(RenderSize.Width - s, RenderSize.Height - rs)
				);
			//bottom
			DrawLine(
				drawingContext, null, m_FramePen,
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
