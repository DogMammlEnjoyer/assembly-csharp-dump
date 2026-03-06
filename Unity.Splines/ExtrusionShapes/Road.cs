using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.ExtrusionShapes
{
	[Serializable]
	public sealed class Road : IExtrudeShape
	{
		public int SideCount
		{
			get
			{
				return 3;
			}
		}

		public float2 GetPosition(float t, int index)
		{
			return Road.k_Sides[3 - index];
		}

		private static readonly float2[] k_Sides = new float2[]
		{
			new float2(-0.6f, -0.1f),
			new float2(-0.5f, 0f),
			new float2(0.5f, 0f),
			new float2(0.6f, -0.1f)
		};
	}
}
