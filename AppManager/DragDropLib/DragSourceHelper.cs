using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;


namespace DragDropLib
{
	public static class DragSourceHelper
	{
		public static System.Windows.DataObject CreateFromElement(
			FrameworkElement element,
			System.Windows.Point startPoint)
		{
			RenderTargetBitmap rbmp = new RenderTargetBitmap(
				(int)element.ActualWidth + 8,
				(int)element.ActualHeight + 8,
				96.0,
				96.0,
				PixelFormats.Default);

			rbmp.Render(element);

			Bitmap bmp = new Bitmap(
				(int)element.ActualWidth + 8,
				(int)element.ActualHeight + 8,
				System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

			BitmapData bdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			rbmp.CopyPixels(new Int32Rect(0, 0, bmp.Width, bmp.Height), bdata.Scan0, bdata.Stride * bmp.Height, bdata.Stride);

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

			System.Windows.DataObject data = new System.Windows.DataObject(new DragDropLib.DataObject());

			ShDragImage shdi = new ShDragImage();
			Win32Size size;
			size.cx = bmp.Width;
			size.cy = bmp.Height;
			shdi.sizeDragImage = size;
			Win32Point wpt;
			wpt.x = (int)startPoint.X;
			wpt.y = (int)startPoint.Y;
			shdi.ptOffset = wpt;
			shdi.hbmpDragImage = bmp.GetHbitmap();
			shdi.crColorKey = System.Drawing.Color.Magenta.ToArgb();

			IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();
			sourceHelper.InitializeFromBitmap(ref shdi, data);

			return data;
		}


		public static void OnDragEnter(DragEventArgs e, FrameworkElement element)
		{
			Win32Point wp;
			e.Effects = DragDropEffects.Copy;
			System.Windows.Point p = e.GetPosition(element);
			wp.x = (int)p.X;
			wp.y = (int)p.Y;
			WindowInteropHelper wndHelper = new WindowInteropHelper(FindAncestorOrSelf<Window>(element));
			IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
			dropHelper.DragEnter(wndHelper.Handle, (ComIDataObject)e.Data, ref wp, (int)e.Effects);

		}

		public static void OnDragLeave(DragEventArgs e)
		{
			IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
			dropHelper.DragLeave();
		}

		public static void OnDragOver(DragEventArgs e, FrameworkElement element)
		{
			Win32Point wp;
			e.Effects = DragDropEffects.Copy;
			System.Windows.Point p = e.GetPosition(element);
			wp.x = (int)p.X;
			wp.y = (int)p.Y;
			IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
			dropHelper.DragOver(ref wp, (int)e.Effects);
		}

		public static void OnDrop(DragEventArgs e, FrameworkElement element)
		{
			Win32Point wp;
			e.Effects = DragDropEffects.Copy;
			System.Windows.Point p = e.GetPosition(element);
			wp.x = (int)p.X;
			wp.y = (int)p.Y;
			IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
			dropHelper.Drop((ComIDataObject)e.Data, ref wp, (int)e.Effects);
		}


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
