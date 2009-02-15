using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Drawing = System.Drawing;
using Imaging = System.Drawing.Imaging;


namespace DragDropLib
{
	public abstract class DragHelperBase : DropTargetHelper
	{
		public event EventHandler<ObjectEventArgs> PrepareItem;
		public event EventHandler DragStart;
		public event EventHandler DragEnd;


		protected bool					_IsDown = false;
		protected Point				_DragStartPoint;
		protected string				_DataFormat;
		protected Type					_DataType;
		protected FrameworkElement	_Element;


		public DragHelperBase(FrameworkElement control, string dataFormat, Type dataType)
			: base (control)
		{
			_Element = control;
			_DataFormat = dataFormat;
			_DataType = dataType;
			_DropTargetHelper = (IDropTargetHelper)new DragDropHelper();

			_Element.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
			_Element.PreviewMouseMove += PreviewMouseMove;
			_Element.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUp;
		}


		protected abstract void PrepareDrag(MouseButtonEventArgs e, FrameworkElement element);

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
				object dragObject;

				DragDropEffects effects = DragStarted(element, out dragObject);
				DragEnded(effects, dragObject);
			}
		}

		protected DragDropEffects DragStarted(
			FrameworkElement element,
			out object dragObject)
		{
			dragObject = GetDragObject(element);
			if (dragObject == null)
				return DragDropEffects.None;

			DataObject dataObject = new DataObject();
			var data = new System.Windows.DataObject(dataObject);
			string serObj = SerializeItem(dragObject);
			dataObject.SetManagedData(_DataFormat, serObj);

			var bmp = CreateElementBitmap(element, dragObject);

			Point pt = GetDragPoint(element);
			CreateDragHelper(bmp, pt, data);

			OnDragStarted(dragObject);

			return DragDrop.DoDragDrop(
				GetDragSource(),
				data,
				DragDropEffects.Copy | DragDropEffects.Move);
		}

		protected abstract object GetDragObject(FrameworkElement element);

		protected abstract Drawing.Bitmap CreateElementBitmap(FrameworkElement element, object dragObject);

		protected abstract Point GetDragPoint(FrameworkElement element);

		protected abstract DependencyObject GetDragSource();

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

		protected virtual void OnDragStarted(object dragItem)
		{
			if (DragStart != null)
				DragStart(this, EventArgs.Empty);
		}

		protected virtual void DragEnded(DragDropEffects effects, object dragItem)
		{
			ResetDrag();
			OnDragEnded(effects, dragItem);
		}

		protected virtual void OnDragEnded(DragDropEffects effects, object dragItem)
		{
			if (DragEnd != null)
				DragEnd(this, EventArgs.Empty);
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
				HandleDropedObject(element, e);

				e.Handled = true;
			}
		}

		protected abstract void HandleDropedObject(FrameworkElement element, DragEventArgs e);

		protected virtual void PrepeareDropedObject(object item)
		{
			if (PrepareItem != null)
				PrepareItem(this, new ObjectEventArgs(item));
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

	public class ObjectEventArgs : EventArgs
	{
		public ObjectEventArgs(object obj)
		{
			Obj = obj;
		}

		public object Obj { get; set; }
	}
}
