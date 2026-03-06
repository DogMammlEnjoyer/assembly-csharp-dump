using System;
using System.Collections.Generic;
using System.Threading;
using g3;

namespace gs
{
	public class MeshSpatialSort
	{
		public MeshSpatialSort()
		{
			this.Components = new List<MeshSpatialSort.ComponentMesh>();
		}

		public void AddMesh(DMesh3 mesh, object identifier, DMeshAABBTree3 spatial = null)
		{
			MeshSpatialSort.ComponentMesh componentMesh = new MeshSpatialSort.ComponentMesh(mesh, identifier, spatial);
			if (spatial == null && (componentMesh.IsClosed || this.AllowOpenContainers))
			{
				componentMesh.Spatial = new DMeshAABBTree3(mesh, true);
			}
			this.Components.Add(componentMesh);
		}

		public void Sort()
		{
			int N = this.Components.Count;
			MeshSpatialSort.ComponentMesh[] comps = this.Components.ToArray();
			int j;
			Array.Sort<MeshSpatialSort.ComponentMesh>(comps, delegate(MeshSpatialSort.ComponentMesh i, MeshSpatialSort.ComponentMesh j)
			{
				if (!j.Bounds.Contains(j.Bounds))
				{
					return 1;
				}
				return -1;
			});
			bool[] bIsContained = new bool[N];
			Dictionary<int, List<int>> ContainSets = new Dictionary<int, List<int>>();
			Dictionary<int, List<int>> ContainedParents = new Dictionary<int, List<int>>();
			SpinLock dataLock = default(SpinLock);
			gParallel.ForEach<int>(Interval1i.Range(N), delegate(int i)
			{
				MeshSpatialSort.ComponentMesh componentMesh4 = comps[i];
				if (!componentMesh4.IsClosed && !this.AllowOpenContainers)
				{
					return;
				}
				for (int m = 0; m < N; m++)
				{
					if (i != m)
					{
						MeshSpatialSort.ComponentMesh componentMesh5 = comps[m];
						if (componentMesh4.Bounds.Contains(componentMesh5.Bounds) && componentMesh4.Contains(componentMesh5, 0.5))
						{
							bool flag = false;
							dataLock.Enter(ref flag);
							componentMesh5.InsideOf.Add(componentMesh4);
							componentMesh4.InsideSet.Add(componentMesh5);
							if (!ContainSets.ContainsKey(i))
							{
								ContainSets.Add(i, new List<int>());
							}
							ContainSets[i].Add(m);
							bIsContained[m] = true;
							if (!ContainedParents.ContainsKey(m))
							{
								ContainedParents.Add(m, new List<int>());
							}
							ContainedParents[m].Add(i);
							dataLock.Exit();
						}
					}
				}
			});
			List<MeshSpatialSort.MeshSolid> list = new List<MeshSpatialSort.MeshSolid>();
			HashSet<MeshSpatialSort.ComponentMesh> hashSet = new HashSet<MeshSpatialSort.ComponentMesh>();
			Dictionary<MeshSpatialSort.ComponentMesh, int> dictionary = new Dictionary<MeshSpatialSort.ComponentMesh, int>();
			List<int> list2 = new List<int>();
			for (int l = 0; l < N; l++)
			{
				MeshSpatialSort.ComponentMesh componentMesh = comps[l];
				if (!bIsContained[l])
				{
					MeshSpatialSort.MeshSolid item = new MeshSpatialSort.MeshSolid
					{
						Outer = componentMesh
					};
					int count = list.Count;
					dictionary[componentMesh] = count;
					hashSet.Add(componentMesh);
					if (ContainSets.ContainsKey(l))
					{
						list2.Add(l);
					}
					list.Add(item);
				}
			}
			while (list2.Count > 0)
			{
				List<int> list3 = new List<int>();
				foreach (int num in list2)
				{
					MeshSpatialSort.ComponentMesh key = comps[num];
					int index = dictionary[key];
					foreach (int num2 in ContainSets[num])
					{
						MeshSpatialSort.ComponentMesh item2 = comps[num2];
						if (ContainedParents[num2].Count <= 1)
						{
							list[index].Cavities.Add(item2);
							hashSet.Add(item2);
							if (ContainSets.ContainsKey(num2))
							{
								list3.Add(num2);
							}
						}
					}
					list3.Add(num);
				}
				foreach (int num3 in list3)
				{
					ContainSets.Remove(num3);
					foreach (int key2 in new List<int>(ContainedParents.Keys))
					{
						if (ContainedParents[key2].Contains(num3))
						{
							ContainedParents[key2].Remove(num3);
						}
					}
				}
				list2.Clear();
				for (j = 0; j < N; j++)
				{
					MeshSpatialSort.ComponentMesh componentMesh2 = comps[j];
					if (!hashSet.Contains(componentMesh2) && ContainSets.ContainsKey(j) && ContainedParents[j].Count <= 0)
					{
						MeshSpatialSort.MeshSolid item3 = new MeshSpatialSort.MeshSolid
						{
							Outer = componentMesh2
						};
						int count2 = list.Count;
						dictionary[componentMesh2] = count2;
						hashSet.Add(componentMesh2);
						if (ContainSets.ContainsKey(j))
						{
							list2.Add(j);
						}
						list.Add(item3);
					}
				}
			}
			for (int k = 0; k < N; k++)
			{
				MeshSpatialSort.ComponentMesh componentMesh3 = comps[k];
				if (!hashSet.Contains(componentMesh3))
				{
					MeshSpatialSort.MeshSolid item4 = new MeshSpatialSort.MeshSolid
					{
						Outer = componentMesh3
					};
					list.Add(item4);
				}
			}
			this.Solids = list;
		}

