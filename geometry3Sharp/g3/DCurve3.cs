using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace g3
{
	public class DCurve3 : ISampledCurve3d
	{
		public bool Closed { get; set; }

		public DCurve3()
		{
			this.vertices = new List<Vector3d>();
			this.Closed = false;
			this.Timestamp = 1;
		}

		public DCurve3(List<Vector3d> verticesIn, bool bClosed, bool bTakeOwnership = false)
		{
			if (bTakeOwnership)
			{
				this.vertices = verticesIn;
			}
			else
			{
				this.vertices = new List<Vector3d>(verticesIn);
			}
			this.Closed = bClosed;
			this.Timestamp = 1;
		}

		public DCurve3(IEnumerable<Vector3d> verticesIn, bool bClosed)
		{
			this.vertices = new List<Vector3d>(verticesIn);
			this.Closed = bClosed;
			this.Timestamp = 1;
		}

		public DCurve3(DCurve3 copy)
		{
			this.vertices = new List<Vector3d>(copy.vertices);
			this.Closed = copy.Closed;
			this.Timestamp = 1;
		}

		public DCurve3(ISampledCurve3d icurve)
		{
			this.vertices = new List<Vector3d>(icurve.Vertices);
			this.Closed = icurve.Closed;
			this.Timestamp = 1;
		}

		public DCurve3(Polygon2d poly, int ix = 0, int iy = 1)
		{
			int vertexCount = poly.VertexCount;
			this.vertices = new List<Vector3d>(vertexCount);
			for (int i = 0; i < vertexCount; i++)
			{
				Vector3d zero = Vector3d.Zero;
				zero[ix] = poly[i].x;
				zero[iy] = poly[i].y;
				this.vertices.Add(zero);
			}
			this.Closed = true;
			this.Timestamp = 1;
		}

		public DCurve3(Vector3[] v_in, bool bClosed)
		{
			this.Closed = bClosed;
			this.vertices = (from vertex in v_in.ToList<Vector3>()
			select vertex).ToList<Vector3d>();
			this.Timestamp = 1;
		}

		public void AppendVertex(Vector3d v)
		{
			this.vertices.Add(v);
			this.Timestamp++;
		}

		public int VertexCount
		{
			get
			{
				return this.vertices.Count;
			}
		}

		public int SegmentCount
		{
			get
			{
				if (!this.Closed)
				{
					return this.vertices.Count - 1;
				}
				return this.vertices.Count;
			}
		}

		public Vector3d GetVertex(int i)
		{
			return this.vertices[i];
		}

		public IEnumerable<Vector3d> VertexItr()
		{
			return this.vertices;
		}

		public void SetVertex(int i, Vector3d v)
		{
			this.vertices[i] = v;
			this.Timestamp++;
		}

		public void SetVertices(VectorArray3d v)
		{
			this.vertices = new List<Vector3d>();
			for (int i = 0; i < v.Count; i++)
			{
				this.vertices.Add(v[i]);
			}
			this.Timestamp++;
		}

		public void SetVertices(IEnumerable<Vector3d> v)
		{
			this.vertices = new List<Vector3d>(v);
			this.Timestamp++;
		}

		public void SetVertices(List<Vector3d> vertices, bool bTakeOwnership)
		{
			if (bTakeOwnership)
			{
				this.vertices = vertices;
			}
			else
			{
				this.vertices = new List<Vector3d>(vertices);
			}
			this.Timestamp++;
		}

		public void ClearVertices()
		{
			this.vertices = new List<Vector3d>();
			this.Closed = false;
			this.Timestamp++;
		}

		public void RemoveVertex(int idx)
		{
			this.vertices.RemoveAt(idx);
			this.Timestamp++;
		}

		public void Reverse()
		{
			this.vertices.Reverse();
			this.Timestamp++;
		}

		public Vector3d this[int key]
		{
			get
			{
				return this.vertices[key];
			}
			set
			{
				this.vertices[key] = value;
				this.Timestamp++;
			}
		}

		public Vector3d Start
		{
			get
			{
				return this.vertices[0];
			}
		}

		public Vector3d End
		{
			get
			{
				if (!this.Closed)
				{
					return this.vertices.Last<Vector3d>();
				}
				return this.vertices[0];
			}
		}

		public IEnumerable<Vector3d> Vertices
		{
			get
			{
				return this.vertices;
			}
		}

		public Segment3d GetSegment(int iSegment)
		{
			if (!this.Closed)
			{
				return new Segment3d(this.vertices[iSegment], this.vertices[iSegment + 1]);
			}
			return new Segment3d(this.vertices[iSegment], this.vertices[(iSegment + 1) % this.vertices.Count]);
		}

		public IEnumerable<Segment3d> SegmentItr()
		{
			if (this.Closed)
			{
				int NV = this.vertices.Count;
				int num;
				for (int i = 0; i < NV; i = num)
				{
					yield return new Segment3d(this.vertices[i], this.vertices[(i + 1) % NV]);
					num = i + 1;
				}
			}
			else
			{
				int NV = this.vertices.Count - 1;
				int num;
				for (int i = 0; i < NV; i = num)
				{
					yield return new Segment3d(this.vertices[i], this.vertices[i + 1]);
					num = i + 1;
				}
			}
			yield break;
		}

		public Vector3d PointAt(int iSegment, double fSegT)
		{
			Segment3d segment3d = new Segment3d(this.vertices[iSegment], this.vertices[(iSegment + 1) % this.vertices.Count]);
			return segment3d.PointAt(fSegT);
		}

		public AxisAlignedBox3d GetBoundingBox()
		{
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			foreach (Vector3d v in this.vertices)
			{
				empty.Contain(v);
			}
			return empty;
		}

		public double ArcLength
		{
			get
			{
				return CurveUtils.ArcLength(this.vertices, this.Closed);
			}
		}

		public Vector3d Tangent(int i)
		{
			return CurveUtils.GetTangent(this.vertices, i, this.Closed);
		}

		public Vector3d Centroid(int i)
		{
			if (this.Closed)
			{
				int count = this.vertices.Count;
				if (i == 0)
				{
					return 0.5 * (this.vertices[1] + this.vertices[count - 1]);
				}
				return 0.5 * (this.vertices[(i + 1) % count] + this.vertices[i - 1]);
			}
			else
			{
				if (i == 0 || i == this.vertices.Count - 1)
				{
					return this.vertices[i];
				}
				return 0.5 * (this.vertices[i + 1] + this.vertices[i - 1]);
			}
		}

		public Index2i Neighbours(int i)
		{
			int count = this.vertices.Count;
			if (this.Closed)
			{
				if (i == 0)
				{
					return new Index2i(count - 1, 1);
				}
				return new Index2i(i - 1, (i + 1) % count);
			}
			else
			{
				if (i == 0)
				{
					return new Index2i(-1, 1);
				}
				if (i == count - 1)
				{
					return new Index2i(count - 2, -1);
				}
				return new Index2i(i - 1, i + 1);
			}
		}

		public double OpeningAngleDeg(int i)
		{
			int num = i - 1;
			int num2 = i + 1;
			if (this.Closed)
			{
				int count = this.vertices.Count;
				num = ((i == 0) ? (count - 1) : num);
				num2 %= count;
			}
			else if (i == 0 || i == this.vertices.Count - 1)
			{
				return 180.0;
			}
			Vector3d v = this.vertices[num] - this.vertices[i];
			Vector3d v2 = this.vertices[num2] - this.vertices[i];
			v.Normalize(2.220446049250313E-16);
			v2.Normalize(2.220446049250313E-16);
			return Vector3d.AngleD(v, v2);
		}

		public int NearestVertex(Vector3d p)
		{
			double num = double.MaxValue;
			int result = -1;
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				double num2 = this.vertices[i].DistanceSquared(ref p);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		public double DistanceSquared(Vector3d p, out int iNearSeg, out double fNearSegT)
		{
			iNearSeg = -1;
			fNearSegT = double.MaxValue;
			double num = double.MaxValue;
			int num2 = this.Closed ? this.vertices.Count : (this.vertices.Count - 1);
			for (int i = 0; i < num2; i++)
			{
				int index = i;
				int index2 = (i + 1) % this.vertices.Count;
				Segment3d segment3d = new Segment3d(this.vertices[index], this.vertices[index2]);
				double num3 = (p - segment3d.Center).Dot(segment3d.Direction);
				double num4;
				if (num3 >= segment3d.Extent)
				{
					num4 = segment3d.P1.DistanceSquared(p);
				}
				else if (num3 <= -segment3d.Extent)
				{
					num4 = segment3d.P0.DistanceSquared(p);
				}
				else
				{
					num4 = (segment3d.PointAt(num3) - p).LengthSquared;
				}
				if (num4 < num)
				{
					num = num4;
					iNearSeg = i;
					fNearSegT = num3;
				}
			}
			return num;
		}

		public double DistanceSquared(Vector3d p)
		{
			int num;
			double num2;
			return this.DistanceSquared(p, out num, out num2);
		}

		public DCurve3 ResampleSharpTurns(double sharp_thresh = 90.0, double flat_thresh = 189.0, double corner_t = 0.01)
		{
			int count = this.vertices.Count;
			DCurve3 dcurve = new DCurve3
			{
				Closed = this.Closed
			};
			double t = 1.0 - corner_t;
			for (int i = 0; i < count; i++)
			{
				double num = Math.Abs(this.OpeningAngleDeg(i));
				if (num <= flat_thresh || i <= 0)
				{
					if (num > sharp_thresh)
					{
						dcurve.AppendVertex(this.vertices[i]);
					}
					else
					{
						Vector3d b = this.vertices[(i + 1) % count];
						Vector3d a = this.vertices[(i == 0) ? (count - 1) : (i - 1)];
						dcurve.AppendVertex(Vector3d.Lerp(a, this.vertices[i], t));
						dcurve.AppendVertex(this.vertices[i]);
						dcurve.AppendVertex(Vector3d.Lerp(this.vertices[i], b, corner_t));
					}
				}
			}
			return dcurve;
		}

		public Vector3d Center()
		{
			Vector3d vector3d = Vector3d.Zero;
			int num = this.SegmentCount;
			if (!this.Closed)
			{
				num++;
			}
			foreach (Vector3d v in this.Vertices)
			{
				vector3d += v;
			}
			vector3d /= (double)num;
			return vector3d;
		}

		public Vector3d CenterMark()
		{
			Vector3d vector3d = this.Center();
			return this.GetSegment(this.NearestSegment(vector3d)).NearestPoint(vector3d);
		}

		public int NearestSegment(Vector3d position)
		{
			int result;
			double num;
			this.DistanceSquared(position, out result, out num);
			return result;
		}

		protected List<Vector3d> vertices;

		public int Timestamp;
	}
}
