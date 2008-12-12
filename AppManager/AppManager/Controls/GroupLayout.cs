using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using AppManager.Classes;


namespace AppManager.Controls
{
	public class GroupLayout : GroupBox
	{
		public LayoutPanel ContentPanel = new LayoutPanel();


		public GroupLayout()
		{
			Content = ContentPanel;
		}
	}
}
