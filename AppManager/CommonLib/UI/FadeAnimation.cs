using System;
using System.Windows;
using System.Windows.Media.Animation;


namespace CommonLib.UI
{
	public class FadeAnimarion
	{
		protected FrameworkElement _LoadingElement;
		protected FrameworkElement _PrimaryElement;
		protected bool _IsLoading;

		protected Storyboard _FadeIn;
		protected Storyboard _FadeOut;


		public event EventHandler<ValueEventArgs<UIElement>> ElementHidden;


		public FadeAnimarion(FrameworkElement loadingElement)
		{
			_LoadingElement = loadingElement;
			_PrimaryElement = loadingElement;
		}

		public FadeAnimarion(FrameworkElement loadingElement, FrameworkElement primaryElement)
		{
			_LoadingElement = loadingElement;
			_PrimaryElement = primaryElement;
		}


		public void SetLoadingState(bool isLoading)
		{
			_IsLoading = isLoading;

			if (_FadeOut == null)
				_FadeOut = CreateStoryBoard(false);

			if (_FadeIn == null)
				_FadeIn = CreateStoryBoard(true);

			_FadeOut.Stop(_PrimaryElement);
			_FadeIn.Stop(_LoadingElement);

			SetStoryTargets(_IsLoading);

			_FadeOut.Begin(_PrimaryElement, HandoffBehavior.Compose, true);
			//_FadeOut.Begin();
		}


		protected virtual void OnElementHidden(ValueEventArgs<UIElement> e)
		{
			if (ElementHidden != null)
				ElementHidden(this, e);

			_FadeIn.Begin(_LoadingElement, HandoffBehavior.Compose, true);
			//_FadeIn.Begin();
		}

		protected Storyboard CreateStoryBoard(bool fadeIn)
		{
			Duration frame = new Duration(TimeSpan.FromMilliseconds(500));
			Duration t = new Duration(TimeSpan.FromMilliseconds(1000));

			Storyboard sb = new Storyboard();
			sb.Duration = frame;

			var animation = new DoubleAnimation()
			{
				By = fadeIn ? 0.05 : -0.05,
				From = fadeIn ? 0.00 : 1.00,
				To = fadeIn ? 1.00 : 0.00,
				Duration = frame
			};

			if (!fadeIn)
				animation.Completed += new EventHandler(fadeOut_Completed);

			sb.Children.Add(animation);
			sb.Duration = t;

			return sb;
		}

		protected void SetStoryTargets(bool loading)
		{
			if (loading)
			{
				Storyboard.SetTarget(_FadeIn.Children[0], _LoadingElement);
				Storyboard.SetTarget(_FadeOut.Children[0], _PrimaryElement);

				Storyboard.SetTargetProperty(_FadeIn.Children[0], new PropertyPath("(UIElement.Opacity)"));
				Storyboard.SetTargetProperty(_FadeOut.Children[0], new PropertyPath("(UIElement.Opacity)"));
			}
			else
			{
				Storyboard.SetTarget(_FadeOut.Children[0], _LoadingElement);
				Storyboard.SetTarget(_FadeIn.Children[0], _PrimaryElement);

				Storyboard.SetTargetProperty(_FadeOut.Children[0], new PropertyPath("(UIElement.Opacity)"));
				Storyboard.SetTargetProperty(_FadeIn.Children[0], new PropertyPath("(UIElement.Opacity)"));
			}
		}


		private void fadeOut_Completed(object sender, EventArgs e)
		{
			//System.Windows.Browser.HtmlPage.Window.Alert("fadeOut_Completed " + _IsLoading);
			OnElementHidden(new ValueEventArgs<UIElement>(
				_IsLoading ? _LoadingElement : _PrimaryElement));
		}

		//private void fadeIn_Completed(object sender, EventArgs e)
		//{
		//   System.Windows.Browser.HtmlPage.Window.Alert("fadeIn_Completed " + _IsLoading);
		//   ContentBorder.Child = !_IsLoading ? (UIElement)LabelLoading : ContentHolder;
		//}
	}
}
