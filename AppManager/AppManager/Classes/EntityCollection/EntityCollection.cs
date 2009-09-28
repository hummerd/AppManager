using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace AppManager.EntityCollection
{
	public class EntityCollection<TEntity> : ObservableCollection<TEntity>
		where TEntity : class, IClonableEntity<TEntity>
	{
		protected List<TEntity> _Deleted;
		protected ReadOnlyCollection<TEntity> _DeletedItems;


		public EntityCollection()
		{
			_Deleted = new List<TEntity>();
			_DeletedItems = new ReadOnlyCollection<TEntity>(_Deleted);
		}

		//public EntityCollection(IEnumerable<TEntity> collection)
		//    : base(collection)
		//{
		//    _Deleted = new List<TEntity>();
		//    _DeletedItems = new ReadOnlyCollection<TEntity>(_Deleted);
		//}


		public ReadOnlyCollection<TEntity> DeletedItems
		{
			get { return _DeletedItems; }
			set { _DeletedItems = value; }
		}


		public void AddRange(IEnumerable<TEntity> items)
		{
			foreach (var item in items)
				Add(item);
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


		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				_Deleted.AddRange(this);

			if (e.Action == NotifyCollectionChangedAction.Remove ||
				 e.Action == NotifyCollectionChangedAction.Replace)
				if (e.OldItems != null && e.OldItems.Count > 0)
					foreach (object item in e.OldItems)
						_Deleted.Add(item as TEntity);

			base.OnCollectionChanged(e);
		}
	}
}
