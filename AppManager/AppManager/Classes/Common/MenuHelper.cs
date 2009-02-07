﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;


namespace AppManager.Common
{
	public class MenuHelper
	{
		public static ContextMenu CopyMenu(ContextMenu menu)
		{
			ContextMenu copy = new ContextMenu();
			copy.Style = menu.Style;

			foreach (var item in menu.Items)
			{
				MenuItem mi = item as MenuItem;
				if (mi != null)
				{
					MenuItem mic = new MenuItem()
					{
						Header = mi.Header,
						Style = mi.Style
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
