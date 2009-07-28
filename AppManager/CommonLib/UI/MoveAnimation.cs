using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;


namespace CommonLib.UI
{
	public class MoveAnimation
	{
		public MoveAnimation(FrameworkElement loadingElement)
		{
			TranslateTransform tt = new TranslateTransform();
			loadingElement.RenderTransform = tt;

			var sb = CreateStoryBoard();
			tt.BeginAnimation(TranslateTransform.XProperty, sb, HandoffBehavior.Compose);
			//sb.Begin(tt, HandoffBehavior.Compose, true);
		}


		protected AnimationTimeline CreateStoryBoard()
		{
			Duration frame = new Duration(TimeSpan.FromMilliseconds(500));

			//Storyboard sb = new Storyboard();
			//sb.Duration = frame;

			var animation = new DoubleAnimation()
			{
				By = 1.0,
				From = 0.0,
				To = 20.00,
				Duration = frame
			};

			//sb.Children.Add(animation);
			//sb.Duration = frame;
			
			//Storyboard.SetTargetProperty(animation, new PropertyPath("(TranslateTransform.X)"));

			return animation;
		}
	}
}
