using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.PInvoke;
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
				Kernel32.LoadLibrary("atl.dll");
				Kernel32.LoadLibrary("linkworld.dll");
				Kernel32.LoadLibrary("linkinfo.dll");
				Kernel32.LoadLibrary("mlang.dll");
				Kernel32.LoadLibrary("ntshrui.dll");
				Kernel32.LoadLibrary("sxs.dll");
				Kernel32.LoadLibrary("userenv.dll");
				//Kernel32.LoadLibraryEx(
				//   "c_" + Encoding.ASCII.WindowsCodePage + ".nls", 
				//   IntPtr.Zero, 
				//   LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE);
			}
			catch
			{ ; }
		}
		

		public event EventHandler DragStart;
		public event EventHandler<DragEventArgs> DragOver;
		public event EventHandler<DragEndEventArgs> DragEnd;
		public event EventHandler<DragEventArgs> DragDroped;
		public event EventHandler<DragEventArgs> DragLeave;


		protected bool						_IsDown = false;
		protected Point					_DragStartPoint;
		protected List<IDragHandler>	_DragHandlers = new List<IDragHandler>();
		protected FrameworkElement		_Element;
		protected bool						_UseTunneling;


		public DragHelperBase(
			FrameworkElement control, 
			string dataFormat, 
			Type dataType, 
			bool useTunneling,
			IObjectSerializer serializer)
			: base (control)
		{
			_DragHandlers.Add(
				new SimpleDragDataHandler(dataFormat, dataType, serializer)
				);

			if (control == null) // this code should work only in case of init
				return;

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
				DragEnded(effects, dragObject, element);
			}
		}

		protected DragDropEffects DragStarted(
			FrameworkElement element,
			out object dragObject)
		{
			dragObject = GetDragObject(element);
			if (dragObject == null)
				return DragDropEffects.None;

			System.Diagnostics.Debug.WriteLine("Drag start1 " + DateTime.Now);

			DataObject dataObject = new DataObject();
			var data = new System.Windows.DataObject(dataObject);
			foreach (var item in _DragHandlers)
				item.SetDragData(dataObject, dragObject);

			System.Diagnostics.Debug.WriteLine("Drag start2 " + DateTime.Now);

			var bmp = CreateElementBitmap(element, dragObject, 1);

			Point pt = GetDragPoint(element);
			CreateDragHelper(bmp, pt, data);

			OnDragStarted(dragObject);

			System.Diagnostics.Debug.WriteLine("Drag start3 " + DateTime.Now);
			System.Diagnostics.Debug.WriteLine("");

			return DragDrop.DoDragDrop(
				GetDragSource(element),
				data,
				DragDropEffects.Copy | DragDropEffects.Move);
		}

		protected abstract object GetDragObject(FrameworkElement element);

		#region Drawing

		/// <summary>
		/// Sets the drag image by rendering the specified UIElement.
		/// </summary>
		/// <param name="dataObject">The DataObject to set the drag image for.</param>
		/// <param name="element">The element to render as the drag image.</param>
		/// <param name="cursorOffset">The offset of the cursor relative to the UIElement.</param>
		protected virtual Drawing.Bitmap CreateElementBitmap(FrameworkElement element, object dragObject, int scale)
		{
			Size size = element.RenderSize;

			// Get the device's DPI so we render at full size
			int dpix, dpiy;
			GetDeviceDpi(element, out dpix, out dpiy);

			// Create our renderer at full size
			RenderTargetBitmap renderSource = new RenderTargetBitmap(
				(int)size.Width, (int)size.Height, dpix, dpiy, PixelFormats.Pbgra32);

			// Render the element
			renderSource.Render(element);

			// Set the drag image by the bitmap source
			return GetBitmapFromBitmapSource(renderSource, Colors.Magenta);
		}


		/// <summary>
		/// Gets the device capabilities.
		/// </summary>
		/// <param name="reference">A reference UIElement for getting the relevant device caps.</param>
		/// <param name="dpix">The horizontal DPI.</param>
		/// <param name="dpiy">The vertical DPI.</param>
		protected void GetDeviceDpi(Visual reference, out int dpix, out int dpiy)
		{
			Matrix m = PresentationSource.FromVisual(reference).CompositionTarget.TransformToDevice;
			dpix = (int)(96 * m.M11);
			dpiy = (int)(96 * m.M22);
		}

		/// <summary>
		/// Gets a System.Drawing.Bitmap from a BitmapSource.
		/// </summary>
		/// <param name="source">The source image from which to create our Bitmap.</param>
		/// <param name="transparencyKey">The transparency key. This is used by the DragDropHelper
		/// in rendering transparent pixels.</param>
		/// <returns>An instance of Bitmap which is a copy of the BitmapSource's image.</returns>
		protected Drawing.Bitmap GetBitmapFromBitmapSource(BitmapSource source, Color transparencyKey)
		{
			// Copy at full size
			Int32Rect sourceRect = new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight);

			// Convert to our destination pixel format
			Imaging.PixelFormat pxFormat = ConvertPixelFormat(source.Format);

			// Create the Bitmap, full size, full rez
			Drawing.Bitmap bmp = new Drawing.Bitmap(sourceRect.Width, sourceRect.Height, pxFormat);
			// If the format is an indexed format, copy the color palette
			if ((pxFormat & Imaging.PixelFormat.Indexed) == Imaging.PixelFormat.Indexed)
				ConvertColorPalette(bmp.Palette, source.Palette);

			// Get the transparency key as a System.Drawing.Color
			Drawing.Color transKey = ToDrawingColor(transparencyKey);

			// Lock our Bitmap bits, we need to write to it
			Imaging.BitmapData bmpData = bmp.LockBits(
				ToDrawingRectangle(sourceRect),
				Imaging.ImageLockMode.ReadWrite,
				pxFormat);
			{
				// Copy the source bitmap data to our new Bitmap
				source.CopyPixels(sourceRect, bmpData.Scan0, bmpData.Stride * sourceRect.Height, bmpData.Stride);

				// The drag image seems to work in full 32-bit color, except when
				// alpha equals zero. Then it renders those pixels at black. So
				// we make a pass and set all those pixels to the transparency key
				// color. This is only implemented for 32-bit pixel colors for now.
				if ((pxFormat & Imaging.PixelFormat.Alpha) == Imaging.PixelFormat.Alpha)
					ReplaceTransparentPixelsWithTransparentKey(bmpData, transKey);
			}
			// Done, unlock the bits
			bmp.UnlockBits(bmpData);

			return bmp;
		}

		protected void PrepareBitmap(Drawing.Bitmap bmp)
		{
			var bdata = bmp.LockBits(
				new Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
				Imaging.ImageLockMode.ReadWrite,
				Imaging.PixelFormat.Format32bppPArgb);

			ReplaceTransparentPixelsWithTransparentKey(bdata, Drawing.Color.Magenta);

			bmp.UnlockBits(bdata);
		}

		/// <summary>
		/// Replaces any pixel with a zero alpha value with the specified transparency key.
		/// </summary>
		/// <param name="bmpData">The bitmap data in which to perform the operation.</param>
		/// <param name="transKey">The transparency color. This color is rendered transparent
		/// by the DragDropHelper.</param>
		/// <remarks>
		/// This function only supports 32-bit pixel formats for now.
		/// </remarks>
		protected void ReplaceTransparentPixelsWithTransparentKey(Imaging.BitmapData bmpData, Drawing.Color transKey)
		{
			Imaging.PixelFormat pxFormat = bmpData.PixelFormat;

			if (Imaging.PixelFormat.Format32bppArgb == pxFormat
				|| Imaging.PixelFormat.Format32bppPArgb == pxFormat)
			{
				int transKeyArgb = transKey.ToArgb();

				// We will just iterate over the data... we don't care about pixel location,
				// just that every pixel is checked.
				unsafe
				{
					byte* pscan = (byte*)bmpData.Scan0.ToPointer();
					{
						for (int y = 0; y < bmpData.Height; ++y, pscan += bmpData.Stride)
						{
							int* prgb = (int*)pscan;
							for (int x = 0; x < bmpData.Width; ++x, ++prgb)
							{
								// If the alpha value is zero, replace this pixel's color
								// with the transparency key.
								if ((*prgb & 0xFF000000L) == 0L)
									*prgb = transKeyArgb;
							}
						}
					}
				}
			}
			else
			{
				// If it is anything else, we aren't supporting it, but we
				// won't throw, cause it isn't an error
				System.Diagnostics.Trace.TraceWarning("Not converting transparent colors to transparency key.");
				return;
			}
		}

		/// <summary>
		/// Converts a System.Windows.Media.Color to System.Drawing.Color.
		/// </summary>
		/// <param name="color">System.Windows.Media.Color value to convert.</param>
		/// <returns>System.Drawing.Color value.</returns>
		protected Drawing.Color ToDrawingColor(Color color)
		{
			return Drawing.Color.FromArgb(
				color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Converts a System.Windows.Int32Rect to a System.Drawing.Rectangle value.
		/// </summary>
		/// <param name="rect">The System.Windows.Int32Rect to convert.</param>
		/// <returns>The System.Drawing.Rectangle converted value.</returns>
		protected Drawing.Rectangle ToDrawingRectangle(Int32Rect rect)
		{
			return new Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>
		/// Converts the entries in a BitmapPalette to ColorPalette entries.
		/// </summary>
		/// <param name="destPalette">ColorPalette destination palette.</param>
		/// <param name="bitmapPalette">BitmapPalette source palette.</param>
		protected void ConvertColorPalette(Imaging.ColorPalette destPalette, BitmapPalette bitmapPalette)
		{
			Drawing.Color[] destEntries = destPalette.Entries;
			IList<Color> sourceEntries = bitmapPalette.Colors;

			if (destEntries.Length < sourceEntries.Count)
				throw new ArgumentException("Destination palette has less entries than the source palette");

			for (int i = 0, count = sourceEntries.Count; i < count; ++i)
				destEntries[i] = ToDrawingColor(sourceEntries[i]);
		}

		/// <summary>
		/// Converts a System.Windows.Media.PixelFormat instance to a
		/// System.Drawing.Imaging.PixelFormat value.
		/// </summary>
		/// <param name="pixelFormat">The input PixelFormat.</param>
		/// <returns>The converted value.</returns>
		protected Imaging.PixelFormat ConvertPixelFormat(PixelFormat pixelFormat)
		{
			if (PixelFormats.Bgr24 == pixelFormat)
				return Imaging.PixelFormat.Format24bppRgb;
			if (PixelFormats.Bgr32 == pixelFormat)
				return Imaging.PixelFormat.Format32bppRgb;
			if (PixelFormats.Bgr555 == pixelFormat)
				return Imaging.PixelFormat.Format16bppRgb555;
			if (PixelFormats.Bgr565 == pixelFormat)
				return Imaging.PixelFormat.Format16bppRgb565;
			if (PixelFormats.Bgra32 == pixelFormat)
				return Imaging.PixelFormat.Format32bppArgb;
			if (PixelFormats.BlackWhite == pixelFormat)
				return Imaging.PixelFormat.Format1bppIndexed;
			if (PixelFormats.Gray16 == pixelFormat)
				return Imaging.PixelFormat.Format16bppGrayScale;
			if (PixelFormats.Indexed1 == pixelFormat)
				return Imaging.PixelFormat.Format1bppIndexed;
			if (PixelFormats.Indexed4 == pixelFormat)
				return Imaging.PixelFormat.Format4bppIndexed;
			if (PixelFormats.Indexed8 == pixelFormat)
				return Imaging.PixelFormat.Format8bppIndexed;
			if (PixelFormats.Pbgra32 == pixelFormat)
				return Imaging.PixelFormat.Format32bppPArgb;
			if (PixelFormats.Prgba64 == pixelFormat)
				return Imaging.PixelFormat.Format64bppPArgb;
			if (PixelFormats.Rgb24 == pixelFormat)
				return Imaging.PixelFormat.Format24bppRgb;
			if (PixelFormats.Rgb48 == pixelFormat)
				return Imaging.PixelFormat.Format48bppRgb;
			if (PixelFormats.Rgba64 == pixelFormat)
				return Imaging.PixelFormat.Format64bppArgb;

			throw new NotSupportedException("The pixel format of the source bitmap is not supported.");
		}


		#endregion




		//protected virtual Drawing.Bitmap CreateElementBitmap(FrameworkElement element, object dragObject, int scale)
		//{
		//    int dpix, dpiy;
		//    GetDeviceDpi(element, out dpix, out dpiy);

		//    RenderTargetBitmap rbmp = new RenderTargetBitmap(
		//        (int)element.ActualWidth + 8,
		//        (int)element.ActualHeight + 8,
		//        dpix * 96.0,
		//        dpiy * 96.0,
		//        PixelFormats.Pbgra32);

		//    rbmp.Render(element);
		//    return BmpFromBmpSource(rbmp, scale);
		//}

		///// <summary>
		///// Gets the device capabilities.
		///// </summary>
		///// <param name="reference">A reference UIElement for getting the relevant device caps.</param>
		///// <param name="dpix">The horizontal DPI.</param>
		///// <param name="dpiy">The vertical DPI.</param>
		//protected void GetDeviceDpi(Visual reference, out int dpix, out int dpiy)
		//{
		//    Matrix m = PresentationSource.FromVisual(reference).CompositionTarget.TransformToDevice;
		//    dpix = (int)(96 * m.M11);
		//    dpiy = (int)(96 * m.M22);
		//}

		//protected Drawing.Bitmap BmpFromBmpSource(BitmapSource src, int scale)
		//{
		//    var bmp = new Drawing.Bitmap(
		//        (int)src.PixelWidth,
		//        (int)src.PixelHeight,
		//        Imaging.PixelFormat.Format32bppPArgb);

		//    var bdata = bmp.LockBits(
		//        new Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
		//        Imaging.ImageLockMode.ReadWrite,
		//        Imaging.PixelFormat.Format32bppPArgb);

		//    src.CopyPixels(
		//        new Int32Rect(0, 0, bmp.Width, bmp.Height),
		//        bdata.Scan0,
		//        bdata.Stride * bmp.Height,
		//        bdata.Stride);

		//    bmp.UnlockBits(bdata);

		//    PrepareBitmap(bmp);

		//    if (scale == 1)
		//        return bmp;

		//    System.Drawing.Bitmap nb = new System.Drawing.Bitmap(bmp.Width * scale, bmp.Height * scale);
		//    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(nb))
		//        g.DrawImage(bmp, 1, 1, bmp.Width * scale, bmp.Height * scale);

		//    return nb;
		//}


		//protected void PrepareBitmap(Drawing.Bitmap bmp)
		//{
		//    var bdata = bmp.LockBits(
		//        new Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
		//        Imaging.ImageLockMode.ReadWrite,
		//        Imaging.PixelFormat.Format32bppPArgb);

		//    int magenta = System.Drawing.Color.Magenta.ToArgb();
		//    unsafe
		//    {
		//        byte* pscan = (byte*)bdata.Scan0.ToPointer();
		//        for (int y = 0; y < bmp.Height; ++y, pscan += bdata.Stride)
		//        {
		//            int* prgb = (int*)pscan;
		//            for (int x = 0; x < bmp.Width; ++x, ++prgb)
		//            {
		//                if ((*prgb & 0xff000000L) == 0L)
		//                    *prgb = magenta;
		//            }
		//        }
		//    }

		//    bmp.UnlockBits(bdata);
		//}

		protected abstract Point GetDragPoint(FrameworkElement element);

		protected abstract DependencyObject GetDragSource(FrameworkElement element);

		protected void CreateDragHelper(
			Drawing.Bitmap bitmap,
			Point startPoint,
			System.Windows.DataObject dataObject)
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

			shdi.crColorKey = Drawing.Color.Magenta.ToArgb();

			// This HBITMAP will be managed by the DragDropHelper
			// as soon as we pass it to InitializeFromBitmap. If we fail
			// to make the hand off, we'll delete it to prevent a mem leak.
			IntPtr hbmp = bitmap.GetHbitmap();
			shdi.hbmpDragImage = hbmp;

			try
			{
				IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();

				try
				{
					sourceHelper.InitializeFromBitmap(ref shdi, (System.Runtime.InteropServices.ComTypes.IDataObject)dataObject);
				}
				catch (NotImplementedException ex)
				{
					throw new Exception("A NotImplementedException was caught. This could be because you forgot to construct your DataObject using a DragDropLib.DataObject", ex);
				}
			}
			catch
			{
				// We failed to initialize the drag image, so the DragDropHelper
				// won't be managing our memory. Release the HBITMAP we allocated.
				GDI32.DeleteObject(hbmp);
			}
		}

		protected virtual void OnDragStarted(object dragItem)
		{
			if (DragStart != null)
				DragStart(this, EventArgs.Empty);
		}

		protected virtual void DragEnded(DragDropEffects effects, object dragItem, FrameworkElement dragSource)
		{
			ResetDrag();

			foreach (var item in _DragHandlers)
				item.DragEnded(effects, dragSource);

			OnDragEnded(effects, dragItem, dragSource);
		}

		protected virtual void OnDragEnded(DragDropEffects effects, object dragItem, FrameworkElement dragSource)
		{
			if (DragEnd != null)
				DragEnd(this, new DragEndEventArgs() { DropEffects = effects, DropObject = dragItem, DragSource = dragSource });
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

			PrepareSupportedEffect(e, element);
		}

		protected override void OnDragLeave(DragEventArgs e, FrameworkElement element)
		{
			base.OnDragLeave(e, element);

			if (e.Handled)
				return;

			PrepareSupportedEffect(e, element);
	
			if (e.Handled && DragLeave != null)
				DragLeave(this, e);
		}

		protected override void OnDragOver(DragEventArgs e, FrameworkElement element)
		{
			base.OnDragOver(e, element);

			if (e.Handled)
				return;

			PrepareSupportedEffect(e, element);

			if (e.Handled && DragOver != null)
				DragOver(this, e);

			//foreach (var item in _DragHandlers)
			//   if (item.SupportDataFormat(e) != DragDropEffects.None)
			//      item.DataDragOver(element, e);
		}

		protected override void OnDrop(DragEventArgs e, FrameworkElement element)
		{
			PrepareSupportedEffect(e, element);
			e.Handled = false;

			base.OnDrop(e, element);

			if (e.Handled)
				return;

			bool handled = false;
			foreach (var item in _DragHandlers)
				if (!handled)
					handled |= item.HandleDragData(element, e);

			e.Handled = handled;
			
			ResetDrag();

			if (e.Handled && DragDroped != null)
				DragDroped(this, e);
			
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


		protected void PrepareSupportedEffect(DragEventArgs e, FrameworkElement element)
		{
			DragDropEffects effect = DragDropEffects.None;
			foreach (var item in _DragHandlers)
			{
				effect |= item.SupportDataFormat(element, e);
				if (effect != DragDropEffects.None)
					break;
			}

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
