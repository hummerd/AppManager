using System;
using System.Collections;
using System.Collections.Generic;
using AppManager.EntityCollection;


namespace AppManager.Entities
{
	public class DeletedAppCollection : EntityCollection<DeletedApp>
	{
		public DeletedAppCollection()
		{

		}

		public DeletedAppCollection(IEnumerable<DeletedApp> collection)
			: base(collection)
		{

		}

		public DeletedAppCollection(IList collection)
		{
			foreach (DeletedApp item in collection)
			{
				Add(item);
			}
		}


		public DeletedApp FindByApp(AppType appType, AppInfo appInfo)
		{
			for (int i = 0; i < Count; i++)
			{
				var item = this[i];

				if (item.App.EqualsByExecPath(appInfo) &&
					SameOrWithoutAppType(item.DeletedFrom, appType))
					return item;
			}

			return null;
		}

		public void RegisterSource(AppGroup appGroup)
		{ 
			appGroup.AppInfoDeleted += (s, e) =>
				AddApp(s as AppType, e.Value, true);
		}

		public void AddApp(AppType appType, AppInfo appInfo, bool resetImage)
		{
			if (FindByApp(appType, appInfo) != null)
				return;
	
			if (resetImage)
				appInfo.AppImage = null;

			if (appType != null)
				appType = appType.CloneWithoutItems();

			Add(new DeletedApp { App = appInfo, DeletedFrom = appType });
		}


		protected bool SameOrWithoutAppType(AppType appType1, AppType appType2)
		{
			if (appType1 == null &&  appType2 == null)
				return true;
			
			if (appType1 != null ||  appType2 != null)
				return false;

			return string.Equals(
				appType1.AppTypeName,
				appType2.AppTypeName,
				StringComparison.CurrentCultureIgnoreCase);
		}
	}


	public class DeletedApp : EntityBase<DeletedApp>
	{
		public AppType DeletedFrom { get; set; }
		public AppInfo App { get; set; }


		protected override void MergeEntity(DeletedApp source, bool clone)
		{
			base.MergeEntity(source, clone);

			if (App != source.App)
				App = source.App;

			if (DeletedFrom != source.DeletedFrom)
				DeletedFrom = source.DeletedFrom;

			if (!clone)
				App.ImagePath = null;
		}
	}
}
