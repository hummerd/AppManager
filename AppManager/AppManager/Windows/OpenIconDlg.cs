using System;
using System.IO;
using System.Windows.Forms;
using CommonLib.PInvoke;
using CommonLib.Shell.OpenFileDialogExtension;


namespace AppManager.Windows
{
	public partial class OpenIconDlg : UserControl
	{
		protected OpenFileDialogEx _OpenFile; 


		public OpenIconDlg()
		{
			InitializeComponent();
		}


		public DialogResult ShowOpenFileDialog(IWin32Window owner)
		{
			_OpenFile = new OpenFileDialogEx();
			_OpenFile.Filter = Strings.ICON_FILTER;
			_OpenFile.Title = Strings.CHANGE_ICON;
			_OpenFile.SelectionChanged += (s, e) => OnFileNameChanged(_OpenFile.FileName);
			return _OpenFile.ShowDialog(this, owner);
		}


		public string SelectedFile
		{
			get
			{
				return _OpenFile.FileName;
			}
		}

		public int SelectedIconIndex
		{
			get
			{
				if (listViewIcon.SelectedIndices.Count > 0)
					return listViewIcon.SelectedIndices[0];

				return -1;
			}
		}


		protected void OnFileNameChanged(string filePath)
		{
			if (!File.Exists(filePath))
			{
				listViewIcon.Clear();
				imageList.Images.Clear();
				return;
			}

			listViewIcon.Clear();
			IconEnumerator.FillImageList(filePath, imageList, true);

			var cnt = imageList.Images.Count;
			for (int i = 0; i < cnt; i++)
				listViewIcon.Items.Add(String.Empty, i);

			if (cnt > 0)
				listViewIcon.Items[0].Selected = true;
		}


		private void listViewIcon_DoubleClick(object sender, EventArgs e)
		{
			_OpenFile.CloseDialog(true);
		}
	}
}
