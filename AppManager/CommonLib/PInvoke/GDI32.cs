using System;
using System.Runtime.InteropServices;


namespace CommonLib.PInvoke
{
		/// <summary>
		/// Functions and delegates used for performing PInvoke for Win32 calls in GDI32.dll
		/// </summary>
	public class GDI32
	{
		private GDI32() { }

		/// <summary>
		/// See MSDN documentation for the Win32 function DeleteDC.
		/// </summary>
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRectRgn(
			int nLeftRect,   // x-coordinate of upper-left corner
			int nTopRect,    // y-coordinate of upper-left corner
			int nRightRect,  // x-coordinate of lower-right corner
			int nBottomRect  // y-coordinate of lower-right corner
		);

		/// <summary>
		/// See MSDN documentation for the Win32 function CreateRoundRectRgn.
		/// </summary>
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		public enum CombineRgnStyles : int
		{
			RGN_AND = 1,
			RGN_OR = 2,
			RGN_XOR = 3,
			RGN_DIFF = 4,
			RGN_COPY = 5,
			RGN_MIN = RGN_AND,
			RGN_MAX = RGN_COPY
		}

		/// <summary>
		/// See MSDN documentation for the Win32 function DeleteDC.
		/// </summary>
		[DllImport("gdi32.dll")]
		public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1,
			IntPtr hrgnSrc2, CombineRgnStyles fnCombineMode);

		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hdc, DeviceCap deviceCap);

		#region GDI32 Enums
		/// <summary>
		/// Specifies a raster-operation code.
		/// These codes define how the color data for the source rectangle is to be combined
		/// with the color data for the destination rectangle to achieve the final color. 
		/// </summary>
		[Flags]
		public enum RasterOp : uint
		{
			/// <summary>
			/// dest = source 
			/// </summary>
			SRCCOPY = 0x00CC0020,
			/// <summary>
			/// dest = source OR dest
			/// </summary>
			SRCPAINT = 0x00EE0086,
			/// <summary>
			/// dest = source AND dest
			/// </summary>
			SRCAND = 0x008800C6,
			/// <summary>
			/// dest = source XOR dest
			/// </summary>
			SRCINVERT = 0x00660046,
			/// <summary>
			/// dest = source AND (NOT dest )
			/// </summary>
			SRCERASE = 0x00440328,
			/// <summary>
			/// dest = (NOT source)
			/// </summary>
			NOTSRCCOPY = 0x00330008,
			/// <summary>
			/// dest = (NOT src) AND (NOT dest)
			/// </summary>
			NOTSRCERASE = 0x001100A6,
			/// <summary>
			/// dest = (source AND pattern)
			/// </summary>
			MERGECOPY = 0x00C000CA,
			/// <summary>
			/// dest = (NOT source) OR dest
			/// </summary>
			MERGEPAINT = 0x00BB0226,
			/// <summary>
			/// dest = pattern
			/// </summary>
			PATCOPY = 0x00F00021,
			/// <summary>
			/// dest = DPSnoo
			/// </summary>
			PATPAINT = 0x00FB0A09,
			/// <summary>
			/// dest = pattern XOR dest
			/// </summary>
			PATINVERT = 0x005A0049,
			/// <summary>
			/// dest = (NOT dest)
			/// </summary>
			DSTINVERT = 0x00550009,
			/// <summary>
			/// dest = BLACK
			/// </summary>
			BLACKNESS = 0x00000042,
			/// <summary>
			/// dest = WHITE 
			/// </summary>
			WHITENESS = 0x00FF0062,
			/// <summary>
			/// Do not Mirror the bitmap in this call
			/// </summary>
			NOMIRRORBITMAP = 0x80000000,
			/// <summary>
			/// Include layered windows
			/// </summary>
			CAPTUREBLT = 0x40000000
		}

		/// <summary>
		/// 
		/// </summary>
		public enum BMIColorFormat
		{
			/// <summary>
			/// The color table should consist of literal red, green, blue (RGB) values.
			/// </summary>
			DIB_RGB_COLORS = 0,
			/// <summary>
			/// The color table should consist of an array of 16-bit indexes into the current logical palette.
			/// </summary>
			DIB_PAL_COLORS = 1,
		}

		/// <summary>
		/// 
		/// </summary>
		public enum BinaryROPMode
		{
			/// <summary>
			/// 
			/// </summary>
			R2_BLACK = 1,
			/// <summary>
			/// 
			/// </summary>
			R2_COPYPEN = 13,
			/// <summary>
			/// 
			/// </summary>
			R2_MASKNOTPEN = 3,
			/// <summary>
			/// 
			/// </summary>
			R2_MASKPEN = 9,
			/// <summary>
			/// 
			/// </summary>
			R2_MASKPENNOT = 5,
			/// <summary>
			/// 
			/// </summary>
			R2_MERGENOTPEN = 12,
			/// <summary>
			/// 
			/// </summary>
			R2_MERGEPEN = 15,
			/// <summary>
			/// 
			/// </summary>
			R2_MERGEPENNOT = 14,
			/// <summary>
			/// 
			/// </summary>
			R2_NOP = 11,
			/// <summary>
			/// 
			/// </summary>
			R2_NOT = 6,
			/// <summary>
			/// 
			/// </summary>
			R2_NOTCOPYPEN = 4,
			/// <summary>
			/// 
			/// </summary>
			R2_NOTMASKPEN = 8,
			/// <summary>
			/// 
			/// </summary>
			R2_NOTMERGEPEN = 2,
			/// <summary>
			/// 
			/// </summary>
			R2_NOTXORPEN = 10,
			/// <summary>
			/// 
			/// </summary>
			R2_WHITE = 16,
			/// <summary>
			/// 
			/// </summary>
			R2_XORPEN = 7
		}


		public enum DeviceCap
		{
			/// <summary>
			/// Device driver version
			/// </summary>
			DRIVERVERSION = 0,
			/// <summary>
			/// Device classification
			/// </summary>
			TECHNOLOGY = 2,
			/// <summary>
			/// Horizontal size in millimeters
			/// </summary>
			HORZSIZE = 4,
			/// <summary>
			/// Vertical size in millimeters
			/// </summary>
			VERTSIZE = 6,
			/// <summary>
			/// Horizontal width in pixels
			/// </summary>
			HORZRES = 8,
			/// <summary>
			/// Vertical height in pixels
			/// </summary>
			VERTRES = 10,
			/// <summary>
			/// Number of bits per pixel
			/// </summary>
			BITSPIXEL = 12,
			/// <summary>
			/// Number of planes
			/// </summary>
			PLANES = 14,
			/// <summary>
			/// Number of brushes the device has
			/// </summary>
			NUMBRUSHES = 16,
			/// <summary>
			/// Number of pens the device has
			/// </summary>
			NUMPENS = 18,
			/// <summary>
			/// Number of markers the device has
			/// </summary>
			NUMMARKERS = 20,
			/// <summary>
			/// Number of fonts the device has
			/// </summary>
			NUMFONTS = 22,
			/// <summary>
			/// Number of colors the device supports
			/// </summary>
			NUMCOLORS = 24,
			/// <summary>
			/// Size required for device descriptor
			/// </summary>
			PDEVICESIZE = 26,
			/// <summary>
			/// Curve capabilities
			/// </summary>
			CURVECAPS = 28,
			/// <summary>
			/// Line capabilities
			/// </summary>
			LINECAPS = 30,
			/// <summary>
			/// Polygonal capabilities
			/// </summary>
			POLYGONALCAPS = 32,
			/// <summary>
			/// Text capabilities
			/// </summary>
			TEXTCAPS = 34,
			/// <summary>
			/// Clipping capabilities
			/// </summary>
			CLIPCAPS = 36,
			/// <summary>
			/// Bitblt capabilities
			/// </summary>
			RASTERCAPS = 38,
			/// <summary>
			/// Length of the X leg
			/// </summary>
			ASPECTX = 40,
			/// <summary>
			/// Length of the Y leg
			/// </summary>
			ASPECTY = 42,
			/// <summary>
			/// Length of the hypotenuse
			/// </summary>
			ASPECTXY = 44,
			/// <summary>
			/// Shading and Blending caps
			/// </summary>
			SHADEBLENDCAPS = 45,

			/// <summary>
			/// Logical pixels inch in X
			/// </summary>
			LOGPIXELSX = 88,
			/// <summary>
			/// Logical pixels inch in Y
			/// </summary>
			LOGPIXELSY = 90,

			/// <summary>
			/// Number of entries in physical palette
			/// </summary>
			SIZEPALETTE = 104,
			/// <summary>
			/// Number of reserved entries in palette
			/// </summary>
			NUMRESERVED = 106,
			/// <summary>
			/// Actual color resolution
			/// </summary>
			COLORRES = 108,

			// Printing related DeviceCaps. These replace the appropriate Escapes
			/// <summary>
			/// Physical Width in device units
			/// </summary>
			PHYSICALWIDTH = 110,
			/// <summary>
			/// Physical Height in device units
			/// </summary>
			PHYSICALHEIGHT = 111,
			/// <summary>
			/// Physical Printable Area x margin
			/// </summary>
			PHYSICALOFFSETX = 112,
			/// <summary>
			/// Physical Printable Area y margin
			/// </summary>
			PHYSICALOFFSETY = 113,
			/// <summary>
			/// Scaling factor x
			/// </summary>
			SCALINGFACTORX = 114,
			/// <summary>
			/// Scaling factor y
			/// </summary>
			SCALINGFACTORY = 115,

			/// <summary>
			/// Current vertical refresh rate of the display device (for displays only) in Hz
			/// </summary>
			VREFRESH = 116,
			/// <summary>
			/// Horizontal width of entire desktop in pixels
			/// </summary>
			DESKTOPVERTRES = 117,
			/// <summary>
			/// Vertical height of entire desktop in pixels
			/// </summary>
			DESKTOPHORZRES = 118,
			/// <summary>
			/// Preferred blt alignment
			/// </summary>
			BLTALIGNMENT = 119
		}
		#endregion
	}
}