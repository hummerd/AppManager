using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;


namespace DragDropLib
{
	public class DragContextEventArgs : EventArgs
	{
		public DragEventArgs EventArguments { get; set; }

		public object DropObject { get; set; }
	}

	public class DragEndEventArgs : EventArgs
	{
		public DragDropEffects DropEffects { get; set; }

		public object DropObject { get; set; }

		public FrameworkElement DragSource { get; set; }
	}
}
