using System;
using System.Windows;


namespace DragDropLib
{
	public class FileDropHandler : IDragHandler
	{
		public event EventHandler<FileDropEventArgs> AddFiles;


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

		public void SetDragData(DataObject dragData, object dragObject)
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
