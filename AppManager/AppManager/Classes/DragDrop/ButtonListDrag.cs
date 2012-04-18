using System;
using System.Windows.Controls;
using DragDropLib;


namespace AppManager.DragDrop
{
	public class ButtonListDrag<TItemType> : ItemsViewDragHelper<TItemType>
	{
		public const string DragDataFormat = "AM_AppInfoDataFormat";


		public ButtonListDrag(ItemsControl control)
			: base(control, DragDataFormat,  AppGroupLoader.Default)
		{

		}
	}
}
