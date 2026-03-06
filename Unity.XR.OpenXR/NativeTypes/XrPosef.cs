using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
	public struct XrPosef
	{
		public XrPosef(Vector3 vec3, Quaternion quaternion)
		{
			this.Position = new XrVector3f(vec3);
			this.Orientation = new XrQuaternionf(quaternion);
		}

		public XrQuaternionf Orientation;

		public XrVector3f Position;
	}
}
