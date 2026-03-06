using System;
using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
	public class CylinderSurface : MonoBehaviour, ISurface, IBounds
	{
		public bool IsValid
		{
			get
			{
				return this._cylinder != null && this.Radius > 0f;
			}
		}

		public float Radius
		{
			get
			{
				return this._cylinder.Radius;
			}
		}

		public Cylinder Cylinder
		{
			get
			{
				return this._cylinder;
			}
		}

		public Transform Transform
		{
			get
			{
				return this._cylinder.transform;
			}
		}

		public Bounds Bounds
		{
			get
			{
				float num = Mathf.Max(this.Transform.lossyScale.x, Mathf.Max(this.Transform.lossyScale.y, this.Transform.lossyScale.z)) * (this.Height + this.Radius);
				return new Bounds(this.Transform.position, new Vector3(num, num, num));
			}
		}

		public CylinderSurface.NormalFacing Facing
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

		public float Height
		{
			get
			{
				return this._height;
			}
			set
			{
				this._height = value;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			hit = default(SurfaceHit);
			if (!this.IsValid)
			{
				return false;
			}
			Vector3 vector;
			Vector3 a = vector = this._cylinder.transform.InverseTransformPoint(point);
			if (this._height > 0f)
			{
				vector.y = Mathf.Clamp(vector.y, -this._height / 2f, this._height / 2f);
			}
			Vector3 vector2 = Vector3.Project(vector, Vector3.up);
			Vector3 vector3 = (vector == vector2) ? Vector3.forward : (vector - vector2).normalized;
			bool flag = (vector - vector2).magnitude > this.Radius;
			Vector3 b = vector2 + vector3 * this.Radius;
			float val = Vector3.Distance(a, b);
			if (maxDistance > 0f && this.TransformScale(val) > maxDistance)
			{
				return false;
			}
			Vector3 direction;
			switch (this._facing)
			{
			default:
				direction = (flag ? vector3 : (-vector3));
				break;
			case CylinderSurface.NormalFacing.In:
				direction = -vector3;
				break;
			case CylinderSurface.NormalFacing.Out:
				direction = vector3;
				break;
			}
			hit.Point = this._cylinder.transform.TransformPoint(vector2 + vector3 * this.Radius);
			hit.Normal = this._cylinder.transform.TransformDirection(direction);
			hit.Distance = this.TransformScale(val);
			return true;
		}

		public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			hit = default(SurfaceHit);
			if (!this.IsValid)
			{
				return false;
			}
			Transform transform = this._cylinder.transform;
			Ray ray2 = ray;
			Vector3 origin = transform.InverseTransformPoint(ray2.origin);
			Transform transform2 = this._cylinder.transform;
			ray2 = ray;
			Vector3 vector = transform2.InverseTransformDirection(ray2.direction);
			Ray ray3 = new Ray(origin, vector.normalized);
			vector = ray3.origin;
			Vector3 origin2 = CylinderSurface.CancelY(vector);
			Vector3 vector2 = ray3.direction;
			Ray ray4 = new Ray(origin2, CylinderSurface.CancelY(vector2).normalized);
			Vector3 vector3 = -ray4.origin;
			Vector3 vector4 = ray4.origin + Vector3.Project(vector3, ray4.direction);
			vector = ray3.direction;
			float num = Vector3.Magnitude(CylinderSurface.CancelY(vector));
			float num2 = Vector3.Magnitude(vector4);
			bool flag = vector3.magnitude > this.Radius;
			CylinderSurface.NormalFacing normalFacing = (this._facing == CylinderSurface.NormalFacing.Any && !flag) ? CylinderSurface.NormalFacing.In : this._facing;
			if (num2 > this.Radius || Mathf.Approximately(num, 0f) || (flag && Vector3.Dot(vector3, ray4.direction) < 0f) || (!flag && normalFacing == CylinderSurface.NormalFacing.Out))
			{
				return false;
			}
			float d = Mathf.Sqrt(Mathf.Pow(this.Radius, 2f) - Mathf.Pow(num2, 2f));
			float num3 = Vector3.Distance(ray4.origin, vector4 - ray4.direction * d) / num;
			float num4 = Vector3.Distance(ray4.origin, vector4 + ray4.direction * d) / num;
			Vector3 point = ray3.GetPoint(num3);
			Vector3 point2 = ray3.GetPoint(num4);
			bool flag2 = (maxDistance <= 0f || this.TransformScale(num3) <= maxDistance) && (this._height <= 0f || Mathf.Abs(point.y) <= this._height / 2f);
			bool flag3 = (maxDistance <= 0f || this.TransformScale(num4) <= maxDistance) && (this._height <= 0f || Mathf.Abs(point2.y) <= this._height / 2f);
			if (normalFacing != CylinderSurface.NormalFacing.In && flag2)
			{
				hit.Point = this._cylinder.transform.TransformPoint(point);
				Transform transform3 = this._cylinder.transform;
				vector = CylinderSurface.CancelY(point);
				hit.Normal = transform3.TransformDirection(vector.normalized);
				hit.Distance = this.TransformScale(num3);
			}
			else
			{
				if (!flag3)
				{
					return false;
				}
				hit.Point = this._cylinder.transform.TransformPoint(point2);
				Transform transform4 = this._cylinder.transform;
				vector = -point2;
				vector2 = CylinderSurface.CancelY(vector);
				hit.Normal = transform4.TransformDirection(vector2.normalized);
				hit.Distance = this.TransformScale(num4);
			}
			return true;
		}

		private float TransformScale(float val)
		{
			return val * this._cylinder.transform.lossyScale.x;
		}

		private static Vector3 CancelY(in Vector3 vector)
		{
			return new Vector3(vector.x, 0f, vector.z);
		}

		public void InjectAllCylinderSurface(CylinderSurface.NormalFacing facing, Cylinder cylinder, float height)
		{
			this.InjectNormalFacing(facing);
			this.InjectCylinder(cylinder);
			this.InjectHeight(height);
		}

		public void InjectNormalFacing(CylinderSurface.NormalFacing facing)
		{
			this._facing = facing;
		}

		public void InjectCylinder(Cylinder cylinder)
		{
			this._cylinder = cylinder;
		}

		public void InjectHeight(float height)
		{
			this._height = height;
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		[Tooltip("The cylinder that will drive this surface.")]
		[SerializeField]
		private Cylinder _cylinder;

		[Tooltip("The normal facing of the surface. Hits will be registered either on the outer or inner face of the cylinder depending on this value.")]
		[SerializeField]
		private CylinderSurface.NormalFacing _facing = CylinderSurface.NormalFacing.Out;

		[Tooltip("The height of the cylinder. If zero or negative, height will be infinite.")]
		[SerializeField]
		private float _height = 1f;

		protected bool _started;

		public enum NormalFacing
		{
			Any,
			In,
			Out
		}
	}
}
