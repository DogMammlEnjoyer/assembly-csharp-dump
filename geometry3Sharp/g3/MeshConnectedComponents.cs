using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class MeshConnectedComponents : IEnumerable<MeshConnectedComponents.Component>, IEnumerable
	{
		public MeshConnectedComponents(DMesh3 mesh)
		{
			this.Mesh = mesh;
			this.Components = new List<MeshConnectedComponents.Component>();
		}

		public int Count
		{
			get
			{
				return this.Components.Count;
			}
		}

		public MeshConnectedComponents.Component this[int index]
		{
			get
			{
				return this.Components[index];
			}
		}

		public IEnumerator<MeshConnectedComponents.Component> GetEnumerator()
		{
			return this.Components.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Components.GetEnumerator();
		}

		public int LargestByCount
		{
			get
			{
				int num = 0;
				int num2 = this.Components[num].Indices.Length;
				for (int i = 1; i < this.Components.Count; i++)
				{
					if (this.Components[i].Indices.Length > num2)
					{
						num2 = this.Components[i].Indices.Length;
						num = i;
					}
				}
				return num;
			}
		}

		public void SortByCount(bool bIncreasing = true)
		{
			if (bIncreasing)
			{
				this.Components.Sort((MeshConnectedComponents.Component x, MeshConnectedComponents.Component y) => x.Indices.Length.CompareTo(y.Indices.Length));
				return;
			}
			this.Components.Sort((MeshConnectedComponents.Component x, MeshConnectedComponents.Component y) => -x.Indices.Length.CompareTo(y.Indices.Length));
		}

		public void SortByValue(Func<MeshConnectedComponents.Component, double> valueF, bool bIncreasing = true)
		{
			Dictionary<MeshConnectedComponents.Component, double> vals = new Dictionary<MeshConnectedComponents.Component, double>();
			foreach (MeshConnectedComponents.Component component in this.Components)
			{
				vals[component] = valueF(component);
			}
			if (bIncreasing)
			{
				this.Components.Sort((MeshConnectedComponents.Component x, MeshConnectedComponents.Component y) => vals[x].CompareTo(vals[y]));
				return;
			}
			this.Components.Sort((MeshConnectedComponents.Component x, MeshConnectedComponents.Component y) => -vals[x].CompareTo(vals[y]));
		}

		public void FindConnectedT()
		{
			this.Components = new List<MeshConnectedComponents.Component>();
			int maxTriangleID = this.Mesh.MaxTriangleID;
			Func<int, bool> func = (int i) => this.Mesh.IsTriangle(i);
			if (this.FilterF != null)
			{
				func = ((int i) => this.Mesh.IsTriangle(i) && this.FilterF(i));
			}
			byte[] array = new byte[this.Mesh.MaxTriangleID];
			Interval1i empty = Interval1i.Empty;
			if (this.FilterSet != null)
			{
				for (int m = 0; m < maxTriangleID; m++)
				{
					array[m] = byte.MaxValue;
				}
				using (IEnumerator<int> enumerator = this.FilterSet.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int num = enumerator.Current;
						if (func(num))
						{
							array[num] = 0;
							empty.Contain(num);
						}
					}
					goto IL_EB;
				}
			}
			for (int j = 0; j < maxTriangleID; j++)
			{
				if (func(j))
				{
					array[j] = 0;
					empty.Contain(j);
				}
				else
				{
					array[j] = byte.MaxValue;
				}
			}
			IL_EB:
			List<int> list = new List<int>(maxTriangleID / 10);
			List<int> list2 = new List<int>(maxTriangleID / 10);
			IEnumerable<int> enumerable2;
			if (this.FilterSet == null)
			{
				IEnumerable<int> enumerable = empty;
				enumerable2 = enumerable;
			}
			else
			{
				enumerable2 = this.FilterSet;
			}
			foreach (int num2 in enumerable2)
			{
				if (array[num2] != 255)
				{
					int num3 = num2;
					if (this.SeedFilterF == null || this.SeedFilterF(num3))
					{
						list.Add(num3);
						array[num3] = 1;
						while (list.Count > 0)
						{
							int num4 = list[list.Count - 1];
							list.RemoveAt(list.Count - 1);
							array[num4] = 2;
							list2.Add(num4);
							Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(num4);
							for (int k = 0; k < 3; k++)
							{
								int num5 = triNeighbourTris[k];
								if (num5 != -1 && array[num5] == 0)
								{
									list.Add(num5);
									array[num5] = 1;
								}
							}
						}
						MeshConnectedComponents.Component component = new MeshConnectedComponents.Component
						{
							Indices = list2.ToArray()
						};
						this.Components.Add(component);
						for (int l = 0; l < component.Indices.Length; l++)
						{
							array[component.Indices[l]] = byte.MaxValue;
						}
						list2.Clear();
						list.Clear();
					}
				}
			}
		}

		public static DMesh3[] Separate(DMesh3 meshIn)
		{
			MeshConnectedComponents meshConnectedComponents = new MeshConnectedComponents(meshIn);
			meshConnectedComponents.FindConnectedT();
			meshConnectedComponents.SortByCount(false);
			DMesh3[] array = new DMesh3[meshConnectedComponents.Components.Count];
			int num = 0;
			foreach (MeshConnectedComponents.Component component in meshConnectedComponents.Components)
			{
				DSubmesh3 dsubmesh = new DSubmesh3(meshIn, component.Indices);
				array[num++] = dsubmesh.SubMesh;
			}
			return array;
		}

		public static DMesh3 LargestT(DMesh3 meshIn)
		{
			MeshConnectedComponents meshConnectedComponents = new MeshConnectedComponents(meshIn);
			meshConnectedComponents.FindConnectedT();
			meshConnectedComponents.SortByCount(false);
			return new DSubmesh3(meshIn, meshConnectedComponents.Components[0].Indices).SubMesh;
		}

		public static HashSet<int> FindConnectedT(DMesh3 mesh, int tSeed)
		{
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(tSeed);
			List<int> list = new List<int>(64)
			{
				tSeed
			};
			while (list.Count > 0)
			{
				int tID = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				Index3i triNeighbourTris = mesh.GetTriNeighbourTris(tID);
				for (int i = 0; i < 3; i++)
				{
					int num = triNeighbourTris[i];
					if (num != -1 && !hashSet.Contains(num))
					{
						hashSet.Add(num);
						list.Add(num);
					}
				}
			}
			return hashSet;
		}

		public DMesh3 Mesh;

		public IEnumerable<int> FilterSet;

		public Func<int, bool> FilterF;

		public Func<int, bool> SeedFilterF;

		public List<MeshConnectedComponents.Component> Components;

		public struct Component
		{
			public int[] Indices;
		}
	}
}
