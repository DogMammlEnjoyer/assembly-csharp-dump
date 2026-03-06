using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class DGraph3 : DGraph
	{
		public DGraph3()
		{
			this.vertices = new DVector<double>();
		}

		public DGraph3(DGraph3 copy)
		{
			this.vertices = new DVector<double>();
			this.AppendGraph(copy, -1);
		}

		public Vector3d GetVertex(int vID)
		{
			int num = 3 * vID;
			return new Vector3d(this.vertices[num], this.vertices[num + 1], this.vertices[num + 2]);
		}

		public void SetVertex(int vID, Vector3d vNewPos)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				int num = 3 * vID;
				this.vertices[num] = vNewPos.x;
				this.vertices[num + 1] = vNewPos.y;
				this.vertices[num + 2] = vNewPos.z;
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

		public bool GetEdgeV(int eID, ref Vector3d a, ref Vector3d b)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 3 * this.edges[3 * eID];
				a.x = this.vertices[num];
				a.y = this.vertices[num + 1];
				a.z = this.vertices[num + 2];
				int num2 = 3 * this.edges[3 * eID + 1];
				b.x = this.vertices[num2];
				b.y = this.vertices[num2 + 1];
				b.z = this.vertices[num2 + 2];
				return true;
			}
			return false;
		}

		public Segment3d GetEdgeSegment(int eID)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 3 * this.edges[3 * eID];
				int num2 = 3 * this.edges[3 * eID + 1];
				return new Segment3d(new Vector3d(this.vertices[num], this.vertices[num + 1], this.vertices[num + 2]), new Vector3d(this.vertices[num2], this.vertices[num2 + 1], this.vertices[num2 + 2]));
			}
			throw new Exception("DGraph3.GetEdgeSegment: invalid segment with id " + eID.ToString());
		}

		public Vector3d GetEdgeCenter(int eID)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 3 * this.edges[3 * eID];
				int num2 = 3 * this.edges[3 * eID + 1];
				return new Vector3d((this.vertices[num] + this.vertices[num2]) * 0.5, (this.vertices[num + 1] + this.vertices[num2 + 1]) * 0.5, (this.vertices[num + 2] + this.vertices[num2 + 2]) * 0.5);
			}
			throw new Exception("DGraph3.GetEdgeCenter: invalid segment with id " + eID.ToString());
		}

		public IEnumerable<Segment3d> Segments()
		{
			foreach (object obj in this.edges_refcount)
			{
				int eID = (int)obj;
				yield return this.GetEdgeSegment(eID);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public int AppendVertex(Vector3d v)
		{
			return this.AppendVertex(v, Vector3f.One);
		}

		public int AppendVertex(Vector3d v, Vector3f c)
		{
			int num = base.append_vertex_internal();
			int num2 = 3 * num;
			this.vertices.insert(v[2], num2 + 2);
			this.vertices.insert(v[1], num2 + 1);
			this.vertices.insert(v[0], num2);
			if (this.colors != null)
			{
				this.colors.insert(c.z, num2 + 2);
				this.colors.insert(c.y, num2 + 1);
				this.colors.insert(c.x, num2);
			}
			return num;
		}

		public void AppendGraph(DGraph3 graph, int gid = -1)
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

		public IEnumerable<Vector3d> Vertices()
		{
			foreach (object obj in this.vertices_refcount)
			{
				int num = (int)obj;
				int num2 = 3 * num;
				yield return new Vector3d(this.vertices[num2], this.vertices[num2 + 1], this.vertices[num2 + 2]);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public AxisAlignedBox3d GetBounds()
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			using (IEnumerator enumerator = this.vertices_refcount.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					int num4 = (int)enumerator.Current;
					num = this.vertices[3 * num4];
					num2 = this.vertices[3 * num4 + 1];
					num3 = this.vertices[3 * num4 + 2];
				}
			}
			double num5 = num;
			double num6 = num;
			double num7 = num2;
			double num8 = num2;
			double num9 = num3;
			double num10 = num3;
			foreach (object obj in this.vertices_refcount)
			{
				int num11 = (int)obj;
				int num12 = 3 * num11;
				num = this.vertices[num12];
				num2 = this.vertices[num12 + 1];
				num3 = this.vertices[num12 + 2];
				if (num < num5)
				{
					num5 = num;
				}
				else if (num > num6)
				{
					num6 = num;
				}
				if (num2 < num7)
				{
					num7 = num2;
				}
				else if (num2 > num8)
				{
					num8 = num2;
				}
				if (num3 < num9)
				{
					num9 = num3;
				}
				else if (num3 > num10)
				{
					num10 = num3;
				}
			}
			return new AxisAlignedBox3d(num5, num7, num9, num6, num8, num10);
		}

		public AxisAlignedBox3d CachedBounds
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

		protected override int append_new_split_vertex(int a, int b)
		{
			Vector3d v = 0.5 * (this.GetVertex(a) + this.GetVertex(b));
			Vector3f c = this.HasVertexColors ? (0.5f * (this.GetVertexColor(a) + this.GetVertexColor(b))) : Vector3f.One;
			return this.AppendVertex(v, c);
		}

		protected override void subclass_validity_checks(Action<bool> CheckOrFailF)
		{
			foreach (int vID in base.VertexIndices())
			{
				Vector3d vertex = this.GetVertex(vID);
				CheckOrFailF(!double.IsNaN(vertex.LengthSquared));
				CheckOrFailF(!double.IsInfinity(vertex.LengthSquared));
			}
		}

		public static readonly Vector3d InvalidVertex = new Vector3d(double.MaxValue, 0.0, 0.0);

		private DVector<double> vertices;

		private DVector<float> colors;

		private AxisAlignedBox3d cached_bounds;

		private int cached_bounds_timestamp = -1;
	}
}
