using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshFacesFromLoop
	{
		public MeshFacesFromLoop(DMesh3 Mesh, DCurve3 SpaceCurve, ISpatial Spatial)
		{
			this.Mesh = Mesh;
			int vertexCount = SpaceCurve.VertexCount;
			this.InitialLoopT = new int[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				this.InitialLoopT[i] = Spatial.FindNearestTriangle(SpaceCurve[i], double.MaxValue);
			}
			this.find_path();
			this.find_interior_from_tris();
		}

		public MeshFacesFromLoop(DMesh3 Mesh, DCurve3 SpaceCurve, ISpatial Spatial, int tSeed)
		{
			this.Mesh = Mesh;
			int vertexCount = SpaceCurve.VertexCount;
			this.InitialLoopT = new int[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				this.InitialLoopT[i] = Spatial.FindNearestTriangle(SpaceCurve[i], double.MaxValue);
			}
			this.find_path();
			this.find_interior_from_seed(tSeed);
		}

		public int[] ToArray()
		{
			return this.InteriorT.ToArray();
		}

		public MeshFaceSelection ToSelection()
		{
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(this.Mesh);
			meshFaceSelection.Select(this.InteriorT);
			return meshFaceSelection;
		}

		public IList<int> PathTriangles
		{
			get
			{
				return this.PathT;
			}
		}

		public IList<int> InteriorTriangles
		{
			get
			{
				return this.InteriorT;
			}
		}

		private void find_interior_from_tris()
		{
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(this.Mesh);
			meshFaceSelection.Select(this.PathT);
			meshFaceSelection.ExpandToOneRingNeighbours(null);
			meshFaceSelection.Deselect(this.PathT);
			MeshConnectedComponents meshConnectedComponents = new MeshConnectedComponents(this.Mesh);
			meshConnectedComponents.FilterSet = meshFaceSelection;
			meshConnectedComponents.FindConnectedT();
			int num = meshConnectedComponents.Count;
			if (num < 2)
			{
				throw new Exception("MeshFacesFromLoop.find_interior: only found one connected component!");
			}
			meshConnectedComponents.SortByCount(false);
			num = 2;
			MeshFaceSelection[] array = new MeshFaceSelection[num];
			bool[] array2 = new bool[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new MeshFaceSelection(this.Mesh);
				array[i].Select(meshConnectedComponents.Components[i].Indices);
				array2[i] = false;
			}
			HashSet<int> border_tris = new HashSet<int>(this.PathT);
			Func<int, bool> triFilterF = (int tid) => !border_tris.Contains(tid);
			for (int j = 0; j < num; j++)
			{
				array[j].FloodFill(meshConnectedComponents.Components[j].Indices, triFilterF, null);
			}
			Array.Sort<MeshFaceSelection>(array, (MeshFaceSelection a, MeshFaceSelection b) => a.Count.CompareTo(b.Count));
			this.InteriorT = new List<int>(array[0]);
		}

		private void find_interior_from_seed(int tSeed)
		{
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(this.Mesh);
			meshFaceSelection.Select(this.PathT);
			meshFaceSelection.FloodFill(tSeed, null, null);
			this.InteriorT = new List<int>(meshFaceSelection);
		}

		private void find_path()
		{
			this.PathT = new List<int>();
			this.PathT.Add(this.InitialLoopT[0]);
			for (int i = 1; i <= this.InitialLoopT.Length; i++)
			{
				int num = this.PathT[this.PathT.Count - 1];
				int num2 = this.InitialLoopT[i % this.InitialLoopT.Length];
				if (num2 != num)
				{
					Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(num);
					if (triNeighbourTris.a == num2 || triNeighbourTris.b == num2 || triNeighbourTris.c == num2)
					{
						this.PathT.Add(num2);
					}
					else
					{
						List<int> collection = this.find_path(num, num2);
						this.PathT.AddRange(collection);
						this.PathT.Add(num2);
					}
				}
			}
			if (this.PathT[this.PathT.Count - 1] == this.PathT[0])
			{
				this.PathT.RemoveAt(this.PathT.Count - 1);
			}
		}

		private void push_onto_sequence(int parentID)
		{
			Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(parentID);
			for (int i = 0; i < 3; i++)
			{
				if (!this.used.Contains(triNeighbourTris[i]))
				{
					this.sequence.Add(new MeshFacesFromLoop.TriWithParent
					{
						tID = triNeighbourTris[i],
						parentID = parentID
					});
					this.used.Add(triNeighbourTris[i]);
				}
			}
		}

		private List<int> find_path(int t1, int t2)
		{
			this.buffer.Clear();
			this.sequence.Clear();
			this.used.Clear();
			this.used.Add(t1);
			this.push_onto_sequence(t1);
			int num = 0;
			int num2 = -1;
			while (num2 == -1)
			{
				MeshFacesFromLoop.TriWithParent triWithParent = this.sequence[num];
				Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(triWithParent.tID);
				if (triNeighbourTris.a == t2 || triNeighbourTris.b == t2 || triNeighbourTris.c == t2)
				{
					num2 = num;
				}
				else
				{
					this.push_onto_sequence(triWithParent.tID);
				}
				num++;
			}
			if (num2 == -1)
			{
				throw new Exception("MeshFacesFromLoop.find_path : could not find path!!");
			}
			MeshFacesFromLoop.TriWithParent tCur = this.sequence[num2];
			this.buffer.Add(tCur.tID);
			while (tCur.parentID != t1)
			{
				tCur = this.sequence.Find((MeshFacesFromLoop.TriWithParent x) => x.tID == tCur.parentID);
				this.buffer.Add(tCur.tID);
			}
			this.buffer.Reverse();
			return this.buffer;
		}

		public DMesh3 Mesh;

		private int[] InitialLoopT;

		private List<int> PathT;

		private List<int> InteriorT;

		private List<MeshFacesFromLoop.TriWithParent> sequence = new List<MeshFacesFromLoop.TriWithParent>(32);

		private HashSet<int> used = new HashSet<int>();

		private List<int> buffer = new List<int>(32);

		private struct TriWithParent
		{
			public int tID;

			public int parentID;
		}
	}
}
