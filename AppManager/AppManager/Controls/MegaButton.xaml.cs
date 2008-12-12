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

namespace AppManager
{
	/// <summary>
	/// Interaction logic for MegaButton.xaml
	/// </summary>
	public partial class SettingButton
	{
		public SettingButton()
		{
			this.InitializeComponent();
		}

		public ImageSource ButtonImageSource
		{
			get { return ButtonImage.Source; }
			set { ButtonImage.Source = value; }
		}
	}
}