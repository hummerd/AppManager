﻿using System;
using System.Windows;
using System.Windows.Controls;
using DragDropLib;


namespace AppManager
{
	public class ButtonListDrag : DragHelper
	{
		public event EventHandler<StrArrEventArgs> AddFiles;


		public ButtonListDrag(ItemsControl control, string dataFormat, Type dataType)
			: base(control, dataFormat, dataType)
		{

		}


		protected override void OnDragEnter(DragEventArgs e, FrameworkElement element)
		{
			base.OnDragEnter(e, element);
		}

		protected override void OnDragOver(DragEventArgs e, FrameworkElement element)
		{
			base.OnDragOver(e, element);

			if (!e.Handled)
			{
				if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
				{
					e.Effects = DragDropEffects.Copy;
					e.Handled = true;
				}
			}

			if (!e.Handled)
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
		}

		protected override void OnDrop(DragEventArgs e, FrameworkElement element)
		{
			base.OnDrop(e, element);

			if (!e.Handled)
				if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
				{
					string[] files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
					if (AddFiles != null)
						AddFiles(this, new StrArrEventArgs() { StrArray = files });

					e.Handled = true;
				}
		}
	}

	public class StrArrEventArgs : EventArgs
	{
		public string[] StrArray { get; set; }
	}
}
