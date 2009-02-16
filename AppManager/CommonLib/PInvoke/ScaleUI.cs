using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.PInvoke
{
	/// <summary>
	/// 
	/// </summary>
	public static class ScaleUI
	{
		static System.Windows.Point _scaleUI;

		static ScaleUI()
		{
			// This method is dependent on having a WPF window visible
			//System.Windows.Media.Matrix m = System.Windows.PresentationSource.FromVisual( System.Windows.Application.Current.MainWindow ).CompositionTarget.TransformToDevice;
			//_scaleUI = new System.Windows.Point( m.M11, m.M22 );

			// Get the screen scaling from the system
			IntPtr screenDC = User32.GetDC(IntPtr.Zero);
			_scaleUI.X = GDI32.GetDeviceCaps(screenDC, GDI32.DeviceCap.LOGPIXELSX) / 96.0;
			_scaleUI.Y = GDI32.GetDeviceCaps(screenDC, GDI32.DeviceCap.LOGPIXELSY) / 96.0;
			User32.ReleaseDC(IntPtr.Zero, screenDC);
		}

		public static double UpScaleX(double x)
		{
			return x * _scaleUI.X;
		}

		public static double UpScaleY(double y)
		{
			return y * _scaleUI.Y;
		}

		public static double DownScaleX(double x)
		{
			return x / _scaleUI.X;
		}

		public static double DownScaleY(double y)
		{
			return y / _scaleUI.Y;
		}
	}
}
