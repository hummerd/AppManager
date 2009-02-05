using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Imaging = System.Drawing.Imaging;
using System.Collections;

using System.Windows.Interop;


namespace DragDropLib
{
	public class DragHelper : DropTargetHelper
	{
		protected bool						_IsDown = false;
		protected Point					_DragStartPoint;
		protected string					_DataFormat;
		protected Type						_DataType;
		protected ItemsControl			_ItemsControl;


		public DragHelper(ItemsControl control, string dataFormat, Type dataType)
			: base (control)
		{
			_ItemsControl = control;
			_DataFormat = dataFormat;
			_DataType = dataType;
			_DropTargetHelper = (IDropTargetHelper)new DragDropHelper();

			_ItemsControl.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
			_ItemsControl.PreviewMouseMove += PreviewMouseMove;
			_ItemsControl.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUp;

			
			//_ItemsControl.DragEnter += DragEnter;
			//_ItemsControl.DragOver += DragOver;
			//_ItemsControl.DragLeave += DragLeave;
			//_ItemsControl.Drop += Drop;
		}

		
		protected void PrepareDrag(MouseButtonEventArgs e, FrameworkElement element)
		{
			Point pt = e.GetPosition(element);
			var item = _ItemsControl.InputHitTest(pt) as FrameworkElement;
			if (item == null)
				return;

			_IsDown = true;
			_DragStartPoint = pt;
		}

		protected void CheckAndStartDrag(MouseEventArgs e, FrameworkElement element)
		{
			if (!_IsDown)
				return;

			if (Math.Abs(e.GetPosition(element).X - _DragStartPoint.X)
					> SystemParameters.MinimumHorizontalDragDistance ||
				 Math.Abs(e.GetPosition(element).Y - _DragStartPoint.Y)
					> SystemParameters.MinimumVerticalDragDistance)
			{
				_IsDown = false;
				int dragItemPos;
				object dragItem;

				DragDropEffects effects = DragStarted(element, out dragItemPos, out dragItem);
				DragEnded(effects, dragItemPos, dragItem);
			}
		}

		protected DragDropEffects DragStarted(
			FrameworkElement element,
			out int dragItemPos,
			out object dragItem)
		{
			var item = _ItemsControl.InputHitTest(_DragStartPoint) as FrameworkElement;
			item = _ItemsControl.ContainerFromElement(item) as FrameworkElement;

			dragItemPos = -1;
			dragItem = null;

			if (item == null)
				return DragDropEffects.None;

			dragItem = GetItemFromElement(item, out dragItemPos);
			DataObject dataObject = new DataObject();
			var data = new System.Windows.DataObject(dataObject);
			string serObj = SerializeItem(dragItem);
			dataObject.SetManagedData(_DataFormat, serObj);

			var bmp = CreateElementBitmap(VisualTreeHelper.GetChild(item, 0) as FrameworkElement);

			Point pt = element.PointToScreen(_DragStartPoint);
			pt = item.PointFromScreen(pt);
			CreateDragHelper(bmp, pt, data);
			
			return DragDrop.DoDragDrop(
				item,
				data,
				DragDropEffects.Copy | DragDropEffects.Move);
		}

		protected void CreateDragHelper(
			Drawing.Bitmap bitmap,
			Point startPoint,
			System.Windows.DataObject data)
		{
			ShDragImage shdi = new ShDragImage();
			Win32Size size;
			size.cx = bitmap.Width;
			size.cy = bitmap.Height;
			shdi.sizeDragImage = size;
			Win32Point wpt;
			wpt.x = (int)startPoint.X;
			wpt.y = (int)startPoint.Y;
			shdi.ptOffset = wpt;
			shdi.hbmpDragImage = bitmap.GetHbitmap();
			shdi.crColorKey = Drawing.Color.Magenta.ToArgb();

			IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();
			sourceHelper.InitializeFromBitmap(ref shdi, data);
		}

		protected Drawing.Bitmap CreateElementBitmap(FrameworkElement element)
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

		protected string SerializeItem(object obj)
		{
			string result;
			XmlSerializer xser = new XmlSerializer(obj.GetType());

			using (StringWriter sr = new StringWriter())
			{
				xser.Serialize(sr, obj);
				result = sr.ToString();
			}

			return result;
		}

