using CommonLib;

namespace AppManager.Classes.ViewModel
{
    public class ViewBase<TSource> : ISourceReference<TSource>
	{
        protected MainWorkItem m_WorkItem;


        public ViewBase(MainWorkItem workItem)
        {
            m_WorkItem = workItem;
        }


		public TSource Source
		{
			get;
			set;
		}
    }
}
