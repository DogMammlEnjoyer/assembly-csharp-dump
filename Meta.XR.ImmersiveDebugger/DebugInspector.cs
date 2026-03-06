using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.Manager;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger
{
	[ExecuteInEditMode]
	public class DebugInspector : MonoBehaviour
	{
		internal string Category
		{
			get
			{
				return this._category;
			}
		}

		internal DebugInspector.InspectionRegistry Registry
		{
			get
			{
				return this.registry;
			}
		}

		private void OnValidate()
		{
			this.Initialize();
		}

		internal void Initialize()
		{
			this.registry.Initialize(this);
		}

		private void OnEnable()
		{
			this.Initialize();
			if (Application.isPlaying)
			{
				DebugManagerAddon<DebugInspectorManager>.Instance.RegisterInspector(this);
			}
		}

		private void OnDisable()
		{
			DebugManagerAddon<DebugInspectorManager>.Instance.UnregisterInspector(this);
		}

		[Tooltip("Defines a default category for all inspected data handled by this component. These can still be overriden by specifying another category individually in the inspected data properties.")]
		[SerializeField]
		private string _category;

		[SerializeField]
		private DebugInspector.InspectionRegistry registry = new DebugInspector.InspectionRegistry();

		[Serializable]
		internal class InspectionRegistry
		{
			internal List<InspectedHandle> Handles
			{
				get
				{
					return this.handles;
				}
			}

			internal void Initialize(DebugInspector owner)
			{
				foreach (InspectedHandle inspectedHandle in this.handles)
				{
					inspectedHandle.Initialize(owner);
				}
				foreach (Component component in owner.GetComponents<Component>())
				{
					if (!(component == null))
					{
						Type type = component.GetType();
						InspectedHandle item;
						if (!(type == typeof(DebugInspector)) && !this.TryGetHandle(component, out item))
						{
							item = new InspectedHandle(owner, type);
							this.handles.Add(item);
						}
					}
				}
			}

			private bool TryGetHandle(Component component, out InspectedHandle inspectedHandle)
			{
				inspectedHandle = null;
				foreach (InspectedHandle inspectedHandle2 in this.handles)
				{
					if (inspectedHandle2.InstanceHandle.Instance == component)
					{
						inspectedHandle = inspectedHandle2;
						break;
					}
				}
				return inspectedHandle != null;
			}

			[SerializeField]
			private List<InspectedHandle> handles = new List<InspectedHandle>();
		}
	}
}
