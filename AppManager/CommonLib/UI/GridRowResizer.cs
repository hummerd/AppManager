using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;


namespace CommonLib.UI
{
	public class GridRowResizer : ResizerBase
	{
		protected Grid			_Target;
		protected UIElement	_TargetChild;
		protected int			_GridRow;
		protected Brush		_ResizeBackColor;
		protected Brush		_OriginalBackColor;


		public GridRowResizer(Control resizer, string name, Brush resizeBackColor)
			: base(resizer)
		{
			_ResizeBackColor = resizeBackColor;
			_OriginalBackColor = resizer.Background;
			_Target = UIHelper.FindAncestorOrSelf<Grid>(_Resizer, name);
			_Relative = _Target;

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


		protected override void PrepareResize(Point pos)
		{
			base.PrepareResize(pos);
			(_Resizer as Control).Background = _ResizeBackColor;
		}

		protected override void PrepareCursor(Point pos)
		{ 
			_Resizer.Cursor = Cursors.SizeNS;
		}

		protected override void DoResize(Point pos)
		{
			base.DoResize(pos);
			ResizeRows(pos.Y + _AccY);
		}

		protected override void EndResize()
		{
			base.EndResize();
			(_Resizer as Control).Background = _OriginalBackColor;
		}
	}
}
