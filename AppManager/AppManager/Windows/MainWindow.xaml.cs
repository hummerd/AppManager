using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AppManager.Controls;
using System.ComponentModel;
using System.IO;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		protected MainWindowController _Controller = new MainWindowController();

		public MainWindow()
		{
			InitializeComponent();
		}


		public void Init(MainWorkItem workItem)
		{
			ButtonExit.Command = workItem.Commands.Deactivate;
			BtnManageApps.Command = workItem.Commands.ManageApps;
			BtnConfigure.Command = workItem.Commands.Settings;

			InputBindings[0].Command = workItem.Commands.Deactivate;

			ContentPanel.Children.Clear();
			ContentPanel.RowDefinitions.Clear();

			int rowi = 0;
			foreach (var appType in workItem.AppData.AppTypes)
			{
				RowDefinition row = new RowDefinition();
				row.Height = new GridLength(100.0, GridUnitType.Star);
				ContentPanel.RowDefinitions.Add(row);
				
				GroupBox group = new GroupBox();
				group.Margin = new Thickness(7.0);
				group.SetBinding(GroupBox.HeaderProperty, "AppTypeName");
				group.DataContext = appType;

				ButtonList groupContent = new ButtonList();
				groupContent.AllowDrop = true;
				groupContent.Drop += new DragEventHandler(GroupContent_Drop);
				groupContent.ButtonClicked += delegate (object sender, ObjEventArgs e) 
					{ workItem.Commands.RunApp.Execute(e.Obj); };
				groupContent.SetBinding(ButtonList.ItemsSourceProperty, "AppInfos");
				groupContent.DataContext = appType;
				group.Content = groupContent;
				ContentPanel.Children.Add(group);
				Grid.SetRow(group, rowi++);

				if (rowi > 1)
				{
					GridSplitter split = new GridSplitter();
					split.ResizeDirection = GridResizeDirection.Rows;
					split.Height = 3;
					split.VerticalAlignment = VerticalAlignment.Top;
					split.HorizontalAlignment = HorizontalAlignment.Stretch;
					split.ShowsPreview = true;
					split.Background = Brushes.Transparent;
					split.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(Split_DragCompleted);
					Grid.SetRow(split, rowi - 1);
					ContentPanel.Children.Add(split);
				}
			}
		}

		public void SaveState()
		{
			SaveRowHeight();
		}

		public void LoadState()
		{
			LoadRowHeight();
		}


		protected void SaveRowHeight()
		{
			string wl = String.Empty;
			foreach (var item in ContentPanel.RowDefinitions)
			{
				wl = wl + item.Height.Value + ";";
			}

			AppManager.Properties.Settings.Default.MainRowWidth = wl.Trim(';');
			AppManager.Properties.Settings.Default.Save();
		}

		protected void LoadRowHeight()
		{
			string[] w = AppManager.Properties.Settings.Default.MainRowWidth.Split(';');

			int i = 0;
			foreach (var item in w)
			{
				if (i >= ContentPanel.RowDefinitions.Count)
					break;

				if (String.IsNullOrEmpty(item))
					break;

				ContentPanel.RowDefinitions[i].Height = new GridLength(
					double.Parse(item), GridUnitType.Star);

				i++;
			}
		}

		protected string[] GetFilesFromDrop(IDataObject data)
		{
			if (data.GetDataPresent(DataFormats.FileDrop, true))
			{
				return data.GetData(DataFormats.FileDrop, true) as string[];
			}

			return null;
		}


		private void Split_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			SaveRowHeight();
		}

		private void GroupContent_Drop(object sender, DragEventArgs e)
		{
			ButtonList bl = sender as ButtonList;
			e.Handled = true;
			string[] files = GetFilesFromDrop(e.Data);

			_Controller.AddFiles(bl.DataContext as AppType, files);
		}

		private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}
	}
}
