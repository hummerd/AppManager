using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Imaging = System.Drawing.Imaging;


namespace DragDropLib
{
	public abstract class DragHelperBase : DropTargetHelper
	{
		public static void InitDragHelper()
		{
			try
			{
				DataObject dataObject = new DataObject();
				var data = new System.Windows.DataObject(dataObject);

				DragDrop.DoDragDrop(
					null,
					data,
					DragDropEffects.Copy | DragDropEffects.Move);
			}
			catch
			{ ; }
		}
		

		public event EventHandler DragStart;
		public event EventHandler<DragEndEventArgs> DragEnd;


		protected bool						_IsDown = false;
		protected Point					_DragStartPoint;
		protected List<IDragHandler>	_DragHandlers = new List<IDragHandler>();
		protected FrameworkElement		_Element;
		protected bool						_UseTunneling;


		public DragHelperBase(FrameworkElement control, string dataFormat, Type dataType, bool useTunneling)
			: base (control)
		{
			_DragHandlers.Add(
				new SimpleDragDataHandler(dataFormat, dataType)
				);

			_Element = control;
			_DropTargetHelper = (IDropTargetHelper)new DragDropHelper();

			_UseTunneling = useTunneling;
			if (_UseTunneling)
			{
				_Element.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
				_Element.PreviewMouseMove += PreviewMouseMove;
				_Element.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUp;
			}
			else
			{
				_Element.MouseLeftButtonDown += PreviewMouseLeftButtonDown;
				_Element.MouseMove += PreviewMouseMove;
				_Element.MouseLeftButtonUp += PreviewMouseLeftButtonUp;			
			}
		}


		public List<IDragHandler> DragHandlers
		{
			get
			{
				return _DragHandlers;
			}
		}


		protected virtual void PrepareDrag(MouseButtonEventArgs e, FrameworkElement element)
		{
			_IsDown = true;
			_DragStartPoint = e.GetPosition(element);
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
			foreach (var item in _DragHandlers)
				item.SetDragData(dataObject, dragObject);

			var bmp = CreateElementBitmap(element, dragObject, 1);

			Point pt = GetDragPoint(element);
			CreateDragHelper(bmp, pt, data);

			OnDragStarted(dragObject);

			return DragDrop.DoDragDrop(
				GetDragSource(element),
				data,
				DragDropEffects.Copy | DragDropEffects.Move);
		}

		protected abstract object GetDragObject(FrameworkElement element);

		protected virtual Drawing.Bitmap CreateElementBitmap(FrameworkElement element, object dragObject, int scale)
		{
			RenderTargetBitmap rbmp = new RenderTargetBitmap(
				(int)element.ActualWidth + 8,
				(int)element.ActualHeight + 8,
				96.0,
				96.0,
				PixelFormats.Pbgra32);

			rbmp.Render(element);
			return BmpFromBmpSource(rbmp, scale);
		}

		protected Drawing.Bitmap BmpFromBmpSource(BitmapSource src, int scale)
		{
			var bmp = new Drawing.Bitmap(
				(int)src.PixelWidth,
				(int)src.PixelHeight,
				Imaging.PixelFormat.Format32bppPArgb);

			var bdata = bmp.LockBits(
				new Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
				Imaging.ImageLockMode.ReadWrite,
				Imaging.PixelFormat.Format32bppPArgb);

			src.CopyPixels(
				new Int32Rect(0, 0, bmp.Width, bmp.Height),
				bdata.Scan0,
				bdata.Stride * bmp.Height,
				bdata.Stride);

			bmp.UnlockBits(bdata);

			PrepareBitmap(bmp);

			if (scale == 1)
				return bmp;

			System.Drawing.Bitmap nb = new System.Drawing.Bitmap(bmp.Width * scale, bmp.Height * scale);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(nb))
				g.DrawImage(bmp, 1, 1, bmp.Width * scale, bmp.Height * scale);

			return nb;
		}

		protected void PrepareBitmap(Drawing.Bitmap bmp)
		{
			var bdata = bmp.LockBits(
				new Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
				Imaging.ImageLockMode.ReadWrite,
				Imaging.PixelFormat.Format32bppPArgb);

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
		}

		protected abstract Point GetDragPoint(FrameworkElement element);

		protected abstract DependencyObject GetDragSource(FrameworkElement element);

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
				DragEnd(this, new DragEndEventArgs() 
					{ DropEffects = effects, DropObject = dragItem  });
		}

		protected virtual void ResetDrag()
		{ 
			_IsDown = false;
			_DragStartPoint = default(Point);			
		}


		protected override void OnDragEnter(DragEventArgs e, FrameworkElement element)
		{
			base.OnDragEnter(e, element);

			if (e.Handled)
				return;

			PrepareSupportedEffect(e);
		}

		protected override void OnDragOver(DragEventArgs e, FrameworkElement element)
		{
			base.OnDragOver(e, element);

			if (e.Handled)
				return;

			PrepareSupportedEffect(e);
		}

		protected override void OnDrop(DragEventArgs e, FrameworkElement element)
		{
			PrepareSupportedEffect(e);
			e.Handled = false;

			base.OnDrop(e, element);

			if (e.Handled)
				return;

			bool handled = false;
			foreach (var item in _DragHandlers)
				handled |= item.HandleDragData(element, e);

			e.Handled = handled;
			
			ResetDrag();

			//if (e.Data.GetDataPresent(_DataFormat))
			//{
			//   if ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
			//      e.Effects = DragDropEffects.Copy;
			//   else
			//      e.Effects = DragDropEffects.Move;

			//  

			//   object objAdd = DataObject.ReadFromStream(e.Data.GetData(_DataFormat) as MemoryStream);
			//   objAdd = DeserializeItem(objAdd.ToString());

			//   HandleDropedObject(element, e, objAdd);

			//   e.Handled = true;
			//}
		}


		protected void PrepareSupportedEffect(DragEventArgs e)
		{
			DragDropEffects effect = DragDropEffects.None;
			foreach (var item in _DragHandlers)
				effect |= item.SupportDataFormat(e);

			e.Effects = e.AllowedEffects & effect;
			e.Handled = effect != DragDropEffects.None;
		}
		

		private void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_UseTunneling || e.Source == _Element)
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
