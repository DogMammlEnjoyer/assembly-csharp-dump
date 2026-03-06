using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	internal struct Triangle : IEquatable<Triangle>
	{
		public int a
		{
			get
			{
				return this.m_A;
			}
		}

		public int b
		{
			get
			{
				return this.m_B;
			}
		}

		public int c
		{
			get
			{
				return this.m_C;
			}
		}

		public IEnumerable<int> indices
		{
			get
			{
				return new int[]
				{
					this.m_A,
					this.m_B,
					this.m_C
				};
			}
		}

		public Triangle(int a, int b, int c)
		{
			this.m_A = a;
			this.m_B = b;
			this.m_C = c;
		}

		public bool Equals(Triangle other)
		{
			return this.m_A == other.a && this.m_B == other.b && this.m_C == other.c;
		}

		public override bool Equals(object obj)
		{
			if (obj is Triangle)
			{
				Triangle other = (Triangle)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (this.m_A * 397 ^ this.m_B) * 397 ^ this.m_C;
		}

		public bool IsAdjacent(Triangle other)
		{
			return other.ContainsEdge(new Edge(this.a, this.b)) || other.ContainsEdge(new Edge(this.b, this.c)) || other.ContainsEdge(new Edge(this.c, this.a));
		}

		private bool ContainsEdge(Edge edge)
		{
			return new Edge(this.a, this.b) == edge || new Edge(this.b, this.c) == edge || new Edge(this.c, this.a) == edge;
		}

		[SerializeField]
		private int m_A;

		[SerializeField]
		private int m_B;

		[SerializeField]
		private int m_C;
	}
}
