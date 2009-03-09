using System.Windows.Forms;
using System.Windows.Input;


namespace CommonLib
{
	public static class WinFrmMenuAdapter
	{
		public static ToolStripMenuItem CreateMenuItem(string text, ICommand cmd)
		{
			return CreateMenuItem(text, cmd, null);
		}

		public static ToolStripMenuItem CreateMenuItem(string text, ICommand cmd, object tag)
		{
			var tsmi = new ToolStripMenuItem(text);
			tsmi.Tag = tag;
			tsmi.Click += (s, e) => cmd.Execute((s as ToolStripItem).Tag);
			return tsmi;
		}
	}
}
