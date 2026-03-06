using System;

namespace UnityEngine.XR.OpenXR.NativeTypes
{
	public struct XrVector2f
	{
		public XrVector2f(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		public XrVector2f(Vector2 value)
		{
			this.X = value.x;
			this.Y = value.y;
		}

		public float X;

		public float Y;
	}
}
