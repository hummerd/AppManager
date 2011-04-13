using System;
using System.Windows.Controls;
using DragDropLib;


namespace AppManager.DragDrop
{
	public class ButtonListDrag : ItemsDragHelper
	{
		public const string DragDataFormat = "AM_AppInfoDataFormat";


		public ButtonListDrag(ItemsControl control, Type dataType)
			: base(control, DragDataFormat, dataType, AppGroupLoader.Default)
		{

		}
	}
}
