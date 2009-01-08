using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using System.Windows.Media;


namespace DragDropLib
{
	public class DropTargetHelper
	{
		protected IDropTargetHelper _DropTargetHelper;


		public DropTargetHelper(UIElement element)
		{
			_DropTargetHelper = (IDropTargetHelper)new DragDropHelper();

			element.DragEnter += DragEnterTarget;
			element.DragOver += DragOverTarget;
			element.DragLeave += DragLeaveTarget;
			element.Drop += DropTarget;
		}


		protected void OnDragEnterTarget(DragEventArgs e, FrameworkElement element)
		{
			Point p = e.GetPosition(element);
			Win32Point wp = new Win32Point() 
			{
				x = (int)p.X,
				y = (int)p.Y
			};
			
			WindowInteropHelper wndHelper = new WindowInteropHelper(FindAncestorOrSelf<Window>(element));
			_DropTargetHelper.DragEnter(wndHelper.Handle, (ComIDataObject)e.Data, ref wp, (int)e.Effects);
		}

		protected void OnDragLeaveTarget(DragEventArgs e)
		{
			_DropTargetHelper.DragLeave();
		}

		protected void OnDragOverTarget(DragEventArgs e, FrameworkElement element)
		{
			Win32Point wp;
			System.Windows.Point p = e.GetPosition(element);
			wp.x = (int)p.X;
			wp.y = (int)p.Y;
			_DropTargetHelper.DragOver(ref wp, (int)e.Effects);
		}

		protected void OnDropTarget(DragEventArgs e, FrameworkElement element)
		{
			Win32Point wp;
			System.Windows.Point p = e.GetPosition(element);
			wp.x = (int)p.X;
			wp.y = (int)p.Y;
			_DropTargetHelper.Drop((ComIDataObject)e.Data, ref wp, (int)e.Effects);
		}


		private void DragEnterTarget(object sender, DragEventArgs e)
		{
			OnDragEnterTarget(e, sender as FrameworkElement);
		}

		private void DragOverTarget(object sender, DragEventArgs e)
		{
			OnDragOverTarget(e, sender as FrameworkElement);
		}

		private void DragLeaveTarget(object sender, DragEventArgs e)
		{
			OnDragLeaveTarget(e);
		}

		private void DropTarget(object sender, DragEventArgs e)
		{
			OnDropTarget(e, sender as FrameworkElement);
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
	}
}
