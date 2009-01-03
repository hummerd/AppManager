using System;
using System.Runtime.InteropServices;

namespace DragDropLib
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Size
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ShDragImage
    {
        public Win32Size sizeDragImage;
        public Win32Point ptOffset;
        public IntPtr hbmpDragImage;
        public int crColorKey;
    }
}
