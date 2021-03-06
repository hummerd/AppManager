﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;


namespace AppManager.EntityCollection
{
	public class EntityCollection<TEntity> : ObservableCollection<TEntity>
		where TEntity : class, IClonableEntity<TEntity>
	{
		protected List<TEntity> _Deleted;
		protected ReadOnlyCollection<TEntity> _DeletedItems;
		protected bool _Resetting = false;


		public EntityCollection()
		{
			_Deleted = new List<TEntity>();
			_DeletedItems = new ReadOnlyCollection<TEntity>(_Deleted);
		}

		//Not supported in .net 3.0
		//public EntityCollection(IEnumerable<TEntity> collection)
		//    : base(collection)
		//{
		//    _Deleted = new List<TEntity>();
		//    _DeletedItems = new ReadOnlyCollection<TEntity>(_Deleted);
		//}

		public EntityCollection(IList collection)
		{
			TryIncreaseCapacity(collection);

			_Deleted = new List<TEntity>();
			_DeletedItems = new ReadOnlyCollection<TEntity>(_Deleted);

			for (int i = 0; i < collection.Count; i++)
			{
				Add(collection[i] as TEntity);
			}
		}


		public ReadOnlyCollection<TEntity> DeletedItems
		{
			get { return _DeletedItems; }
			set { _DeletedItems = value; }
		}


		public void Reset()
		{
			try
			{
				_Resetting = true;
				Clear();
				_Deleted.Clear();
			}
			finally
			{
				_Resetting = false;
			}
		}

		public void AddRange(IEnumerable<TEntity> items)
		{
			TryIncreaseCapacity(items);
			foreach (var item in items)
				Add(item);
		}

        public void AddRangeBefore(IEnumerable<TEntity> items, TEntity before)
        {
            TryIncreaseCapacity(items);

            int ix = -1;
            if (before != null)
            {
                ix = IndexOf(before);
            }

            if (ix < 0)
            {
                foreach (var item in items)
                    Add(item);
            }
            else
            {
                foreach (var item in items)
                    InsertItem(ix++, item);                
            }
        }

		public TEntity FindBySource(TEntity search)
		{
			foreach (var item in this)
				if (item.CloneSource.Equals(search))
					return item;

			return null;
		}

		public EntityCollection<TEntity> Copy()
		{
			EntityCollection<TEntity> copy = new EntityCollection<TEntity>();
			foreach (IClonableEntity<TEntity> item in this)
				copy.Add(item.CloneEntity());

			return copy;
		}

		public void Combine(EntityCollection<TEntity> sourceCollection, bool clone)
		{
			if (clone)
				AddRange(sourceCollection.Copy());
			else
				MergeCollection(sourceCollection);
		}

		public void MergeCollection(EntityCollection<TEntity> sourceCollection)
		{
			for (int i = 0; i < sourceCollection.Count; i++)
			{
				if (sourceCollection[i].CloneSource == null)
					Insert(i, sourceCollection[i]);
				else
				{
					TEntity soureItem = null;
					int ix = 0;

					for (int j = 0; j < this.Count; j++)
						if (object.ReferenceEquals(sourceCollection[i].CloneSource, this[j]))
						{
							soureItem = this[j];
							ix = j;
						}

					if (soureItem != null)
					{
						if (i != ix)
							Move(ix, i);

						soureItem.MergeEntity(sourceCollection[i]);
					}
				}
			}

			for (int i = 0; i < sourceCollection.DeletedItems.Count; i++)
				Remove(sourceCollection.DeletedItems[i].CloneSource);
		}

        public void DeleteAll(Predicate<TEntity> predicate)
        {
            if (Count <= 0)
                return;

            _Resetting = true;
            try
            {
                for (int i = Count - 1; i >= 0; i--)
                {
                    if (predicate(this[i]))
                        RemoveAt(i);
                }
            }
            finally
            {
                _Resetting = false;
            }
        }


		protected void TryIncreaseCapacity(IEnumerable items)
		{
			var list = Items as List<TEntity>;
			if (list != null)
			{
				var coll = items as ICollection;
				if (coll != null)
					list.Capacity += coll.Count;
			}
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (!_Resetting)
			{
				if (e.Action == NotifyCollectionChangedAction.Reset)
					_Deleted.AddRange(this);

				if (e.Action == NotifyCollectionChangedAction.Remove ||
					 e.Action == NotifyCollectionChangedAction.Replace)
					if (e.OldItems != null && e.OldItems.Count > 0)
						foreach (object item in e.OldItems)
							_Deleted.Add(item as TEntity);
			}

			base.OnCollectionChanged(e);
		}
	}
}
