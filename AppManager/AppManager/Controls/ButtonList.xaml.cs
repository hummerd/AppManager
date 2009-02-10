using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using AppManager.Common;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ButtonList : ListBox
	{
		public event EventHandler<ObjEventArgs> EditItem;
		public event EventHandler<ObjEventArgs> RenameItem;
		public event EventHandler<ObjEventArgs> DeleteItem;

		public event EventHandler<ObjEventArgs> ButtonClicked;
		public event EventHandler<StrArrEventArgs> AddFiles;


		protected ButtonListDrag _DragHelper;
		protected ContextMenu _EditMenu;

		public ButtonList()
		{
			this.InitializeComponent();

			
			ItemContainerStyle = new Style();
			ItemContainerStyle.Resources[SystemColors.HighlightBrushKey] = Brushes.Transparent;
			ItemContainerStyle.Resources[SystemColors.ControlBrushKey] = Brushes.Transparent;

			_DragHelper = new ButtonListDrag(this, "ButtonListDataFormat", typeof(AppInfo));
			_DragHelper.AddFiles += (s, e) => OnAddFiles(e);

			_EditMenu = MenuHelper.CopyMenu(App.Current.Resources["ItemMenu"] as ContextMenu);
			((MenuItem)_EditMenu.Items[0]).Click += (s, ea) => OnEditItem(
				new ObjEventArgs() { Obj = (s as FrameworkElement).DataContext });
			((MenuItem)_EditMenu.Items[1]).Click += (s, ea) => OnRenameItem(
				new ObjEventArgs() { Obj = (s as FrameworkElement).DataContext });
			((MenuItem)_EditMenu.Items[2]).Click += (s, ea) => OnDeleteItem(
				new ObjEventArgs() { Obj = (s as FrameworkElement).DataContext });
		}


		protected void OnEditItem(ObjEventArgs e)
		{
			if (EditItem != null)
				EditItem(this, e);
		}

		protected void OnRenameItem(ObjEventArgs e)
		{
			if (RenameItem != null)
				RenameItem(this, e);
		}

		protected void OnDeleteItem(ObjEventArgs e)
		{
			if (DeleteItem != null)
				DeleteItem(this, e);
		}
		
		protected void OnAddFiles(StrArrEventArgs e)
		{
			if (AddFiles != null)
				AddFiles(this, e);
		}


		//private void ImageButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		//{
		//   ImageButton ib = sender as ImageButton;
		//   if (ButtonClicked != null)
		//      ButtonClicked(this, new ObjEventArgs() { Obj = ib.DataContext });
		//}

		private void ButtonList_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (ButtonClicked != null &&
				 (e.Key == Key.Enter ||
				  e.Key == Key.Space)
				)
			{
				var lbi = Keyboard.FocusedElement as ListBoxItem;
				if (lbi != null && lbi.DataContext != null)
					ButtonClicked(this, new ObjEventArgs() { Obj = lbi.DataContext });
			}
		}

		private void ButtonList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			FrameworkElement item = e.OriginalSource as FrameworkElement;
			if (item != null)
			{
				item = ContainerFromElement(item) as FrameworkElement;
				if (item != null)
				{
					_EditMenu.DataContext = item.DataContext;

					((MenuItem)_EditMenu.Items[0]).Header = Strings.MNU_EDIT + " " + item.DataContext;
					((MenuItem)_EditMenu.Items[1]).Header = Strings.MNU_RENAME + " " + item.DataContext;
					((MenuItem)_EditMenu.Items[2]).Header = Strings.MNU_DELETE + " " + item.DataContext;

					_EditMenu.Placement = PlacementMode.Right;
					_EditMenu.PlacementTarget = item;
					_EditMenu.IsOpen = true;
				}
			}
		}

		private void ImageButton_Click(object sender, RoutedEventArgs e)
		{
			ImageButton ib = sender as ImageButton;
			if (ButtonClicked != null)
				ButtonClicked(this, new ObjEventArgs() { Obj = ib.DataContext });
		}
	}


	public class ObjEventArgs : EventArgs
	{
		public object Obj { get; set; }
	}
}