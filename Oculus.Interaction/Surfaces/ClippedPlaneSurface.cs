using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class ClippedPlaneSurface : MonoBehaviour, IClippedSurface<IBoundsClipper>, ISurfacePatch, ISurface
	{
		private List<IBoundsClipper> Clippers { get; set; }

		public ISurface BackingSurface
		{
			get
			{
				return this._planeSurface;
			}
		}

		public Transform Transform
		{
			get
			{
				return this._planeSurface.Transform;
			}
		}

		public IReadOnlyList<IBoundsClipper> GetClippers()
		{
			if (this.Clippers != null)
			{
				return this.Clippers;
			}
			return this._clippers.ConvertAll<IBoundsClipper>((Object clipper) => clipper as IBoundsClipper);
		}

		protected virtual void Awake()
		{
			this.Clippers = this._clippers.ConvertAll<IBoundsClipper>((Object clipper) => clipper as IBoundsClipper);
		}

		protected virtual void Start()
		{
		}

		public bool ClipBounds(in Bounds bounds, out Bounds clipped)
		{
			clipped = bounds;
			IReadOnlyList<IBoundsClipper> clippers = this.GetClippers();
			for (int i = 0; i < clippers.Count; i++)
			{
				IBoundsClipper boundsClipper = clippers[i];
				Bounds bounds2;
				if (boundsClipper != null && boundsClipper.GetLocalBounds(this.Transform, out bounds2) && !clipped.Clip(bounds2, out clipped))
				{
					return false;
				}
			}
			return true;
		}

		private Vector3 ClampPoint(in Vector3 point, in Bounds bounds)
		{
			Bounds bounds2 = bounds;
			Vector3 min = bounds2.min;
			bounds2 = bounds;
			Vector3 max = bounds2.max;
			Vector3 vector = this.Transform.InverseTransformPoint(point);
			Vector3 position = new Vector3(Mathf.Clamp(vector.x, min.x, max.x), Mathf.Clamp(vector.y, min.y, max.y), Mathf.Clamp(vector.z, min.z, max.z));
			return this.Transform.TransformPoint(position);
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance = 0f)
		{
			Bounds bounds;
			if (this._planeSurface.ClosestSurfacePoint(point, out hit, maxDistance) && this.ClipBounds(ClippedPlaneSurface.PlaneBounds, out bounds))
			{
				Vector3 point2 = hit.Point;
				hit.Point = this.ClampPoint(point2, bounds);
				hit.Distance = Vector3.Distance(point, hit.Point);
				return maxDistance <= 0f || hit.Distance <= maxDistance;
			}
			return false;
		}

		public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0f)
		{
			Bounds bounds;
			return this.BackingSurface.Raycast(ray, out hit, maxDistance) && this.ClipBounds(ClippedPlaneSurface.InfiniteBounds, out bounds) && bounds.size != Vector3.zero && bounds.Contains(this.Transform.InverseTransformPoint(hit.Point));
		}

		public void InjectAllClippedPlaneSurface(PlaneSurface planeSurface, IEnumerable<IBoundsClipper> clippers)
		{
			this.InjectPlaneSurface(planeSurface);
			this.InjectClippers(clippers);
		}

		public void InjectPlaneSurface(PlaneSurface planeSurface)
		{
			this._planeSurface = planeSurface;
		}

		public void InjectClippers(IEnumerable<IBoundsClipper> clippers)
		{
			this._clippers = new List<Object>(from c in clippers
			select c as Object);
			this.Clippers = clippers.ToList<IBoundsClipper>();
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		private static readonly Bounds InfiniteBounds = new Bounds(Vector3.zero, Vector3.one * float.PositiveInfinity);

		private static readonly Bounds PlaneBounds = new Bounds(Vector3.zero, new Vector3(float.PositiveInfinity, float.PositiveInfinity, 1E-05f));

		[Tooltip("The Plane Surface to be clipped.")]
		[SerializeField]
		private PlaneSurface _planeSurface;

		[Tooltip("The clippers that will be used to clip the Plane Surface.")]
		[SerializeField]
		[Interface(typeof(IBoundsClipper), new Type[]
		{

		})]
		private List<Object> _clippers = new List<Object>();
	}
}
