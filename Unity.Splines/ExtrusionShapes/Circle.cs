using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.ExtrusionShapes
{
	[Serializable]
	public sealed class Circle : IExtrudeShape
	{
		public void Setup(ISpline path, int segmentCount)
		{
			this.m_Rads = math.radians(360f / (float)this.SideCount);
		}

		public float2 GetPosition(float t, int index)
		{
			return new float2(math.cos((float)index * this.m_Rads), math.sin((float)index * this.m_Rads));
		}

		public int SideCount
		{
			get
			{
				return this.m_Sides;
			}
			set
			{
				this.m_Sides = value;
			}
		}

		[SerializeField]
		[Min(2f)]
		private int m_Sides = 8;

		private float m_Rads;
	}
}
