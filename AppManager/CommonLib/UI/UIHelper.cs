using System.Windows;
using System.Windows.Media;
using System;


namespace CommonLib
{
	public static class UIHelper
	{
		public static void UpdateOnLoaded(FrameworkElement element, Action<FrameworkElement> act)
		{
			if (element.IsLoaded)
			{
				act(element);
			}
			else
			{
				RoutedEventHandler eh = null;
				eh = new RoutedEventHandler(delegate
				{
					act(element);
					element.Loaded -= eh;
				});
				element.Loaded += eh;//new RoutedEventHandler(element_Loaded);
			}
		}


		public static Color FromARGB(int argb)
		{
			return Color.FromArgb(
				(byte)((argb >> 24) & 0xFF),
				(byte)((argb >> 16) & 0xFF),
				(byte)((argb >> 8 ) & 0xFF),
				(byte)((argb) & 0xFF)
				);
		}

		public static int ToARGB(Color color)
		{
			return
				(int)(color.A) << 24 |
				(int)(color.R) << 16 |
				(int)(color.G) << 8 |
				(int)(color.B) ;
		}

		public static T FindLogicalAncestorOrSelf<T>(DependencyObject obj, string name)
			where T : FrameworkElement
		{
			while (obj != null)
			{
				T objTest = obj as T;

				if (name == null && objTest != null)
					return objTest;
				else if (name != null && objTest != null && objTest.Name == name)
					return objTest;

				obj = LogicalTreeHelper.GetParent(obj);
			}

			return null;
		}

		public static T FindAncestorOrSelf<T>(DependencyObject obj, string name)
			where T : FrameworkElement
		{
			while (obj != null)
			{
				T objTest = obj as T;

				if (name == null && objTest != null)
					return objTest;
				else if (name != null && objTest != null && objTest.Name == name)
					return objTest;

				obj = GetParent(obj);
			}

			return null;
		}

		public static TChildItem FindVisualChild<TChildItem>(DependencyObject obj, string name)
				where TChildItem : FrameworkElement
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				var result = child as TChildItem;

				if (name == null && result != null)
					return result;
				else if (name != null && result != null && result.Name == name)
					return result;
				else
				{
					var childOfChild = FindVisualChild<TChildItem>(child, name);
					if (childOfChild != null)
						return childOfChild;
				}
			}

			return null;
		}


		private static void element_Loaded(object sender, RoutedEventArgs e)
		{
			var element = sender as FrameworkElement;
			element.Loaded -= element_Loaded;


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
