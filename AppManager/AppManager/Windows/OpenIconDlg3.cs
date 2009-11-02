using System;
using System.IO;
using System.Windows.Forms;
using CommonLib.PInvoke;


namespace AppManager.Windows
{
	public class OpenIconDlg3
	{
		protected ListView _IconPreview;
		protected ImageList _Icons;
		protected CommonLib.Shell.CustomOpenFileDialog _Dialog;


		public OpenIconDlg3()
		{
			Panel p = CreateUIExtension();
			_Dialog = new CommonLib.Shell.CustomOpenFileDialog(
				"ico",
				String.Empty,
				"Icon files (*.ico;*.dll;*.exe;*.ocx)\0*.ico;*.dll;*.exe;*.ocx\0\0",
				p);
			_Dialog.SelectionChanged += (path) => SelectionChanged(path);
		}


		public bool Open(out string path, out int index)
		{
			path = String.Empty;
			index = -1;

			try
			{
				if (_Dialog.Show() && _IconPreview.SelectedIndices.Count > 0)
				{
					path = _Dialog.SelectedPath;
					index = _IconPreview.SelectedIndices[0];
					return true;
				}
			}
			finally
			{
				_Dialog.Dispose();
			}

			return false;
		}


		protected void SelectionChanged(string path)
		{
			if (!File.Exists(path))
			{
				_IconPreview.Clear();
				_Icons.Images.Clear();
				return;
			}

			_IconPreview.Clear();
			IconEnumerator.FillImageList(path, _Icons, true);

			var cnt = _Icons.Images.Count;
			for (int i = 0; i < cnt; i++)
				_IconPreview.Items.Add(String.Empty, i);

			if (cnt > 0)
				_IconPreview.Items[0].Selected = true;
		}

		protected Panel CreateUIExtension()
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			Panel mainHost = new Panel();
			mainHost.BorderStyle = BorderStyle.None;
			mainHost.Width = 250;

			_Icons = new ImageList();
			_Icons.ColorDepth = ColorDepth.Depth32Bit;
			_Icons.ImageSize = new System.Drawing.Size(32, 32);
			_Icons.TransparentColor = System.Drawing.Color.Transparent;

			_IconPreview = new ListView();
			_IconPreview.HideSelection = false;
			_IconPreview.DoubleClick += (s, e) => _Dialog.Close(true);
			_IconPreview.LargeImageList = _Icons;
			_IconPreview.Dock = DockStyle.Fill;

			//var split = new Splitter();
			//split.Dock = DockStyle.Left;
			//mainHost.Controls.Add(split);
			mainHost.Controls.Add(_IconPreview);

			return mainHost;
		}
	}
}
