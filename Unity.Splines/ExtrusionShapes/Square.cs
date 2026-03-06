using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.ExtrusionShapes
{
	[Serializable]
	public sealed class Square : IExtrudeShape
	{
		public int SideCount
		{
			get
			{
				return 4;
			}
		}

		public float2 GetPosition(float t, int index)
		{
			return Square.k_Sides[index];
		}

		private static readonly float2[] k_Sides = new float2[]
		{
			new float2(-0.5f, -0.5f),
			new float2(0.5f, -0.5f),
			new float2(0.5f, 0.5f),
			new float2(-0.5f, 0.5f)
		};
	}
}
