using System.Windows.Controls;


namespace CommonLib
{
	public class MenuHelper
	{
		public static ContextMenu CopyMenu(ContextMenu menu)
		{
			ContextMenu copy = new ContextMenu();
			copy.Style = menu.Style;
			copy.ItemContainerStyle = menu.ItemContainerStyle;

			foreach (var item in menu.Items)
			{
				MenuItem mi = item as MenuItem;
				if (mi != null)
				{
					Image newIcon = null;
					var icon = mi.Icon as Image;

					if (icon != null)
						newIcon = new Image() { Source = icon.Source};

					MenuItem mic = new MenuItem()
					{
						Header = mi.Header,
						Icon = newIcon
						//Style = mi.Style,
						//Template = mi.Template
					};

					copy.Items.Add(mic);
				}

				Separator s = item as Separator;
				if (s != null)
					copy.Items.Add(new Separator() { Style = s.Style });
			}

			return copy;
		}
	}
}
