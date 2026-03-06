using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace g3
{
	public class PolyLine3d : IEnumerable<Vector3d>, IEnumerable
	{
		public PolyLine3d()
		{
			this.vertices = new List<Vector3d>();
			this.Timestamp = 0;
		}

		public PolyLine3d(PolyLine3d copy)
		{
			this.vertices = new List<Vector3d>(copy.vertices);
			this.Timestamp = 0;
		}

		public PolyLine3d(Vector3d[] v)
		{
			this.vertices = new List<Vector3d>(v);
			this.Timestamp = 0;
		}

		public PolyLine3d(IEnumerable<Vector3d> v)
		{
			this.vertices = new List<Vector3d>(v);
			this.Timestamp = 0;
		}

		public PolyLine3d(VectorArray3d v)
		{
			this.vertices = new List<Vector3d>(v.AsVector3d());
			this.Timestamp = 0;
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
				return this.vertices[this.vertices.Count - 1];
			}
		}

		public ReadOnlyCollection<Vector3d> Vertices
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

		public void AppendVertex(Vector3d v)
		{
			this.vertices.Add(v);
			this.Timestamp++;
		}

		public Vector3d GetTangent(int i)
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

		public AxisAlignedBox3d GetBounds()
		{
			if (this.vertices.Count == 0)
			{
				return AxisAlignedBox3d.Empty;
			}
			AxisAlignedBox3d result = new AxisAlignedBox3d(this.vertices[0]);
			for (int i = 1; i < this.vertices.Count; i++)
			{
				result.Contain(this.vertices[i]);
			}
			return result;
		}

		public IEnumerable<Segment3d> SegmentItr()
		{
			int num;
			for (int i = 0; i < this.vertices.Count - 1; i = num)
			{
				yield return new Segment3d(this.vertices[i], this.vertices[i + 1]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerator<Vector3d> GetEnumerator()
		{
			return this.vertices.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.vertices.GetEnumerator();
		}

		protected List<Vector3d> vertices;

		public int Timestamp;
	}
}
