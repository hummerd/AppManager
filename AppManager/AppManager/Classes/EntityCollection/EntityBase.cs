using System.ComponentModel;


namespace AppManager.EntityCollection
{
	public class EntityBase<T> : IClonableEntity<T>, INotifyPropertyChanged
		where T : EntityBase<T>, new()
	{
		public event PropertyChangedEventHandler PropertyChanged;


		public int ID { get; set; }

		#region IClonableEntity<NameSynonym> Members

		public T CloneSource
		{
			get;
			set;
		}

		public T CloneEntity()
		{
			T clone = new T();
			clone.MergeEntity((T)this, true);
			clone.CloneSource = (T)this;
			return clone;
		}

		public void MergeEntity(T source)
		{
			MergeEntity(source, false);
		}

		protected virtual void MergeEntity(T source, bool clone)
		{
			ID = source.ID;
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return CloneEntity();
		}

		#endregion


		protected virtual void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}
