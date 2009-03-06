using System;
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
		//protected Point		_InGridPos;
		protected bool			_DoResize = false;
		protected Brush		_ResizeBackColor;
		protected Brush		_OriginalBackColor;
		protected double		_AccY;


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

			_Target.LayoutUpdated += (s, e) => ChangeResizerVisibility();
			ChangeResizerVisibility();

			resizer.MouseDown += new MouseButtonEventHandler(resizer_MouseDown);
			resizer.MouseUp += new MouseButtonEventHandler(resizer_MouseUp);
			resizer.MouseMove += new MouseEventHandler(resizer_MouseMove);
		}
		

		protected void ChangeResizerVisibility()
		{
			_GridRow = Grid.GetRow(_TargetChild);
			if (_GridRow == _Target.RowDefinitions.Count - 1)
				_Resizer.Visibility = Visibility.Hidden;
			else
				_Resizer.Visibility = Visibility.Visible;
		}
		
		protected void ResizeRows(double dragHeight)
		{
			double topRowsHeight = GetRowsHeight(0, _GridRow - 1);
			double bottomRowsHeight = GetRowsHeight(_GridRow + 2, _Target.RowDefinitions.Count - 1);

			if (dragHeight < topRowsHeight + _Target.RowDefinitions[_GridRow].MinHeight)
				dragHeight = topRowsHeight + _Target.RowDefinitions[_GridRow].MinHeight;

			if (dragHeight > _Target.ActualHeight - bottomRowsHeight - _Target.RowDefinitions[_GridRow + 1].MinHeight)
				dragHeight = _Target.ActualHeight - bottomRowsHeight - _Target.RowDefinitions[_GridRow + 1].MinHeight;

			double totalRel = GetTotalRelativeHeight(0, _Target.RowDefinitions.Count - 1);
			double newHeight = dragHeight - topRowsHeight;
			double newNextHeight = _Target.RowDefinitions[_GridRow + 1].ActualHeight + 
				_Target.RowDefinitions[_GridRow].ActualHeight - newHeight;
	
			newHeight = totalRel * newHeight / _Target.ActualHeight;
			newNextHeight = totalRel * newNextHeight / _Target.ActualHeight;

			_Target.RowDefinitions[_GridRow].Height = new GridLength(
				newHeight, GridUnitType.Star);

			_Target.RowDefinitions[_GridRow + 1].Height = new GridLength(
				newNextHeight, GridUnitType.Star);
		}

		protected double GetMinRowsHeight(int sRow, int eRow)
		{
			double result = 0.0;
			for (int i = sRow; i <= eRow; i++)
				result += _Target.RowDefinitions[i].MinHeight;

			return result;
		}

		protected double GetRowsHeight(int sRow, int eRow)
		{
			double result = 0.0;

			if (eRow < 0)
				return result;

			if (sRow >= _Target.RowDefinitions.Count)
				return result;
			
			for (int i = sRow; i <= eRow; i++)
				result += _Target.RowDefinitions[i].ActualHeight;

			return result;
		}

		protected double GetTotalRelativeHeight(int sRow, int eRow)
		{
			double result = 0.0;
			int rowCount = _Target.RowDefinitions.Count;

			for (int i = 0; i < rowCount; i++)
				result += _Target.RowDefinitions[i].Height.Value;

			return result;
		}

		protected void SetRelHeight(int sRow, int eRow/*, double newHeight*/, double newRelHeight)
		{
			double totalRelHeight = 0.0;
			//double totalHeight = 0.0;

			for (int i = sRow; i <= eRow; i++)
			{
				//totalHeight += _Target.RowDefinitions[i].ActualHeight;
				totalRelHeight += _Target.RowDefinitions[i].Height.Value;
			}

			//double newRelHeight = newHeight * totalRelHeight / totalHeight;
			for (int i = sRow; i <= eRow; i++)
			{
				double minRel = 10000 * _Target.RowDefinitions[i].MinHeight / _Target.ActualHeight;
				double p = _Target.RowDefinitions[i].Height.Value / totalRelHeight;

				_Target.RowDefinitions[i].Height = new GridLength(
					Math.Max(p * newRelHeight, minRel), GridUnitType.Star);
			}
		}


		private void resizer_MouseMove(object sender, MouseEventArgs e)
		{
			if (_DoResize)
			{
				ResizeRows(e.GetPosition(_Target).Y + _AccY);
				e.Handled = true;
			}
		}
		
		private void resizer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			_Resizer.CaptureMouse();
			//_InGridPos = e.GetPosition(_Target);
			_DoResize = true;
			_Resizer.Background = _ResizeBackColor;
			_AccY = _Resizer.ActualHeight - e.GetPosition(_Resizer).Y;
		}

		private void resizer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			_Resizer.ReleaseMouseCapture();
			_DoResize = false;
			_Resizer.Background = _OriginalBackColor;
		}
	}
}
