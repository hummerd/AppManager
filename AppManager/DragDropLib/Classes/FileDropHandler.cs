using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using CommonLib;


namespace DragDropLib
{
	public class FileDropHandler : IDragHandler
	{
		public event EventHandler<ValueEventArgs<string[]>> AddFiles;


		#region IDragHandler Members

		public System.Windows.DragDropEffects SupportDataFormat(System.Windows.DragEventArgs dragData)
		{
			if (!dragData.Data.GetDataPresent(DataFormats.FileDrop))
				return DragDropEffects.None;

			return DragDropEffects.Copy;
		}

		public bool HandleDragData(FrameworkElement element, DragEventArgs dragData)
		{
			if (!dragData.Data.GetDataPresent(DataFormats.FileDrop))
				return false;

			string[] files = dragData.Data.GetData(DataFormats.FileDrop, true) as string[];
			if (AddFiles != null)
				AddFiles(element, new ValueEventArgs<string[]>(files));

			return true;
		}

		public void SetDragData(DataObject dragData, object dragObject)
		{ ; }

		#endregion
	}
}