		public List<MeshSpatialSort.ComponentMesh> Components;

		public List<MeshSpatialSort.MeshSolid> Solids;

		public bool AllowOpenContainers;

		public double FastWindingIso = 0.5;

		public class ComponentMesh
		{
			public ComponentMesh(DMesh3 mesh, object identifier, DMeshAABBTree3 spatial)
			{
				this.Mesh = mesh;
				this.Identifier = identifier;
				this.IsClosed = mesh.IsClosed();
				this.Spatial = spatial;
				this.Bounds = mesh.CachedBounds;
			}

			public bool Contains(MeshSpatialSort.ComponentMesh mesh2, double fIso = 0.5)
			{
				if (this.Spatial == null)
				{
					return false;
				}
				this.Spatial.FastWindingNumber(Vector3d.Zero);
				int vertexCount = mesh2.Mesh.VertexCount;
				bool contained = true;
				gParallel.BlockStartEnd(0, vertexCount - 1, delegate(int a, int b)
				{
					if (!contained)
					{
						return;
					}
					int num = a;
					while (num <= b & contained)
					{
						Vector3d vertex = mesh2.Mesh.GetVertex(num);
						if (Math.Abs(this.Spatial.FastWindingNumber(vertex)) < fIso)
						{
							contained = false;
							return;
						}
						num++;
					}
				}, 100, false);
				return contained;
			}

			public object Identifier;

			public DMesh3 Mesh;

			public bool IsClosed;

			public DMeshAABBTree3 Spatial;

			public AxisAlignedBox3d Bounds;

			public List<MeshSpatialSort.ComponentMesh> InsideOf = new List<MeshSpatialSort.ComponentMesh>();

			public List<MeshSpatialSort.ComponentMesh> InsideSet = new List<MeshSpatialSort.ComponentMesh>();
		}

		public class MeshSolid
		{
			public MeshSpatialSort.ComponentMesh Outer;

			public List<MeshSpatialSort.ComponentMesh> Cavities = new List<MeshSpatialSort.ComponentMesh>();
		}
	}
}
