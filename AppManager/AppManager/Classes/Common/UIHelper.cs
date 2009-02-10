using System.Windows;
using System.Windows.Media;


namespace AppManager.Common
{
	class UIHelper
	{
		public static T FindAncestorOrSelf<T>(DependencyObject obj)
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

        public TChildItem FindVisualChild<TChildItem>(DependencyObject obj)
            where TChildItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i) as TChildItem;

                if (child != null)
                    return child;
                else
                {
                    var childOfChild = FindVisualChild<TChildItem>(child);
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
