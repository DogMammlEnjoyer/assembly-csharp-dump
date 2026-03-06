using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class MeshFaceSelection : IEnumerable<int>, IEnumerable
	{
		public MeshFaceSelection(DMesh3 mesh)
		{
			this.Mesh = mesh;
			this.Selected = new HashSet<int>();
			this.temp = new List<int>();
			this.temp2 = new List<int>();
		}

		public MeshFaceSelection(MeshFaceSelection copy)
		{
			this.Mesh = copy.Mesh;
			this.Selected = new HashSet<int>(copy.Selected);
			this.temp = new List<int>();
			this.temp2 = new List<int>();
		}

		protected BitArray Bitmap
		{
			get
			{
				if (this.tempBits == null)
				{
					this.tempBits = new BitArray(this.Mesh.MaxTriangleID);
				}
				return this.tempBits;
			}
		}

		public MeshFaceSelection(DMesh3 mesh, MeshVertexSelection convertV, int minCount = 3) : this(mesh)
		{
			minCount = MathUtil.Clamp(minCount, 1, 3);
			if (minCount == 1)
			{
				using (IEnumerator<int> enumerator = convertV.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int vID = enumerator.Current;
						foreach (int tid in mesh.VtxTrianglesItr(vID))
						{
							this.add(tid);
						}
					}
					return;
				}
			}
			foreach (int num in mesh.TriangleIndices())
			{
				Index3i triangle = mesh.GetTriangle(num);
				if (minCount == 3)
				{
					if (convertV.IsSelected(triangle.a) && convertV.IsSelected(triangle.b) && convertV.IsSelected(triangle.c))
					{
						this.add(num);
					}
				}
				else if ((convertV.IsSelected(triangle.a) ? 1 : 0) + (convertV.IsSelected(triangle.b) ? 1 : 0) + (convertV.IsSelected(triangle.c) ? 1 : 0) >= minCount)
				{
					this.add(num);
				}
			}
		}

		public MeshFaceSelection(DMesh3 mesh, int group_id) : this(mesh)
		{
			this.SelectGroup(group_id);
		}

		public IEnumerator<int> GetEnumerator()
		{
			return this.Selected.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Selected.GetEnumerator();
		}

		private void add(int tid)
		{
			this.Selected.Add(tid);
		}

		private void remove(int tid)
		{
			this.Selected.Remove(tid);
		}

		public int Count
		{
			get
			{
				return this.Selected.Count;
			}
		}

		public bool IsSelected(int tid)
		{
			return this.Selected.Contains(tid);
		}

		public void Select(int tid)
		{
			if (this.Mesh.IsTriangle(tid))
			{
				this.add(tid);
			}
		}

		public void Select(int[] triangles)
		{
			for (int i = 0; i < triangles.Length; i++)
			{
				if (this.Mesh.IsTriangle(triangles[i]))
				{
					this.add(triangles[i]);
				}
			}
		}

		public void Select(List<int> triangles)
		{
			for (int i = 0; i < triangles.Count; i++)
			{
				if (this.Mesh.IsTriangle(triangles[i]))
				{
					this.add(triangles[i]);
				}
			}
		}

		public void Select(IEnumerable<int> triangles)
		{
			foreach (int num in triangles)
			{
				if (this.Mesh.IsTriangle(num))
				{
					this.add(num);
				}
			}
		}

		public void Select(Func<int, bool> selectF)
		{
			this.temp.Clear();
			int maxTriangleID = this.Mesh.MaxTriangleID;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (this.Mesh.IsTriangle(i) && selectF(i))
				{
					this.temp.Add(i);
				}
			}
			this.Select(this.temp);
		}

		public void SelectVertexOneRing(int vid)
		{
			foreach (int tid in this.Mesh.VtxTrianglesItr(vid))
			{
				this.add(tid);
			}
		}

		public void SelectVertexOneRings(int[] vertices)
		{
			foreach (int vID in vertices)
			{
				foreach (int tid in this.Mesh.VtxTrianglesItr(vID))
				{
					this.add(tid);
				}
			}
		}

		public void SelectVertexOneRings(IEnumerable<int> vertices)
		{
			foreach (int vID in vertices)
			{
				foreach (int tid in this.Mesh.VtxTrianglesItr(vID))
				{
					this.add(tid);
				}
			}
		}

		public void SelectEdgeTris(int eid)
		{
			Index2i edgeT = this.Mesh.GetEdgeT(eid);
			this.add(edgeT.a);
			if (edgeT.b != -1)
			{
				this.add(edgeT.b);
			}
		}

		public void Deselect(int tid)
		{
			this.remove(tid);
		}

		public void Deselect(int[] triangles)
		{
			for (int i = 0; i < triangles.Length; i++)
			{
				this.remove(triangles[i]);
			}
		}

		public void Deselect(IEnumerable<int> triangles)
		{
			foreach (int tid in triangles)
			{
				this.remove(tid);
			}
		}

		public void DeselectAll()
		{
			this.Selected.Clear();
		}

		public void SelectGroup(int gid)
		{
			int maxTriangleID = this.Mesh.MaxTriangleID;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (this.Mesh.IsTriangle(i) && this.Mesh.GetTriangleGroup(i) == gid)
				{
					this.add(i);
				}
			}
		}

		public void SelectGroupInverse(int gid)
		{
			int maxTriangleID = this.Mesh.MaxTriangleID;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (this.Mesh.IsTriangle(i) && this.Mesh.GetTriangleGroup(i) != gid)
				{
					this.add(i);
				}
			}
		}

		public void DeselectGroup(int gid)
		{
			int maxTriangleID = this.Mesh.MaxTriangleID;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (this.Mesh.IsTriangle(i) && this.Mesh.GetTriangleGroup(i) == gid)
				{
					this.remove(i);
				}
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

		public List<int> FindNeighbourTris()
		{
			List<int> list = new List<int>();
			foreach (int tID in this.Selected)
			{
				Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(tID);
				for (int i = 0; i < 3; i++)
				{
					if (triNeighbourTris[i] != -1 && !this.IsSelected(triNeighbourTris[i]))
					{
						list.Add(triNeighbourTris[i]);
					}
				}
			}
			return list;
		}

		public List<int> FindBorderTris()
		{
			List<int> list = new List<int>();
			foreach (int num in this.Selected)
			{
				Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(num);
				if (!this.IsSelected(triNeighbourTris.a) || !this.IsSelected(triNeighbourTris.b) || !this.IsSelected(triNeighbourTris.c))
				{
					list.Add(num);
				}
			}
			return list;
		}

		public void ExpandToFaceNeighbours(Func<int, bool> FilterF = null)
		{
			this.temp.Clear();
			foreach (int tID in this.Selected)
			{
				Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(tID);
				for (int i = 0; i < 3; i++)
				{
					if ((FilterF == null || FilterF(triNeighbourTris[i])) && triNeighbourTris[i] != -1 && !this.IsSelected(triNeighbourTris[i]))
					{
						this.temp.Add(triNeighbourTris[i]);
					}
				}
			}
			for (int j = 0; j < this.temp.Count; j++)
			{
				this.add(this.temp[j]);
			}
		}

		public void ExpandToFaceNeighbours(int rounds, Func<int, bool> FilterF = null)
		{
			for (int i = 0; i < rounds; i++)
			{
				this.ExpandToFaceNeighbours(FilterF);
			}
		}

		public void ExpandToOneRingNeighbours(Func<int, bool> FilterF = null)
		{
			this.temp.Clear();
			foreach (int tID in this.Selected)
			{
				Index3i triangle = this.Mesh.GetTriangle(tID);
				for (int i = 0; i < 3; i++)
				{
					int vID = triangle[i];
					foreach (int num in this.Mesh.VtxTrianglesItr(vID))
					{
						if ((FilterF == null || FilterF(num)) && !this.IsSelected(num))
						{
							this.temp.Add(num);
						}
					}
				}
			}
			for (int j = 0; j < this.temp.Count; j++)
			{
				this.add(this.temp[j]);
			}
		}

		public void ExpandToOneRingNeighbours(int nRings, Func<int, bool> FilterF = null)
		{
			if (nRings == 1)
			{
				this.ExpandToOneRingNeighbours(FilterF);
				return;
			}
			List<int> list = this.temp;
			List<int> list2 = this.temp2;
			list2.Clear();
			list2.AddRange(this.Selected);
			this.Bitmap.SetAll(false);
			foreach (int index in this.Selected)
			{
				this.Bitmap.Set(index, true);
			}
			for (int i = 0; i < nRings; i++)
			{
				list.Clear();
				foreach (int tID in list2)
				{
					Index3i triangle = this.Mesh.GetTriangle(tID);
					for (int j = 0; j < 3; j++)
					{
						int vID = triangle[j];
						foreach (int num in this.Mesh.VtxTrianglesItr(vID))
						{
							if ((FilterF == null || FilterF(num)) && !this.Bitmap.Get(num))
							{
								list.Add(num);
								this.Bitmap.Set(num, true);
							}
						}
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					this.add(list[k]);
				}
				List<int> list3 = list2;
				list2 = list;
				list = list3;
			}
		}

		public void ContractBorderByOneRingNeighbours()
		{
			this.temp.Clear();
			foreach (int tID in this.Selected)
			{
				Index3i triangle = this.Mesh.GetTriangle(tID);
				for (int i = 0; i < 3; i++)
				{
					int num = triangle[i];
					foreach (int tid in this.Mesh.VtxTrianglesItr(num))
					{
						if (!this.IsSelected(tid))
						{
							this.temp.Add(num);
							break;
						}
					}
				}
			}
			foreach (int vID in this.temp)
			{
				foreach (int tid2 in this.Mesh.VtxTrianglesItr(vID))
				{
					this.Deselect(tid2);
				}
			}
		}

		public void FloodFill(int tSeed, Func<int, bool> TriFilterF = null, Func<int, bool> EdgeFilterF = null)
		{
			this.FloodFill(new int[]
			{
				tSeed
			}, TriFilterF, EdgeFilterF);
		}

		public void FloodFill(int[] Seeds, Func<int, bool> TriFilterF = null, Func<int, bool> EdgeFilterF = null)
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
				Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(back);
				for (int j = 0; j < 3; j++)
				{
					int num = triNeighbourTris[j];
					if (num != -1 && !this.IsSelected(num) && (TriFilterF == null || TriFilterF(num)) && (EdgeFilterF == null || EdgeFilterF(this.Mesh.GetTriEdge(back, j))))
					{
						this.add(num);
						dvector.push_back(num);
					}
				}
			}
		}

		public bool ClipFins(bool bClipLoners)
		{
			this.temp.Clear();
			foreach (int num in this.Selected)
			{
				if (this.is_fin(num, bClipLoners))
				{
					this.temp.Add(num);
				}
			}
			if (this.temp.Count == 0)
			{
				return false;
			}
			foreach (int tid in this.temp)
			{
				this.remove(tid);
			}
			return true;
		}

		public bool FillEars(bool bFillTinyHoles)
		{
			this.temp.Clear();
			foreach (int tID in this.Selected)
			{
				Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(tID);
				for (int i = 0; i < 3; i++)
				{
					int num = triNeighbourTris[i];
					if (!this.IsSelected(num) && this.is_ear(num, bFillTinyHoles))
					{
						this.temp.Add(num);
					}
				}
			}
			if (this.temp.Count == 0)
			{
				return false;
			}
			foreach (int tid in this.temp)
			{
				this.add(tid);
			}
			return true;
		}

		public bool LocalOptimize(bool bClipFins, bool bFillEars, bool bFillTinyHoles = true, bool bClipLoners = true, bool bRemoveBowties = false)
		{
			bool result = false;
			bool flag = false;
			int num = 0;
			HashSet<int> tempHash = new HashSet<int>();
			while (!flag)
			{
				flag = true;
				if (num++ == 25)
				{
					break;
				}
				if (bClipFins && this.ClipFins(bClipLoners))
				{
					flag = false;
				}
				if (bFillEars && this.FillEars(bFillTinyHoles))
				{
					flag = false;
				}
				if (bRemoveBowties && this.remove_bowties(tempHash))
				{
					flag = false;
				}
				if (!flag)
				{
					result = true;
				}
			}
			if (bRemoveBowties)
			{
				this.remove_bowties(tempHash);
			}
			return result;
		}

		public bool LocalOptimize(bool bRemoveBowties = true)
		{
			return this.LocalOptimize(true, true, true, true, bRemoveBowties);
		}

		public bool RemoveBowties()
		{
			return this.remove_bowties(null);
		}

		public bool remove_bowties(HashSet<int> tempHash)
		{
			bool result = false;
			bool flag = false;
			HashSet<int> hashSet = (tempHash == null) ? new HashSet<int>() : tempHash;
			while (!flag)
			{
				flag = true;
				hashSet.Clear();
				foreach (int tID in this.Selected)
				{
					Index3i triangle = this.Mesh.GetTriangle(tID);
					hashSet.Add(triangle.a);
					hashSet.Add(triangle.b);
					hashSet.Add(triangle.c);
				}
				foreach (int num in hashSet)
				{
					if (this.is_bowtie_vtx(num))
					{
						this.Deselect(this.Mesh.VtxTrianglesItr(num));
						flag = false;
					}
				}
				if (!flag)
				{
					result = true;
				}
			}
			return result;
		}

		private bool is_bowtie_vtx(int vid)
		{
			int num = 0;
			foreach (int eID in this.Mesh.VtxEdgesItr(vid))
			{
				Index2i edgeT = this.Mesh.GetEdgeT(eID);
				if (edgeT.b != -1)
				{
					bool flag = this.IsSelected(edgeT.a);
					bool flag2 = this.IsSelected(edgeT.b);
					if (flag != flag2)
					{
						num++;
					}
				}
				else if (this.IsSelected(edgeT.a))
				{
					num++;
				}
			}
			return num > 2;
		}

		private void count_nbrs(int tid, out int nbr_in, out int nbr_out, out int bdry_e)
		{
			Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(tid);
			nbr_in = 0;
			nbr_out = 0;
			bdry_e = 0;
			for (int i = 0; i < 3; i++)
			{
				int num = triNeighbourTris[i];
				if (num == -1)
				{
					bdry_e++;
				}
				else if (this.IsSelected(num))
				{
					nbr_in++;
				}
				else
				{
					nbr_out++;
				}
			}
		}

		private bool is_ear(int tid, bool include_tiny_holes)
		{
			if (this.IsSelected(tid))
			{
				return false;
			}
			int num;
			int num2;
			int num3;
			this.count_nbrs(tid, out num, out num2, out num3);
			if (num3 == 2 && num == 1)
			{
				return true;
			}
			if (num == 2)
			{
				if (num3 == 1 || num2 == 1)
				{
					return true;
				}
			}
			else if (include_tiny_holes && num == 3)
			{
				return true;
			}
			return false;
		}

		private bool is_fin(int tid, bool include_loners)
		{
			if (!this.IsSelected(tid))
			{
				return false;
			}
			int num;
			int num2;
			int num3;
			this.count_nbrs(tid, out num, out num2, out num3);
			return (num == 1 && num2 == 2) || (include_loners && num == 0 && num2 == 3);
		}

		public DMesh3 Mesh;

		private HashSet<int> Selected;

		private List<int> temp;

		private List<int> temp2;

		private BitArray tempBits;
	}
}
