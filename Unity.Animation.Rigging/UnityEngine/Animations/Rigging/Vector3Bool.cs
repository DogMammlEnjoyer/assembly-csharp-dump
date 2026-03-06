using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct Vector3Bool
	{
		public Vector3Bool(bool val)
		{
			this.z = val;
			this.y = val;
			this.x = val;
		}

		public Vector3Bool(bool x, bool y, bool z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public bool x;

		public bool y;

		public bool z;
	}
}
