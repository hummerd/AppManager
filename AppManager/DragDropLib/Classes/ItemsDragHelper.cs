using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Drawing = System.Drawing;
using CommonLib;


namespace DragDropLib
{
	public class ItemsDragHelper : DragHelperBase
	{
		public event EventHandler<ValueEventArgs<object>> PrepareItem;


		protected ItemsControl		_ItemsControl;
		protected int					_DragItemPos = -1;
		protected FrameworkElement _DragItem;


		public ItemsDragHelper(ItemsControl control, string dataFormat, Type dataType)
			: base(control, dataFormat, dataType)
		{
			_ItemsControl = control;

			(_DragHandlers[0] as SimpleDragDataHandler).ObjectDroped += 
				(s, e) => HandleDropedObject(s as FrameworkElement, e.EventArguments, e.DropObject);
		}


		protected override void PrepareDrag(MouseButtonEventArgs e, FrameworkElement element)
		{
			Point pt = e.GetPosition(element);
			var item = _ItemsControl.InputHitTest(pt) as FrameworkElement;
			if (item == null)
				return;

			base.PrepareDrag(e, element);
		}

		protected override object GetDragObject(FrameworkElement element)
		{
			_DragItem = _ItemsControl.InputHitTest(_DragStartPoint) as FrameworkElement;
			_DragItem = _ItemsControl.ContainerFromElement(_DragItem) as FrameworkElement;

			_DragItemPos = -1;

			if (_DragItem == null)
				return null;

			return GetItemFromElement(_DragItem, out _DragItemPos);
		}

		protected override Drawing.Bitmap CreateElementBitmap(FrameworkElement element, object dragObject, int scale)
		{
			return base.CreateElementBitmap(
				VisualTreeHelper.GetChild(_DragItem, 0) as FrameworkElement, null, 1);
		}

		protected override Point GetDragPoint(FrameworkElement element)
		{
			Point pt = element.PointToScreen(_DragStartPoint);
			return _DragItem.PointFromScreen(pt);
		}
		
		protected override DependencyObject GetDragSource(FrameworkElement element)
		{
			return _DragItem;
		}


		protected object GetItemFromElement(DependencyObject element, out int index)
		{
			index = -1;

			while (element != null)
			{
				if (element == _ItemsControl)
					return null;

				object item = _ItemsControl.ItemContainerGenerator.ItemFromContainer(element);
				index = _ItemsControl.ItemContainerGenerator.IndexFromContainer(element);

				bool itemFound = !object.ReferenceEquals(item, DependencyProperty.UnsetValue);
				if (itemFound)
				{
					return item;
				}
				else
				{
					element = VisualTreeHelper.GetParent(element) as DependencyObject;
				}
			}

			return null;
		}
		
		protected void HandleDropedObject(FrameworkElement element, DragEventArgs e, object dropObject)
		{
			var item = _ItemsControl.InputHitTest(e.GetPosition(element)) as FrameworkElement;
			item = _ItemsControl.ContainerFromElement(item) as FrameworkElement;
			int ix;
			object targetObject = GetItemFromElement(item, out ix);
			IList coll = _ItemsControl.ItemsSource as IList;
			int dropIx = coll.IndexOf(targetObject);

			PrepeareDropedObject(dropObject);

			if (dropIx < 0)
				dropIx = coll.Count;
			coll.Insert(dropIx, dropObject);
		}

		protected virtual void PrepeareDropedObject(object item)
		{
			if (PrepareItem != null)
				PrepareItem(this, new ValueEventArgs<object>(item));
		}
		
		protected override void DragEnded(DragDropEffects effects, object dragItem)
		{
			if ((effects & DragDropEffects.Move) == DragDropEffects.Move)
			{
				IList coll = _ItemsControl.ItemsSource as IList;
				if (coll[_DragItemPos].Equals(dragItem))
					coll.RemoveAt(_DragItemPos);
				else if (_DragItemPos + 1 < coll.Count && coll[_DragItemPos + 1].Equals(dragItem))
					coll.RemoveAt(_DragItemPos + 1);
			}

			base.DragEnded(effects, dragItem);

			_DragItemPos = -1;
			_DragItem = null;
		}
	}
}
