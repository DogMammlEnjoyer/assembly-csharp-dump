using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class MeshVertexSelection : IEnumerable<int>, IEnumerable
	{
		public MeshVertexSelection(DMesh3 mesh)
		{
			this.Mesh = mesh;
			this.Selected = new HashSet<int>();
			this.temp = new List<int>();
		}

		public MeshVertexSelection(DMesh3 mesh, MeshFaceSelection convertT) : this(mesh)
		{
			foreach (int tID in convertT)
			{
				Index3i triangle = mesh.GetTriangle(tID);
				this.add(triangle.a);
				this.add(triangle.b);
				this.add(triangle.c);
			}
		}

		public MeshVertexSelection(DMesh3 mesh, MeshEdgeSelection convertE) : this(mesh)
		{
			foreach (int eID in convertE)
			{
				Index2i edgeV = mesh.GetEdgeV(eID);
				this.add(edgeV.a);
				this.add(edgeV.b);
			}
		}

		public HashSet<int> ExtractSelected()
		{
			HashSet<int> selected = this.Selected;
			this.Selected = new HashSet<int>();
			return selected;
		}

		public IEnumerator<int> GetEnumerator()
		{
			return this.Selected.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Selected.GetEnumerator();
		}

		private void add(int vID)
		{
			this.Selected.Add(vID);
		}

		private void remove(int vID)
		{
			this.Selected.Remove(vID);
		}

		public int Count
		{
			get
			{
				return this.Selected.Count;
			}
		}

		public bool IsSelected(int vID)
		{
			return this.Selected.Contains(vID);
		}

		public void Select(int vID)
		{
			if (this.Mesh.IsVertex(vID))
			{
				this.add(vID);
			}
		}

		public void Select(int[] vertices)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				if (this.Mesh.IsVertex(vertices[i]))
				{
					this.add(vertices[i]);
				}
			}
		}

		public void Select(IEnumerable<int> vertices)
		{
			foreach (int vID in vertices)
			{
				if (this.Mesh.IsVertex(vID))
				{
					this.add(vID);
				}
			}
		}

		public void SelectTriangleVertices(int[] triangles)
		{
			for (int i = 0; i < triangles.Length; i++)
			{
				Index3i triangle = this.Mesh.GetTriangle(triangles[i]);
				this.add(triangle.a);
				this.add(triangle.b);
				this.add(triangle.c);
			}
		}

		public void SelectTriangleVertices(IEnumerable<int> triangles)
		{
			foreach (int tID in triangles)
			{
				Index3i triangle = this.Mesh.GetTriangle(tID);
				this.add(triangle.a);
				this.add(triangle.b);
				this.add(triangle.c);
			}
		}

		public void SelectTriangleVertices(MeshFaceSelection triangles)
		{
			foreach (int tID in triangles)
			{
				Index3i triangle = this.Mesh.GetTriangle(tID);
				this.add(triangle.a);
				this.add(triangle.b);
				this.add(triangle.c);
			}
		}

		public void SelectInteriorVertices(MeshFaceSelection triangles)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (int tID in triangles)
			{
				Index3i triangle = this.Mesh.GetTriangle(tID);
				for (int i = 0; i < 3; i++)
				{
					int num = triangle[i];
					if (!this.Selected.Contains(num) && !hashSet.Contains(num))
					{
						bool flag = true;
						foreach (int tid in this.Mesh.VtxTrianglesItr(num))
						{
							if (!triangles.IsSelected(tid))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							this.add(num);
						}
						else
						{
							hashSet.Add(num);
						}
					}
				}
			}
		}

		public void SelectConnectedBoundaryV(int vSeed)
		{
			if (!this.Mesh.IsBoundaryVertex(vSeed))
			{
				throw new Exception("MeshConnectedComponents.FindConnectedBoundaryV: vSeed is not a boundary vertex");
			}
			HashSet<int> hashSet = (this.Selected.Count == 0) ? this.Selected : new HashSet<int>();
			hashSet.Add(vSeed);
			List<int> list = this.temp;
			list.Clear();
			list.Add(vSeed);
			while (list.Count > 0)
			{
				int vID = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				foreach (int num in this.Mesh.VtxVerticesItr(vID))
				{
					if (this.Mesh.IsBoundaryVertex(num) && !hashSet.Contains(num))
					{
						hashSet.Add(num);
						list.Add(num);
					}
				}
			}
			if (hashSet != this.Selected)
			{
				foreach (int vID2 in hashSet)
				{
					this.add(vID2);
				}
			}
			this.temp.Clear();
		}

		public void SelectEdgeVertices(int[] edges)
		{
			for (int i = 0; i < edges.Length; i++)
			{
				Index2i edgeV = this.Mesh.GetEdgeV(edges[i]);
				this.add(edgeV.a);
				this.add(edgeV.b);
			}
		}

		public void SelectEdgeVertices(IEnumerable<int> edges)
		{
			foreach (int eID in edges)
			{
				Index2i edgeV = this.Mesh.GetEdgeV(eID);
				this.add(edgeV.a);
				this.add(edgeV.b);
			}
		}

		public void Deselect(int vID)
		{
			this.remove(vID);
		}

		public void Deselect(int[] vertices)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				this.remove(vertices[i]);
			}
		}

		public void Deselect(IEnumerable<int> vertices)
		{
			foreach (int vID in vertices)
			{
				this.remove(vID);
			}
		}

		public void DeselectEdge(int eid)
		{
			Index2i edgeV = this.Mesh.GetEdgeV(eid);
			this.remove(edgeV.a);
			this.remove(edgeV.b);
		}

		public void DeselectEdges(IEnumerable<int> edges)
		{
			foreach (int eID in edges)
			{
				Index2i edgeV = this.Mesh.GetEdgeV(eID);
				this.remove(edgeV.a);
				this.remove(edgeV.b);
			}
		}

		public int[] ToArray()
		{
			int[] array = new int[this.Selected.Count];
			int num = 0;
			foreach (int num2 in this.Selected)
			{
				array[num++] = num2;
			}
			return array;
		}

		public void ExpandToOneRingNeighbours(Func<int, bool> FilterF = null)
		{
			this.temp.Clear();
			foreach (int vID in this.Selected)
			{
				foreach (int num in this.Mesh.VtxVerticesItr(vID))
				{
					if ((FilterF == null || FilterF(num)) && !this.IsSelected(num))
					{
						this.temp.Add(num);
					}
				}
			}
			for (int i = 0; i < this.temp.Count; i++)
			{
				this.add(this.temp[i]);
			}
		}

		public void ExpandToOneRingNeighbours(int nRings, Func<int, bool> FilterF = null)
		{
			for (int i = 0; i < nRings; i++)
			{
				this.ExpandToOneRingNeighbours(FilterF);
			}
		}

		public void FloodFill(int vSeed, Func<int, bool> VertIncludedF = null)
		{
			this.FloodFill(new int[]
			{
				vSeed
			}, VertIncludedF);
		}

		public void FloodFill(int[] Seeds, Func<int, bool> VertIncludedF = null)
		{
			DVector<int> dvector = new DVector<int>(Seeds);
			for (int i = 0; i < Seeds.Length; i++)
			{
				this.add(Seeds[i]);
			}
			while (dvector.size > 0)
			{
				int back = dvector.back;
				dvector.pop_back();
				foreach (int num in this.Mesh.VtxVerticesItr(back))
				{
					if (!this.IsSelected(num) && VertIncludedF(num))
					{
						this.add(num);
						dvector.push_back(num);
					}
				}
			}
		}

		public DMesh3 Mesh;

		private HashSet<int> Selected;

		private List<int> temp;
	}
}
