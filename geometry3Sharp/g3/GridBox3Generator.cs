using System;
using System.Collections.Generic;

namespace g3
{
	public class GridBox3Generator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			int N = (this.EdgeVertices > 1) ? this.EdgeVertices : 2;
			int num = N - 2;
			int num2 = N - 1;
			int num3 = N * N;
			this.vertices = new VectorArray3d(this.NoSharedVertices ? (num3 * 6) : (8 + num * 12 + num * num * 6), false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			this.triangles = new IndexArray3i(2 * num2 * num2 * 6);
			this.groups = new int[this.triangles.Count];
			Vector3d[] array = this.Box.ComputeVertices();
			int vi = 0;
			int num4 = 0;
			if (this.NoSharedVertices)
			{
				for (int i = 0; i < 6; i++)
				{
					Vector3d vector3d = array[gIndices.BoxFaces[i, 0]];
					Vector3d vector3d2 = array[gIndices.BoxFaces[i, 1]];
					Vector3d vector3d3 = array[gIndices.BoxFaces[i, 2]];
					Vector3d vector3d4 = array[gIndices.BoxFaces[i, 3]];
					Vector3f value = (float)Math.Sign(gIndices.BoxFaceNormals[i]) * (Vector3f)this.Box.Axis(Math.Abs(gIndices.BoxFaceNormals[i]) - 1);
					int vi4 = vi;
					for (int j = 0; j < N; j++)
					{
						double num5 = (double)j / (double)(N - 1);
						for (int k = 0; k < N; k++)
						{
							double num6 = (double)k / (double)(N - 1);
							this.normals[vi] = value;
							this.uv[vi] = new Vector2f(num6, num5);
							VectorArray3d vertices = this.vertices;
							int vi2 = vi;
							vi = vi2 + 1;
							vertices[vi2] = base.bilerp(ref vector3d, ref vector3d2, ref vector3d3, ref vector3d4, num6, num5);
						}
					}
					for (int l = 0; l < num2; l++)
					{
						for (int m = 0; m < num2; m++)
						{
							int num7 = vi4 + l * N + m;
							int num8 = vi4 + (l + 1) * N + m;
							int b3 = num7 + 1;
							int num9 = num8 + 1;
							this.groups[num4] = i;
							this.triangles.Set(num4++, num7, b3, num9, this.Clockwise);
							this.groups[num4] = i;
							this.triangles.Set(num4++, num7, num9, num8, this.Clockwise);
						}
					}
				}
			}
			else
			{
				Vector3i[] array2 = new Vector3i[array.Length];
				for (int n = 0; n < array.Length; n++)
				{
					Vector3d vector3d5 = array[n] - this.Box.Center;
					array2[n] = new Vector3i((vector3d5.x < 0.0) ? 0 : (N - 1), (vector3d5.y < 0.0) ? 0 : (N - 1), (vector3d5.z < 0.0) ? 0 : (N - 1));
				}
				int[] array3 = new int[num3];
				Dictionary<Vector3i, int> edgeVerts = new Dictionary<Vector3i, int>();
				Action<Vector3d, Vector3d, Vector3i, Vector3i> <>9__0;
				for (int num10 = 0; num10 < 6; num10++)
				{
					int num11 = gIndices.BoxFaces[num10, 0];
					int num12 = gIndices.BoxFaces[num10, 1];
					int num13 = gIndices.BoxFaces[num10, 2];
					int num14 = gIndices.BoxFaces[num10, 3];
					Vector3d vector3d6 = array[num11];
					Vector3i vector3i = array2[num11];
					Vector3d vector3d7 = array[num12];
					Vector3i vector3i2 = array2[num12];
					Vector3d vector3d8 = array[num13];
					Vector3i vector3i3 = array2[num13];
					Vector3d vector3d9 = array[num14];
					Vector3i vector3i4 = array2[num14];
					Action<Vector3d, Vector3d, Vector3i, Vector3i> action;
					if ((action = <>9__0) == null)
					{
						action = (<>9__0 = delegate(Vector3d a, Vector3d b, Vector3i ai, Vector3i bi)
						{
							for (int num30 = 0; num30 < N; num30++)
							{
								double t = (double)num30 / (double)(N - 1);
								Vector3i key2 = this.lerp(ref ai, ref bi, t);
								if (!edgeVerts.ContainsKey(key2))
								{
									Vector3d value4 = Vector3d.Lerp(ref a, ref b, t);
									this.normals[vi] = (Vector3f)value4.Normalized;
									this.uv[vi] = Vector2f.Zero;
									edgeVerts[key2] = vi;
									VectorArray3d vertices2 = this.vertices;
									int vi3 = vi;
									vi = vi3 + 1;
									vertices2[vi3] = value4;
								}
							}
						});
					}
					Action<Vector3d, Vector3d, Vector3i, Vector3i> action2 = action;
					action2(vector3d6, vector3d7, vector3i, vector3i2);
					action2(vector3d7, vector3d8, vector3i2, vector3i3);
					action2(vector3d8, vector3d9, vector3i3, vector3i4);
					action2(vector3d9, vector3d6, vector3i4, vector3i);
				}
				for (int num15 = 0; num15 < 6; num15++)
				{
					int num16 = gIndices.BoxFaces[num15, 0];
					int num17 = gIndices.BoxFaces[num15, 1];
					int num18 = gIndices.BoxFaces[num15, 2];
					int num19 = gIndices.BoxFaces[num15, 3];
					Vector3d vector3d10 = array[num16];
					Vector3i vector3i5 = array2[num16];
					Vector3d vector3d11 = array[num17];
					Vector3i vector3i6 = array2[num17];
					Vector3d vector3d12 = array[num18];
					Vector3i vector3i7 = array2[num18];
					Vector3d vector3d13 = array[num19];
					Vector3i vector3i8 = array2[num19];
					Vector3f value2 = (float)Math.Sign(gIndices.BoxFaceNormals[num15]) * (Vector3f)this.Box.Axis(Math.Abs(gIndices.BoxFaceNormals[num15]) - 1);
					for (int num20 = 0; num20 < N; num20++)
					{
						double num21 = (double)num20 / (double)(N - 1);
						for (int num22 = 0; num22 < N; num22++)
						{
							double num23 = (double)num22 / (double)(N - 1);
							Vector3i key = base.bilerp(ref vector3i5, ref vector3i6, ref vector3i7, ref vector3i8, num23, num21);
							int num24;
							if (!edgeVerts.TryGetValue(key, out num24))
							{
								Vector3d value3 = base.bilerp(ref vector3d10, ref vector3d11, ref vector3d12, ref vector3d13, num23, num21);
								int vi2 = vi;
								vi = vi2 + 1;
								num24 = vi2;
								this.normals[num24] = value2;
								this.uv[num24] = new Vector2f(num23, num21);
								this.vertices[num24] = value3;
							}
							array3[num20 * N + num22] = num24;
						}
					}
					for (int num25 = 0; num25 < num2; num25++)
					{
						int num26 = num25 + 1;
						for (int num27 = 0; num27 < num2; num27++)
						{
							int num28 = num27 + 1;
							int a2 = array3[num25 * N + num27];
							int b2 = array3[num25 * N + num28];
							int num29 = array3[num26 * N + num28];
							int c = array3[num26 * N + num27];
							this.groups[num4] = num15;
							this.triangles.Set(num4++, a2, b2, num29, this.Clockwise);
							this.groups[num4] = num15;
							this.triangles.Set(num4++, a2, num29, c, this.Clockwise);
						}
					}
				}
			}
			return this;
		}

		public Box3d Box = Box3d.UnitZeroCentered;

		public int EdgeVertices = 8;

		public bool NoSharedVertices;
	}
}
