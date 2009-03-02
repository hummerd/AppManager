using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;


namespace AppManager
{
	public class CommandKeyBinding : KeyBinding
	{
		// Using a DependencyProperty as the backing store for KeyCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty KeyCommandProperty =
			 DependencyProperty.Register("KeyCommand", typeof(ICommand), typeof(CommandKeyBinding), new UIPropertyMetadata(null));


		public CommandKeyBinding()
		{
		}


		public ICommand KeyCommand
		{
			get { return (ICommand)GetValue(KeyCommandProperty); }
			set { SetValue(KeyCommandProperty, value); Command = value; }
		}
	}
}
