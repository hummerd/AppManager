﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;


namespace CommonLib
{
	public class Resizer
	{
		protected Grid			_Target;
		protected Control		_Resizer;
		protected UIElement	_TargetChild;
		protected int			_GridRow;
		protected Point		_InGridPos;
		protected bool			_DoResize = false;
		protected Brush		_ResizeBackColor;
		protected Brush		_OriginalBackColor;


		public Resizer(Control resizer, string name, Brush resizeBackColor)
		{
			_ResizeBackColor = resizeBackColor;
			_Resizer = resizer;
			_Target = UIHelper.FindAncestorOrSelf<Grid>(_Resizer, name);
			_OriginalBackColor = resizer.Background;

			var parent = VisualTreeHelper.GetParent(_Resizer);
			while (parent != null)
			{
				if (_Target.Children.Contains(parent as UIElement))
				{
					_TargetChild = parent as UIElement;
					break;				
				}

				parent = VisualTreeHelper.GetParent(parent);
			}
			
			_GridRow = Grid.GetRow(_TargetChild);

			resizer.MouseDown += new MouseButtonEventHandler(resizer_MouseDown);
			resizer.MouseUp += new MouseButtonEventHandler(resizer_MouseUp);
			resizer.MouseMove += new MouseEventHandler(resizer_MouseMove);
		}


		private void resizer_MouseMove(object sender, MouseEventArgs e)
		{
			if (_DoResize)
			{
				double newHeight = e.GetPosition(_Target).Y;
				double totalRelHeight = 0.0;
				double totalHeight = 0.0;

				for (int i = 0; i <= _GridRow; i++)
				{
					totalHeight += _Target.RowDefinitions[i].ActualHeight;
					totalRelHeight += _Target.RowDefinitions[i].Height.Value;
				}

				double newRelHeight = newHeight * totalRelHeight / totalHeight;
				for (int i = 0; i <= _GridRow; i++)
				{
					double p = _Target.RowDefinitions[i].Height.Value / totalRelHeight;
					_Target.RowDefinitions[i].Height = new GridLength(p * newRelHeight, GridUnitType.Star);
				}
			}
		}
		
		private void resizer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			_Resizer.CaptureMouse();
			_InGridPos = e.GetPosition(_Target);
			_DoResize = true;
			_Resizer.Background = _ResizeBackColor;
		}

		private void resizer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			_Resizer.ReleaseMouseCapture();
			_DoResize = false;
			_Resizer.Background = _OriginalBackColor;
		}
	}
}
