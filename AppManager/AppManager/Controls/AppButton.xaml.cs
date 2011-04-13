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
using CommonLib.UI;

namespace AppManager
{
	/// <summary>
	/// Interaction logic for ImageButton.xaml
	/// </summary>
	public partial class AppButton
	{
		public static readonly DependencyProperty ButtonTextProperty =
			 DependencyProperty.Register("ButtonText", typeof(string), typeof(AppButton), new UIPropertyMetadata(String.Empty));

		public static readonly DependencyProperty ButtonImageSourceProperty =
			 DependencyProperty.Register("ButtonImageSource", typeof(BitmapSource), typeof(AppButton), new UIPropertyMetadata(null));


		protected Label _ButtonText = null;
		//protected FadeAnimarion _Animation = null;


		public AppButton()
		{
			this.InitializeComponent();
			//_Animation = new FadeAnimarion(ButtonImage);
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
						anim.ElementHidden += (s, ea) =>
							{
								ButtonImage.Source = (BitmapSource)e.NewValue;
								this.UpdateLayout();
							};
					}
				}
			}
		}
				
		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			ContentPanel.Width = this.Width - 8;
			ContentPanel.Height = this.Height - 8;

			return base.ArrangeOverride(arrangeBounds);
		}

		protected void SetButtonText(string text)
		{
			if (text == null)
			{
				ContentPanel.Children.Remove(_ButtonText);
				_ButtonText = null;
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

					_ButtonText.VerticalContentAlignment = System.Windows.VerticalAlignment.Bottom;
					_ButtonText.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
					_ButtonText.Padding = new Thickness(0.0);
					DockPanel.SetDock(_ButtonText, Dock.Bottom);

					ContentPanel.Children.Add(_ButtonText);
				}

				_ButtonText.Content = text;
			}
		}
	}
}