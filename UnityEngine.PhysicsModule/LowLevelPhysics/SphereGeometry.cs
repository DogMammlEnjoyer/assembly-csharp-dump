using System;

namespace UnityEngine.LowLevelPhysics
{
	public struct SphereGeometry : IGeometry
	{
		public float Radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = value;
			}
		}

		public SphereGeometry(float radius)
		{
			this.m_Radius = radius;
		}

		public GeometryType GeometryType
		{
			get
			{
				return GeometryType.Sphere;
			}
		}

		private float m_Radius;
	}
}
