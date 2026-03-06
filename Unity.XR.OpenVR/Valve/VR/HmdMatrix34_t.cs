using System;
using UnityEngine;

namespace Valve.VR
{
	public struct HmdMatrix34_t
	{
		public Vector3 GetPosition()
		{
			return new Vector3(this.m3, this.m7, -this.m11);
		}

		public bool IsRotationValid()
		{
			return (this.m2 != 0f || this.m6 != 0f || this.m10 != 0f) && (this.m1 != 0f || this.m5 != 0f || this.m9 != 0f);
		}

		public Quaternion GetRotation()
		{
			if (this.IsRotationValid())
			{
				float w = Mathf.Sqrt(Mathf.Max(0f, 1f + this.m0 + this.m5 + this.m10)) / 2f;
				float x = Mathf.Sqrt(Mathf.Max(0f, 1f + this.m0 - this.m5 - this.m10)) / 2f;
				float y = Mathf.Sqrt(Mathf.Max(0f, 1f - this.m0 + this.m5 - this.m10)) / 2f;
				float z = Mathf.Sqrt(Mathf.Max(0f, 1f - this.m0 - this.m5 + this.m10)) / 2f;
				HmdMatrix34_t._copysign(ref x, -this.m9 - -this.m6);
				HmdMatrix34_t._copysign(ref y, -this.m2 - -this.m8);
				HmdMatrix34_t._copysign(ref z, this.m4 - this.m1);
				return new Quaternion(x, y, z, w);
			}
			return Quaternion.identity;
		}

		private static void _copysign(ref float sizeval, float signval)
		{
			if (signval > 0f != sizeval > 0f)
			{
				sizeval = -sizeval;
			}
		}

		public float m0;

		public float m1;

		public float m2;

		public float m3;

		public float m4;

		public float m5;

		public float m6;

		public float m7;

		public float m8;

		public float m9;

		public float m10;

		public float m11;
	}
}
