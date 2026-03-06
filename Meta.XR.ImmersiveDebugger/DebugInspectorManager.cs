using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger
{
	internal class DebugInspectorManager : DebugManagerAddon<DebugInspectorManager>
	{
		public void RegisterInspector(DebugInspector inspector)
		{
			this._inspectors.Add(inspector);
			this.ProcessInspector(inspector);
		}

		public void UnregisterInspector(DebugInspector inspector)
		{
			this.UnprocessInspector(inspector);
			this._inspectors.Remove(inspector);
		}

		protected override Telemetry.Method Method
		{
			get
			{
				return Telemetry.Method.DebugInspector;
			}
		}

		protected override void OnReadyInternal()
		{
			foreach (DebugInspector inspector in this._inspectors)
			{
				this.ProcessInspector(inspector);
			}
		}

		private void ProcessInspector(DebugInspector inspector)
		{
			if (DebugManagerAddon<DebugInspectorManager>._uiPanel == null)
			{
				return;
			}
			foreach (InspectedHandle inspectedHandle in inspector.Registry.Handles)
			{
				if (inspectedHandle.Visible)
				{
					InstanceHandle instanceHandle = inspectedHandle.InstanceHandle;
					this._instanceCache.RegisterHandle(instanceHandle);
					foreach (InspectedMember inspectedMember in inspectedHandle.inspectedMembers)
					{
						if (inspectedMember.Visible)
						{
							MemberInfo memberInfo = inspectedMember.MemberInfo;
							if (!(memberInfo == null))
							{
								DebugMember attribute = inspectedMember.attribute;
								if (attribute != null)
								{
									this.UpdateCategory(attribute, inspector);
									DebugManagerAddon<DebugInspectorManager>._uiPanel.RegisterInspector(instanceHandle, DebugInspectorManager.FetchCategory(attribute));
									foreach (IDebugManager debugManager in this._subDebugManagers)
									{
										debugManager.ProcessTypeFromInspector(instanceHandle.Type, instanceHandle, memberInfo, attribute);
									}
								}
							}
						}
					}
				}
			}
		}

		private void UnprocessInspector(DebugInspector inspector)
		{
			if (DebugManagerAddon<DebugInspectorManager>._uiPanel == null)
			{
				return;
			}
			foreach (InspectedHandle inspectedHandle in inspector.Registry.Handles)
			{
				InstanceHandle instanceHandle = inspectedHandle.InstanceHandle;
				foreach (InspectedMember inspectedMember in inspectedHandle.inspectedMembers)
				{
					DebugMember attribute = inspectedMember.attribute;
					if (attribute != null)
					{
						DebugManagerAddon<DebugInspectorManager>._uiPanel.UnregisterInspector(instanceHandle, DebugInspectorManager.FetchCategory(attribute), false);
					}
				}
				this._instanceCache.UnregisterHandle(instanceHandle);
			}
		}

		private void UpdateCategory(DebugMember attribute, DebugInspector inspector)
		{
			if (string.IsNullOrEmpty(attribute.Category))
			{
				attribute.Category = inspector.Category;
			}
		}

		private static Category FetchCategory(DebugMember attribute)
		{
			return new Category
			{
				Id = attribute.Category
			};
		}

		private readonly List<DebugInspector> _inspectors = new List<DebugInspector>();
	}
}
