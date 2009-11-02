namespace AppManager.Windows
{
	partial class OpenIconDlg
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.listViewIcon = new System.Windows.Forms.ListView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// listViewIcon
			// 
			this.listViewIcon.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewIcon.HideSelection = false;
			this.listViewIcon.LargeImageList = this.imageList;
			this.listViewIcon.Location = new System.Drawing.Point(0, 0);
			this.listViewIcon.MultiSelect = false;
			this.listViewIcon.Name = "listViewIcon";
			this.listViewIcon.ShowGroups = false;
			this.listViewIcon.Size = new System.Drawing.Size(255, 246);
			this.listViewIcon.TabIndex = 0;
			this.listViewIcon.UseCompatibleStateImageBehavior = false;
			this.listViewIcon.DoubleClick += new System.EventHandler(this.listViewIcon_DoubleClick);
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList.ImageSize = new System.Drawing.Size(32, 32);
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// OpenIconDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listViewIcon);
			this.Name = "OpenIconDlg";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewIcon;
		private System.Windows.Forms.ImageList imageList;
	}
}
