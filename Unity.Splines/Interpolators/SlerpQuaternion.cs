using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.Interpolators
{
	public struct SlerpQuaternion : IInterpolator<quaternion>
	{
		public quaternion Interpolate(quaternion a, quaternion b, float t)
		{
			return math.slerp(a, b, t);
		}
	}
}
