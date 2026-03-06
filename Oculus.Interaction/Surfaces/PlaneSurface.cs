using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class PlaneSurface : MonoBehaviour, ISurface, IBounds
	{
		public PlaneSurface.NormalFacing Facing
		{
			get
			{
				return this._facing;
			}
			set
			{
				this._facing = value;
			}
		}

		public bool DoubleSided
		{
			get
			{
				return this._doubleSided;
			}
			set
			{
				this._doubleSided = value;
			}
		}

		public Vector3 Normal
		{
			get
			{
				if (this._facing != PlaneSurface.NormalFacing.Forward)
				{
					return -base.transform.forward;
				}
				return base.transform.forward;
			}
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			hit = default(SurfaceHit);
			Vector3 vector;
			float num;
			this.GetPlaneParameters(out vector, out num);
			float num2 = Vector3.Dot(vector, point) + num;
			float num3 = Mathf.Abs(num2);
			if (maxDistance > 0f && num3 > maxDistance)
			{
				return false;
			}
			hit.Point = point - vector * num2;
			hit.Distance = num3;
			hit.Normal = vector.normalized;
			return true;
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
				Vector3 normal = this.Normal;
				Vector3 size = new Vector3((Mathf.Abs(normal.x) == 1f) ? float.Epsilon : float.PositiveInfinity, (Mathf.Abs(normal.y) == 1f) ? float.Epsilon : float.PositiveInfinity, (Mathf.Abs(normal.z) == 1f) ? float.Epsilon : float.PositiveInfinity);
				return new Bounds(base.transform.position, size);
			}
		}

		public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			hit = default(SurfaceHit);
			PlaneSurface.<>c__DisplayClass16_0 CS$<>8__locals1;
			float num;
			this.GetPlaneParameters(out CS$<>8__locals1.planeNormal, out num);
			Vector3 planeNormal = CS$<>8__locals1.planeNormal;
			Ray ray2 = ray;
			CS$<>8__locals1.originDistance = Vector3.Dot(planeNormal, ray2.origin) + num;
			if (!this._doubleSided && CS$<>8__locals1.originDistance <= 0f)
			{
				return false;
			}
			float num2;
			if (!PlaneSurface.<Raycast>g__Raycast|16_0(ray, out num2, ref CS$<>8__locals1))
			{
				return false;
			}
			if (maxDistance > 0f && num2 > maxDistance)
			{
				return false;
			}
			ray2 = ray;
			hit.Point = ray2.GetPoint(num2);
			hit.Distance = num2;
			hit.Normal = CS$<>8__locals1.planeNormal.normalized;
			return true;
		}

		private void GetPlaneParameters(out Vector3 planeNormal, out float planeDistance)
		{
			Vector3 rhs;
			Quaternion rotation;
			base.transform.GetPositionAndRotation(out rhs, out rotation);
			planeNormal = rotation * ((this._facing == PlaneSurface.NormalFacing.Forward) ? PlaneSurface._forward : PlaneSurface._back);
			planeDistance = -Vector3.Dot(planeNormal, rhs);
		}

		public Plane GetPlane()
		{
			return new Plane(this.Normal, base.transform.position);
		}

		public void InjectAllPlaneSurface(PlaneSurface.NormalFacing facing, bool doubleSided)
		{
			this.InjectNormalFacing(facing);
			this.InjectDoubleSided(doubleSided);
		}

		public void InjectNormalFacing(PlaneSurface.NormalFacing facing)
		{
			this._facing = facing;
		}

		public void InjectDoubleSided(bool doubleSided)
		{
			this._doubleSided = doubleSided;
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		[CompilerGenerated]
		internal static bool <Raycast>g__Raycast|16_0(in Ray ray, out float enter, ref PlaneSurface.<>c__DisplayClass16_0 A_2)
		{
			Ray ray2 = ray;
			float num = Vector3.Dot(ray2.direction, A_2.planeNormal);
			if (Mathf.Approximately(num, 0f))
			{
				enter = 0f;
				return false;
			}
			enter = -A_2.originDistance / num;
			return enter > 0f;
		}

		[Tooltip("The normal facing of the surface. Hits will be registered either on the front or back of the plane depending on this value.")]
		[SerializeField]
		private PlaneSurface.NormalFacing _facing;

		[SerializeField]
		[Tooltip("Raycasts hit either side of plane, but hit normal will still respect plane facing.")]
		private bool _doubleSided;

		private static Vector3 _forward = Vector3.forward;

		private static Vector3 _back = Vector3.back;

		public enum NormalFacing
		{
			Backward,
			Forward
		}
	}
}
