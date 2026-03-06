using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	public struct Edge : IEquatable<Edge>
	{
		public Edge(int a, int b)
		{
			this.a = a;
			this.b = b;
		}

		public bool IsValid()
		{
			return this.a > -1 && this.b > -1 && this.a != this.b;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[",
				this.a.ToString(),
				", ",
				this.b.ToString(),
				"]"
			});
		}

		public bool Equals(Edge other)
		{
			return (this.a == other.a && this.b == other.b) || (this.a == other.b && this.b == other.a);
		}

		public override bool Equals(object obj)
		{
			return obj is Edge && this.Equals((Edge)obj);
		}

		public override int GetHashCode()
		{
			return (27 * 29 + ((this.a < this.b) ? this.a : this.b)) * 29 + ((this.a < this.b) ? this.b : this.a);
		}

		public static Edge operator +(Edge a, Edge b)
		{
			return new Edge(a.a + b.a, a.b + b.b);
		}

		public static Edge operator -(Edge a, Edge b)
		{
			return new Edge(a.a - b.a, a.b - b.b);
		}

		public static Edge operator +(Edge a, int b)
		{
			return new Edge(a.a + b, a.b + b);
		}

		public static Edge operator -(Edge a, int b)
		{
			return new Edge(a.a - b, a.b - b);
		}

		public static bool operator ==(Edge a, Edge b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Edge a, Edge b)
		{
			return !(a == b);
		}

		public static Edge Add(Edge a, Edge b)
		{
			return a + b;
		}

		public static Edge Subtract(Edge a, Edge b)
		{
			return a - b;
		}

		public bool Equals(Edge other, Dictionary<int, int> lookup)
		{
			if (lookup == null)
			{
				return this.Equals(other);
			}
			int num = lookup[this.a];
			int num2 = lookup[this.b];
			int num3 = lookup[other.a];
			int num4 = lookup[other.b];
			return (num == num3 && num2 == num4) || (num == num4 && num2 == num3);
		}

		public bool Contains(int index)
		{
			return this.a == index || this.b == index;
		}

		public bool Contains(Edge other)
		{
			return this.a == other.a || this.b == other.a || this.a == other.b || this.b == other.a;
		}

		internal bool Contains(int index, Dictionary<int, int> lookup)
		{
			int num = lookup[index];
			return lookup[this.a] == num || lookup[this.b] == num;
		}

		internal static void GetIndices(IEnumerable<Edge> edges, List<int> indices)
		{
			indices.Clear();
			foreach (Edge edge in edges)
			{
				indices.Add(edge.a);
				indices.Add(edge.b);
			}
		}

		public int a;

		public int b;

		public static readonly Edge Empty = new Edge(-1, -1);
	}
}
