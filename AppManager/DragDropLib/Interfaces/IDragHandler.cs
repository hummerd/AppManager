using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;


namespace DragDropLib
{
	public interface IDragHandler
	{
		DragDropEffects SupportDataFormat(DragEventArgs dragData);
		bool HandleDragData(FrameworkElement element, DragEventArgs dragData);
		void SetDragData(DataObject dragData, object dragObject);
	}
}
