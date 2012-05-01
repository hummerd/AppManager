using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.UI;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for ImageButton.xaml
	/// </summary>
	public partial class AppButton
	{
		public static readonly DependencyProperty IsTitleVisibleProperty =
			DependencyProperty.Register("IsTitleVisible", typeof(bool), typeof(AppButton), new UIPropertyMetadata(true));

		public static readonly DependencyProperty ButtonTextProperty =
			 DependencyProperty.Register("ButtonText", typeof(string), typeof(AppButton), new UIPropertyMetadata(String.Empty));

		public static readonly DependencyProperty ButtonImageSourceProperty =
			 DependencyProperty.Register("ButtonImageSource", typeof(BitmapSource), typeof(AppButton), new UIPropertyMetadata(null));


		protected Label _ButtonText = null;


		public AppButton()
		{
			this.InitializeComponent();
			//_Animation = new FadeAnimarion(ButtonImage);
		}


		public bool IsTitleVisible
		{
			get 
			{
				return (bool)GetValue(IsTitleVisibleProperty); 
			}
			set 
			{
				SetValue(IsTitleVisibleProperty, value);
				SetAppTitleVisibility(value);
			}
		}

		public string ButtonText
		{
			get
			{
				return (string)GetValue(ButtonTextProperty);
			}
			set
			{
				SetValue(ButtonTextProperty, value);
				SetButtonText(value);
			}
		}

		public BitmapSource ButtonImageSource
		{
			get { return (BitmapSource)GetValue(ButtonImageSourceProperty); }
			set { SetValue(ButtonImageSourceProperty, value); ButtonImage.Source = value; }
		}
		

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ButtonTextProperty)
			{ 
				if (e.NewValue != null)
					SetButtonText((string)e.NewValue);
			}
			else if (e.Property == ButtonImageSourceProperty)
			{
				if (e.NewValue != null)
				{
					if (ButtonImage.Source == null)
						ButtonImage.Source = (BitmapSource)e.NewValue;
					else
					{
						var anim = new FadeAnimarion(ButtonImage);
						anim.SetLoadingState(true);
						anim.ElementHidden += (s, ea) => ButtonImage.Source = (BitmapSource)e.NewValue;
					}
				}
			}
			else if (e.Property == IsTitleVisibleProperty)
			{
				if (e.NewValue != null)
					SetAppTitleVisibility((bool)e.NewValue);
			}
		}

		protected void SetAppTitleVisibility(bool visible)
		{
            ContentPanel.Margin = visible
                ? new Thickness(0, 2, 0, 1)
                : new Thickness(0, 4, 0, 4);

            if (_ButtonText == null)
				return;

            if (!visible)
            {
                ContentPanel.Children.Remove(_ButtonText);
            }
            else if (!ContentPanel.Children.Contains(_ButtonText))
            {
                ContentPanel.Children.Add(_ButtonText);
            }
		}

		protected void SetButtonText(string text)
		{
			if (text == null)
			{
				if (_ButtonText != null)
				{
					ContentPanel.Children.Remove(_ButtonText);
                    _ButtonText = null;
				}
			}
			else
			{
				if (_ButtonText == null)
				{
					_ButtonText = new Label();
					_ButtonText.FontFamily = new FontFamily("Tahoma");
					_ButtonText.FontSize = 11;
					_ButtonText.MaxWidth = 80;
					_ButtonText.Foreground = Resources["AppNameBrush"] as Brush;
					_ButtonText.IsHitTestVisible = false;

					_ButtonText.VerticalContentAlignment = VerticalAlignment.Bottom;
					_ButtonText.HorizontalContentAlignment = HorizontalAlignment.Center;
					_ButtonText.Padding = new Thickness(0.0);
					DockPanel.SetDock(_ButtonText, Dock.Bottom);

                    if (IsTitleVisible)
                    {
                        ContentPanel.Children.Add(_ButtonText);
                    }
				}

				_ButtonText.Content = text;
			}
		}
	}
}