using System.Windows;


namespace DragDropLib
{
	public interface IDragHandler
	{
		DragDropEffects SupportDataFormat(FrameworkElement element, DragEventArgs dragData);
		bool HandleDragData(FrameworkElement element, DragEventArgs dragData);
		void SetDragData(DataObject dragData, object dragObject);
		void DragEnded(DragDropEffects effects, FrameworkElement dragSource);
		//void DataDragOver(FrameworkElement element, DragEventArgs dragData);
	}
}
