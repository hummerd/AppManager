using System.Collections;
using System.Windows;
using System.Windows.Controls;
using CommonLib;


namespace DragDropLib
{
	public class ItemsViewDragHelper<TModel> : ItemsDragHelper
	{
		public ItemsViewDragHelper(
			ItemsControl control, 
			string dataFormat, 
			IObjectSerializer serializer)
			:base(
				control, 
				dataFormat,
				typeof(TModel),
				serializer)
		{ ; }

		protected override object GetDragObject(System.Windows.FrameworkElement element)
		{
			var dobj = base.GetDragObject(element);
			var sr = dobj as ISourceReference<TModel>;
			if (sr == null)
				return null;

			return sr.Source;
		}

		protected override void DragEnded(DragDropEffects effects, object dragItem, FrameworkElement dragSource)
		{
			if ((effects & DragDropEffects.Move) == DragDropEffects.Move)
			{
				IList coll = _ItemsControl.ItemsSource as IList;
				var ixObj = coll[_DragItemPos] as ISourceReference<TModel>;

				ISourceReference<TModel> ixNextObj = null;
				if (_DragItemPos + 1 < coll.Count)
					ixNextObj = coll[_DragItemPos + 1] as ISourceReference<TModel>;

				if (ixObj.Source.Equals(dragItem))
					dragItem = ixObj;
				else if (ixNextObj != null && ixNextObj.Source.Equals(dragItem))
					dragItem = ixNextObj;
			}

			base.DragEnded(effects, dragItem, dragSource);
		}
	}
}
