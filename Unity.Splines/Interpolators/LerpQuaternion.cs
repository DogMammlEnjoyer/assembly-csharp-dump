using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.Interpolators
{
	public struct LerpQuaternion : IInterpolator<quaternion>
	{
		public quaternion Interpolate(quaternion a, quaternion b, float t)
		{
			return math.nlerp(a, b, t);
		}
	}
}
