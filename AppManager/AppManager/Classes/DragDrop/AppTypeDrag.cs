using System.Windows;
using AppManager.Entities;
using DragDropLib;


namespace AppManager.DragDrop
{
	public class AppTypeDrag : SimpleDragHelper
	{
		public const string DragDataFormat = "AM_AppTypeDataFormat";


		public AppTypeDrag(FrameworkElement control)
			: base(
				control, 
				DragDataFormat, 
				typeof(AppType), 
				AppManager.Properties.Resources.cnruninstall_3273_32,
				"Header",
				AppGroupLoader.Default)
		{

		}
	}
}
