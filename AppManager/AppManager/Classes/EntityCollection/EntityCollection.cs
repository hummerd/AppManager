using System;
using System.Collections.Generic;
using System.Text;
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

		public EntityCollection(IEnumerable<TEntity> collection)
			: base(collection)
		{
			_Deleted = new List<TEntity>();
			_DeletedItems = new ReadOnlyCollection<TEntity>(_Deleted);
		}


		public ReadOnlyCollection<TEntity> DeletedItems
		{
			get { return _DeletedItems; }
			set { _DeletedItems = value; }
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
					foreach (var item in this)
						if (object.ReferenceEquals(sourceCollection[i].CloneSource, item))
							soureItem = item;

					//TEntity soureItem = this.First(
					//   srch => object.ReferenceEquals(sourceCollection[i].CloneSource, srch));

					if (soureItem != null)
						soureItem.MergeEntity(sourceCollection[i]);
				}	
			}

			for (int i = 0; i < sourceCollection.DeletedItems.Count; i++)
				Remove(sourceCollection.DeletedItems[i].CloneSource);
		}


		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				_Deleted.AddRange(this);

			if (e.OldItems != null && e.OldItems.Count > 0)
				foreach (object item in e.OldItems)
					_Deleted.Add(item as TEntity);

			base.OnCollectionChanged(e);
		}
	}
}
