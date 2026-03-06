using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	public class SimpleQuadMesh
	{
		public SimpleQuadMesh()
		{
			this.Initialize(true, true, true, true);
		}

		public void Initialize(bool bWantNormals = true, bool bWantColors = true, bool bWantUVs = true, bool bWantFaceGroups = true)
		{
			this.Vertices = new DVector<double>();
			this.Normals = (bWantNormals ? new DVector<float>() : null);
			this.Colors = (bWantColors ? new DVector<float>() : null);
			this.UVs = (bWantUVs ? new DVector<float>() : null);
			this.Quads = new DVector<int>();
			this.FaceGroups = (bWantFaceGroups ? new DVector<int>() : null);
		}

		public MeshComponents Components
		{
			get
			{
				MeshComponents meshComponents = MeshComponents.None;
				if (this.Normals != null)
				{
					meshComponents |= MeshComponents.VertexNormals;
				}
				if (this.Colors != null)
				{
					meshComponents |= MeshComponents.VertexColors;
				}
				if (this.UVs != null)
				{
					meshComponents |= MeshComponents.VertexUVs;
				}
				if (this.FaceGroups != null)
				{
					meshComponents |= MeshComponents.FaceGroups;
				}
				return meshComponents;
			}
		}

		public int AppendVertex(double x, double y, double z)
		{
			int result = this.Vertices.Length / 3;
			if (this.HasVertexNormals)
			{
				this.Normals.Add(0f);
				this.Normals.Add(1f);
				this.Normals.Add(0f);
			}
			if (this.HasVertexColors)
			{
				this.Colors.Add(1f);
				this.Colors.Add(1f);
				this.Colors.Add(1f);
			}
			if (this.HasVertexUVs)
			{
				this.UVs.Add(0f);
				this.UVs.Add(0f);
			}
			this.Vertices.Add(x);
			this.Vertices.Add(y);
			this.Vertices.Add(z);
			return result;
		}

		public int AppendVertex(Vector3d v)
		{
			return this.AppendVertex(v.x, v.y, v.z);
		}

		public int AppendVertex(NewVertexInfo info)
		{
			int result = this.Vertices.Length / 3;
			if (info.bHaveN && this.HasVertexNormals)
			{
				this.Normals.Add(info.n[0]);
				this.Normals.Add(info.n[1]);
				this.Normals.Add(info.n[2]);
			}
			else if (this.HasVertexNormals)
			{
				this.Normals.Add(0f);
				this.Normals.Add(1f);
				this.Normals.Add(0f);
			}
			if (info.bHaveC && this.HasVertexColors)
			{
				this.Colors.Add(info.c[0]);
				this.Colors.Add(info.c[1]);
				this.Colors.Add(info.c[2]);
			}
			else if (this.HasVertexColors)
			{
				this.Colors.Add(1f);
				this.Colors.Add(1f);
				this.Colors.Add(1f);
			}
			if (info.bHaveUV && this.HasVertexUVs)
			{
				this.UVs.Add(info.uv[0]);
				this.UVs.Add(info.uv[1]);
			}
			else if (this.HasVertexUVs)
			{
				this.UVs.Add(0f);
				this.UVs.Add(0f);
			}
			this.Vertices.Add(info.v[0]);
			this.Vertices.Add(info.v[1]);
			this.Vertices.Add(info.v[2]);
			return result;
		}

		public int AppendQuad(int i, int j, int k, int l, int g = -1)
		{
			int result = this.Quads.Length / 4;
			if (this.HasFaceGroups)
			{
				this.FaceGroups.Add((g == -1) ? 0 : g);
			}
			this.Quads.Add(i);
			this.Quads.Add(j);
			this.Quads.Add(k);
			this.Quads.Add(l);
			return result;
		}

		public int VertexCount
		{
			get
			{
				return this.Vertices.Length / 3;
			}
		}

		public int QuadCount
		{
			get
			{
				return this.Quads.Length / 4;
			}
		}

		public int MaxVertexID
		{
			get
			{
				return this.VertexCount;
			}
		}

		public int MaxQuadID
		{
			get
			{
				return this.QuadCount;
			}
		}

		public bool IsVertex(int vID)
		{
			return vID * 3 < this.Vertices.Length;
		}

		public bool IsQuad(int qID)
		{
			return qID * 4 < this.Quads.Length;
		}

		public bool HasVertexColors
		{
			get
			{
				return this.Colors != null && this.Colors.Length == this.Vertices.Length;
			}
		}

		public bool HasVertexNormals
		{
			get
			{
				return this.Normals != null && this.Normals.Length == this.Vertices.Length;
			}
		}

		public bool HasVertexUVs
		{
			get
			{
				return this.UVs != null && this.UVs.Length / 2 == this.Vertices.Length / 3;
			}
		}

		public Vector3d GetVertex(int i)
		{
			return new Vector3d(this.Vertices[3 * i], this.Vertices[3 * i + 1], this.Vertices[3 * i + 2]);
		}

		public Vector3f GetVertexNormal(int i)
		{
			return new Vector3f(this.Normals[3 * i], this.Normals[3 * i + 1], this.Normals[3 * i + 2]);
		}

		public Vector3f GetVertexColor(int i)
		{
			return new Vector3f(this.Colors[3 * i], this.Colors[3 * i + 1], this.Colors[3 * i + 2]);
		}

		public Vector2f GetVertexUV(int i)
		{
			return new Vector2f(this.UVs[2 * i], this.UVs[2 * i + 1]);
		}

		public NewVertexInfo GetVertexAll(int i)
		{
			NewVertexInfo result = default(NewVertexInfo);
			result.v = this.GetVertex(i);
			if (this.HasVertexNormals)
			{
				result.bHaveN = true;
				result.n = this.GetVertexNormal(i);
			}
			else
			{
				result.bHaveN = false;
			}
			if (this.HasVertexColors)
			{
				result.bHaveC = true;
				result.c = this.GetVertexColor(i);
			}
			else
			{
				result.bHaveC = false;
			}
			if (this.HasVertexUVs)
			{
				result.bHaveUV = true;
				result.uv = this.GetVertexUV(i);
			}
			else
			{
				result.bHaveUV = false;
			}
			return result;
		}

		public bool HasFaceGroups
		{
			get
			{
				return this.FaceGroups != null && this.FaceGroups.Length == this.Quads.Length / 4;
			}
		}

		public Index4i GetQuad(int i)
		{
			return new Index4i(this.Quads[4 * i], this.Quads[4 * i + 1], this.Quads[4 * i + 2], this.Quads[4 * i + 3]);
		}

		public int GetFaceGroup(int i)
		{
			return this.FaceGroups[i];
		}

		public IEnumerable<Vector3d> VerticesItr()
		{
			int N = this.VertexCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return new Vector3d(this.Vertices[3 * i], this.Vertices[3 * i + 1], this.Vertices[3 * i + 2]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<Vector3f> NormalsItr()
		{
			int N = this.VertexCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return new Vector3f(this.Normals[3 * i], this.Normals[3 * i + 1], this.Normals[3 * i + 2]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<Vector3f> ColorsItr()
		{
			int N = this.VertexCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return new Vector3f(this.Colors[3 * i], this.Colors[3 * i + 1], this.Colors[3 * i + 2]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<Vector2f> UVsItr()
		{
			int N = this.VertexCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return new Vector2f(this.UVs[2 * i], this.UVs[2 * i + 1]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<Index4i> QuadsItr()
		{
			int N = this.QuadCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return new Index4i(this.Quads[4 * i], this.Quads[4 * i + 1], this.Quads[4 * i + 2], this.Quads[4 * i + 3]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<int> FaceGroupsItr()
		{
			int N = this.QuadCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return this.FaceGroups[i];
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<int> VertexIndices()
		{
			int N = this.VertexCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return i;
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<int> QuadIndices()
		{
			int N = this.QuadCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return i;
				num = i + 1;
			}
			yield break;
		}

		public void SetVertex(int i, Vector3d v)
		{
			this.Vertices[3 * i] = v.x;
			this.Vertices[3 * i + 1] = v.y;
			this.Vertices[3 * i + 2] = v.z;
		}

		public void SetVertexNormal(int i, Vector3f n)
		{
			this.Normals[3 * i] = n.x;
			this.Normals[3 * i + 1] = n.y;
			this.Normals[3 * i + 2] = n.z;
		}

		public void SetVertexColor(int i, Vector3f c)
		{
			this.Colors[3 * i] = c.x;
			this.Colors[3 * i + 1] = c.y;
			this.Colors[3 * i + 2] = c.z;
		}

		public void SetVertexUV(int i, Vector2f uv)
		{
			this.UVs[2 * i] = uv.x;
			this.UVs[2 * i + 1] = uv.y;
		}

		public double[] GetVertexArray()
		{
			return this.Vertices.GetBuffer();
		}

		public float[] GetVertexArrayFloat()
		{
			float[] array = new float[this.Vertices.Length];
			for (int i = 0; i < this.Vertices.Length; i++)
			{
				array[i] = (float)this.Vertices[i];
			}
			return array;
		}

		public float[] GetVertexNormalArray()
		{
			if (!this.HasVertexNormals)
			{
				return null;
			}
			return this.Normals.GetBuffer();
		}

		public float[] GetVertexColorArray()
		{
			if (!this.HasVertexColors)
			{
				return null;
			}
			return this.Colors.GetBuffer();
		}

		public float[] GetVertexUVArray()
		{
			if (!this.HasVertexUVs)
			{
				return null;
			}
			return this.UVs.GetBuffer();
		}

		public int[] GetQuadArray()
		{
			return this.Quads.GetBuffer();
		}

		public int[] GetFaceGroupsArray()
		{
			if (!this.HasFaceGroups)
			{
				return null;
			}
			return this.FaceGroups.GetBuffer();
		}

		public static IOWriteResult WriteOBJ(SimpleQuadMesh mesh, string sPath, WriteOptions options)
		{
			StreamWriter streamWriter = new StreamWriter(sPath);
			if (streamWriter.BaseStream == null)
			{
				return new IOWriteResult(IOCode.FileAccessError, "Could not open file " + sPath + " for writing");
			}
			bool flag = options.bPerVertexColors && mesh.HasVertexColors;
			bool flag2 = options.bPerVertexNormals && mesh.HasVertexNormals;
			bool flag3 = options.bPerVertexUVs && mesh.HasVertexUVs;
			if (mesh.UVs != null)
			{
				flag3 = false;
			}
			int[] array = new int[mesh.MaxVertexID];
			int num = 1;
			foreach (int num2 in mesh.VertexIndices())
			{
				array[num2] = num++;
				Vector3d vertex = mesh.GetVertex(num2);
				if (flag)
				{
					Vector3d vector3d = mesh.GetVertexColor(num2);
					streamWriter.WriteLine("v {0} {1} {2} {3:F8} {4:F8} {5:F8}", new object[]
					{
						vertex[0],
						vertex[1],
						vertex[2],
						vector3d[0],
						vector3d[1],
						vector3d[2]
					});
				}
				else
				{
					streamWriter.WriteLine("v {0} {1} {2}", vertex[0], vertex[1], vertex[2]);
				}
				if (flag2)
				{
					Vector3d vector3d2 = mesh.GetVertexNormal(num2);
					streamWriter.WriteLine("vn {0:F10} {1:F10} {2:F10}", vector3d2[0], vector3d2[1], vector3d2[2]);
				}
				if (flag3)
				{
					Vector2f vertexUV = mesh.GetVertexUV(num2);
					streamWriter.WriteLine("vt {0:F10} {1:F10}", vertexUV.x, vertexUV.y);
				}
			}
			foreach (int i in mesh.QuadIndices())
			{
				Index4i quad = mesh.GetQuad(i);
				quad[0] = array[quad[0]];
				quad[1] = array[quad[1]];
				quad[2] = array[quad[2]];
				quad[3] = array[quad[3]];
				SimpleQuadMesh.write_quad(streamWriter, ref quad, flag2, flag3, ref quad);
			}
			streamWriter.Close();
			return IOWriteResult.Ok;
		}

		private static void write_quad(TextWriter writer, ref Index4i q, bool bNormals, bool bUVs, ref Index4i tuv)
		{
			if (!bNormals && !bUVs)
			{
				writer.WriteLine("f {0} {1} {2} {3}", new object[]
				{
					q[0],
					q[1],
					q[2],
					q[3]
				});
				return;
			}
			if (bNormals && !bUVs)
			{
				writer.WriteLine("f {0}//{0} {1}//{1} {2}//{2} {3}//{3}", new object[]
				{
					q[0],
					q[1],
					q[2],
					q[3]
				});
				return;
			}
			if (!bNormals && bUVs)
			{
				writer.WriteLine("f {0}/{4} {1}/{5} {2}/{6} {3}/{7}", new object[]
				{
					q[0],
					q[1],
					q[2],
					q[3],
					tuv[0],
					tuv[1],
					tuv[2],
					tuv[3]
				});
				return;
			}
			writer.WriteLine("f {0}/{4}/{0} {1}/{5}/{1} {2}/{6}/{2} {3}/{7}/{3}", new object[]
			{
				q[0],
				q[1],
				q[2],
				q[3],
				tuv[0],
				tuv[1],
				tuv[2],
				tuv[3]
			});
		}

		public DVector<double> Vertices;

		public DVector<float> Normals;

		public DVector<float> Colors;

		public DVector<float> UVs;

		public DVector<int> Quads;

		public DVector<int> FaceGroups;
	}
}
