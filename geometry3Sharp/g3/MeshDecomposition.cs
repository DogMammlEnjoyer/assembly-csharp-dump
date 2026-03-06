using System;
using System.Collections;

namespace g3
{
	public class MeshDecomposition
	{
		public int MaxComponentSize { get; set; }

		public IMeshComponentManager Manager { get; set; }

		public MeshDecomposition(DMesh3 mesh, IMeshComponentManager manager)
		{
			this.MaxComponentSize = 62000;
			this.mesh = mesh;
			this.Manager = manager;
		}

		public void BuildLinear()
		{
			int maxVertexID = this.mesh.MaxVertexID;
			if (this.TrackVertexMapping)
			{
				this.mapTo = new Index2i[maxVertexID];
				for (int i = 0; i < maxVertexID; i++)
				{
					this.mapTo[i] = Index2i.Zero;
				}
				this.mapToMulti = new DVector<int>();
			}
			int[] mapToCur = new int[maxVertexID];
			Array.Clear(mapToCur, 0, mapToCur.Length);
			int maxTriangleID = this.mesh.MaxTriangleID;
			int[] cur_subt = new int[this.MaxComponentSize];
			int subti = 0;
			int subi = 1;
			int[] cur_subv = new int[this.MaxComponentSize];
			BitArray vert_bits = new BitArray(this.mesh.MaxVertexID);
			int subvcount = 0;
			Action action = delegate()
			{
				MeshDecomposition <>4__this = this;
				int subi = subi;
				subi++;
				Index2i index2i;
				int length;
				MeshDecomposition.Component c = <>4__this.extract_submesh(subi, cur_subt, subti, mapToCur, cur_subv, out index2i, out length);
				this.Manager.AddComponent(c);
				Array.Clear(cur_subt, 0, subti);
				subti = 0;
				Array.Clear(mapToCur, index2i.a, index2i.b - index2i.a + 1);
				Array.Clear(cur_subv, 0, length);
				subvcount = 0;
				vert_bits.SetAll(false);
			};
			int[] tri_order_by_axis_sort = this.get_tri_order_by_axis_sort();
			int num = tri_order_by_axis_sort.Length;
			for (int j = 0; j < num; j++)
			{
				int num2 = tri_order_by_axis_sort[j];
				Index3i triangle = this.mesh.GetTriangle(num2);
				int num3;
				if (!vert_bits[triangle.a])
				{
					vert_bits[triangle.a] = true;
					num3 = subvcount;
					subvcount = num3 + 1;
				}
				if (!vert_bits[triangle.b])
				{
					vert_bits[triangle.b] = true;
					num3 = subvcount;
					subvcount = num3 + 1;
				}
				if (!vert_bits[triangle.c])
				{
					vert_bits[triangle.c] = true;
					num3 = subvcount;
					subvcount = num3 + 1;
				}
				int[] cur_subt2 = cur_subt;
				num3 = subti;
				subti = num3 + 1;
				cur_subt2[num3] = num2;
				if (subti == this.MaxComponentSize || subvcount > this.MaxComponentSize - 3)
				{
					action();
				}
			}
			if (subti > 0)
			{
				action();
			}
		}

		private int[] get_tri_order_by_axis_sort()
		{
			int num = 0;
			int[] array = new int[this.mesh.TriangleCount];
			int maxTriangleID = this.mesh.MaxTriangleID;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (this.mesh.IsTriangle(i))
				{
					array[num++] = i;
				}
			}
			Vector3d[] centroids = new Vector3d[this.mesh.MaxTriangleID];
			gParallel.ForEach<int>(this.mesh.TriangleIndices(), delegate(int ti)
			{
				if (this.mesh.IsTriangle(ti))
				{
					centroids[ti] = this.mesh.GetTriCentroid(ti);
				}
			});
			Array.Sort<int>(array, delegate(int t0, int t1)
			{
				double x = centroids[t0].x;
				double x2 = centroids[t1].x;
				if (x == x2)
				{
					return 0;
				}
				if (x >= x2)
				{
					return 1;
				}
				return -1;
			});
			return array;
		}

		private MeshDecomposition.Component extract_submesh(int submesh_index, int[] subt, int Nt, int[] mapToCur, int[] subv, out Index2i mapRange, out int max_subv)
		{
			int num = 0;
			MeshDecomposition.Component component = default(MeshDecomposition.Component);
			component.id = submesh_index;
			component.triangles = new int[Nt * 3];
			component.tri_count = Nt;
			mapRange = new Index2i(int.MaxValue, int.MinValue);
			for (int i = 0; i < Nt; i++)
			{
				int tID = subt[i];
				Index3i triangle = this.mesh.GetTriangle(tID);
				for (int j = 0; j < 3; j++)
				{
					int num2 = triangle[j];
					if (mapToCur[num2] == 0)
					{
						mapToCur[num2] = num + 1;
						subv[num] = num2;
						if (num2 < mapRange.a)
						{
							mapRange.a = num2;
						}
						else if (num2 > mapRange.b)
						{
							mapRange.b = num2;
						}
						if (this.TrackVertexMapping)
						{
							this.add_submesh_mapv(num2, component.id, num);
						}
						num++;
					}
					component.triangles[3 * i + j] = mapToCur[num2] - 1;
				}
			}
			component.source_vertices = new int[num];
			Array.Copy(subv, component.source_vertices, num);
			max_subv = num;
			return component;
		}

		private void add_submesh_mapv(int orig_vid, int submesh_i, int submesh_vid)
		{
			if (this.mapTo[orig_vid].a == 0)
			{
				this.mapTo[orig_vid].a = submesh_i;
				this.mapTo[orig_vid].b = submesh_vid;
				return;
			}
			if (this.mapTo[orig_vid].a > 0)
			{
				int size = this.mapToMulti.size;
				this.mapToMulti.push_back(this.mapTo[orig_vid].a);
				this.mapToMulti.push_back(this.mapTo[orig_vid].b);
				this.mapToMulti.push_back(-1);
				int size2 = this.mapToMulti.size;
				this.mapToMulti.push_back(submesh_i);
				this.mapToMulti.push_back(submesh_vid);
				this.mapToMulti.push_back(size);
				this.mapTo[orig_vid].a = -2;
				this.mapTo[orig_vid].b = size2;
				return;
			}
			Index2i[] array = this.mapTo;
			array[orig_vid].a = array[orig_vid].a - 1;
			int b = this.mapTo[orig_vid].b;
			int size3 = this.mapToMulti.size;
			this.mapToMulti.push_back(submesh_i);
			this.mapToMulti.push_back(submesh_vid);
			this.mapToMulti.push_back(b);
			this.mapTo[orig_vid].b = size3;
		}

		private DMesh3 mesh;

		public bool TrackVertexMapping = true;

		private Index2i[] mapTo;

		private DVector<int> mapToMulti;

		public struct Component
		{
			public int id;

			public int[] triangles;

			public int tri_count;

			public int[] source_vertices;
		}
	}
}
