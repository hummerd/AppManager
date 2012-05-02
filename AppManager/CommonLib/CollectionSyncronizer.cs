using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;


namespace CommonLib
{
	public interface ISourceReference<T>
	{
		T Source { get; set; }
	}

    public class CollectionEventArgs<TTarget> : EventArgs
    {
        public CollectionEventArgs(ICollection<TTarget> collection)
        {
            Collection = collection;
        }

        public ICollection<TTarget> Collection { get; private set; }
    }

	public class CollectionSyncronizer<TSource, TTarget>
		where TTarget : ISourceReference<TSource>
	{
        public delegate bool Equal<T1, T2>(T1 obj1, T2 obj2);


        public static void FillCollection<TSrc, TDst>(
            IList<TDst> collection,
            Converter<TSrc, TDst> converter,
            IEnumerable items,
            int startAt)
        {
            int j = startAt;
            foreach (var item in items)
            {
                if (j >= collection.Count)
                    collection.Add(converter((TSrc)item));
                else
                    collection.Insert(j, converter((TSrc)item));

                j++;
            }
        }

        public static void Syncronize<TSrc, TTrg>(
            NotifyCollectionChangedAction action,
            IList<TTrg> collection,
            Equal<TTrg, object> eq,
            Converter<TSrc, TTrg> converter,
            IEnumerable items,
            int startAt)
            //where TTrg : ISourceReference<TSrc>
        {
            if (action == NotifyCollectionChangedAction.Add)
            {
                FillCollection(collection, converter, items, startAt);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                RemoveItems(collection, eq, items);
            }
            else if (action == NotifyCollectionChangedAction.Move)
            {
                foreach (var item in items)
                {
                    var found = FindElement(collection, t => eq(t, item));
                    collection.Remove(found);
                    collection.Insert(startAt++, found);
                }
            }
            else if (action == NotifyCollectionChangedAction.Replace)
            {

            }
            else if (action == NotifyCollectionChangedAction.Reset)
            {
                collection.Clear();
                FillCollection(collection, converter, items, 0);
            }
        }

        public static void RemoveItems<T>(IList<T> collection, Equal<T, object> eq, IEnumerable items)
        {
            foreach (var item in items)
            {
                collection.Remove(
                    FindElement(collection, t => eq(t, item)));
            }
        }

        public static T FindElement<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                    return item;
            }

            return default(T);
        }


        public event EventHandler<CollectionEventArgs<TTarget>> TargetUpdated;

		protected ObservableCollection<TSource> m_Source;
		protected IList<TTarget> m_Target;
		protected Converter<TSource, TTarget> m_Converter;
        protected Converter<TTarget, TSource> m_Restorer;
        protected bool m_DisableUpdate = false;


		public CollectionSyncronizer(
			ObservableCollection<TSource> source,
            ObservableCollection<TTarget> target,
			Converter<TSource, TTarget> converter,
			bool fillTarget
			)
		{
			m_Source = source;
			m_Target = target;
			m_Converter = converter;

			source.CollectionChanged -= source_CollectionChanged;
			source.CollectionChanged += source_CollectionChanged;

            target.CollectionChanged -= target_CollectionChanged;
            target.CollectionChanged += target_CollectionChanged;

			if (fillTarget)
			{
				FillTarget();
			}
		}

		protected void FillTarget()
		{
            m_DisableUpdate = true;
            Syncronize<TSource, TTarget>(
                NotifyCollectionChangedAction.Reset,
                m_Target,
                null,
                m_Converter,
                m_Source,
                0);
            m_DisableUpdate = false;
		}
        

        protected virtual void OnTargetUpdated()
        {
            if (TargetUpdated != null)
                TargetUpdated(this, new CollectionEventArgs<TTarget>(m_Target));
        }


		private void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
            if (m_DisableUpdate)
                return;

            var items = e.Action == NotifyCollectionChangedAction.Remove
                ? e.OldItems
                : e.NewItems;
            m_DisableUpdate = true;

            Syncronize(
                e.Action,
                m_Target,
                (t, d) => ReferenceEquals(t.Source, d),
                m_Converter, 
                items, 
                e.NewStartingIndex);

            m_DisableUpdate = false;
            OnTargetUpdated();
		}

        private void target_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (m_DisableUpdate)
                return;

            var items = e.Action == NotifyCollectionChangedAction.Remove
                ? e.OldItems
                : e.NewItems;
            m_DisableUpdate = true;

            Syncronize<TTarget, TSource>(
                e.Action,
                m_Source,
                (s, d) => ReferenceEquals(s, ((TTarget)d).Source),
                o => o.Source,
                items,
                e.NewStartingIndex);

            m_DisableUpdate = false;
           // OnTargetUpdated();
        }
	}
}
