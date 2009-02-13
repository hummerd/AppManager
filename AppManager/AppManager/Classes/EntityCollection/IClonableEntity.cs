using System;


namespace AppManager.EntityCollection
{
	public interface IClonableEntity<TEntity> : ICloneable
	{
		TEntity CloneSource { get; set; }

		TEntity CloneEntity();
		void MergeEntity(TEntity source);
	}
}
