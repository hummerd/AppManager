using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.PInvoke;

namespace CommonLib.UI
{
    public static class ImageHelper
    {
        /// <summary>
        /// Scale to system large icon
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static BitmapSource TransformToLargeIcon(BitmapSource src)
        {
            var cx = User32.GetSystemMetrics(SystemMetrics.SM_CXICON);
            var cy = User32.GetSystemMetrics(SystemMetrics.SM_CYICON);

            if (cx != (int)src.Width ||
                cy != (int)src.Height)
            {
                return new TransformedBitmap(
                    src,
                    new ScaleTransform(cx / src.Width, cy / src.Height)
                    );
            }

            return src;
        }
    }
}
