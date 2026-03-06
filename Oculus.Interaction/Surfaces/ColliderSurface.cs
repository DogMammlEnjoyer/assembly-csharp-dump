using System;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class ColliderSurface : MonoBehaviour, ISurface, IBounds
	{
		protected virtual void Start()
		{
		}

		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		public Bounds Bounds
		{
			get
			{
				return this._collider.bounds;
			}
		}

		public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0f)
		{
			hit = default(SurfaceHit);
			RaycastHit raycastHit;
			if (this._collider.Raycast(ray, out raycastHit, (maxDistance <= 0f) ? 3.4028235E+38f : maxDistance))
			{
				hit.Point = raycastHit.point;
				hit.Normal = raycastHit.normal;
				hit.Distance = raycastHit.distance;
				return true;
			}
			return false;
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance = 0f)
		{
			Vector3 vector = this._collider.ClosestPoint(point) - point;
			Ray ray;
			if (vector.x == 0f && vector.y == 0f && vector.z == 0f)
			{
				Vector3 vector2 = this._collider.bounds.center - point;
				ray = new Ray(point - vector2, vector2);
				return this.Raycast(ray, out hit, float.MaxValue);
			}
			ray = new Ray(point, vector);
			return this.Raycast(ray, out hit, maxDistance);
		}

		public void InjectAllColliderSurface(Collider collider)
		{
			this.InjectCollider(collider);
		}

		public void InjectCollider(Collider collider)
		{
			this._collider = collider;
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		[Tooltip("The Surface will be represented by this collider.")]
		[SerializeField]
		private Collider _collider;
	}
}
