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

	public class CollectionSyncronizer<TSource, TTarget>
		where TTarget : ISourceReference<TSource>
	{
		protected ObservableCollection<TSource> m_Source;
		protected ICollection<TTarget> m_Target;
		protected Converter<TSource, TTarget> m_Converter;


		public CollectionSyncronizer(
			ObservableCollection<TSource> source, 
			ICollection<TTarget> target,
			Converter<TSource, TTarget> converter,
			bool fillTarget
			)
		{
			m_Source = source;
			m_Target = target;
			m_Converter = converter;

			source.CollectionChanged -= source_CollectionChanged;
			source.CollectionChanged += source_CollectionChanged;

			if (fillTarget)
			{
				FillTarget();
			}
		}

		protected void FillTarget()
		{
			FillTarget(m_Source);
		}

		protected void FillTarget(IEnumerable items)
		{
			foreach (var item in items)
			{
				m_Target.Add(m_Converter((TSource)item));
			}
		}

		protected void RemoveTarget(IEnumerable items)
		{
			foreach (var item in items)
			{
				m_Target.Remove(
					FindTarget(t => ReferenceEquals(t.Source, item)));
			}
		}

		protected TTarget FindTarget(Predicate<TTarget> predicate)
		{
			foreach (var item in m_Target)
			{
				if (predicate(item))
					return item;
			}

			return default(TTarget);
		}

		private void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				FillTarget(e.NewItems);
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				RemoveTarget(e.OldItems);
			}
			else if (e.Action == NotifyCollectionChangedAction.Move)
			{
			
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{

			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				m_Target.Clear();
				FillTarget();
			}
		}
	}
}
