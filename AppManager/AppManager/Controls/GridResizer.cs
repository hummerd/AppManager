using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using CommonLib;
using CommonLib.UI;


namespace AppManager.Controls
{
	public class GridResizer : Label
	{
		public GridResizer()
		{
			TargetGridName = String.Empty;
		}
		
		
		public string TargetGridName
		{ get; set; }

		public Brush ResizeBackColor
		{ get; set; }


		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			new GridRowResizer(this, TargetGridName, ResizeBackColor);
		}
	}
}
