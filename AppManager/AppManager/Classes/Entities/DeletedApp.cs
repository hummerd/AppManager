using AppManager.EntityCollection;
using System;
using System.Collections;
using System.Collections.Generic;


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


		public void RegisterSource(AppGroup appGroup)
		{ 
			appGroup.AppInfoDeleted += (s, e) =>
				AddApp(s as AppType, e.Value);
		}

		public void AddApp(AppType appType, AppInfo appInfo)
		{
			foreach (var item in this)
			{
				if (item.App.EqualsByExecPath(appInfo) &&
					string.Equals(
						item.DeletedFrom.AppTypeName,
						appType.AppTypeName,
						StringComparison.CurrentCultureIgnoreCase))
					return;
			}

			appInfo.AppImage = null;
			Add(new DeletedApp() { App = appInfo, DeletedFrom = appType });
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
		}
	}
}
