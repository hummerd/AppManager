using System.Windows;
using AppManager.Entities;
using CommonLib;
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

		protected override object GetDragObject(FrameworkElement element)
		{
			var sr = base.GetDragObject(element) as ISourceReference<AppType>;
			return sr.Source;
		}
	}
}
