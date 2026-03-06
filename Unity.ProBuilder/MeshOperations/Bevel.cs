using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder.MeshOperations
{
	public static class Bevel
	{
		public static List<Face> BevelEdges(ProBuilderMesh mesh, IList<Edge> edges, float amount)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			Dictionary<int, int> sharedVertexLookup = mesh.sharedVertexLookup;
			List<Vertex> list = new List<Vertex>(mesh.GetVertices(null));
			List<EdgeLookup> list2 = EdgeLookup.GetEdgeLookup(edges, sharedVertexLookup).Distinct<EdgeLookup>().ToList<EdgeLookup>();
			List<WingedEdge> wingedEdges = WingedEdge.GetWingedEdges(mesh, false);
			List<FaceRebuildData> list3 = new List<FaceRebuildData>();
			Dictionary<Face, List<int>> ignore = new Dictionary<Face, List<int>>();
			HashSet<int> hashSet = new HashSet<int>();
			int num = 0;
			Dictionary<int, List<SimpleTuple<FaceRebuildData, List<int>>>> dictionary = new Dictionary<int, List<SimpleTuple<FaceRebuildData, List<int>>>>();
			Dictionary<int, List<WingedEdge>> spokes = WingedEdge.GetSpokes(wingedEdges);
			HashSet<int> hashSet2 = new HashSet<int>();
			foreach (EdgeLookup edgeLookup in list2)
			{
				if (hashSet2.Add(edgeLookup.common.a))
				{
					foreach (WingedEdge wingedEdge in spokes[edgeLookup.common.a])
					{
						Edge local = wingedEdge.edge.local;
						amount = Mathf.Min(Vector3.Distance(list[local.a].position, list[local.b].position) - 0.001f, amount);
					}
				}
				if (hashSet2.Add(edgeLookup.common.b))
				{
					foreach (WingedEdge wingedEdge2 in spokes[edgeLookup.common.b])
					{
						Edge local2 = wingedEdge2.edge.local;
						amount = Mathf.Min(Vector3.Distance(list[local2.a].position, list[local2.b].position) - 0.001f, amount);
					}
				}
			}
			if (amount < 0.001f)
			{
				Log.Info("Bevel Distance > Available Surface");
				return null;
			}
			using (List<EdgeLookup>.Enumerator enumerator = list2.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EdgeLookup lup = enumerator.Current;
					WingedEdge wingedEdge3 = wingedEdges.FirstOrDefault((WingedEdge x) => x.edge.Equals(lup));
					if (wingedEdge3 != null && wingedEdge3.opposite != null)
					{
						num++;
						ignore.AddOrAppend(wingedEdge3.face, wingedEdge3.edge.common.a);
						ignore.AddOrAppend(wingedEdge3.face, wingedEdge3.edge.common.b);
						ignore.AddOrAppend(wingedEdge3.opposite.face, wingedEdge3.edge.common.a);
						ignore.AddOrAppend(wingedEdge3.opposite.face, wingedEdge3.edge.common.b);
						hashSet.Add(wingedEdge3.edge.common.a);
						hashSet.Add(wingedEdge3.edge.common.b);
						Bevel.SlideEdge(list, wingedEdge3, amount);
						Bevel.SlideEdge(list, wingedEdge3.opposite, amount);
						list3.AddRange(Bevel.GetBridgeFaces(list, wingedEdge3, wingedEdge3.opposite, dictionary));
					}
				}
			}
			if (num < 1)
			{
				Log.Info("Cannot Bevel Open Edges");
				return null;
			}
			List<Face> list4 = new List<Face>(from x in list3
			select x.face);
			Dictionary<Face, List<SimpleTuple<WingedEdge, int>>> dictionary2 = new Dictionary<Face, List<SimpleTuple<WingedEdge, int>>>();
			using (HashSet<int>.Enumerator enumerator3 = hashSet.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					int c = enumerator3.Current;
					IEnumerable<WingedEdge> enumerable = from x in wingedEdges
					where x.edge.common.Contains(c) && (!ignore.ContainsKey(x.face) || !ignore[x.face].Contains(c))
					select x;
					HashSet<Face> hashSet3 = new HashSet<Face>();
					foreach (WingedEdge wingedEdge4 in enumerable)
					{
						if (hashSet3.Add(wingedEdge4.face))
						{
							dictionary2.AddOrAppend(wingedEdge4.face, new SimpleTuple<WingedEdge, int>(wingedEdge4, c));
						}
					}
				}
			}
			foreach (KeyValuePair<Face, List<SimpleTuple<WingedEdge, int>>> keyValuePair in dictionary2)
			{
				Dictionary<int, List<int>> dictionary3;
				FaceRebuildData faceRebuildData = VertexEditing.ExplodeVertex(list, keyValuePair.Value, amount, out dictionary3);
				if (faceRebuildData != null)
				{
					list3.Add(faceRebuildData);
					foreach (KeyValuePair<int, List<int>> keyValuePair2 in dictionary3)
					{
						dictionary.AddOrAppend(keyValuePair2.Key, new SimpleTuple<FaceRebuildData, List<int>>(faceRebuildData, keyValuePair2.Value));
					}
				}
			}
			FaceRebuildData.Apply(list3, mesh, list, null);
			int num2 = mesh.DeleteFaces(dictionary2.Keys).Length;
			mesh.sharedTextures = new SharedVertex[0];
			mesh.sharedVertices = SharedVertex.GetSharedVerticesWithPositions(mesh.positionsInternal);
			SharedVertex[] sharedIndexes = mesh.sharedVerticesInternal;
			sharedVertexLookup = mesh.sharedVertexLookup;
			List<HashSet<int>> list5 = new List<HashSet<int>>();
			foreach (KeyValuePair<int, List<SimpleTuple<FaceRebuildData, List<int>>>> keyValuePair3 in dictionary)
			{
				if (keyValuePair3.Value.Sum((SimpleTuple<FaceRebuildData, List<int>> x) => x.item2.Count) >= 3)
				{
					HashSet<int> hashSet4 = new HashSet<int>();
					foreach (SimpleTuple<FaceRebuildData, List<int>> simpleTuple in keyValuePair3.Value)
					{
						int num3 = simpleTuple.item1.Offset() - num2;
						for (int i = 0; i < simpleTuple.item2.Count; i++)
						{
							hashSet4.Add(sharedVertexLookup[simpleTuple.item2[i] + num3]);
						}
					}
					list5.Add(hashSet4);
				}
			}
			List<WingedEdge> wingedEdges2 = WingedEdge.GetWingedEdges(mesh, from x in list3
			select x.face, false);
			list = new List<Vertex>(mesh.GetVertices(null));
			List<FaceRebuildData> list6 = new List<FaceRebuildData>();
			Func<int, int> <>9__7;
			Func<int, int> <>9__8;
			foreach (HashSet<int> hashSet5 in list5)
			{
				if (hashSet5.Count >= 3)
				{
					if (hashSet5.Count < 4)
					{
						IEnumerable<int> source = hashSet5;
						Func<int, int> selector;
						if ((selector = <>9__7) == null)
						{
							selector = (<>9__7 = ((int x) => sharedIndexes[x][0]));
						}
						List<Vertex> vertices = new List<Vertex>(mesh.GetVertices(source.Select(selector).ToList<int>()));
						list6.Add(AppendElements.FaceWithVertices(vertices, true));
					}
					else
					{
						List<int> list7 = WingedEdge.SortCommonIndexesByAdjacency(wingedEdges2, hashSet5);
						if (list7 != null)
						{
							IEnumerable<int> source2 = list7;
							Func<int, int> selector2;
							if ((selector2 = <>9__8) == null)
							{
								selector2 = (<>9__8 = ((int x) => sharedIndexes[x][0]));
							}
							List<Vertex> path = new List<Vertex>(mesh.GetVertices(source2.Select(selector2).ToList<int>()));
							list6.AddRange(AppendElements.TentCapWithVertices(path));
						}
					}
				}
			}
			FaceRebuildData.Apply(list6, mesh, list, null);
			mesh.sharedVertices = SharedVertex.GetSharedVerticesWithPositions(mesh.positionsInternal);
			HashSet<Face> hashSet6 = new HashSet<Face>(from x in list6
			select x.face);
			hashSet6.UnionWith(list4);
			list3.AddRange(list6);
			List<WingedEdge> wingedEdges3 = WingedEdge.GetWingedEdges(mesh, from x in list3
			select x.face, false);
			int num4 = 0;
			while (num4 < wingedEdges3.Count && hashSet6.Count > 0)
			{
				WingedEdge wingedEdge5 = wingedEdges3[num4];
				if (hashSet6.Contains(wingedEdge5.face))
				{
					hashSet6.Remove(wingedEdge5.face);
					using (WingedEdgeEnumerator wingedEdgeEnumerator = new WingedEdgeEnumerator(wingedEdge5))
					{
						while (wingedEdgeEnumerator.MoveNext())
						{
							WingedEdge wingedEdge6 = wingedEdgeEnumerator.Current;
							if (wingedEdge6.opposite != null && !hashSet6.Contains(wingedEdge6.opposite.face))
							{
								wingedEdge6.face.submeshIndex = wingedEdge6.opposite.face.submeshIndex;
								wingedEdge6.face.uv = new AutoUnwrapSettings(wingedEdge6.opposite.face.uv);
								SurfaceTopology.ConformOppositeNormal(wingedEdge6.opposite);
								break;
							}
						}
					}
				}
				num4++;
			}
			mesh.ToMesh(MeshTopology.Triangles);
			return list4;
		}

		private static List<FaceRebuildData> GetBridgeFaces(IList<Vertex> vertices, WingedEdge left, WingedEdge right, Dictionary<int, List<SimpleTuple<FaceRebuildData, List<int>>>> holes)
		{
			List<FaceRebuildData> list = new List<FaceRebuildData>();
			FaceRebuildData faceRebuildData = new FaceRebuildData();
			EdgeLookup edge = left.edge;
			EdgeLookup edge2 = right.edge;
			faceRebuildData.vertices = new List<Vertex>
			{
				vertices[edge.local.a],
				vertices[edge.local.b],
				vertices[(edge.common.a == edge2.common.a) ? edge2.local.a : edge2.local.b],
				vertices[(edge.common.a == edge2.common.a) ? edge2.local.b : edge2.local.a]
			};
			Vector3 lhs = Math.Normal(vertices, left.face.indexesInternal);
			Vector3 rhs = Math.Normal(faceRebuildData.vertices, Bevel.k_BridgeIndexesTri);
			int[] array = new int[]
			{
				2,
				1,
				0,
				2,
				3,
				1
			};
			if (Vector3.Dot(lhs, rhs) < 0f)
			{
				Array.Reverse<int>(array);
			}
			faceRebuildData.face = new Face(array, left.face.submeshIndex, AutoUnwrapSettings.tile, -1, -1, -1, false);
			list.Add(faceRebuildData);
			holes.AddOrAppend(edge.common.a, new SimpleTuple<FaceRebuildData, List<int>>(faceRebuildData, new List<int>
			{
				0,
				2
			}));
			holes.AddOrAppend(edge.common.b, new SimpleTuple<FaceRebuildData, List<int>>(faceRebuildData, new List<int>
			{
				1,
				3
			}));
			return list;
		}

		private static void SlideEdge(IList<Vertex> vertices, WingedEdge we, float amount)
		{
			we.face.manualUV = true;
			we.face.textureGroup = -1;
			Edge leadingEdge = Bevel.GetLeadingEdge(we, we.edge.common.a);
			Edge leadingEdge2 = Bevel.GetLeadingEdge(we, we.edge.common.b);
			if (!leadingEdge.IsValid() || !leadingEdge2.IsValid())
			{
				return;
			}
			Vertex vertex = vertices[leadingEdge.a] - vertices[leadingEdge.b];
			vertex.Normalize();
			Vertex vertex2 = vertices[leadingEdge2.a] - vertices[leadingEdge2.b];
			vertex2.Normalize();
			vertices[we.edge.local.a].Add(vertex * amount);
			vertices[we.edge.local.b].Add(vertex2 * amount);
		}

		private static Edge GetLeadingEdge(WingedEdge wing, int common)
		{
			if (wing.previous.edge.common.a == common)
			{
				return new Edge(wing.previous.edge.local.b, wing.previous.edge.local.a);
			}
			if (wing.previous.edge.common.b == common)
			{
				return new Edge(wing.previous.edge.local.a, wing.previous.edge.local.b);
			}
			if (wing.next.edge.common.a == common)
			{
				return new Edge(wing.next.edge.local.b, wing.next.edge.local.a);
			}
			if (wing.next.edge.common.b == common)
			{
				return new Edge(wing.next.edge.local.a, wing.next.edge.local.b);
			}
			return Edge.Empty;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Bevel()
		{
			int[] array = new int[3];
			array[0] = 2;
			array[1] = 1;
			Bevel.k_BridgeIndexesTri = array;
		}

		private static readonly int[] k_BridgeIndexesTri;
	}
}
