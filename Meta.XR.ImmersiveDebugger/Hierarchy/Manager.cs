using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Hierarchy
{
	internal class Manager : DebugManagerAddon<Manager>
	{
		protected override Telemetry.Method Method
		{
			get
			{
				return Telemetry.Method.Hierarchy;
			}
		}

		public void ProcessItem(Item item)
		{
			InstanceHandle handle = item.Handle;
			this._instanceCache.RegisterHandle(handle);
			IDebugUIPanel uiPanel = DebugManagerAddon<Manager>._uiPanel;
			if (uiPanel != null)
			{
				uiPanel.RegisterInspector(handle, item.Category);
			}
			ComponentItem componentItem = item as ComponentItem;
			if (componentItem == null)
			{
				return;
			}
			foreach (MemberInfo memberInfo in componentItem.TypedOwner.GetType().GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (memberInfo.IsCompatibleWithDebugInspector() && (memberInfo.IsPublic() || RuntimeSettings.Instance.HierarchyViewShowsPrivateMembers))
				{
					foreach (IDebugManager debugManager in this._subDebugManagers)
					{
						debugManager.ProcessTypeFromHierarchy(item, memberInfo);
					}
				}
			}
		}

		public void UnprocessItem(Item item)
		{
			InstanceHandle handle = item.Handle;
			IDebugUIPanel uiPanel = DebugManagerAddon<Manager>._uiPanel;
			if (uiPanel != null)
			{
				uiPanel.UnregisterInspector(handle, item.Category, false);
			}
			this._instanceCache.UnregisterHandle(handle);
		}

		public void Refresh()
		{
			if (this._sceneRegistry.ComputeNeedsRefresh())
			{
				this._sceneRegistry.BuildChildren();
			}
		}

		private readonly SceneRegistry _sceneRegistry = new SceneRegistry();
	}
}
