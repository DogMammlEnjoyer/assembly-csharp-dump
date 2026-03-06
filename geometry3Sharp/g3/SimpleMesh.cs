using System;
using System.Collections.Generic;

namespace g3
{
	public class SimpleMesh : IDeformableMesh, IMesh, IPointSet
	{
		public SimpleMesh()
		{
			this.Initialize(true, true, true, true);
		}

		public SimpleMesh(IMesh copy)
		{
			this.Initialize(copy.HasVertexNormals, copy.HasVertexColors, copy.HasVertexUVs, copy.HasTriangleGroups);
			int[] array = new int[copy.MaxVertexID];
			foreach (int num in copy.VertexIndices())
			{
				NewVertexInfo vertexAll = copy.GetVertexAll(num);
				int num2 = this.AppendVertex(vertexAll);
				array[num] = num2;
			}
			foreach (int i in copy.TriangleIndices())
			{
				Index3i triangle = copy.GetTriangle(i);
				triangle[0] = array[triangle[0]];
				triangle[1] = array[triangle[1]];
				triangle[2] = array[triangle[2]];
				if (copy.HasTriangleGroups)
				{
					this.AppendTriangle(triangle[0], triangle[1], triangle[2], copy.GetTriangleGroup(i));
				}
				else
				{
					this.AppendTriangle(triangle[0], triangle[1], triangle[2], -1);
				}
			}
		}

		public void Initialize(bool bWantNormals = true, bool bWantColors = true, bool bWantUVs = true, bool bWantFaceGroups = true)
		{
			this.Vertices = new DVector<double>();
			this.Normals = (bWantNormals ? new DVector<float>() : null);
			this.Colors = (bWantColors ? new DVector<float>() : null);
			this.UVs = (bWantUVs ? new DVector<float>() : null);
			this.Triangles = new DVector<int>();
			this.FaceGroups = (bWantFaceGroups ? new DVector<int>() : null);
		}

		public void Initialize(VectorArray3d v, VectorArray3i t, VectorArray3f n = null, VectorArray3f c = null, VectorArray2f uv = null, int[] g = null)
		{
			this.Vertices = new DVector<double>(v);
			this.Triangles = new DVector<int>(t);
			this.Normals = (this.Colors = (this.UVs = null));
			this.FaceGroups = null;
			if (n != null)
			{
				this.Normals = new DVector<float>(n);
			}
			if (c != null)
			{
				this.Colors = new DVector<float>(c);
			}
			if (uv != null)
			{
				this.UVs = new DVector<float>(uv);
			}
			if (g != null)
			{
				this.FaceGroups = new DVector<int>(g);
			}
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

		public int Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		private void updateTimeStamp()
		{
			this.timestamp++;
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
			this.updateTimeStamp();
			return result;
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
			this.updateTimeStamp();
			return result;
		}

		public void AppendVertices(VectorArray3d v, VectorArray3f n = null, VectorArray3f c = null, VectorArray2f uv = null)
		{
			bool hasVertexNormals = this.HasVertexNormals;
			bool hasVertexColors = this.HasVertexColors;
			bool hasVertexUVs = this.HasVertexUVs;
			this.Vertices.Add(v.array);
			if (n != null && hasVertexNormals)
			{
				this.Normals.Add(n.array);
			}
			else if (hasVertexNormals)
			{
				DVector<float> normals = this.Normals;
				float[] array = new float[3];
				array[1] = 1f;
				normals.Add(array, v.Count);
			}
			if (c != null && hasVertexColors)
			{
				this.Colors.Add(c.array);
			}
			else if (hasVertexColors)
			{
				this.Colors.Add(new float[]
				{
					1f,
					1f,
					1f
				}, v.Count);
			}
			if (uv != null && hasVertexUVs)
			{
				this.UVs.Add(uv.array);
			}
			else if (hasVertexUVs)
			{
				this.UVs.Add(new float[2], v.Count);
			}
			this.updateTimeStamp();
		}

		public int AppendTriangle(int i, int j, int k, int g = -1)
		{
			int result = this.Triangles.Length / 3;
			if (this.HasTriangleGroups)
			{
				this.FaceGroups.Add((g == -1) ? 0 : g);
			}
			this.Triangles.Add(i);
			this.Triangles.Add(j);
			this.Triangles.Add(k);
			this.updateTimeStamp();
			return result;
		}

		public void AppendTriangles(int[] vTriangles, int[] vertexMap, int g = -1)
		{
			for (int i = 0; i < vTriangles.Length; i++)
			{
				this.Triangles.Add(vertexMap[vTriangles[i]]);
			}
			if (this.HasTriangleGroups)
			{
				for (int j = 0; j < vTriangles.Length / 3; j++)
				{
					this.FaceGroups.Add((g == -1) ? 0 : g);
				}
			}
			this.updateTimeStamp();
		}

		public void AppendTriangles(IndexArray3i t, int[] groups = null)
		{
			this.Triangles.Add(t.array);
			if (this.HasTriangleGroups)
			{
				if (groups != null)
				{
					this.FaceGroups.Add(groups);
				}
				else
				{
					this.FaceGroups.Add(0, t.Count);
				}
			}
			this.updateTimeStamp();
		}

		public void Translate(double tx, double ty, double tz)
		{
			int vertexCount = this.VertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				DVector<double> vertices = this.Vertices;
				int i2 = 3 * i;
				vertices[i2] += tx;
				vertices = this.Vertices;
				i2 = 3 * i + 1;
				vertices[i2] += ty;
				vertices = this.Vertices;
				i2 = 3 * i + 2;
				vertices[i2] += tz;
			}
			this.updateTimeStamp();
		}

