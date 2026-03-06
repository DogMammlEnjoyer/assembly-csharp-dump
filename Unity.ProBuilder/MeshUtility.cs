using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.ProBuilder
{
	public static class MeshUtility
	{
		internal static Vertex[] GeneratePerTriangleMesh(Mesh mesh)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			Vertex[] vertices = mesh.GetVertices();
			int subMeshCount = mesh.subMeshCount;
			Vertex[] array = new Vertex[mesh.triangles.Length];
			int[][] array2 = new int[subMeshCount][];
			int num = 0;
			for (int i = 0; i < subMeshCount; i++)
			{
				array2[i] = mesh.GetTriangles(i);
				int num2 = array2[i].Length;
				for (int j = 0; j < num2; j++)
				{
					array[num++] = new Vertex(vertices[array2[i][j]]);
					array2[i][j] = num - 1;
				}
			}
			Vertex.SetMesh(mesh, array);
			mesh.subMeshCount = subMeshCount;
			for (int k = 0; k < subMeshCount; k++)
			{
				mesh.SetTriangles(array2[k], k);
			}
			return array;
		}

		public static void GenerateTangent(Mesh mesh)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			int[] triangles = mesh.triangles;
			Vector3[] vertices = mesh.vertices;
			Vector2[] uv = mesh.uv;
			Vector3[] normals = mesh.normals;
			int num = triangles.Length;
			int num2 = vertices.Length;
			Vector3[] array = new Vector3[num2];
			Vector3[] array2 = new Vector3[num2];
			Vector4[] array3 = new Vector4[num2];
			for (long num3 = 0L; num3 < (long)num; num3 += 3L)
			{
				long num4 = (long)triangles[(int)(checked((IntPtr)num3))];
				long num5 = (long)triangles[(int)(checked((IntPtr)(unchecked(num3 + 1L))))];
				long num6 = (long)triangles[(int)(checked((IntPtr)(unchecked(num3 + 2L))))];
				Vector3 vector;
				Vector3 vector2;
				Vector3 vector3;
				Vector2 vector4;
				Vector2 vector5;
				Vector2 vector6;
				checked
				{
					vector = vertices[(int)((IntPtr)num4)];
					vector2 = vertices[(int)((IntPtr)num5)];
					vector3 = vertices[(int)((IntPtr)num6)];
					vector4 = uv[(int)((IntPtr)num4)];
					vector5 = uv[(int)((IntPtr)num5)];
					vector6 = uv[(int)((IntPtr)num6)];
				}
				float num7 = vector2.x - vector.x;
				float num8 = vector3.x - vector.x;
				float num9 = vector2.y - vector.y;
				float num10 = vector3.y - vector.y;
				float num11 = vector2.z - vector.z;
				float num12 = vector3.z - vector.z;
				float num13 = vector5.x - vector4.x;
				float num14 = vector6.x - vector4.x;
				float num15 = vector5.y - vector4.y;
				float num16 = vector6.y - vector4.y;
				float num17 = 1f / (num13 * num16 - num14 * num15);
				Vector3 b = new Vector3((num16 * num7 - num15 * num8) * num17, (num16 * num9 - num15 * num10) * num17, (num16 * num11 - num15 * num12) * num17);
				Vector3 b2 = new Vector3((num13 * num8 - num14 * num7) * num17, (num13 * num10 - num14 * num9) * num17, (num13 * num12 - num14 * num11) * num17);
				checked
				{
					array[(int)((IntPtr)num4)] += b;
					array[(int)((IntPtr)num5)] += b;
					array[(int)((IntPtr)num6)] += b;
					array2[(int)((IntPtr)num4)] += b2;
					array2[(int)((IntPtr)num5)] += b2;
					array2[(int)((IntPtr)num6)] += b2;
				}
			}
			for (long num18 = 0L; num18 < (long)num2; num18 += 1L)
			{
				checked
				{
					Vector3 lhs = normals[(int)((IntPtr)num18)];
					Vector3 vector7 = array[(int)((IntPtr)num18)];
					Vector3.OrthoNormalize(ref lhs, ref vector7);
					array3[(int)((IntPtr)num18)].x = vector7.x;
					array3[(int)((IntPtr)num18)].y = vector7.y;
					array3[(int)((IntPtr)num18)].z = vector7.z;
					array3[(int)((IntPtr)num18)].w = ((Vector3.Dot(Vector3.Cross(lhs, vector7), array2[(int)((IntPtr)num18)]) < 0f) ? -1f : 1f);
				}
			}
			mesh.tangents = array3;
		}

		public static Mesh DeepCopy(Mesh source)
		{
			Mesh mesh = new Mesh();
			MeshUtility.CopyTo(source, mesh);
			return mesh;
		}

		public static void CopyTo(Mesh source, Mesh destination)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			Vector3[] array = new Vector3[source.vertices.Length];
			int[][] array2 = new int[source.subMeshCount][];
			Vector2[] array3 = new Vector2[source.uv.Length];
			Vector2[] array4 = new Vector2[source.uv2.Length];
			Vector4[] array5 = new Vector4[source.tangents.Length];
			Vector3[] array6 = new Vector3[source.normals.Length];
			Color32[] array7 = new Color32[source.colors32.Length];
			Array.Copy(source.vertices, array, array.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = source.GetTriangles(i);
			}
			Array.Copy(source.uv, array3, array3.Length);
			Array.Copy(source.uv2, array4, array4.Length);
			Array.Copy(source.normals, array6, array6.Length);
			Array.Copy(source.tangents, array5, array5.Length);
			Array.Copy(source.colors32, array7, array7.Length);
			destination.Clear();
			destination.name = source.name;
			destination.vertices = array;
			destination.subMeshCount = array2.Length;
			for (int j = 0; j < array2.Length; j++)
			{
				destination.SetTriangles(array2[j], j);
			}
			destination.uv = array3;
			destination.uv2 = array4;
			destination.tangents = array5;
			destination.normals = array6;
			destination.colors32 = array7;
		}

		internal static T GetMeshChannel<T>(GameObject gameObject, Func<Mesh, T> attributeGetter) where T : IList
		{
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}
			if (attributeGetter == null)
			{
				throw new ArgumentNullException("attributeGetter");
			}
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			Mesh mesh = (component != null) ? component.sharedMesh : null;
			T t = default(T);
			if (mesh == null)
			{
				return t;
			}
			int vertexCount = mesh.vertexCount;
			MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
			Mesh mesh2 = (component2 != null) ? component2.additionalVertexStreams : null;
			if (mesh2 != null)
			{
				t = attributeGetter(mesh2);
				if (t != null && t.Count == vertexCount)
				{
					return t;
				}
			}
			t = attributeGetter(mesh);
			if (t == null || t.Count != vertexCount)
			{
				return default(T);
			}
			return t;
		}

		private static void PrintAttribute<T>(StringBuilder sb, string title, IEnumerable<T> attrib, string fmt)
		{
			sb.AppendLine("  - " + title);
			if (attrib != null && attrib.Any<T>())
			{
				using (IEnumerator<T> enumerator = attrib.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T t = enumerator.Current;
						sb.AppendLine(string.Format("    " + fmt, t));
					}
					return;
				}
			}
			sb.AppendLine("\tnull");
		}

		public static string Print(Mesh mesh)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			StringBuilder stringBuilder = new StringBuilder();
			Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			Color[] colors = mesh.colors;
			Vector4[] tangents = mesh.tangents;
			List<Vector4> list = new List<Vector4>();
			Vector2[] uv = mesh.uv2;
			List<Vector4> list2 = new List<Vector4>();
			List<Vector4> list3 = new List<Vector4>();
			mesh.GetUVs(0, list);
			mesh.GetUVs(2, list2);
			mesh.GetUVs(3, list3);
			stringBuilder.AppendLine("# Sanity Check");
			stringBuilder.AppendLine(MeshUtility.SanityCheck(mesh));
			stringBuilder.AppendLine(string.Format("# Attributes ({0})", mesh.vertexCount));
			MeshUtility.PrintAttribute<Vector3>(stringBuilder, string.Format("positions ({0})", vertices.Length), vertices, "pos: {0:F2}");
			MeshUtility.PrintAttribute<Vector3>(stringBuilder, string.Format("normals ({0})", normals.Length), normals, "nrm: {0:F2}");
			MeshUtility.PrintAttribute<Color>(stringBuilder, string.Format("colors ({0})", colors.Length), colors, "col: {0:F2}");
			MeshUtility.PrintAttribute<Vector4>(stringBuilder, string.Format("tangents ({0})", tangents.Length), tangents, "tan: {0:F2}");
			MeshUtility.PrintAttribute<Vector4>(stringBuilder, string.Format("uv0 ({0})", list.Count), list, "uv0: {0:F2}");
			MeshUtility.PrintAttribute<Vector2>(stringBuilder, string.Format("uv2 ({0})", uv.Length), uv, "uv2: {0:F2}");
			MeshUtility.PrintAttribute<Vector4>(stringBuilder, string.Format("uv3 ({0})", list2.Count), list2, "uv3: {0:F2}");
			MeshUtility.PrintAttribute<Vector4>(stringBuilder, string.Format("uv4 ({0})", list3.Count), list3, "uv4: {0:F2}");
			stringBuilder.AppendLine("# Topology");
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				MeshTopology topology = mesh.GetTopology(i);
				int[] indices = mesh.GetIndices(i);
				stringBuilder.AppendLine(string.Format("  Submesh[{0}] ({1})", i, topology));
				switch (topology)
				{
				case MeshTopology.Triangles:
					for (int j = 0; j < indices.Length; j += 3)
					{
						stringBuilder.AppendLine(string.Format("\t{0}, {1}, {2}", indices[j], indices[j + 1], indices[j + 2]));
					}
					break;
				case MeshTopology.Quads:
					for (int k = 0; k < indices.Length; k += 4)
					{
						stringBuilder.AppendLine(string.Format("\t{0}, {1}, {2}, {3}", new object[]
						{
							indices[k],
							indices[k + 1],
							indices[k + 2],
							indices[k + 3]
						}));
					}
					break;
				case MeshTopology.Lines:
					for (int l = 0; l < indices.Length; l += 2)
					{
						stringBuilder.AppendLine(string.Format("\t{0}, {1}", indices[l], indices[l + 1]));
					}
					break;
				case MeshTopology.Points:
					for (int m = 0; m < indices.Length; m++)
					{
						stringBuilder.AppendLine(string.Format("\t{0}", indices[m]));
					}
					break;
				}
			}
			return stringBuilder.ToString();
		}

		public static uint GetIndexCount(Mesh mesh)
		{
			uint num = 0U;
			if (mesh == null)
			{
				return num;
			}
			int i = 0;
			int subMeshCount = mesh.subMeshCount;
			while (i < subMeshCount)
			{
				num += mesh.GetIndexCount(i);
				i++;
			}
			return num;
		}

		public static uint GetPrimitiveCount(Mesh mesh)
		{
			uint num = 0U;
			if (mesh == null)
			{
				return num;
			}
			int i = 0;
			int subMeshCount = mesh.subMeshCount;
			while (i < subMeshCount)
			{
				if (mesh.GetTopology(i) == MeshTopology.Triangles)
				{
					num += mesh.GetIndexCount(i) / 3U;
				}
				else if (mesh.GetTopology(i) == MeshTopology.Quads)
				{
					num += mesh.GetIndexCount(i) / 4U;
				}
				i++;
			}
			return num;
		}

		public static void Compile(ProBuilderMesh probuilderMesh, Mesh targetMesh, MeshTopology preferredTopology = MeshTopology.Triangles)
		{
			if (probuilderMesh == null)
			{
				throw new ArgumentNullException("probuilderMesh");
			}
			if (targetMesh == null)
			{
				throw new ArgumentNullException("targetMesh");
			}
			targetMesh.Clear();
			targetMesh.vertices = probuilderMesh.positionsInternal;
			targetMesh.uv = probuilderMesh.texturesInternal;
			if (probuilderMesh.HasArrays(MeshArrays.Texture2))
			{
				List<Vector4> uvs = new List<Vector4>();
				probuilderMesh.GetUVs(2, uvs);
				targetMesh.SetUVs(2, uvs);
			}
			if (probuilderMesh.HasArrays(MeshArrays.Texture3))
			{
				List<Vector4> uvs2 = new List<Vector4>();
				probuilderMesh.GetUVs(3, uvs2);
				targetMesh.SetUVs(3, uvs2);
			}
			targetMesh.normals = probuilderMesh.GetNormals();
			targetMesh.tangents = probuilderMesh.GetTangents();
			if (probuilderMesh.HasArrays(MeshArrays.Color))
			{
				targetMesh.colors = probuilderMesh.colorsInternal;
			}
			int submeshCount = probuilderMesh.GetComponent<Renderer>().sharedMaterials.Length;
			Submesh[] submeshes = Submesh.GetSubmeshes(probuilderMesh.facesInternal, submeshCount, preferredTopology);
			targetMesh.subMeshCount = submeshes.Length;
			for (int i = 0; i < targetMesh.subMeshCount; i++)
			{
				targetMesh.SetIndices(submeshes[i].m_Indexes, submeshes[i].m_Topology, i, false);
			}
		}

		public static Vertex[] GetVertices(this Mesh mesh)
		{
			if (mesh == null)
			{
				return null;
			}
			int vertexCount = mesh.vertexCount;
			Vertex[] array = new Vertex[vertexCount];
			Vector3[] vertices = mesh.vertices;
			Color[] colors = mesh.colors;
			Vector3[] normals = mesh.normals;
			Vector4[] tangents = mesh.tangents;
			Vector2[] uv = mesh.uv;
			Vector2[] uv2 = mesh.uv2;
			List<Vector4> list = new List<Vector4>();
			List<Vector4> list2 = new List<Vector4>();
			mesh.GetUVs(2, list);
			mesh.GetUVs(3, list2);
			bool flag = vertices != null && vertices.Length == vertexCount;
			bool flag2 = colors != null && colors.Length == vertexCount;
			bool flag3 = normals != null && normals.Length == vertexCount;
			bool flag4 = tangents != null && tangents.Length == vertexCount;
			bool flag5 = uv != null && uv.Length == vertexCount;
			bool flag6 = uv2 != null && uv2.Length == vertexCount;
			bool flag7 = list.Count == vertexCount;
			bool flag8 = list2.Count == vertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				array[i] = new Vertex();
				if (flag)
				{
					array[i].position = vertices[i];
				}
				if (flag2)
				{
					array[i].color = colors[i];
				}
				if (flag3)
				{
					array[i].normal = normals[i];
				}
				if (flag4)
				{
					array[i].tangent = tangents[i];
				}
				if (flag5)
				{
					array[i].uv0 = uv[i];
				}
				if (flag6)
				{
					array[i].uv2 = uv2[i];
				}
				if (flag7)
				{
					array[i].uv3 = list[i];
				}
				if (flag8)
				{
					array[i].uv4 = list2[i];
				}
			}
			return array;
		}

		public static void CollapseSharedVertices(Mesh mesh, Vertex[] vertices = null)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			bool flag = vertices != null;
			if (vertices == null)
			{
				vertices = mesh.GetVertices();
			}
			int subMeshCount = mesh.subMeshCount;
			List<Dictionary<Vertex, int>> list = new List<Dictionary<Vertex, int>>();
			int[][] array = new int[subMeshCount][];
			int num = 0;
			for (int i = 0; i < subMeshCount; i++)
			{
				array[i] = mesh.GetTriangles(i);
				Dictionary<Vertex, int> dictionary = new Dictionary<Vertex, int>();
				for (int j = 0; j < array[i].Length; j++)
				{
					Vertex key = vertices[array[i][j]];
					int num2;
					if (dictionary.TryGetValue(key, out num2))
					{
						array[i][j] = num2;
					}
					else
					{
						array[i][j] = num;
						dictionary.Add(key, num);
						num++;
					}
				}
				list.Add(dictionary);
			}
			Vertex[] array2 = list.SelectMany((Dictionary<Vertex, int> x) => x.Keys).ToArray<Vertex>();
			flag |= (array2.Length != vertices.Length);
			if (flag)
			{
				Vertex.SetMesh(mesh, array2);
				mesh.subMeshCount = subMeshCount;
				for (int k = 0; k < subMeshCount; k++)
				{
					mesh.SetTriangles(array[k], k);
				}
			}
		}

		public static void FitToSize(ProBuilderMesh mesh, Bounds currentSize, Vector3 sizeToFit)
		{
			if (mesh.vertexCount < 1)
			{
				return;
			}
			Vector3 vector = sizeToFit.Abs().DivideBy(currentSize.size);
			if (vector == Vector3.one || vector == Vector3.zero)
			{
				return;
			}
			Vector3[] positionsInternal = mesh.positionsInternal;
			if (Math.Abs(currentSize.size.x) < 0.001f)
			{
				vector.x = 0f;
			}
			if (Math.Abs(currentSize.size.y) < 0.001f)
			{
				vector.y = 0f;
			}
			if (Math.Abs(currentSize.size.z) < 0.001f)
			{
				vector.z = 0f;
			}
			int i = 0;
			int vertexCount = mesh.vertexCount;
			while (i < vertexCount)
			{
				positionsInternal[i] -= currentSize.center;
				positionsInternal[i].Scale(vector);
				positionsInternal[i] += currentSize.center;
				i++;
			}
			mesh.Rebuild();
		}

		internal static string SanityCheck(ProBuilderMesh mesh)
		{
			return MeshUtility.SanityCheck(mesh.GetVertices(null));
		}

		internal static string SanityCheck(Mesh mesh)
		{
			return MeshUtility.SanityCheck(mesh.GetVertices());
		}

		internal static string SanityCheck(IList<Vertex> vertices)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			int count = vertices.Count;
			while (i < count)
			{
				Vertex vertex = vertices[i];
				if (!Math.IsNumber(vertex.position) || !Math.IsNumber(vertex.color) || !Math.IsNumber(vertex.uv0) || !Math.IsNumber(vertex.normal) || !Math.IsNumber(vertex.tangent) || !Math.IsNumber(vertex.uv2) || !Math.IsNumber(vertex.uv3) || !Math.IsNumber(vertex.uv4))
				{
					stringBuilder.AppendFormat("vertex {0} contains invalid values:\n{1}\n\n", i, vertex.ToString(null));
				}
				i++;
			}
			return stringBuilder.ToString();
		}

		internal static bool IsUsedInParticleSystem(ProBuilderMesh pbmesh)
		{
			ParticleSystem particleSystem;
			if (pbmesh.TryGetComponent<ParticleSystem>(out particleSystem))
			{
				ParticleSystem.ShapeModule shape = particleSystem.shape;
				if (shape.meshRenderer == pbmesh.renderer)
				{
					shape.meshRenderer = null;
					return true;
				}
			}
			return false;
		}

		internal static void RestoreParticleSystem(ProBuilderMesh pbmesh)
		{
			ParticleSystem particleSystem;
			if (pbmesh.TryGetComponent<ParticleSystem>(out particleSystem))
			{
				particleSystem.shape.meshRenderer = pbmesh.renderer;
			}
		}

		internal static Bounds GetBounds(this ProBuilderMesh mesh)
		{
			if (mesh.mesh != null)
			{
				return mesh.mesh.bounds;
			}
			return Math.GetBounds(mesh.positionsInternal, null);
		}
	}
}
