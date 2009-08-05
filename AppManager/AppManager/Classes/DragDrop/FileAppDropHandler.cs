using System.Windows;
using System.Windows.Controls;
using DragDropLib;


namespace AppManager.DragDrop
{
	public class FileAppDropHandler : FileDropHandler
	{
		public override DragDropEffects SupportDataFormat(FrameworkElement element, DragEventArgs dragData)
		{
			if ((dragData.KeyStates & DragDropKeyStates.AltKey) == DragDropKeyStates.AltKey)
			{
				var input = (element as ItemsControl).InputHitTest(dragData.GetPosition(element)) as FrameworkElement;
				if (input != null)
				{
					var appInfo = input.DataContext as AppInfo;
					if (appInfo != null)
						return DragDropEffects.Link;
				}
			}

			return base.SupportDataFormat(element, dragData);
		}
	}
}
