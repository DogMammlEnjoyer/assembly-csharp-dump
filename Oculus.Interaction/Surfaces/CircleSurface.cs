using System;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class CircleSurface : MonoBehaviour, ISurfacePatch, ISurface
	{
		public Transform Transform
		{
			get
			{
				return this._planeSurface.Transform;
			}
		}

		public ISurface BackingSurface
		{
			get
			{
				return this._planeSurface;
			}
		}

		protected virtual void Start()
		{
		}

		public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0f)
		{
			return this._planeSurface.Raycast(ray, out hit, maxDistance) && Vector3.SqrMagnitude(this.Transform.InverseTransformPoint(hit.Point)) <= this._radius * this._radius;
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance = 0f)
		{
			if (!this._planeSurface.ClosestSurfacePoint(point, out hit, maxDistance))
			{
				return false;
			}
			Vector3 position = Vector3.ClampMagnitude(this.Transform.InverseTransformPoint(hit.Point), this._radius);
			Vector3 vector = this.Transform.TransformPoint(position);
			hit.Point = vector;
			hit.Distance = Vector3.Distance(point, vector);
			return maxDistance <= 0f || hit.Distance <= maxDistance;
		}

		[Obsolete("Use InjectAllCircleSurface instead.")]
		public void InjectAllCircleProximityField(PlaneSurface planeSurface)
		{
			this.InjectAllCircleSurface(planeSurface);
		}

		public void InjectAllCircleSurface(PlaneSurface planeSurface)
		{
			this.InjectPlaneSurface(planeSurface);
		}

		public void InjectPlaneSurface(PlaneSurface planeSurface)
		{
			this._planeSurface = planeSurface;
		}

		public void InjectOptionalRadius(float radius)
		{
			this._radius = radius;
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		[Tooltip("The circle will lay upon this plane, with the circle's center at the plane surface's origin.")]
		[SerializeField]
		private PlaneSurface _planeSurface;

		[Tooltip("The radius of the circle.")]
		[SerializeField]
		private float _radius = 0.1f;
	}
}
