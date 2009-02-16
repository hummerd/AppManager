using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using CommonLib;


namespace DragDropLib
{
	public class SimpleDragHelper : DragHelperBase
	{
		protected Drawing.Bitmap _Bitmap;
		protected string _DragElementName;


		public SimpleDragHelper(
			FrameworkElement control, 
			string dataFormat, 
			Type dataType,
			Drawing.Bitmap bmp,
			string dragElementName)
			: base(control, dataFormat, dataType, false)
		{
			_Bitmap = bmp;
			_DragElementName = dragElementName;
		}


		protected override object GetDragObject(FrameworkElement element)
		{
			return element.DataContext;
		}

		protected override Drawing.Bitmap CreateElementBitmap(FrameworkElement element, object dragObject, int scale)
		{
			var res = UIHelper.FindVisualChild<FrameworkElement>(element, _DragElementName);
			
			var bmp = CombineBitmap(
				_Bitmap,
				base.CreateElementBitmap(res, null, 1));
			PrepareBitmap(bmp);

			return bmp;
		}

		protected Drawing.Bitmap CombineBitmap(Drawing.Bitmap bmp1, Drawing.Bitmap bmp2)
		{
			int
				w = bmp1.Width + bmp2.Width,
				h = Math.Max(bmp1.Height, bmp2.Height);

			var result = new Drawing.Bitmap(w, h);

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result))
			{
				int
					l = 0,
					t = (h - bmp1.Height) / 2;

				g.DrawImage(bmp1, l, t, bmp1.Width, bmp1.Height);

				l = bmp1.Width;
				t = (h - bmp2.Height) / 2;
				g.DrawImage(bmp2, l, t + 4, bmp2.Width, bmp2.Height);
			}

			return result;
		}

		protected override Point GetDragPoint(FrameworkElement element)
		{
			return new Point(_Bitmap.Width / 2, _Bitmap.Width / 2);
		}

		protected override DependencyObject GetDragSource(FrameworkElement element)
		{
			return element;
		}

		//protected override void HandleDropedObject(FrameworkElement element, DragEventArgs e, object dragObject)
		//{

		//}

		//protected override void DragEnded(DragDropEffects effects, object dragItem)
		//{

		//}
	}
}
