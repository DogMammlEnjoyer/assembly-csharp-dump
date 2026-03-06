using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Oculus.Interaction.Surfaces
{
	public class UnionClippedPlaneSurface : MonoBehaviour, IClippedSurface<IBoundsClipper>, ISurfacePatch, ISurface
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

		[Obsolete("Use the non-alloc version instead")]
		public List<Bounds> GetLocalBounds()
		{
			List<Bounds> result = new List<Bounds>();
			this.GetLocalBoundsNonAlloc(ref result);
			return result;
		}

		public void GetLocalBoundsNonAlloc(ref List<Bounds> clipBounds)
		{
			IReadOnlyList<IBoundsClipper> clippers = this.GetClippers();
			for (int i = 0; i < clippers.Count; i++)
			{
				IBoundsClipper boundsClipper = clippers[i];
				Bounds item;
				if (boundsClipper != null && boundsClipper.GetLocalBounds(this.Transform, out item))
				{
					clipBounds.Add(item);
				}
			}
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
			if (!this._planeSurface.ClosestSurfacePoint(point, out hit, maxDistance))
			{
				return false;
			}
			List<Bounds> list = CollectionPool<List<Bounds>, Bounds>.Get();
			this.GetLocalBoundsNonAlloc(ref list);
			List<ValueTuple<Vector3, float>> list2 = new List<ValueTuple<Vector3, float>>();
			foreach (Bounds bounds in list)
			{
				Vector3 size = bounds.size;
				size.z = 1E-05f;
				bounds.size = size;
				Vector3 point2 = hit.Point;
				Vector3 vector = this.ClampPoint(point2, bounds);
				float num = Vector3.Distance(point, vector);
				if (maxDistance <= 0f || num <= maxDistance)
				{
					list2.Add(new ValueTuple<Vector3, float>(vector, num));
				}
			}
			CollectionPool<List<Bounds>, Bounds>.Release(list);
			list = null;
			if (list2.Count == 0)
			{
				return false;
			}
			ValueTuple<Vector3, float> valueTuple = list2[0];
			for (int i = 1; i < list2.Count; i++)
			{
				ValueTuple<Vector3, float> valueTuple2 = list2[i];
				if (valueTuple2.Item2 < valueTuple.Item2)
				{
					valueTuple = valueTuple2;
				}
			}
			hit.Point = valueTuple.Item1;
			hit.Distance = valueTuple.Item2;
			return true;
		}

		public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0f)
		{
			if (this.BackingSurface.Raycast(ray, out hit, maxDistance))
			{
				List<Bounds> list = CollectionPool<List<Bounds>, Bounds>.Get();
				this.GetLocalBoundsNonAlloc(ref list);
				foreach (Bounds bounds in list)
				{
					if (bounds.Contains(this.Transform.InverseTransformPoint(hit.Point)))
					{
						CollectionPool<List<Bounds>, Bounds>.Release(list);
						return true;
					}
				}
				CollectionPool<List<Bounds>, Bounds>.Release(list);
				return false;
			}
			return false;
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
