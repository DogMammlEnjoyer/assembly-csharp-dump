using System;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	public abstract class SplineComponent : MonoBehaviour
	{
		protected float3 GetAxis(SplineComponent.AlignAxis axis)
		{
			return this.m_AlignAxisToVector[(int)axis];
		}

		private readonly float3[] m_AlignAxisToVector = new float3[]
		{
			math.right(),
			math.up(),
			math.forward(),
			math.left(),
			math.down(),
			math.back()
		};

		public enum AlignAxis
		{
			[InspectorName("Object X+")]
			XAxis,
			[InspectorName("Object Y+")]
			YAxis,
			[InspectorName("Object Z+")]
			ZAxis,
			[InspectorName("Object X-")]
			NegativeXAxis,
			[InspectorName("Object Y-")]
			NegativeYAxis,
			[InspectorName("Object Z-")]
			NegativeZAxis
		}
	}
}
