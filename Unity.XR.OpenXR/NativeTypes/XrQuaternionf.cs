using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
	public struct XrQuaternionf
	{
		public XrQuaternionf(float x, float y, float z, float w)
		{
			this.X = -x;
			this.Y = -y;
			this.Z = z;
			this.W = w;
		}

		public XrQuaternionf(Quaternion quaternion)
		{
			this.X = -quaternion.x;
			this.Y = -quaternion.y;
			this.Z = quaternion.z;
			this.W = quaternion.w;
		}

		public float X;

		public float Y;

		public float Z;

		public float W;
	}
}
