using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;


namespace AppManager.Commands
{
	public abstract class CommandBase : ICommand
	{
		public event EventHandler CanExecuteChanged;


		protected MainWorkItem _WorkItem;


		public CommandBase(MainWorkItem workItem)
		{
			_WorkItem = workItem;
		}


		public virtual bool CanExecute(object parameter)
		{
			return true;
		}

		public abstract void Execute(object parameter);


		protected virtual void OnCanExecuteChanged(EventArgs e)
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, e);
		}
	}
}
