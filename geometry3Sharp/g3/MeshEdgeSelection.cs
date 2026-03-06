using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class MeshEdgeSelection : IEnumerable<int>, IEnumerable
	{
		public MeshEdgeSelection(DMesh3 mesh)
		{
			this.Mesh = mesh;
			this.Selected = new HashSet<int>();
			this.temp = new List<int>();
		}

		public MeshEdgeSelection(MeshEdgeSelection copy)
		{
			this.Mesh = copy.Mesh;
			this.Selected = new HashSet<int>(copy.Selected);
			this.temp = new List<int>();
		}

		protected BitArray Bitmap
		{
			get
			{
				if (this.tempBits == null)
				{
					this.tempBits = new BitArray(this.Mesh.MaxEdgeID);
				}
				return this.tempBits;
			}
		}

		public MeshEdgeSelection(DMesh3 mesh, MeshVertexSelection convertV, int minCount = 2) : this(mesh)
		{
			minCount = MathUtil.Clamp(minCount, 1, 2);
			foreach (int num in mesh.EdgeIndices())
			{
				Index2i edgeV = mesh.GetEdgeV(num);
				if ((convertV.IsSelected(edgeV.a) ? 1 : 0) + (convertV.IsSelected(edgeV.b) ? 1 : 0) >= minCount)
				{
					this.add(num);
				}
			}
		}

		public MeshEdgeSelection(DMesh3 mesh, MeshFaceSelection convertT, int minCount = 1) : this(mesh)
		{
			minCount = MathUtil.Clamp(minCount, 1, 2);
			if (minCount == 1)
			{
				using (IEnumerator<int> enumerator = convertT.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int tID = enumerator.Current;
						Index3i triEdges = mesh.GetTriEdges(tID);
						this.add(triEdges.a);
						this.add(triEdges.b);
						this.add(triEdges.c);
					}
					return;
				}
			}
			foreach (int num in mesh.EdgeIndices())
			{
				Index2i edgeT = mesh.GetEdgeT(num);
				if (convertT.IsSelected(edgeT.a) && convertT.IsSelected(edgeT.b))
				{
					this.add(num);
				}
			}
		}

		public IEnumerator<int> GetEnumerator()
		{
			return this.Selected.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Selected.GetEnumerator();
		}

		private void add(int eid)
		{
			this.Selected.Add(eid);
		}

		private void remove(int eid)
		{
			this.Selected.Remove(eid);
		}

		public int Count
		{
			get
			{
				return this.Selected.Count;
			}
		}

		public bool IsSelected(int eid)
		{
			return this.Selected.Contains(eid);
		}

		public void Select(int eid)
		{
			if (this.Mesh.IsEdge(eid))
			{
				this.add(eid);
			}
		}

		public void Select(int[] edges)
		{
			for (int i = 0; i < edges.Length; i++)
			{
				if (this.Mesh.IsEdge(edges[i]))
				{
					this.add(edges[i]);
				}
			}
		}

		public void Select(List<int> edges)
		{
			for (int i = 0; i < edges.Count; i++)
			{
				if (this.Mesh.IsEdge(edges[i]))
				{
					this.add(edges[i]);
				}
			}
		}

		public void Select(IEnumerable<int> edges)
		{
			foreach (int num in edges)
			{
				if (this.Mesh.IsEdge(num))
				{
					this.add(num);
				}
			}
		}

		public void Select(Func<int, bool> selectF)
		{
			this.temp.Clear();
			int maxEdgeID = this.Mesh.MaxEdgeID;
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (this.Mesh.IsEdge(i) && selectF(i))
				{
					this.temp.Add(i);
				}
			}
			this.Select(this.temp);
		}

		public void SelectVertexEdges(int[] vertices)
		{
			foreach (int vID in vertices)
			{
				foreach (int eid in this.Mesh.VtxEdgesItr(vID))
				{
					this.add(eid);
				}
			}
		}

		public void SelectVertexEdges(IEnumerable<int> vertices)
		{
			foreach (int vID in vertices)
			{
				foreach (int eid in this.Mesh.VtxEdgesItr(vID))
				{
					this.add(eid);
				}
			}
		}

		public void SelectTriangleEdges(IEnumerable<int> triangles)
		{
			foreach (int tID in triangles)
			{
				Index3i triEdges = this.Mesh.GetTriEdges(tID);
				this.add(triEdges.a);
				this.add(triEdges.b);
				this.add(triEdges.c);
			}
		}

		public void SelectBoundaryTriEdges(MeshFaceSelection triangles)
		{
			foreach (int num in triangles)
			{
				Index3i triEdges = this.Mesh.GetTriEdges(num);
				for (int i = 0; i < 3; i++)
				{
					Index2i edgeT = this.Mesh.GetEdgeT(triEdges[i]);
					int tid = (edgeT.a == num) ? edgeT.b : edgeT.a;
					if (!triangles.IsSelected(tid))
					{
						this.add(triEdges[i]);
					}
				}
			}
		}

		public void Deselect(int tid)
		{
			this.remove(tid);
		}

		public void Deselect(int[] edges)
		{
			for (int i = 0; i < edges.Length; i++)
			{
				this.remove(edges[i]);
			}
		}

		public void Deselect(IEnumerable<int> edges)
		{
			foreach (int eid in edges)
			{
				this.remove(eid);
			}
		}

		public void DeselectAll()
		{
			this.Selected.Clear();
		}

		public int[] ToArray()
		{
			return this.Selected.ToArray<int>();
		}

		public DMesh3 Mesh;

		private HashSet<int> Selected;

		private List<int> temp;

		private BitArray tempBits;
	}
}
