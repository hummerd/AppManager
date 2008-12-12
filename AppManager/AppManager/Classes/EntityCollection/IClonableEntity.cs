using System;
using System.Collections.Generic;
using System.Text;

namespace AppManager.EntityCollection
{
	public interface IClonableEntity<TEntity> : ICloneable
	{
		TEntity CloneSource { get; set; }

		TEntity CloneEntity();
		void MergeEntity(TEntity source);
	}
}
