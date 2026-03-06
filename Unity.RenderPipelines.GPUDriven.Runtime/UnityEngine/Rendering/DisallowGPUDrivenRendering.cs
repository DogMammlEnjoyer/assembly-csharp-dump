using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering
{
	[ExecuteInEditMode]
	internal class DisallowGPUDrivenRendering : MonoBehaviour
	{
		public bool applyToChildrenRecursively
		{
			get
			{
				return this.m_applyToChildrenRecursively;
			}
			set
			{
				this.m_applyToChildrenRecursively = value;
				this.OnDisable();
				this.OnEnable();
			}
		}

		private void OnEnable()
		{
			this.m_AppliedRecursively = this.applyToChildrenRecursively;
			if (this.applyToChildrenRecursively)
			{
				DisallowGPUDrivenRendering.AllowGPUDrivenRenderingRecursively(base.transform, false);
				return;
			}
			DisallowGPUDrivenRendering.AllowGPUDrivenRendering(base.transform, false);
		}

		private void OnDisable()
		{
			if (this.m_AppliedRecursively)
			{
				DisallowGPUDrivenRendering.AllowGPUDrivenRenderingRecursively(base.transform, true);
				return;
			}
			DisallowGPUDrivenRendering.AllowGPUDrivenRendering(base.transform, true);
		}

		private static void AllowGPUDrivenRendering(Transform transform, bool allow)
		{
			MeshRenderer component = transform.GetComponent<MeshRenderer>();
			if (component)
			{
				component.allowGPUDrivenRendering = allow;
			}
		}

		private static void AllowGPUDrivenRenderingRecursively(Transform transform, bool allow)
		{
			DisallowGPUDrivenRendering.AllowGPUDrivenRendering(transform, allow);
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (!transform2.GetComponent<DisallowGPUDrivenRendering>())
				{
					DisallowGPUDrivenRendering.AllowGPUDrivenRenderingRecursively(transform2, allow);
				}
			}
		}

		private void OnValidate()
		{
			this.OnDisable();
			if (base.enabled)
			{
				this.OnEnable();
			}
		}

		private bool m_AppliedRecursively;

		[FormerlySerializedAs("applyToChildrenRecursively")]
		public bool m_applyToChildrenRecursively;
	}
}
