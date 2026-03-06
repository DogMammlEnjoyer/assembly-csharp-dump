using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	public struct EdgeLookup : IEquatable<EdgeLookup>
	{
		public Edge local
		{
			get
			{
				return this.m_Local;
			}
			set
			{
				this.m_Local = value;
			}
		}

		public Edge common
		{
			get
			{
				return this.m_Common;
			}
			set
			{
				this.m_Common = value;
			}
		}

		public EdgeLookup(Edge common, Edge local)
		{
			this.m_Common = common;
			this.m_Local = local;
		}

		public EdgeLookup(int cx, int cy, int x, int y)
		{
			this.m_Common = new Edge(cx, cy);
			this.m_Local = new Edge(x, y);
		}

		public bool Equals(EdgeLookup other)
		{
			return other.common.Equals(this.common);
		}

		public override bool Equals(object obj)
		{
			return obj != null && this.Equals((EdgeLookup)obj);
		}

		public override int GetHashCode()
		{
			return this.common.GetHashCode();
		}

		public static bool operator ==(EdgeLookup a, EdgeLookup b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(EdgeLookup a, EdgeLookup b)
		{
			return !object.Equals(a, b);
		}

		public override string ToString()
		{
			return string.Format("Common: ({0}, {1}), local: ({2}, {3})", new object[]
			{
				this.common.a,
				this.common.b,
				this.local.a,
				this.local.b
			});
		}

		public static IEnumerable<EdgeLookup> GetEdgeLookup(IEnumerable<Edge> edges, Dictionary<int, int> lookup)
		{
			return from x in edges
			select new EdgeLookup(new Edge(lookup[x.a], lookup[x.b]), x);
		}

		public static HashSet<EdgeLookup> GetEdgeLookupHashSet(IEnumerable<Edge> edges, Dictionary<int, int> lookup)
		{
			if (lookup == null || edges == null)
			{
				return null;
			}
			HashSet<EdgeLookup> hashSet = new HashSet<EdgeLookup>();
			foreach (Edge edge in edges)
			{
				hashSet.Add(new EdgeLookup(new Edge(lookup[edge.a], lookup[edge.b]), edge));
			}
			return hashSet;
		}

		private Edge m_Local;

		private Edge m_Common;
	}
}
