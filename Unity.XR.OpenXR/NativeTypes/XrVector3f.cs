using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
	public struct XrVector3f
	{
		public XrVector3f(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = -z;
		}

		public XrVector3f(Vector3 value)
		{
			this.X = value.x;
			this.Y = value.y;
			this.Z = -value.z;
		}

		public float X;

		public float Y;

		public float Z;
	}
}
