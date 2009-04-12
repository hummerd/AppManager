using System.Windows;
using System.Windows.Media;


namespace CommonLib
{
	public class UIHelper
	{
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
