using System;
using System.IO;
using System.Windows.Forms;
using CommonLib.PInvoke;
using CommonLib.Shell.OpenFileDialogExtension;


namespace AppManager.Windows
{
	public partial class OpenIconDlg : OpenFileDialogEx
	{
		public OpenIconDlg()
		{
			Application.EnableVisualStyles();
			InitializeComponent();
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


		public override void OnFileNameChanged(string filePath)
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

		public override void OnFolderNameChanged(string folderName)
		{
		}

		public override void OnClosingDialog()
		{
		}


		private void listViewIcon_DoubleClick(object sender, EventArgs e)
		{
			CloseDialog(true);
		}
	}
}
