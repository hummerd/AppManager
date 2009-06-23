using System;
using System.Windows;
using System.Windows.Controls;


namespace DragDropLib
{
	public class FileDropHandler : IDragHandler
	{
		public event EventHandler<FileDropEventArgs> AddFiles;


		#region IDragHandler Members

		public virtual DragDropEffects SupportDataFormat(FrameworkElement element, DragEventArgs dragData)
		{
			if (!dragData.Data.GetDataPresent(DataFormats.FileDrop))
				return DragDropEffects.None;

			return DragDropEffects.Copy;
		}

		public virtual bool HandleDragData(FrameworkElement element, DragEventArgs dragData)
		{
			if (!dragData.Data.GetDataPresent(DataFormats.FileDrop))
				return false;

			string[] files = dragData.Data.GetData(DataFormats.FileDrop, true) as string[];
			if (AddFiles != null)
			{
				var fdea = new FileDropEventArgs()
				{
					DropPoint = dragData.GetPosition(element),
					Files = files,
					KeyState = dragData.KeyStates
				};
				AddFiles(element, fdea);
			}

			return true;
		}

		public virtual void SetDragData(DataObject dragData, object dragObject)
		{ ; }

		#endregion
	}


	public class FileDropEventArgs : EventArgs
	{
		public string[] Files { get; set; }
		public Point DropPoint { get; set; }
		public DragDropKeyStates KeyState { get; set; }
	}
}
