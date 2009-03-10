using System;
using System.Windows;
using System.Windows.Controls;
using DragDropLib;
using CommonLib;


namespace AppManager
{
	public class ButtonListDrag : ItemsDragHelper
	{
		public const string DragDataFormat = "AM_AppInfoDataFormat";


		public event EventHandler<ValueEventArgs<string[]>> AddFiles;


		public ButtonListDrag(ItemsControl control, Type dataType)
			: base(control, DragDataFormat, dataType)
		{

		}

		protected override void OnDrop(DragEventArgs e, FrameworkElement element)
		{
			base.OnDrop(e, element);

			//if (!e.Handled)
			//   if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
			//   {
			//      string[] files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
			//      if (AddFiles != null)
			//         AddFiles(this, new ValueEventArgs<string[]>(files));

			//      e.Handled = true;
			//   }
		}
	}
}
