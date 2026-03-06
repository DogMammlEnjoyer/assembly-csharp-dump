using System;

namespace UnityEngine.ProBuilder
{
	internal static class VectorHash
	{
		private static int HashFloat(float f)
		{
			return (int)((ulong)(f * 1000f) % 2147483647UL);
		}

		public static int GetHashCode(Vector2 v)
		{
			return (27 * 29 + VectorHash.HashFloat(v.x)) * 29 + VectorHash.HashFloat(v.y);
		}

		public static int GetHashCode(Vector3 v)
		{
			return ((27 * 29 + VectorHash.HashFloat(v.x)) * 29 + VectorHash.HashFloat(v.y)) * 29 + VectorHash.HashFloat(v.z);
		}

		public static int GetHashCode(Vector4 v)
		{
			return (((27 * 29 + VectorHash.HashFloat(v.x)) * 29 + VectorHash.HashFloat(v.y)) * 29 + VectorHash.HashFloat(v.z)) * 29 + VectorHash.HashFloat(v.w);
		}

		public const float FltCompareResolution = 1000f;
	}
}