		protected object DeserializeItem(string obj)
		{
			XmlSerializer xser = new XmlSerializer(_DataType);

			object result;
			using (TextReader xr = new StringReader(obj))
			{
				result = xser.Deserialize(xr);
			}

			return result;
		}

		protected void DragEnded(DragDropEffects effects, int dragItemPos, object dragItem)
		{
			if ((effects & DragDropEffects.Move) == DragDropEffects.Move)
			{
				IList coll = _ItemsControl.ItemsSource as IList;
				if (coll[dragItemPos].Equals(dragItem))
					coll.RemoveAt(dragItemPos);
				else if (dragItemPos + 1 < coll.Count && coll[dragItemPos + 1].Equals(dragItem))
					coll.RemoveAt(dragItemPos + 1);
			}

			ResetDrag();
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

		protected void ResetDrag()
		{ 
			_IsDown = false;
			_DragStartPoint = default(Point);			
		}


		//protected override void OnDragEnter(DragEventArgs e, FrameworkElement element)
		//{
		//   base.OnDragEnter(e, element);
		//}

		//protected override void OnDragLeave(DragEventArgs e)
		//{
		//   base.
		//}

		protected override void OnDragOver(DragEventArgs e, FrameworkElement element)
		{
			base.OnDragOver(e, element);

			if (e.Data.GetDataPresent(_DataFormat))
			{
				if ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
					e.Effects = DragDropEffects.Copy;
				else
					e.Effects = DragDropEffects.Move;

				e.Handled = true;
			}
		}

		protected override void OnDrop(DragEventArgs e, FrameworkElement element)
		{
			base.OnDrop(e, element);

			if (e.Data.GetDataPresent(_DataFormat))
			{
				if ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
					e.Effects = DragDropEffects.Copy;
				else
					e.Effects = DragDropEffects.Move;

				ResetDrag();

				var item = _ItemsControl.InputHitTest(e.GetPosition(element)) as FrameworkElement;
				item = _ItemsControl.ContainerFromElement(item) as FrameworkElement;
				int ix;
				object dropObj = GetItemFromElement(item, out ix);
				IList coll = _ItemsControl.ItemsSource as IList;
				int dropIx = coll.IndexOf(dropObj);

				object objAdd = DataObject.ReadFromStream(e.Data.GetData(_DataFormat) as MemoryStream);
				objAdd = DeserializeItem(objAdd.ToString());

				if (dropIx < 0)
					dropIx = coll.Count;
				coll.Insert(dropIx, objAdd);

				e.Handled = true;
			}
		}


		private void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			PrepareDrag(e, sender as FrameworkElement);
		}

		private void PreviewMouseMove(object sender, MouseEventArgs e)
		{
			CheckAndStartDrag(e, sender as FrameworkElement);
		}

		private void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ResetDrag();
		}


		//private void DragEnter(object sender, DragEventArgs e)
		//{
		//   OnDragEnter(e, sender as FrameworkElement);
		//}

		//private void DragOver(object sender, DragEventArgs e)
		//{
		//   OnDragOver(e, sender as FrameworkElement);
		//}

		//private void DragLeave(object sender, DragEventArgs e)
		//{
		//   OnDragLeave(e);
		//}

		//private void Drop(object sender, DragEventArgs e)
		//{
		//   OnDrop(e, sender as FrameworkElement);
		//}


		private static DependencyObject GetParent(DependencyObject obj)
		{
			if (obj == null)
				return null;

			ContentElement ce = obj as ContentElement;

			if (ce != null)
			{
				DependencyObject parent = ContentOperations.GetParent(ce);

				if (parent != null)
					return parent;

				FrameworkContentElement fce = ce as FrameworkContentElement;
				return fce != null ? fce.Parent : null;
			}

			return VisualTreeHelper.GetParent(obj);
		}

		private static T FindAncestorOrSelf<T>(DependencyObject obj)
			 where T : DependencyObject
		{
			while (obj != null)
			{
				T objTest = obj as T;

				if (objTest != null)
					return objTest;

				obj = GetParent(obj);
			}

			return null;
		}
	}
}
