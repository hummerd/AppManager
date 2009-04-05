using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using CommonLib.Windows;


namespace VersionBuilder
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (e.Args.Length > 0)
			{
				try
				{
					VersionFactory vf = new VersionFactory();
					vf.CreateVersion(
						e.Args[0].Substring(1, e.Args[0].Length - 1),
						new Version(e.Args[1].Substring(1, e.Args[1].Length - 1)),
						e.Args[2].Substring(1, e.Args[2].Length - 1),
						e.Args[3].Substring(1, e.Args[3].Length - 1),
						e.Args[4].Substring(1, e.Args[4].Length - 1)
						);

					Shutdown();
				}
				catch (Exception exc)
				{
					string args = String.Empty;
					foreach (var item in e.Args)
					{
						args += Environment.NewLine + item;
					}

					ErrorBox.Show("Error", args, String.Empty);
					ErrorBox.Show("Error", exc);
					Shutdown(-1 * e.Args.Length);
				}
			}
			else
			{
				MainWindow mw = new MainWindow();
				MainWindow = mw;
				mw.Closed += (s, ea) => Shutdown();
				mw.Show();
			}
		}
	}
}
