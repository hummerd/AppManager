using System.Windows.Forms;
using System.Windows.Input;


namespace AppManager.Common
{
	public static class WinFrmMenuAdapter
	{
		public static ToolStripMenuItem CreateMenuItem(string text, ICommand cmd)
		{
			var tsmi = new ToolStripMenuItem(text);
			tsmi.Click += (s, e) => cmd.Execute((s as ToolStripItem).Tag);
			return tsmi;
		}
	}
}
