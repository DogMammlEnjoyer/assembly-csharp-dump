using System;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class PhysicsLayerSurface : MonoBehaviour, ISurface
	{
		public LayerMask LayerMask
		{
			get
			{
				return this._layerMask;
			}
			set
			{
				this._layerMask = value;
			}
		}

		public int CloseCollidersCacheSize
		{
			get
			{
				return this._closeCollidersCacheSize;
			}
			set
			{
				this._closeCollidersCacheSize = value;
			}
		}

		public Transform Transform
		{
			get
			{
				return null;
			}
		}

		protected virtual void Awake()
		{
			this._sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			this._sphereCollider.isTrigger = true;
			this._sphereCollider.enabled = false;
		}

		private void OnDestroy()
		{
			if (this._sphereCollider != null)
			{
				Object.Destroy(this._sphereCollider);
			}
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit surfaceHit, float maxDistance = 0f)
		{
			if (this._cachedCloseColliders == null || this._cachedCloseColliders.Length != this._closeCollidersCacheSize)
			{
				this._cachedCloseColliders = new Collider[this._closeCollidersCacheSize];
			}
			float num = (maxDistance > 0f) ? maxDistance : float.MaxValue;
			surfaceHit = default(SurfaceHit);
			int value = this._layerMask.value;
			int num2 = Physics.OverlapSphereNonAlloc(point, num, this._cachedCloseColliders, value, QueryTriggerInteraction.Ignore);
			if (num2 == 0)
			{
				return false;
			}
			float num3 = num;
			bool result = false;
			this._sphereCollider.enabled = true;
			for (int i = 0; i < num2; i++)
			{
				Collider collider = this._cachedCloseColliders[i];
				this._sphereCollider.radius = num3;
				Vector3 a;
				float num4;
				RaycastHit raycastHit;
				if (Physics.ComputePenetration(this._sphereCollider, point, Quaternion.identity, collider, collider.transform.position, collider.transform.rotation, out a, out num4) && collider.Raycast(new Ray(point, -a), out raycastHit, num3) && raycastHit.distance < num3)
				{
					result = true;
					num3 = raycastHit.distance;
					surfaceHit = new SurfaceHit
					{
						Point = raycastHit.point,
						Normal = raycastHit.normal,
						Distance = raycastHit.distance
					};
				}
			}
			this._sphereCollider.enabled = false;
			return result;
		}

		public bool Raycast(in Ray ray, out SurfaceHit surfaceHit, float maxDistance = 0f)
		{
			int value = this._layerMask.value;
			float maxDistance2 = (maxDistance > 0f) ? maxDistance : float.MaxValue;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, maxDistance2, value, QueryTriggerInteraction.Ignore))
			{
				surfaceHit = new SurfaceHit
				{
					Point = raycastHit.point,
					Normal = raycastHit.normal,
					Distance = raycastHit.distance
				};
				return true;
			}
			surfaceHit = default(SurfaceHit);
			return false;
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		[SerializeField]
		[Tooltip("Collision layers to detect hits against. -1 includes all layers.")]
		private LayerMask _layerMask = -1;

		[SerializeField]
		[Optional]
		[Tooltip("When using ClosestSurfacePoint, the maximum number of Colliders to check")]
		private int _closeCollidersCacheSize = 20;

		private Collider[] _cachedCloseColliders;

		private SphereCollider _sphereCollider;
	}
}
