using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder.MeshOperations
{
	public static class ConnectElements
	{
		public static Face[] Connect(this ProBuilderMesh mesh, IEnumerable<Face> faces)
		{
			List<Face> list = mesh.EnsureFacesAreComposedOfContiguousTriangles(faces);
			HashSet<Face> hashSet = new HashSet<Face>(faces);
			if (list.Count > 0)
			{
				foreach (Face item in list)
				{
					hashSet.Add(item);
				}
			}
			IEnumerable<Edge> edges = hashSet.SelectMany((Face x) => x.edgesInternal);
			Face[] result;
			Edge[] array;
			mesh.Connect(edges, out result, out array, true, false, hashSet);
			return result;
		}

		public static SimpleTuple<Face[], Edge[]> Connect(this ProBuilderMesh mesh, IEnumerable<Edge> edges)
		{
			Face[] item;
			Edge[] item2;
			mesh.Connect(edges, out item, out item2, true, true, null);
			return new SimpleTuple<Face[], Edge[]>(item, item2);
		}

		public static int[] Connect(this ProBuilderMesh mesh, IList<int> indexes)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}
			int num = mesh.sharedVerticesInternal.Length;
			Dictionary<int, int> lookup = mesh.sharedVertexLookup;
			HashSet<int> hashSet = new HashSet<int>(from x in indexes
			select lookup[x]);
			HashSet<int> hashSet2 = new HashSet<int>();
			foreach (int num2 in hashSet)
			{
				hashSet2.UnionWith(mesh.sharedVerticesInternal[num2].arrayInternal);
			}
			Dictionary<Face, List<int>> dictionary = new Dictionary<Face, List<int>>();
			List<Vertex> vertices = new List<Vertex>(mesh.GetVertices(null));
			foreach (Face face in mesh.facesInternal)
			{
				int[] distinctIndexesInternal = face.distinctIndexesInternal;
				for (int j = 0; j < distinctIndexesInternal.Length; j++)
				{
					if (hashSet2.Contains(distinctIndexesInternal[j]))
					{
						dictionary.AddOrAppend(face, distinctIndexesInternal[j]);
					}
				}
			}
			List<ConnectFaceRebuildData> list = new List<ConnectFaceRebuildData>();
			List<Face> list2 = new List<Face>();
			HashSet<int> hashSet3 = new HashSet<int>(from x in mesh.facesInternal
			select x.textureGroup);
			int num3 = 1;
			foreach (KeyValuePair<Face, List<int>> keyValuePair in dictionary)
			{
				Face key = keyValuePair.Key;
				List<ConnectFaceRebuildData> list3 = (keyValuePair.Value.Count == 2) ? ConnectElements.ConnectIndexesPerFace(key, keyValuePair.Value[0], keyValuePair.Value[1], vertices, lookup) : ConnectElements.ConnectIndexesPerFace(key, keyValuePair.Value, vertices, lookup, num++);
				if (list3 != null)
				{
					if (key.textureGroup < 0)
					{
						while (hashSet3.Contains(num3))
						{
							num3++;
						}
						hashSet3.Add(num3);
					}
					foreach (ConnectFaceRebuildData connectFaceRebuildData in list3)
					{
						connectFaceRebuildData.faceRebuildData.face.textureGroup = ((key.textureGroup < 0) ? num3 : key.textureGroup);
						connectFaceRebuildData.faceRebuildData.face.uv = new AutoUnwrapSettings(key.uv);
						connectFaceRebuildData.faceRebuildData.face.smoothingGroup = key.smoothingGroup;
						connectFaceRebuildData.faceRebuildData.face.manualUV = key.manualUV;
						connectFaceRebuildData.faceRebuildData.face.submeshIndex = key.submeshIndex;
					}
					list2.Add(key);
					list.AddRange(list3);
				}
			}
			FaceRebuildData.Apply(from x in list
			select x.faceRebuildData, mesh, vertices, null);
			int num4 = mesh.DeleteFaces(list2).Length;
			lookup = mesh.sharedVertexLookup;
			HashSet<int> hashSet4 = new HashSet<int>();
			for (int k = 0; k < list.Count; k++)
			{
				for (int l = 0; l < list[k].newVertexIndexes.Count; l++)
				{
					hashSet4.Add(lookup[list[k].newVertexIndexes[l] + (list[k].faceRebuildData.Offset() - num4)]);
				}
			}
			mesh.ToMesh(MeshTopology.Triangles);
			return (from x in hashSet4
			select mesh.sharedVerticesInternal[x][0]).ToArray<int>();
		}

		internal static ActionResult Connect(this ProBuilderMesh mesh, IEnumerable<Edge> edges, out Face[] addedFaces, out Edge[] connections, bool returnFaces = false, bool returnEdges = false, HashSet<Face> faceMask = null)
		{
			Dictionary<int, int> sharedVertexLookup = mesh.sharedVertexLookup;
			Dictionary<int, int> sharedTextureLookup = mesh.sharedTextureLookup;
			HashSet<EdgeLookup> hashSet = new HashSet<EdgeLookup>(EdgeLookup.GetEdgeLookup(edges, sharedVertexLookup));
			List<WingedEdge> wingedEdges = WingedEdge.GetWingedEdges(mesh, false);
			Dictionary<Face, List<WingedEdge>> dictionary = new Dictionary<Face, List<WingedEdge>>();
			foreach (WingedEdge wingedEdge in wingedEdges)
			{
				if (hashSet.Contains(wingedEdge.edge))
				{
					List<WingedEdge> list;
					if (dictionary.TryGetValue(wingedEdge.face, out list))
					{
						list.Add(wingedEdge);
					}
					else
					{
						dictionary.Add(wingedEdge.face, new List<WingedEdge>
						{
							wingedEdge
						});
					}
				}
			}
			Dictionary<Face, List<WingedEdge>> dictionary2 = new Dictionary<Face, List<WingedEdge>>();
			foreach (KeyValuePair<Face, List<WingedEdge>> keyValuePair in dictionary)
			{
				if (keyValuePair.Value.Count <= 1)
				{
					WingedEdge opposite = keyValuePair.Value[0].opposite;
					List<WingedEdge> list2;
					if (opposite == null || !dictionary.TryGetValue(opposite.face, out list2) || list2.Count <= 1)
					{
						continue;
					}
				}
				dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
			}
			List<Vertex> vertices = new List<Vertex>(mesh.GetVertices(null));
			List<ConnectFaceRebuildData> list3 = new List<ConnectFaceRebuildData>();
			List<Face> list4 = new List<Face>();
			HashSet<int> hashSet2 = new HashSet<int>(from x in mesh.facesInternal
			select x.textureGroup);
			int num = 1;
			foreach (KeyValuePair<Face, List<WingedEdge>> keyValuePair2 in dictionary2)
			{
				Face key = keyValuePair2.Key;
				List<WingedEdge> value = keyValuePair2.Value;
				int count = value.Count;
				Vector3 lhs = Math.Normal(vertices, key.indexesInternal);
				if (count == 1 || (faceMask != null && !faceMask.Contains(key)))
				{
					ConnectFaceRebuildData connectFaceRebuildData;
					if (ConnectElements.InsertVertices(key, value, vertices, out connectFaceRebuildData))
					{
						Vector3 rhs = Math.Normal(connectFaceRebuildData.faceRebuildData.vertices, connectFaceRebuildData.faceRebuildData.face.indexesInternal);
						if (Vector3.Dot(lhs, rhs) < 0f)
						{
							connectFaceRebuildData.faceRebuildData.face.Reverse();
						}
						list3.Add(connectFaceRebuildData);
					}
				}
				else if (count > 1)
				{
					List<ConnectFaceRebuildData> list5 = (count == 2) ? ConnectElements.ConnectEdgesInFace(key, value[0], value[1], vertices) : ConnectElements.ConnectEdgesInFace(key, value, vertices);
					if (key.textureGroup < 0)
					{
						while (hashSet2.Contains(num))
						{
							num++;
						}
						hashSet2.Add(num);
					}
					if (list5 == null)
					{
						connections = null;
						addedFaces = null;
						return new ActionResult(ActionResult.Status.Failure, "Unable to connect faces");
					}
					foreach (ConnectFaceRebuildData connectFaceRebuildData2 in list5)
					{
						list4.Add(connectFaceRebuildData2.faceRebuildData.face);
						Vector3 rhs2 = Math.Normal(connectFaceRebuildData2.faceRebuildData.vertices, connectFaceRebuildData2.faceRebuildData.face.indexesInternal);
						if (Vector3.Dot(lhs, rhs2) < 0f)
						{
							connectFaceRebuildData2.faceRebuildData.face.Reverse();
						}
						connectFaceRebuildData2.faceRebuildData.face.textureGroup = ((key.textureGroup < 0) ? num : key.textureGroup);
						connectFaceRebuildData2.faceRebuildData.face.uv = new AutoUnwrapSettings(key.uv);
						connectFaceRebuildData2.faceRebuildData.face.submeshIndex = key.submeshIndex;
						connectFaceRebuildData2.faceRebuildData.face.smoothingGroup = key.smoothingGroup;
						connectFaceRebuildData2.faceRebuildData.face.manualUV = key.manualUV;
					}
					list3.AddRange(list5);
				}
			}
			FaceRebuildData.Apply(from x in list3
			select x.faceRebuildData, mesh, vertices, null);
			mesh.sharedTextures = new SharedVertex[0];
			int num2 = mesh.DeleteFaces(dictionary2.Keys).Length;
			mesh.sharedVertices = SharedVertex.GetSharedVerticesWithPositions(mesh.positionsInternal);
			mesh.ToMesh(MeshTopology.Triangles);
			if (returnEdges)
			{
				HashSet<int> appended = new HashSet<int>();
				for (int i = 0; i < list3.Count; i++)
				{
					for (int j = 0; j < list3[i].newVertexIndexes.Count; j++)
					{
						appended.Add(list3[i].newVertexIndexes[j] + list3[i].faceRebuildData.Offset() - num2);
					}
				}
				Dictionary<int, int> sharedVertexLookup2 = mesh.sharedVertexLookup;
				IEnumerable<EdgeLookup> edgeLookup = EdgeLookup.GetEdgeLookup(from x in list3.SelectMany((ConnectFaceRebuildData x) => x.faceRebuildData.face.edgesInternal)
				where appended.Contains(x.a) && appended.Contains(x.b)
				select x, sharedVertexLookup2);
				connections = (from x in edgeLookup.Distinct<EdgeLookup>()
				select x.local).ToArray<Edge>();
			}
			else
			{
				connections = null;
			}
			if (returnFaces)
			{
				addedFaces = list4.ToArray();
			}
			else
			{
				addedFaces = null;
			}
			return new ActionResult(ActionResult.Status.Success, string.Format("Connected {0} Edges", list3.Count / 2));
		}

		private static List<ConnectFaceRebuildData> ConnectEdgesInFace(Face face, WingedEdge a, WingedEdge b, List<Vertex> vertices)
		{
			List<Edge> list = WingedEdge.SortEdgesByAdjacency(face);
			List<Vertex>[] array = new List<Vertex>[]
			{
				new List<Vertex>(),
				new List<Vertex>()
			};
			List<int>[] array2 = new List<int>[]
			{
				new List<int>(),
				new List<int>()
			};
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				array[num % 2].Add(vertices[list[i].a]);
				if (list[i].Equals(a.edge.local) || list[i].Equals(b.edge.local))
				{
					Vertex item = Vertex.Mix(vertices[list[i].a], vertices[list[i].b], 0.5f);
					array2[num % 2].Add(array[num % 2].Count);
					array[num % 2].Add(item);
					num++;
					array2[num % 2].Add(array[num % 2].Count);
					array[num % 2].Add(item);
				}
			}
			List<ConnectFaceRebuildData> list2 = new List<ConnectFaceRebuildData>();
			for (int j = 0; j < array.Length; j++)
			{
				FaceRebuildData faceRebuildData = AppendElements.FaceWithVertices(array[j], false);
				if (faceRebuildData != null)
				{
					list2.Add(new ConnectFaceRebuildData(faceRebuildData, array2[j]));
				}
			}
			return list2;
		}

		private static List<ConnectFaceRebuildData> ConnectEdgesInFace(Face face, List<WingedEdge> edges, List<Vertex> vertices)
		{
			List<Edge> list = WingedEdge.SortEdgesByAdjacency(face);
			int count = edges.Count;
			Vertex item = Vertex.Average(vertices, face.distinctIndexesInternal);
			List<List<Vertex>> list2 = ArrayUtility.Fill<List<Vertex>>((int x) => new List<Vertex>(), count);
			List<List<int>> list3 = ArrayUtility.Fill<List<int>>((int x) => new List<int>(), count);
			HashSet<Edge> hashSet = new HashSet<Edge>(from x in edges
			select x.edge.local);
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				list2[num % count].Add(vertices[list[i].a]);
				if (hashSet.Contains(list[i]))
				{
					Vertex item2 = Vertex.Mix(vertices[list[i].a], vertices[list[i].b], 0.5f);
					list3[num].Add(list2[num].Count);
					list2[num].Add(item2);
					list3[num].Add(list2[num].Count);
					list2[num].Add(item);
					num = (num + 1) % count;
					list2[num].Add(item2);
				}
			}
			List<ConnectFaceRebuildData> list4 = new List<ConnectFaceRebuildData>();
			for (int j = 0; j < list2.Count; j++)
			{
				FaceRebuildData faceRebuildData = AppendElements.FaceWithVertices(list2[j], false);
				if (faceRebuildData == null)
				{
					list4.Clear();
					return null;
				}
				list4.Add(new ConnectFaceRebuildData(faceRebuildData, list3[j]));
			}
			return list4;
		}

		private static bool InsertVertices(Face face, List<WingedEdge> edges, List<Vertex> vertices, out ConnectFaceRebuildData data)
		{
			List<Edge> list = WingedEdge.SortEdgesByAdjacency(face);
			List<Vertex> list2 = new List<Vertex>();
			List<int> list3 = new List<int>();
			HashSet<Edge> hashSet = new HashSet<Edge>(from x in edges
			select x.edge.local);
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(vertices[list[i].a]);
				if (hashSet.Contains(list[i]))
				{
					list3.Add(list2.Count);
					list2.Add(Vertex.Mix(vertices[list[i].a], vertices[list[i].b], 0.5f));
				}
			}
			FaceRebuildData faceRebuildData = AppendElements.FaceWithVertices(list2, false);
			if (faceRebuildData != null)
			{
				faceRebuildData.face.textureGroup = face.textureGroup;
				faceRebuildData.face.uv = new AutoUnwrapSettings(face.uv);
				faceRebuildData.face.smoothingGroup = face.smoothingGroup;
				faceRebuildData.face.manualUV = face.manualUV;
				faceRebuildData.face.submeshIndex = face.submeshIndex;
				data = new ConnectFaceRebuildData(faceRebuildData, list3);
				return true;
			}
			data = null;
			return false;
		}

		private static List<ConnectFaceRebuildData> ConnectIndexesPerFace(Face face, int a, int b, List<Vertex> vertices, Dictionary<int, int> lookup)
		{
			List<Edge> list = WingedEdge.SortEdgesByAdjacency(face);
			List<Vertex>[] array = new List<Vertex>[]
			{
				new List<Vertex>(),
				new List<Vertex>()
			};
			List<int>[] array2 = new List<int>[]
			{
				new List<int>(),
				new List<int>()
			};
			List<int>[] array3 = new List<int>[]
			{
				new List<int>(),
				new List<int>()
			};
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Contains(a) && list[i].Contains(b))
				{
					return null;
				}
				int a2 = list[i].a;
				array[num].Add(vertices[a2]);
				array2[num].Add(lookup[a2]);
				if (a2 == a || a2 == b)
				{
					num = (num + 1) % 2;
					array3[num].Add(array[num].Count);
					array[num].Add(vertices[a2]);
					array2[num].Add(lookup[a2]);
				}
			}
			List<ConnectFaceRebuildData> list2 = new List<ConnectFaceRebuildData>();
			Vector3 lhs = Math.Normal(vertices, face.indexesInternal);
			for (int j = 0; j < array.Length; j++)
			{
				FaceRebuildData faceRebuildData = AppendElements.FaceWithVertices(array[j], false);
				faceRebuildData.sharedIndexes = array2[j];
				Vector3 rhs = Math.Normal(array[j], faceRebuildData.face.indexesInternal);
				if (Vector3.Dot(lhs, rhs) < 0f)
				{
					faceRebuildData.face.Reverse();
				}
				list2.Add(new ConnectFaceRebuildData(faceRebuildData, array3[j]));
			}
			return list2;
		}

		private static List<ConnectFaceRebuildData> ConnectIndexesPerFace(Face face, List<int> indexes, List<Vertex> vertices, Dictionary<int, int> lookup, int sharedIndexOffset)
		{
			if (indexes.Count < 3)
			{
				return null;
			}
			List<Edge> list = WingedEdge.SortEdgesByAdjacency(face);
			int count = indexes.Count;
			List<List<Vertex>> list2 = ArrayUtility.Fill<List<Vertex>>((int x) => new List<Vertex>(), count);
			List<List<int>> list3 = ArrayUtility.Fill<List<int>>((int x) => new List<int>(), count);
			List<List<int>> list4 = ArrayUtility.Fill<List<int>>((int x) => new List<int>(), count);
			Vertex item = Vertex.Average(vertices, indexes);
			Vector3 lhs = Math.Normal(vertices, face.indexesInternal);
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				int a = list[i].a;
				list2[num].Add(vertices[a]);
				list3[num].Add(lookup[a]);
				if (indexes.Contains(a))
				{
					list4[num].Add(list2[num].Count);
					list2[num].Add(item);
					list3[num].Add(sharedIndexOffset);
					num = (num + 1) % count;
					list4[num].Add(list2[num].Count);
					list2[num].Add(vertices[a]);
					list3[num].Add(lookup[a]);
				}
			}
			List<ConnectFaceRebuildData> list5 = new List<ConnectFaceRebuildData>();
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j].Count >= 3)
				{
					FaceRebuildData faceRebuildData = AppendElements.FaceWithVertices(list2[j], false);
					faceRebuildData.sharedIndexes = list3[j];
					Vector3 rhs = Math.Normal(list2[j], faceRebuildData.face.indexesInternal);
					if (Vector3.Dot(lhs, rhs) < 0f)
					{
						faceRebuildData.face.Reverse();
					}
					list5.Add(new ConnectFaceRebuildData(faceRebuildData, list4[j]));
				}
			}
			return list5;
		}
	}
}