		public void Scale(double sx, double sy, double sz)
		{
			int vertexCount = this.VertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				DVector<double> vertices = this.Vertices;
				int i2 = 3 * i;
				vertices[i2] *= sx;
				vertices = this.Vertices;
				i2 = 3 * i + 1;
				vertices[i2] *= sy;
				vertices = this.Vertices;
				i2 = 3 * i + 2;
				vertices[i2] *= sz;
			}
			this.updateTimeStamp();
		}

		public void Scale(double s)
		{
			this.Scale(s, s, s);
			this.updateTimeStamp();
		}

		public int VertexCount
		{
			get
			{
				return this.Vertices.Length / 3;
			}
		}

		public int TriangleCount
		{
			get
			{
				return this.Triangles.Length / 3;
			}
		}

		public int MaxVertexID
		{
			get
			{
				return this.VertexCount;
			}
		}

		public int MaxTriangleID
		{
			get
			{
				return this.TriangleCount;
			}
		}

		public bool IsVertex(int vID)
		{
			return vID * 3 < this.Vertices.Length;
		}

		public bool IsTriangle(int tID)
		{
			return tID * 3 < this.Triangles.Length;
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

		public bool HasTriangleGroups
		{
			get
			{
				return this.FaceGroups != null && this.FaceGroups.Length == this.Triangles.Length / 3;
			}
		}

		public Index3i GetTriangle(int i)
		{
			return new Index3i(this.Triangles[3 * i], this.Triangles[3 * i + 1], this.Triangles[3 * i + 2]);
		}

		public int GetTriangleGroup(int i)
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

		public IEnumerable<Index3i> TrianglesItr()
		{
			int N = this.TriangleCount;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return new Index3i(this.Triangles[3 * i], this.Triangles[3 * i + 1], this.Triangles[3 * i + 2]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<int> TriangleGroupsItr()
		{
			int N = this.TriangleCount;
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

		public IEnumerable<int> TriangleIndices()
		{
			int N = this.TriangleCount;
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
			this.updateTimeStamp();
		}

		public void SetVertexNormal(int i, Vector3f n)
		{
			this.Normals[3 * i] = n.x;
			this.Normals[3 * i + 1] = n.y;
			this.Normals[3 * i + 2] = n.z;
			this.updateTimeStamp();
		}

		public void SetVertexColor(int i, Vector3f c)
		{
			this.Colors[3 * i] = c.x;
			this.Colors[3 * i + 1] = c.y;
			this.Colors[3 * i + 2] = c.z;
			this.updateTimeStamp();
		}

		public void SetVertexUV(int i, Vector2f uv)
		{
			this.UVs[2 * i] = uv.x;
			this.UVs[2 * i + 1] = uv.y;
			this.updateTimeStamp();
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

		public int[] GetTriangleArray()
		{
			return this.Triangles.GetBuffer();
		}

		public int[] GetFaceGroupsArray()
		{
			if (!this.HasTriangleGroups)
			{
				return null;
			}
			return this.FaceGroups.GetBuffer();
		}

		public unsafe void GetVertexBuffer(double* pBuffer)
		{
			DVector<double>.FastGetBuffer(this.Vertices, pBuffer);
		}

		public unsafe void GetVertexNormalBuffer(float* pBuffer)
		{
			if (this.HasVertexNormals)
			{
				DVector<float>.FastGetBuffer(this.Normals, pBuffer);
			}
		}

		public unsafe void GetVertexColorBuffer(float* pBuffer)
		{
			if (this.HasVertexColors)
			{
				DVector<float>.FastGetBuffer(this.Colors, pBuffer);
			}
		}

		public unsafe void GetVertexUVBuffer(float* pBuffer)
		{
			if (this.HasVertexUVs)
			{
				DVector<float>.FastGetBuffer(this.UVs, pBuffer);
			}
		}

		public unsafe void GetTriangleBuffer(int* pBuffer)
		{
			DVector<int>.FastGetBuffer(this.Triangles, pBuffer);
		}

		public unsafe void GetFaceGroupsBuffer(int* pBuffer)
		{
			if (this.HasTriangleGroups)
			{
				DVector<int>.FastGetBuffer(this.FaceGroups, pBuffer);
			}
		}

		public DVector<double> Vertices;

		public DVector<float> Normals;

		public DVector<float> Colors;

		public DVector<float> UVs;

		public DVector<int> Triangles;

		public DVector<int> FaceGroups;

		private int timestamp;
	}
}
