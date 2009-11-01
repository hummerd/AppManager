using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Collections.Generic;


namespace CommonLib.PInvoke
{
	public static class IconEnumerator
	{
		public static BitmapSource[] EnumerateIconsWpf(string path)
		{
			int count = Shell32.ExtractIconEx(path, -1, null, null, 0);

			IntPtr[] largeIcons = new IntPtr[count];
			Shell32.ExtractIconEx(path, 0, largeIcons, null, count);

			var icons = new BitmapSource[count];
			for (int i = 0; i < count; i++)
			{
				icons[i] = Imaging.CreateBitmapSourceFromHIcon(
					largeIcons[i],
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions()
					);

				User32.DestroyIcon(largeIcons[i]);
			}

			return icons;
		}

		public static void FillImageList(string path, ImageList images, bool clear)
		{
			if (clear)
			{
				images.Images.Clear();
			}

			int count = Shell32.ExtractIconEx(path, -1, null, null, 0);
			IntPtr[] largeIcons = new IntPtr[1];
			//IntPtr[] smallIcons = new IntPtr[1];
	
			for (int i = 0; i < count; i++)
			{
				Shell32.ExtractIconEx(path, i, largeIcons, null, 1);
				var bmp = Icon.FromHandle(largeIcons[0]);
				images.Images.Add(bmp);
				User32.DestroyIcon(largeIcons[0]);
			}
		}

		public static Icon[] EnumerateIcons(string path)
		{
			int count = Shell32.ExtractIconEx(path, -1, null, null, 0);

			IntPtr[] largeIcons = new IntPtr[count];
			Shell32.ExtractIconEx(path, 0, largeIcons, null, count);

		 	var il = new Icon[count];
			
			for (int i = 0; i < count; i++)
			{
				il[i] = Icon.FromHandle(largeIcons[i]);
			}

			return il;
		}
	}
}
