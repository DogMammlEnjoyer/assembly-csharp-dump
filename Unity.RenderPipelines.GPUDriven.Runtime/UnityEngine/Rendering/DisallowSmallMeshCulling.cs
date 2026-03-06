using System;

namespace UnityEngine.Rendering
{
	[ExecuteInEditMode]
	internal class DisallowSmallMeshCulling : MonoBehaviour
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
				DisallowSmallMeshCulling.AllowSmallMeshCullingRecursively(base.transform, false);
				return;
			}
			DisallowSmallMeshCulling.AllowSmallMeshCulling(base.transform, false);
		}

		private void OnDisable()
		{
			if (this.m_AppliedRecursively)
			{
				DisallowSmallMeshCulling.AllowSmallMeshCullingRecursively(base.transform, true);
				return;
			}
			DisallowSmallMeshCulling.AllowSmallMeshCulling(base.transform, true);
		}

		private static void AllowSmallMeshCulling(Transform transform, bool allow)
		{
			MeshRenderer component = transform.GetComponent<MeshRenderer>();
			if (component)
			{
				component.smallMeshCulling = allow;
			}
		}

		private static void AllowSmallMeshCullingRecursively(Transform transform, bool allow)
		{
			DisallowSmallMeshCulling.AllowSmallMeshCulling(transform, allow);
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (!transform2.GetComponent<DisallowGPUDrivenRendering>())
				{
					DisallowSmallMeshCulling.AllowSmallMeshCullingRecursively(transform2, allow);
				}
			}
		}

		private void OnValidate()
		{
			this.OnDisable();
			this.OnEnable();
		}

		private bool m_AppliedRecursively;

		public bool m_applyToChildrenRecursively;
	}
}
