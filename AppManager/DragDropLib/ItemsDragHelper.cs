using System;
using System.Collections.Generic;
using System.Text;
using Drawing = System.Drawing;
using Imaging = System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Input;
using System.IO;


namespace DragDropLib
{
	public class ItemsDragHelper : DragHelperBase
	{
		protected ItemsControl		_ItemsControl;
		protected int					_DragItemPos;
		protected FrameworkElement _DragItem;


		public ItemsDragHelper(ItemsControl control, string dataFormat, Type dataType)
			: base(control, dataFormat, dataType)
		{
			_ItemsControl = control;
		}


		protected override void PrepareDrag(MouseButtonEventArgs e, FrameworkElement element)
		{
			Point pt = e.GetPosition(element);
			var item = _ItemsControl.InputHitTest(pt) as FrameworkElement;
			if (item == null)
				return;

			_IsDown = true;
			_DragStartPoint = pt;
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

		protected override Drawing.Bitmap CreateElementBitmap(FrameworkElement element, object dragObject)
		{
			RenderTargetBitmap rbmp = new RenderTargetBitmap(
				(int)element.ActualWidth + 8,
				(int)element.ActualHeight + 8,
				96.0,
				96.0,
				PixelFormats.Pbgra32);

			rbmp.Render(element);

			var bmp = new Drawing.Bitmap(
				(int)element.ActualWidth + 8,
				(int)element.ActualHeight + 8,
				System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

			var bdata = bmp.LockBits(
				new Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
				Imaging.ImageLockMode.ReadWrite,
				Imaging.PixelFormat.Format32bppPArgb);
			rbmp.CopyPixels(
				new Int32Rect(0, 0, bmp.Width, bmp.Height),
				bdata.Scan0,
				bdata.Stride * bmp.Height,
				bdata.Stride);

			int magenta = System.Drawing.Color.Magenta.ToArgb();
			unsafe
			{
				byte* pscan = (byte*)bdata.Scan0.ToPointer();
				for (int y = 0; y < bmp.Height; ++y, pscan += bdata.Stride)
				{
					int* prgb = (int*)pscan;
					for (int x = 0; x < bmp.Width; ++x, ++prgb)
					{
						if ((*prgb & 0xff000000L) == 0L)
							*prgb = magenta;
					}
				}
			}

			bmp.UnlockBits(bdata);
			return bmp;
		}

		protected override Point GetDragPoint(FrameworkElement element)
		{
			Point pt = element.PointToScreen(_DragStartPoint);
			return _DragItem.PointFromScreen(pt);
		}

		protected override DependencyObject GetDragSource()
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

		protected override void HandleDropedObject(FrameworkElement element, DragEventArgs e)
		{
			var item = _ItemsControl.InputHitTest(e.GetPosition(element)) as FrameworkElement;
			item = _ItemsControl.ContainerFromElement(item) as FrameworkElement;
			int ix;
			object dropObj = GetItemFromElement(item, out ix);
			IList coll = _ItemsControl.ItemsSource as IList;
			int dropIx = coll.IndexOf(dropObj);

			object objAdd = DataObject.ReadFromStream(e.Data.GetData(_DataFormat) as MemoryStream);
			objAdd = DeserializeItem(objAdd.ToString());

			PrepeareDropedObject(objAdd);

			if (dropIx < 0)
				dropIx = coll.Count;
			coll.Insert(dropIx, objAdd);
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
		}


		//protected DragDropEffects DragStarted(
		//   FrameworkElement element,
		//   out int dragItemPos,
		//   out object dragItem)
		//{
		//   var item = _ItemsControl.InputHitTest(_DragStartPoint) as FrameworkElement;
		//   item = _ItemsControl.ContainerFromElement(item) as FrameworkElement;

		//   dragItemPos = -1;
		//   dragItem = null;

		//   if (item == null)
		//      return DragDropEffects.None;

		//   dragItem = GetItemFromElement(item, out dragItemPos);
		//   DataObject dataObject = new DataObject();
		//   var data = new System.Windows.DataObject(dataObject);
		//   string serObj = SerializeItem(dragItem);
		//   dataObject.SetManagedData(_DataFormat, serObj);

		//   var bmp = CreateElementBitmap(VisualTreeHelper.GetChild(item, 0) as FrameworkElement);

		//   Point pt = element.PointToScreen(_DragStartPoint);
		//   pt = item.PointFromScreen(pt);
		//   CreateDragHelper(bmp, pt, data);

		//   OnDragStarted(dragItem);

		//   return DragDrop.DoDragDrop(
		//      item,
		//      data,
		//      DragDropEffects.Copy | DragDropEffects.Move);
		//}
	}
}
