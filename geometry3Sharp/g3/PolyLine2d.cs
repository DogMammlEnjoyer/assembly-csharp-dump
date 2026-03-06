using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace g3
{
	public class PolyLine2d : IEnumerable<Vector2d>, IEnumerable
	{
		public PolyLine2d()
		{
			this.vertices = new List<Vector2d>();
			this.Timestamp = 0;
		}

		public PolyLine2d(PolyLine2d copy)
		{
			this.vertices = new List<Vector2d>(copy.vertices);
			this.Timestamp = 0;
		}

		public PolyLine2d(Polygon2d copy, bool bDuplicateFirstLast)
		{
			this.vertices = new List<Vector2d>(copy.VerticesItr(bDuplicateFirstLast));
			this.Timestamp = 0;
		}

		public PolyLine2d(IList<Vector2d> copy)
		{
			this.vertices = new List<Vector2d>(copy);
			this.Timestamp = 0;
		}

		public PolyLine2d(IEnumerable<Vector2d> copy)
		{
			this.vertices = new List<Vector2d>(copy);
			this.Timestamp = 0;
		}

		public PolyLine2d(Vector2d[] v)
		{
			this.vertices = new List<Vector2d>(v);
			this.Timestamp = 0;
		}

		public PolyLine2d(VectorArray2d v)
		{
			this.vertices = new List<Vector2d>(v.AsVector2d());
			this.Timestamp = 0;
		}

		public Vector2d this[int key]
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

		public Vector2d Start
		{
			get
			{
				return this.vertices[0];
			}
		}

		public Vector2d End
		{
			get
			{
				return this.vertices[this.vertices.Count - 1];
			}
		}

		public ReadOnlyCollection<Vector2d> Vertices
		{
			get
			{
				return this.vertices.AsReadOnly();
			}
		}

		public int VertexCount
		{
			get
			{
				return this.vertices.Count;
			}
		}

		public virtual void AppendVertex(Vector2d v)
		{
			this.vertices.Add(v);
			this.Timestamp++;
		}

		public virtual void AppendVertices(IEnumerable<Vector2d> v)
		{
			this.vertices.AddRange(v);
			this.Timestamp++;
		}

		public virtual void Reverse()
		{
			this.vertices.Reverse();
			this.Timestamp++;
		}

		public Vector2d GetTangent(int i)
		{
			if (i == 0)
			{
				return (this.vertices[1] - this.vertices[0]).Normalized;
			}
			if (i == this.vertices.Count - 1)
			{
				return (this.vertices[this.vertices.Count - 1] - this.vertices[this.vertices.Count - 2]).Normalized;
			}
			return (this.vertices[i + 1] - this.vertices[i - 1]).Normalized;
		}

		public Vector2d GetNormal(int i)
		{
			return this.GetTangent(i).Perp;
		}

		public AxisAlignedBox2d GetBounds()
		{
			if (this.vertices.Count == 0)
			{
				return AxisAlignedBox2d.Empty;
			}
			AxisAlignedBox2d result = new AxisAlignedBox2d(this.vertices[0]);
			for (int i = 1; i < this.vertices.Count; i++)
			{
				result.Contain(this.vertices[i]);
			}
			return result;
		}

		public AxisAlignedBox2d Bounds
		{
			get
			{
				return this.GetBounds();
			}
		}

		public double DistanceSquared(Vector2d point)
		{
			double num = double.MaxValue;
			for (int i = 0; i < this.vertices.Count - 1; i++)
			{
				Segment2d segment2d = new Segment2d(this.vertices[i], this.vertices[i + 1]);
				double num2 = segment2d.DistanceSquared(point);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		public Segment2d Segment(int iSegment)
		{
			return new Segment2d(this.vertices[iSegment], this.vertices[iSegment + 1]);
		}

		public IEnumerable<Segment2d> SegmentItr()
		{
			int num;
			for (int i = 0; i < this.vertices.Count - 1; i = num)
			{
				yield return new Segment2d(this.vertices[i], this.vertices[i + 1]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerator<Vector2d> GetEnumerator()
		{
			return this.vertices.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.vertices.GetEnumerator();
		}

		[Obsolete("This method name is confusing. Will remove in future. Use ArcLength instead")]
		public double Length
		{
			get
			{
				return this.ArcLength;
			}
		}

		public double ArcLength
		{
			get
			{
				double num = 0.0;
				int count = this.vertices.Count;
				for (int i = 0; i < count - 1; i++)
				{
					num += this.vertices[i].Distance(this.vertices[i + 1]);
				}
				return num;
			}
		}

		public void VertexOffset(double dist)
		{
			Vector2d[] array = new Vector2d[this.vertices.Count];
			for (int i = 0; i < this.vertices.Count; i++)
			{
				array[i] = this.vertices[i] + dist * this.GetNormal(i);
			}
			for (int j = 0; j < this.vertices.Count; j++)
			{
				this.vertices[j] = array[j];
			}
		}

		public bool TrimStart(double dist)
		{
			int count = this.vertices.Count;
			int num = 0;
			double num2 = this.vertices[num].Distance(this.vertices[num + 1]);
			double num3 = 0.0;
			while (num < count - 2 && num3 + num2 < dist)
			{
				num3 += num2;
				num++;
				num2 = this.vertices[num].Distance(this.vertices[num + 1]);
			}
			if (num == count - 2 && num3 + num2 <= dist)
			{
				return false;
			}
			double t = (dist - num3) / num2;
			Vector2d value = this.Segment(num).PointBetween(t);
			if (num > 0)
			{
				this.vertices.RemoveRange(0, num);
			}
			this.vertices[0] = value;
			return true;
		}

		public bool TrimEnd(double dist)
		{
			int count = this.vertices.Count;
			int num = count - 1;
			double num2 = this.vertices[num].Distance(this.vertices[num - 1]);
			double num3 = 0.0;
			while (num > 1 && num3 + num2 < dist)
			{
				num3 += num2;
				num--;
				num2 = this.vertices[num].Distance(this.vertices[num - 1]);
			}
			if (num == 1 && num3 + num2 <= dist)
			{
				return false;
			}
			double num4 = (dist - num3) / num2;
			Vector2d value = this.Segment(num - 1).PointBetween(1.0 - num4);
			if (num < count - 1)
			{
				this.vertices.RemoveRange(num, count - 1 - num);
			}
			this.vertices[num] = value;
			return true;
		}

		public bool Trim(double each_end_dist)
		{
			return this.ArcLength >= 2.0 * each_end_dist && this.TrimEnd(each_end_dist) && this.TrimStart(each_end_dist);
		}

		protected static void simplifyDP(double tol, Vector2d[] v, int j, int k, bool[] mk)
		{
			if (k <= j + 1)
			{
				return;
			}
			int num = j;
			double num2 = 0.0;
			double num3 = tol * tol;
			Segment2d segment2d = new Segment2d(v[j], v[k]);
			for (int i = j + 1; i < k; i++)
			{
				double num4 = segment2d.DistanceSquared(v[i]);
				if (num4 > num2)
				{
					num = i;
					num2 = num4;
				}
			}
			if (num2 > num3)
			{
				mk[num] = true;
				PolyLine2d.simplifyDP(tol, v, j, num, mk);
				PolyLine2d.simplifyDP(tol, v, num, k, mk);
			}
		}

		public virtual void Simplify(double clusterTol = 0.0001, double lineDeviationTol = 0.01, bool bSimplifyStraightLines = true)
		{
			int count = this.vertices.Count;
			Vector2d[] array = new Vector2d[count];
			bool[] array2 = new bool[count];
			int i;
			for (i = 0; i < count; i++)
			{
				array2[i] = false;
			}
			double num = clusterTol * clusterTol;
			array[0] = this.vertices[0];
			int num2 = i = 1;
			int num3 = 0;
			while (i < count)
			{
				if ((this.vertices[i] - this.vertices[num3]).LengthSquared >= num)
				{
					array[num2++] = this.vertices[i];
					num3 = i;
				}
				i++;
			}
			if (num3 < count - 1)
			{
				array[num2++] = this.vertices[count - 1];
			}
			if (lineDeviationTol > 0.0)
			{
				array2[0] = (array2[num2 - 1] = true);
				PolyLine2d.simplifyDP(lineDeviationTol, array, 0, num2 - 1, array2);
			}
			else
			{
				for (i = 0; i < num2; i++)
				{
					array2[i] = true;
				}
			}
			this.vertices = new List<Vector2d>();
			for (i = 0; i < num2; i++)
			{
				if (array2[i])
				{
					this.vertices.Add(array[i]);
				}
			}
			this.Timestamp++;
		}

		public PolyLine2d Transform(ITransform2 xform)
		{
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				this.vertices[i] = xform.TransformP(this.vertices[i]);
			}
			return this;
		}

		public static PolyLine2d MakeBoxSpiral(Vector2d center, double len, double spacing)
		{
			PolyLine2d polyLine2d = new PolyLine2d();
			polyLine2d.AppendVertex(center);
			Vector2d v = center;
			v.x += spacing / 2.0;
			polyLine2d.AppendVertex(v);
			v.y += spacing;
			polyLine2d.AppendVertex(v);
			double num = spacing / 2.0 + spacing;
			double num2 = spacing / 2.0;
			double num3 = spacing;
			double num4 = -1.0;
			while (num < len)
			{
				num2 += spacing;
				v.x += num4 * num2;
				polyLine2d.AppendVertex(v);
				num += num2;
				num3 += spacing;
				v.y += num4 * num3;
				polyLine2d.AppendVertex(v);
				num += num3;
				num4 *= -1.0;
			}
			return polyLine2d;
		}

		protected List<Vector2d> vertices;

		public int Timestamp;
	}
}
