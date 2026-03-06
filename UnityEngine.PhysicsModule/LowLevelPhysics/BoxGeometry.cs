using System;

namespace UnityEngine.LowLevelPhysics
{
	public struct BoxGeometry : IGeometry
	{
		public Vector3 HalfExtents
		{
			get
			{
				return this.m_HalfExtents;
			}
			set
			{
				this.m_HalfExtents = value;
			}
		}

		public BoxGeometry(Vector3 halfExtents)
		{
			this.m_HalfExtents = halfExtents;
		}

		public GeometryType GeometryType
		{
			get
			{
				return GeometryType.Box;
			}
		}

		private Vector3 m_HalfExtents;
	}
}
