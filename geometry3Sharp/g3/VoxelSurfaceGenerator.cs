using System;
using System.Collections.Generic;

namespace g3
{
	public class VoxelSurfaceGenerator
	{
		private void append_mesh()
		{
			if (this.Meshes == null || this.Meshes.Count == 0)
			{
				this.Meshes = new List<DMesh3>();
			}
			this.cur_mesh = new DMesh3(MeshComponents.VertexNormals);
			if (this.ColorSourceF != null)
			{
				this.cur_mesh.EnableVertexColors(Colorf.White);
			}
			this.Meshes.Add(this.cur_mesh);
		}

		private void check_counts_or_append(int newV, int newT)
		{
			if (this.cur_mesh.MaxVertexID + newV < this.MaxMeshElementCount && this.cur_mesh.MaxTriangleID + newT < this.MaxMeshElementCount)
			{
				return;
			}
			this.append_mesh();
		}

		public void Generate()
		{
			this.append_mesh();
			AxisAlignedBox3i gridBounds = this.Voxels.GridBounds;
			gridBounds.Max -= Vector3i.One;
			int[] array = new int[4];
			foreach (Vector3i vector3i in this.Voxels.NonZeros())
			{
				this.check_counts_or_append(6, 2);
				Box3d unitZeroCentered = Box3d.UnitZeroCentered;
				unitZeroCentered.Center = (Vector3d)vector3i;
				int i = 0;
				while (i < 6)
				{
					Index3i v = vector3i + gIndices.GridOffsets6[i];
					if (gridBounds.Contains(v))
					{
						if (!this.SkipInteriorFaces || !this.Voxels.Get(v))
						{
							goto IL_CB;
						}
					}
					else if (this.CapAtBoundary)
					{
						goto IL_CB;
					}
					IL_1B6:
					i++;
					continue;
					IL_CB:
					int value = gIndices.BoxFaceNormals[i];
					Vector3f n = (Vector3f)((double)Math.Sign(value) * unitZeroCentered.Axis(Math.Abs(value) - 1));
					NewVertexInfo info = new NewVertexInfo(Vector3d.Zero, n);
					if (this.ColorSourceF != null)
					{
						info.c = this.ColorSourceF(vector3i);
						info.bHaveC = true;
					}
					for (int j = 0; j < 4; j++)
					{
						info.v = unitZeroCentered.Corner(gIndices.BoxFaces[i, j]);
						array[j] = this.cur_mesh.AppendVertex(info);
					}
					Index3i tv = new Index3i(array[0], array[1], array[2], this.Clockwise);
					Index3i tv2 = new Index3i(array[0], array[2], array[3], this.Clockwise);
					this.cur_mesh.AppendTriangle(tv, -1);
					this.cur_mesh.AppendTriangle(tv2, -1);
					goto IL_1B6;
				}
			}
		}

		public IBinaryVoxelGrid Voxels;

		public bool SkipInteriorFaces = true;

		public bool CapAtBoundary = true;

		public bool Clockwise;

		public Func<Vector3i, Colorf> ColorSourceF;

		public int MaxMeshElementCount = int.MaxValue;

		public List<DMesh3> Meshes;

		private DMesh3 cur_mesh;
	}
}
