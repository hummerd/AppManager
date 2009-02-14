using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using AppManager.Common;
using DragDropLib;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ButtonList : ListBox
	{
		public const string DragDataFormat = "ButtonListDataFormat";


		public event EventHandler DragStart;
		public event EventHandler DragEnd;

		public event EventHandler<ObjectEventArgs> EditItem;
		public event EventHandler<ObjectEventArgs> RenameItem;
		public event EventHandler<ObjectEventArgs> DeleteItem;
		public event EventHandler<ObjectEventArgs> PrepareItem;

		public event EventHandler<ObjectEventArgs> ButtonClicked;
		public event EventHandler<ValueEventArgs<string[]>> AddFiles;


		protected ButtonListDrag _DragHelper;
		protected ContextMenu _EditMenu;


		public ButtonList()
		{
			this.InitializeComponent();

			ItemContainerStyle = new Style();
			ItemContainerStyle.Resources[SystemColors.HighlightBrushKey] = Brushes.Transparent;
			ItemContainerStyle.Resources[SystemColors.ControlBrushKey] = Brushes.Transparent;

			_DragHelper = new ButtonListDrag(this, DragDataFormat, typeof(AppInfo));
			_DragHelper.AddFiles += (s, e) => OnAddFiles(e);
			_DragHelper.DragEnd += (s, e) => OnDragEnded();
			_DragHelper.DragStart += (s, e) => OnDragStarted();
			_DragHelper.PrepareItem += (s, e) => OnPrepareItem(e);

			_EditMenu = MenuHelper.CopyMenu(App.Current.Resources["ItemMenu"] as ContextMenu);
			((MenuItem)_EditMenu.Items[0]).Click += (s, ea) => OnEditItem(
				new ObjectEventArgs((s as FrameworkElement).DataContext));
			((MenuItem)_EditMenu.Items[1]).Click += (s, ea) => OnRenameItem(
				new ObjectEventArgs((s as FrameworkElement).DataContext));
			((MenuItem)_EditMenu.Items[2]).Click += (s, ea) => OnDeleteItem(
				new ObjectEventArgs((s as FrameworkElement).DataContext));
		}


		protected virtual void OnPrepareItem(ObjectEventArgs e)
		{
			if (PrepareItem != null)
				PrepareItem(this, e);
		}

		protected virtual void OnDragStarted()
		{
			if (DragStart != null)
				DragStart(this, EventArgs.Empty);
		}

		protected virtual void OnDragEnded()
		{
			if (DragEnd != null)
				DragEnd(this, EventArgs.Empty);
		}

		protected virtual void OnEditItem(ObjectEventArgs e)
		{
			if (EditItem != null)
				EditItem(this, e);
		}

		protected virtual void OnRenameItem(ObjectEventArgs e)
		{
			if (RenameItem != null)
				RenameItem(this, e);
		}

		protected virtual void OnDeleteItem(ObjectEventArgs e)
		{
			if (DeleteItem != null)
				DeleteItem(this, e);
		}

		protected virtual void OnAddFiles(ValueEventArgs<string[]> e)
		{
			if (AddFiles != null)
				AddFiles(this, e);
		}


		private void ButtonList_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (ButtonClicked != null &&
				 (e.Key == Key.Enter ||
				  e.Key == Key.Space)
				)
			{
				var lbi = Keyboard.FocusedElement as ListBoxItem;
				if (lbi != null && lbi.DataContext != null)
					ButtonClicked(this, new ObjectEventArgs(lbi.DataContext));
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
				ButtonClicked(this, new ObjectEventArgs(ib.DataContext));
		}
	}
}