using System;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public struct RotatedCapsule
	{
		public float CalcVolume()
		{
			float num = Mathf.Max(this.height - this.radius * 2f, 0f);
			return 3.1415927f * (this.radius * this.radius) * (1.3333334f * this.radius * num);
		}

		public void DrawWireframe()
		{
			Vector3 b = this.center - this.dir * Mathf.Max(this.height * 0.5f - this.radius, 0f);
			Vector3 a = this.center + this.dir * Mathf.Max(this.height * 0.5f - this.radius, 0f);
			float d = (a - b).magnitude * 0.5f;
			Vector3 vector = Vector3.Cross(this.dir, (Mathf.Abs(Vector3.Dot(this.dir, Vector3.up)) > 0.9f) ? Vector3.right : Vector3.up);
			Vector3 a2 = Vector3.Cross(this.dir, vector);
			Gizmos.DrawWireSphere(b, this.radius);
			Gizmos.DrawWireSphere(a, this.radius);
			Gizmos.DrawLine(this.center + vector * this.radius - this.dir * d, this.center + vector * this.radius + this.dir * d);
			Gizmos.DrawLine(this.center - vector * this.radius - this.dir * d, this.center - vector * this.radius + this.dir * d);
			Gizmos.DrawLine(this.center + a2 * this.radius - this.dir * d, this.center + a2 * this.radius + this.dir * d);
			Gizmos.DrawLine(this.center - a2 * this.radius - this.dir * d, this.center - a2 * this.radius + this.dir * d);
		}

		public Vector3 center;

		public Vector3 dir;

		public float radius;

		public float height;
	}
}
