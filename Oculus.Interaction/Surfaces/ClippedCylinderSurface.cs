using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class ClippedCylinderSurface : MonoBehaviour, IClippedSurface<ICylinderClipper>, ISurfacePatch, ISurface
	{
		private List<ICylinderClipper> Clippers { get; set; }

		public Transform Transform
		{
			get
			{
				return this._cylinderSurface.Transform;
			}
		}

		public ISurface BackingSurface
		{
			get
			{
				return this._cylinderSurface;
			}
		}

		public Cylinder Cylinder
		{
			get
			{
				return this._cylinderSurface.Cylinder;
			}
		}

		public IReadOnlyList<ICylinderClipper> GetClippers()
		{
			if (this.Clippers != null)
			{
				return this.Clippers;
			}
			return this._clippers.ConvertAll<ICylinderClipper>((Object clipper) => clipper as ICylinderClipper);
		}

		public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0f)
		{
			if (this.BackingSurface.Raycast(ray, out hit, maxDistance))
			{
				Vector3 point = hit.Point;
				SurfaceHit surfaceHit;
				if (this.ClosestSurfacePoint(point, out surfaceHit, 0f))
				{
					return hit.Point.Approximately(surfaceHit.Point, 0.0001f);
				}
			}
			return false;
		}

		protected virtual void Awake()
		{
			this.Clippers = this._clippers.ConvertAll<ICylinderClipper>((Object clipper) => clipper as ICylinderClipper);
		}

		protected virtual void Start()
		{
		}

		public bool GetClipped(out CylinderSegment clipped)
		{
			bool flag = false;
			bool flag2 = true;
			float num = float.MinValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MaxValue;
			IReadOnlyList<ICylinderClipper> clippers = this.GetClippers();
			for (int i = 0; i < clippers.Count; i++)
			{
				ICylinderClipper cylinderClipper = clippers[i];
				CylinderSegment cylinderSegment;
				if (cylinderClipper != null && cylinderClipper.GetCylinderSegment(out cylinderSegment))
				{
					flag = true;
					float b = cylinderSegment.Rotation - cylinderSegment.ArcDegrees / 2f;
					float b2 = cylinderSegment.Rotation + cylinderSegment.ArcDegrees / 2f;
					num = Mathf.Max(num, b);
					num2 = Mathf.Min(num2, b2);
					if (!cylinderSegment.IsInfiniteHeight)
					{
						flag2 = false;
						num3 = Mathf.Max(num3, cylinderSegment.Bottom);
						num4 = Mathf.Min(num4, cylinderSegment.Top);
					}
				}
			}
			if (!flag)
			{
				clipped = CylinderSegment.Infinite();
				return true;
			}
			if (num > num2 || (!flag2 && num3 > num4))
			{
				clipped = default(CylinderSegment);
				return false;
			}
			float rotation = Mathf.Lerp(num, num2, 0.5f) % 360f;
			if (flag2)
			{
				num3 = 1f;
				num4 = -1f;
			}
			clipped = new CylinderSegment(rotation, num2 - num, num3, num4);
			return true;
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance = 0f)
		{
			CylinderSegment cylinderSegment;
			if (!this.GetClipped(out cylinderSegment))
			{
				hit = default(SurfaceHit);
				return false;
			}
			Vector3 vector;
			Vector3 a = vector = this.Cylinder.transform.InverseTransformPoint(point);
			if (!cylinderSegment.IsInfiniteHeight)
			{
				vector.y = Mathf.Clamp(vector.y, cylinderSegment.Bottom, cylinderSegment.Top);
			}
			Vector3 vector2;
			if (!cylinderSegment.IsInfiniteArc)
			{
				float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f % 360f;
				float num2 = cylinderSegment.Rotation % 360f;
				if (num > num2 + 180f)
				{
					num -= 360f;
				}
				else if (num < num2 - 180f)
				{
					num += 360f;
				}
				num = Mathf.Clamp(num, num2 - cylinderSegment.ArcDegrees / 2f, num2 + cylinderSegment.ArcDegrees / 2f);
				vector.x = Mathf.Sin(num * 0.017453292f) * this.Cylinder.Radius;
				vector.z = Mathf.Cos(num * 0.017453292f) * this.Cylinder.Radius;
				vector2 = new Vector3(0f, vector.y, 0f);
			}
			else
			{
				vector2 = new Vector3(0f, vector.y, 0f);
				float num3 = Vector3.Distance(vector, vector2);
				vector = Vector3.MoveTowards(vector, vector2, num3 - this.Cylinder.Radius);
			}
			bool flag = (a - vector2).magnitude > this.Cylinder.Radius;
			Vector3 vector3 = (vector2 - vector).normalized;
			switch (this._cylinderSurface.Facing)
			{
			default:
				vector3 = (flag ? (-vector3) : vector3);
				break;
			case CylinderSurface.NormalFacing.In:
				break;
			case CylinderSurface.NormalFacing.Out:
				vector3 = -vector3;
				break;
			}
			hit = default(SurfaceHit);
			hit.Point = this.Cylinder.transform.TransformPoint(vector);
			hit.Distance = Vector3.Distance(point, hit.Point);
			hit.Normal = this.Cylinder.transform.TransformDirection(vector3).normalized;
			return maxDistance <= 0f || hit.Distance <= maxDistance;
		}

		public void InjectAllClippedCylinderSurface(CylinderSurface surface, IEnumerable<ICylinderClipper> clippers)
		{
			this.InjectCylinderSurface(surface);
			this.InjectClippers(clippers);
		}

		public void InjectCylinderSurface(CylinderSurface surface)
		{
			this._cylinderSurface = surface;
		}

		public void InjectClippers(IEnumerable<ICylinderClipper> clippers)
		{
			this._clippers = new List<Object>(from c in clippers
			select c as Object);
			this.Clippers = clippers.ToList<ICylinderClipper>();
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		private const float EPSILON = 0.0001f;

		[Tooltip("The Cylinder Surface to be clipped.")]
		[SerializeField]
		private CylinderSurface _cylinderSurface;

		[Tooltip("The clippers that will be used to clip the Cylinder Surface.")]
		[SerializeField]
		[Interface(typeof(ICylinderClipper), new Type[]
		{

		})]
		private List<Object> _clippers = new List<Object>();
	}
}
