using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class DGraph2 : DGraph
	{
		public DGraph2()
		{
			this.vertices = new DVector<double>();
		}

		public DGraph2(DGraph2 copy)
		{
			this.vertices = new DVector<double>();
			this.AppendGraph(copy, -1);
		}

		public Vector2d GetVertex(int vID)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return DGraph2.InvalidVertex;
			}
			return new Vector2d(this.vertices[2 * vID], this.vertices[2 * vID + 1]);
		}

		public void SetVertex(int vID, Vector2d vNewPos)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				int num = 2 * vID;
				this.vertices[num] = vNewPos.x;
				this.vertices[num + 1] = vNewPos.y;
				base.updateTimeStamp(true);
			}
		}

		public Vector3f GetVertexColor(int vID)
		{
			if (this.colors == null)
			{
				return Vector3f.One;
			}
			int num = 3 * vID;
			return new Vector3f(this.colors[num], this.colors[num + 1], this.colors[num + 2]);
		}

		public void SetVertexColor(int vID, Vector3f vNewColor)
		{
			if (this.HasVertexColors)
			{
				int num = 3 * vID;
				this.colors[num] = vNewColor.x;
				this.colors[num + 1] = vNewColor.y;
				this.colors[num + 2] = vNewColor.z;
				base.updateTimeStamp(false);
			}
		}

		public bool GetEdgeV(int eID, ref Vector2d a, ref Vector2d b)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 2 * this.edges[3 * eID];
				a.x = this.vertices[num];
				a.y = this.vertices[num + 1];
				int num2 = 2 * this.edges[3 * eID + 1];
				b.x = this.vertices[num2];
				b.y = this.vertices[num2 + 1];
				return true;
			}
			return false;
		}

		public Segment2d GetEdgeSegment(int eID)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 2 * this.edges[3 * eID];
				int num2 = 2 * this.edges[3 * eID + 1];
				return new Segment2d(new Vector2d(this.vertices[num], this.vertices[num + 1]), new Vector2d(this.vertices[num2], this.vertices[num2 + 1]));
			}
			throw new Exception("DGraph2.GetEdgeSegment: invalid segment with id " + eID.ToString());
		}

		public Vector2d GetEdgeCenter(int eID)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 2 * this.edges[3 * eID];
				int num2 = 2 * this.edges[3 * eID + 1];
				return new Vector2d((this.vertices[num] + this.vertices[num2]) * 0.5, (this.vertices[num + 1] + this.vertices[num2 + 1]) * 0.5);
			}
			throw new Exception("DGraph2.GetEdgeCenter: invalid segment with id " + eID.ToString());
		}

		public int AppendVertex(Vector2d v)
		{
			return this.AppendVertex(v, Vector3f.One);
		}

		public int AppendVertex(Vector2d v, Vector3f c)
		{
			int num = base.append_vertex_internal();
			int num2 = 2 * num;
			this.vertices.insert(v[1], num2 + 1);
			this.vertices.insert(v[0], num2);
			if (this.colors != null)
			{
				num2 = 3 * num;
				this.colors.insert(c.z, num2 + 2);
				this.colors.insert(c.y, num2 + 1);
				this.colors.insert(c.x, num2);
			}
			return num;
		}

		public void AppendPolygon(Polygon2d poly, int gid = -1)
		{
			int v = -1;
			int num = -1;
			int vertexCount = poly.VertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				int num2 = this.AppendVertex(poly[i]);
				if (num == -1)
				{
					v = num2;
				}
				else
				{
					base.AppendEdge(num, num2, gid);
				}
				num = num2;
			}
			base.AppendEdge(num, v, gid);
		}

		public void AppendPolygon(GeneralPolygon2d poly, int gid = -1)
		{
			this.AppendPolygon(poly.Outer, gid);
			foreach (Polygon2d poly2 in poly.Holes)
			{
				this.AppendPolygon(poly2, gid);
			}
		}

		public void AppendPolyline(PolyLine2d poly, int gid = -1)
		{
			int v = -1;
			int vertexCount = poly.VertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				int num = this.AppendVertex(poly[i]);
				if (i > 0)
				{
					base.AppendEdge(v, num, gid);
				}
				v = num;
			}
		}

		public void AppendGraph(DGraph2 graph, int gid = -1)
		{
			int[] array = new int[graph.MaxVertexID];
			foreach (int num in graph.VertexIndices())
			{
				array[num] = this.AppendVertex(graph.GetVertex(num));
			}
			foreach (int num2 in graph.EdgeIndices())
			{
				Index2i edgeV = graph.GetEdgeV(num2);
				int gid2 = (gid == -1) ? graph.GetEdgeGroup(num2) : gid;
				base.AppendEdge(array[edgeV.a], array[edgeV.b], gid2);
			}
		}

		public bool HasVertexColors
		{
			get
			{
				return this.colors != null;
			}
		}

		public void EnableVertexColors(Vector3f initial_color)
		{
			if (this.HasVertexColors)
			{
				return;
			}
			this.colors = new DVector<float>();
			int maxVertexID = base.MaxVertexID;
			this.colors.resize(3 * maxVertexID);
			for (int i = 0; i < maxVertexID; i++)
			{
				int num = 3 * i;
				this.colors[num] = initial_color.x;
				this.colors[num + 1] = initial_color.y;
				this.colors[num + 2] = initial_color.z;
			}
		}

		public void DiscardVertexColors()
		{
			this.colors = null;
		}

		public IEnumerable<Vector2d> Vertices()
		{
			foreach (object obj in this.vertices_refcount)
			{
				int num = (int)obj;
				int num2 = 2 * num;
				yield return new Vector2d(this.vertices[num2], this.vertices[num2 + 1]);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public int[] SortedVtxEdges(int vID)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return null;
			}
			List<int> list = this.vertex_edges[vID];
			int count = list.Count;
			int[] array = new int[count];
			double[] array2 = new double[count];
			Vector2d vector2d = new Vector2d(this.vertices[2 * vID], this.vertices[2 * vID + 1]);
			for (int i = 0; i < count; i++)
			{
				int num = base.edge_other_v(list[i], vID);
				double x = this.vertices[2 * num] - vector2d.x;
				double y = this.vertices[2 * num + 1] - vector2d.y;
				array2[i] = MathUtil.Atan2Positive(y, x);
				array[i] = list[i];
			}
			Array.Sort<double, int>(array2, array);
			return array;
		}

		public AxisAlignedBox2d GetBounds()
		{
			double num = 0.0;
			double num2 = 0.0;
			using (IEnumerator enumerator = this.vertices_refcount.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					int num3 = (int)enumerator.Current;
					num = this.vertices[2 * num3];
					num2 = this.vertices[2 * num3 + 1];
				}
			}
			double num4 = num;
			double num5 = num;
			double num6 = num2;
			double num7 = num2;
			foreach (object obj in this.vertices_refcount)
			{
				int num8 = (int)obj;
				num = this.vertices[2 * num8];
				num2 = this.vertices[2 * num8 + 1];
				if (num < num4)
				{
					num4 = num;
				}
				else if (num > num5)
				{
					num5 = num;
				}
				if (num2 < num6)
				{
					num6 = num2;
				}
				else if (num2 > num7)
				{
					num7 = num2;
				}
			}
			return new AxisAlignedBox2d(num4, num6, num5, num7);
		}

		public AxisAlignedBox2d CachedBounds
		{
			get
			{
				if (this.cached_bounds_timestamp != base.Timestamp)
				{
					this.cached_bounds = this.GetBounds();
					this.cached_bounds_timestamp = base.Timestamp;
				}
				return this.cached_bounds;
			}
		}

		public double OpeningAngle(int vID, double invalidValue = 1.7976931348623157E+308)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return invalidValue;
			}
			List<int> list = this.vertex_edges[vID];
			if (list.Count != 2)
			{
				return invalidValue;
			}
			int num = base.edge_other_v(list[0], vID);
			int num2 = base.edge_other_v(list[1], vID);
			Vector2d o = new Vector2d(this.vertices[2 * vID], this.vertices[2 * vID + 1]);
			Vector2d vector2d = new Vector2d(this.vertices[2 * num], this.vertices[2 * num + 1]);
			Vector2d vector2d2 = new Vector2d(this.vertices[2 * num2], this.vertices[2 * num2 + 1]);
			vector2d -= o;
			if (vector2d.Normalize(2.220446049250313E-16) == 0.0)
			{
				return invalidValue;
			}
			vector2d2 -= o;
			if (vector2d2.Normalize(2.220446049250313E-16) == 0.0)
			{
				return invalidValue;
			}
			return Vector2d.AngleD(vector2d, vector2d2);
		}

		protected override int append_new_split_vertex(int a, int b)
		{
			Vector2d v = 0.5 * (this.GetVertex(a) + this.GetVertex(b));
			Vector3f c = this.HasVertexColors ? (0.5f * (this.GetVertexColor(a) + this.GetVertexColor(b))) : Vector3f.One;
			return this.AppendVertex(v, c);
		}

		protected override void subclass_validity_checks(Action<bool> CheckOrFailF)
		{
			foreach (int vID in base.VertexIndices())
			{
				Vector2d vertex = this.GetVertex(vID);
				CheckOrFailF(!double.IsNaN(vertex.LengthSquared));
				CheckOrFailF(!double.IsInfinity(vertex.LengthSquared));
			}
		}

		public static readonly Vector2d InvalidVertex = new Vector2d(double.MaxValue, 0.0);

		private DVector<double> vertices;

		private DVector<float> colors;

		private AxisAlignedBox2d cached_bounds;

		private int cached_bounds_timestamp = -1;
	}
}
