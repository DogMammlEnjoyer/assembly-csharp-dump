using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class MeshEditor
	{
		public MeshEditor(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public virtual int[] AddTriangleStrip(IList<Frame3f> frames, IList<Interval1d> spans, int group_id = -1)
		{
			int count = frames.Count;
			if (count != spans.Count)
			{
				throw new Exception("MeshEditor.AddTriangleStrip: spans list is not the same size!");
			}
			int[] array = new int[2 * (count - 1)];
			int num = -1;
			int v = -1;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				Frame3f frame3f = frames[i];
				Interval1d interval1d = spans[i];
				Vector3d v2 = frame3f.Origin + (float)interval1d.a * frame3f.Y;
				Vector3d v3 = frame3f.Origin + (float)interval1d.b * frame3f.Y;
				int num3 = this.Mesh.AppendVertex(v2);
				int num4 = this.Mesh.AppendVertex(v3);
				if (num != -1)
				{
					array[num2++] = this.Mesh.AppendTriangle(num, num4, v, -1);
					array[num2++] = this.Mesh.AppendTriangle(num, num3, num4, -1);
				}
				num = num3;
				v = num4;
			}
			return array;
		}

		public virtual int[] AddTriangleFan_OrderedVertexLoop(int center, int[] vertex_loop, int group_id = -1)
		{
			int num = vertex_loop.Length;
			int[] array = new int[num];
			int i = 0;
			while (i < num)
			{
				int kk = vertex_loop[i];
				int jj = vertex_loop[(i + 1) % num];
				Index3i tv = new Index3i(center, jj, kk);
				int num2 = this.Mesh.AppendTriangle(tv, group_id);
				if (num2 >= 0)
				{
					array[i] = num2;
					i++;
				}
				else
				{
					if (i > 0 && !this.remove_triangles(array, i))
					{
						throw new Exception("MeshEditor.AddTriangleFan_OrderedVertexLoop: failed to add fan, and also falied to back out changes.");
					}
					return null;
				}
			}
			return array;
		}

		public virtual int[] AddTriangleFan_OrderedEdgeLoop(int center, int[] edge_loop, int group_id = -1)
		{
			int num = edge_loop.Length;
			int[] array = new int[num];
			int i = 0;
			while (i < num)
			{
				if (this.Mesh.IsBoundaryEdge(edge_loop[i]))
				{
					Index2i orientedBoundaryEdgeV = this.Mesh.GetOrientedBoundaryEdgeV(edge_loop[i]);
					int a = orientedBoundaryEdgeV.a;
					int b = orientedBoundaryEdgeV.b;
					Index3i tv = new Index3i(center, b, a);
					int num2 = this.Mesh.AppendTriangle(tv, group_id);
					if (num2 >= 0)
					{
						array[i] = num2;
						i++;
						continue;
					}
				}
				if (i > 0 && !this.remove_triangles(array, i - 1))
				{
					throw new Exception("MeshEditor.AddTriangleFan_OrderedEdgeLoop: failed to add fan, and also failed to back out changes.");
				}
				return null;
			}
			return array;
		}

		public virtual int[] StitchLoop(int[] vloop1, int[] vloop2, int group_id = -1)
		{
			int num = vloop1.Length;
			if (num != vloop2.Length)
			{
				throw new Exception("MeshEditor.StitchLoop: loops are not the same length!!");
			}
			int[] array = new int[num * 2];
			int i = 0;
			while (i < num)
			{
				int num2 = vloop1[i];
				int ii = vloop1[(i + 1) % num];
				int jj = vloop2[i];
				int kk = vloop2[(i + 1) % num];
				Index3i tv = new Index3i(ii, num2, kk);
				Index3i tv2 = new Index3i(num2, jj, kk);
				int num3 = this.Mesh.AppendTriangle(tv, group_id);
				int num4 = this.Mesh.AppendTriangle(tv2, group_id);
				array[2 * i] = num3;
				array[2 * i + 1] = num4;
				if (num3 >= 0 && num4 >= 0)
				{
					i++;
				}
				else
				{
					if (i > 0 && !this.remove_triangles(array, 2 * i + 1))
					{
						throw new Exception("MeshEditor.StitchLoop: failed to add all triangles, and also failed to back out changes.");
					}
					return null;
				}
			}
			return array;
		}

		public virtual int[] StitchVertexLoops_NearestV(int[] loop0, int[] loop1, int group_id = -1)
		{
			int num = loop0.Length;
			Index2i zero = Index2i.Zero;
			double num2 = double.MaxValue;
			for (int i = 0; i < num; i++)
			{
				Vector3d vertex = this.Mesh.GetVertex(loop0[i]);
				for (int j = 0; j < num; j++)
				{
					double num3 = vertex.DistanceSquared(this.Mesh.GetVertex(loop1[j]));
					if (num3 < num2)
					{
						num2 = num3;
						zero = new Index2i(i, j);
					}
				}
			}
			if (zero.a != zero.b)
			{
				int[] array = new int[num];
				int[] array2 = new int[num];
				for (int k = 0; k < num; k++)
				{
					array[k] = loop0[(zero.a + k) % num];
					array2[k] = loop1[(zero.b + k) % num];
				}
				return this.StitchLoop(array, array2, group_id);
			}
			return this.StitchLoop(loop0, loop1, group_id);
		}

		public virtual int[] StitchUnorderedEdges(List<Index2i> EdgePairs, int group_id, bool bAbortOnFailure, out bool stitch_incomplete)
		{
			int count = EdgePairs.Count;
			int[] array = new int[count * 2];
			if (!bAbortOnFailure)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = -1;
				}
			}
			stitch_incomplete = false;
			int j = 0;
			while (j < count)
			{
				Index2i index2i = EdgePairs[j];
				Index4i edge = this.Mesh.GetEdge(index2i.a);
				if (edge.d != -1)
				{
					if (bAbortOnFailure)
					{
						goto IL_169;
					}
					stitch_incomplete = true;
				}
				else
				{
					Index3i triangle = this.Mesh.GetTriangle(edge.c);
					int a = edge.a;
					int b = edge.b;
					IndexUtil.orient_tri_edge(ref a, ref b, triangle);
					Index4i edge2 = this.Mesh.GetEdge(index2i.b);
					if (edge2.d != -1)
					{
						if (bAbortOnFailure)
						{
							goto IL_169;
						}
						stitch_incomplete = true;
					}
					else
					{
						Index3i triangle2 = this.Mesh.GetTriangle(edge2.c);
						int num = edge2.a;
						int num2 = edge2.b;
						IndexUtil.orient_tri_edge(ref num, ref num2, triangle2);
						int num3 = num;
						num = num2;
						num2 = num3;
						Index3i tv = new Index3i(b, a, num2);
						Index3i tv2 = new Index3i(a, num, num2);
						int num4 = this.Mesh.AppendTriangle(tv, group_id);
						int num5 = this.Mesh.AppendTriangle(tv2, group_id);
						if (num4 < 0 || num5 < 0)
						{
							if (bAbortOnFailure)
							{
								goto IL_169;
							}
							stitch_incomplete = true;
						}
						else
						{
							array[2 * j] = num4;
							array[2 * j + 1] = num5;
						}
					}
				}
				j++;
				continue;
				IL_169:
				if (j > 0 && !this.remove_triangles(array, 2 * (j - 1)))
				{
					throw new Exception("MeshEditor.StitchLoop: failed to add all triangles, and also failed to back out changes.");
				}
				return null;
			}
			return array;
		}

		public virtual int[] StitchUnorderedEdges(List<Index2i> EdgePairs, int group_id = -1, bool bAbortOnFailure = true)
		{
			bool flag = false;
			return this.StitchUnorderedEdges(EdgePairs, group_id, bAbortOnFailure, out flag);
		}

		public virtual int[] StitchSpan(IList<int> vspan1, IList<int> vspan2, int group_id = -1)
		{
			int num = vspan1.Count;
			if (num != vspan2.Count)
			{
				throw new Exception("MeshEditor.StitchSpan: spans are not the same length!!");
			}
			num--;
			int[] array = new int[num * 2];
			int i = 0;
			while (i < num)
			{
				int num2 = vspan1[i];
				int ii = vspan1[i + 1];
				int jj = vspan2[i];
				int kk = vspan2[i + 1];
				Index3i tv = new Index3i(ii, num2, kk);
				Index3i tv2 = new Index3i(num2, jj, kk);
				int num3 = this.Mesh.AppendTriangle(tv, group_id);
				int num4 = this.Mesh.AppendTriangle(tv2, group_id);
				if (num3 >= 0 && num4 >= 0)
				{
					array[2 * i] = num3;
					array[2 * i + 1] = num4;
					i++;
				}
				else
				{
					if (i > 0 && !this.remove_triangles(array, 2 * (i - 1)))
					{
						throw new Exception("MeshEditor.StitchLoop: failed to add all triangles, and also failed to back out changes.");
					}
					return null;
				}
			}
			return array;
		}

		public bool RemoveTriangles(IList<int> triangles, bool bRemoveIsolatedVerts)
		{
			bool result = true;
			for (int i = 0; i < triangles.Count; i++)
			{
				if (triangles[i] != -1 && this.Mesh.RemoveTriangle(triangles[i], bRemoveIsolatedVerts, false) != MeshResult.Ok)
				{
					result = false;
				}
			}
			return result;
		}

		public bool RemoveTriangles(IEnumerable<int> triangles, bool bRemoveIsolatedVerts)
		{
			bool result = true;
			foreach (int tID in triangles)
			{
				if (!this.Mesh.IsTriangle(tID))
				{
					result = false;
				}
				else if (this.Mesh.RemoveTriangle(tID, bRemoveIsolatedVerts, false) != MeshResult.Ok)
				{
					result = false;
				}
			}
			return result;
		}

		public bool RemoveTriangles(Func<int, bool> selectorF, bool bRemoveIsolatedVerts)
		{
			bool result = true;
			int maxTriangleID = this.Mesh.MaxTriangleID;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (this.Mesh.IsTriangle(i) && selectorF(i) && this.Mesh.RemoveTriangle(i, bRemoveIsolatedVerts, false) != MeshResult.Ok)
				{
					result = false;
				}
			}
			return result;
		}

		public static bool RemoveTriangles(DMesh3 Mesh, IList<int> triangles, bool bRemoveIsolatedVerts = true)
		{
			return new MeshEditor(Mesh).RemoveTriangles(triangles, bRemoveIsolatedVerts);
		}

		public static bool RemoveTriangles(DMesh3 Mesh, IEnumerable<int> triangles, bool bRemoveIsolatedVerts = true)
		{
			return new MeshEditor(Mesh).RemoveTriangles(triangles, bRemoveIsolatedVerts);
		}

		public static bool RemoveIsolatedTriangles(DMesh3 mesh)
		{
			return new MeshEditor(mesh).RemoveTriangles(delegate(int tid)
			{
				Index3i triNeighbourTris = mesh.GetTriNeighbourTris(tid);
				return triNeighbourTris.a == -1 && triNeighbourTris.b == -1 && triNeighbourTris.c == -1;
			}, true);
		}

		public static int RemoveFinTriangles(DMesh3 mesh, Func<DMesh3, int, bool> removeF = null, bool bRepeatToConvergence = true)
		{
			new MeshEditor(mesh);
			int num = 0;
			List<int> list = new List<int>();
			for (;;)
			{
				foreach (int num2 in mesh.TriangleIndices())
				{
					Index3i triNeighbourTris = mesh.GetTriNeighbourTris(num2);
					if (((triNeighbourTris.a != -1) ? 1 : 0) + ((triNeighbourTris.b != -1) ? 1 : 0) + ((triNeighbourTris.c != -1) ? 1 : 0) <= 1 && (removeF == null || removeF(mesh, num2)))
					{
						list.Add(num2);
					}
				}
				if (list.Count == 0)
				{
					break;
				}
				num += list.Count;
				MeshEditor.RemoveTriangles(mesh, list, true);
				list.Clear();
				if (!bRepeatToConvergence)
				{
					return num;
				}
			}
			return num;
		}

		public bool SeparateTriangles(IEnumerable<int> triangles, bool bComputeEdgePairs, out List<Index2i> EdgePairs)
		{
			HashSet<int> hashSet = new HashSet<int>(triangles);
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			EdgePairs = null;
			HashSet<int> hashSet2 = null;
			List<Index2i> list = null;
			if (bComputeEdgePairs)
			{
				EdgePairs = new List<Index2i>();
				hashSet2 = new HashSet<int>();
				list = new List<Index2i>();
			}
			foreach (int num in triangles)
			{
				Index3i triEdges = this.Mesh.GetTriEdges(num);
				for (int i = 0; i < 3; i++)
				{
					Index2i edgeT = this.Mesh.GetEdgeT(triEdges[i]);
					if (edgeT.b == -1 || (edgeT.a == num && hashSet.Contains(edgeT.b)) || (edgeT.b == num && hashSet.Contains(edgeT.a)))
					{
						triEdges[i] = -1;
					}
				}
				for (int j = 0; j < 3; j++)
				{
					if (triEdges[j] != -1)
					{
						Index2i edgeV = this.Mesh.GetEdgeV(triEdges[j]);
						if (!dictionary.ContainsKey(edgeV.a))
						{
							dictionary[edgeV.a] = this.Mesh.AppendVertex(this.Mesh, edgeV.a);
						}
						if (!dictionary.ContainsKey(edgeV.b))
						{
							dictionary[edgeV.b] = this.Mesh.AppendVertex(this.Mesh, edgeV.b);
						}
						if (bComputeEdgePairs && !hashSet2.Contains(triEdges[j]))
						{
							hashSet2.Add(triEdges[j]);
							list.Add(edgeV);
							EdgePairs.Add(new Index2i(triEdges[j], -1));
						}
					}
				}
			}
			foreach (int tID in triangles)
			{
				Index3i triangle = this.Mesh.GetTriangle(tID);
				Index3i index3i = triangle;
				for (int k = 0; k < 3; k++)
				{
					int value;
					if (dictionary.TryGetValue(triangle[k], out value))
					{
						index3i[k] = value;
					}
				}
				if (index3i != triangle)
				{
					this.Mesh.SetTriangle(tID, index3i, true);
				}
			}
			if (bComputeEdgePairs)
			{
				for (int l = 0; l < EdgePairs.Count; l++)
				{
					Index2i index2i = list[l];
					int vA = dictionary[index2i.a];
					int vB = dictionary[index2i.b];
					int jj = this.Mesh.FindEdge(vA, vB);
					EdgePairs[l] = new Index2i(EdgePairs[l].a, jj);
				}
			}
			return true;
		}

		public List<int> DuplicateTriangles(IEnumerable<int> triangles, ref IndexMap MapV, int group_id = -1)
		{
			List<int> list = new List<int>();
			foreach (int tID in triangles)
			{
				Index3i triangle = this.Mesh.GetTriangle(tID);
				for (int i = 0; i < 3; i++)
				{
					int num = triangle[i];
					if (!MapV.Contains(num))
					{
						int value = this.Mesh.AppendVertex(this.Mesh, num);
						MapV[num] = value;
						triangle[i] = value;
					}
					else
					{
						triangle[i] = MapV[num];
					}
				}
				int item = this.Mesh.AppendTriangle(triangle, group_id);
				list.Add(item);
			}
			return list;
		}

		public void ReverseTriangles(IEnumerable<int> triangles, bool bFlipVtxNormals = true)
		{
			if (!bFlipVtxNormals)
			{
				using (IEnumerator<int> enumerator = triangles.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int tID = enumerator.Current;
						this.Mesh.ReverseTriOrientation(tID);
					}
					return;
				}
			}
			BitArray bitArray = new BitArray(this.Mesh.MaxVertexID);
			foreach (int tID2 in triangles)
			{
				this.Mesh.ReverseTriOrientation(tID2);
				Index3i triangle = this.Mesh.GetTriangle(tID2);
				for (int i = 0; i < 3; i++)
				{
					int num = triangle[i];
					if (!bitArray[num])
					{
						this.Mesh.SetVertexNormal(num, -this.Mesh.GetVertexNormal(num));
						bitArray[num] = true;
					}
				}
			}
		}

		public void DisconnectBowtie(int vid)
		{
			List<List<int>> list = new List<List<int>>();
			foreach (int num in this.Mesh.VtxTrianglesItr(vid))
			{
				Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(num);
				bool flag = false;
				foreach (List<int> list2 in list)
				{
					if (list2.Contains(triNeighbourTris.a) || list2.Contains(triNeighbourTris.b) || list2.Contains(triNeighbourTris.c))
					{
						list2.Add(num);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					List<int> item = new List<int>
					{
						num
					};
					list.Add(item);
				}
			}
			if (list.Count == 1)
			{
				return;
			}
			list.Sort(new Comparison<List<int>>(MeshEditor.bowtie_sorter));
			for (int i = 1; i < list.Count; i++)
			{
				int num2 = this.Mesh.AppendVertex(this.Mesh, vid);
				foreach (int tID in list[i])
				{
					Index3i triangle = this.Mesh.GetTriangle(tID);
					if (triangle.a == vid)
					{
						triangle.a = num2;
					}
					else if (triangle.b == vid)
					{
						triangle.b = num2;
					}
					else
					{
						triangle.c = num2;
					}
					this.Mesh.SetTriangle(tID, triangle, false);
				}
			}
		}

		private static int bowtie_sorter(List<int> l1, List<int> l2)
		{
			if (l1.Count == l2.Count)
			{
				return 0;
			}
			if (l1.Count <= l2.Count)
			{
				return 1;
			}
			return -1;
		}

		public int DisconnectAllBowties(int nMaxIters = 10)
		{
			List<int> list = new List<int>(MeshIterators.BowtieVertices(this.Mesh));
			int num = 0;
			while (list.Count > 0 && num++ < nMaxIters)
			{
				foreach (int vid in list)
				{
					this.DisconnectBowtie(vid);
				}
				list = new List<int>(MeshIterators.BowtieVertices(this.Mesh));
			}
			return list.Count;
		}

		public bool ReinsertSubmesh(DSubmesh3 sub, ref int[] new_tris, out IndexMap SubToNewV, MeshEditor.DuplicateTriBehavior eDuplicateBehavior = MeshEditor.DuplicateTriBehavior.AssertAbort)
		{
			if (sub.BaseBorderV == null)
			{
				throw new Exception("MeshEditor.ReinsertSubmesh: Submesh does not have required boundary info. Call ComputeBoundaryInfo()!");
			}
			DMesh3 subMesh = sub.SubMesh;
			bool result = true;
			IndexFlagSet indexFlagSet = new IndexFlagSet(subMesh.MaxVertexID, subMesh.TriangleCount / 2);
			SubToNewV = new IndexMap(subMesh.MaxVertexID, subMesh.VertexCount);
			int num = 0;
			int maxTriangleID = subMesh.MaxTriangleID;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (subMesh.IsTriangle(i))
				{
					Index3i triangle = subMesh.GetTriangle(i);
					int triangleGroup = subMesh.GetTriangleGroup(i);
					Index3i zero = Index3i.Zero;
					for (int j = 0; j < 3; j++)
					{
						int num2 = triangle[j];
						int num3 = -1;
						if (!indexFlagSet[num2])
						{
							if (subMesh.IsBoundaryVertex(num2))
							{
								int num4 = (num2 < sub.SubToBaseV.size) ? sub.SubToBaseV[num2] : -1;
								if (num4 >= 0 && this.Mesh.IsVertex(num4) && sub.BaseBorderV[num4] && this.Mesh.IsBoundaryVertex(num4))
								{
									num3 = num4;
								}
							}
							if (num3 == -1)
							{
								num3 = this.Mesh.AppendVertex(subMesh, num2);
							}
							SubToNewV[num2] = num3;
							indexFlagSet[num2] = true;
						}
						else
						{
							num3 = SubToNewV[num2];
						}
						zero[j] = num3;
					}
					if (eDuplicateBehavior != MeshEditor.DuplicateTriBehavior.AssertContinue)
					{
						int num5 = this.Mesh.FindTriangle(zero.a, zero.b, zero.c);
						if (num5 != -1)
						{
							if (eDuplicateBehavior == MeshEditor.DuplicateTriBehavior.AssertAbort)
							{
								return false;
							}
							if (eDuplicateBehavior == MeshEditor.DuplicateTriBehavior.UseExisting)
							{
								if (new_tris != null)
								{
									new_tris[num++] = num5;
									goto IL_1DC;
								}
								goto IL_1DC;
							}
							else if (eDuplicateBehavior == MeshEditor.DuplicateTriBehavior.Replace)
							{
								this.Mesh.RemoveTriangle(num5, false, false);
							}
						}
					}
					int num6 = this.Mesh.AppendTriangle(zero, triangleGroup);
					if (!this.Mesh.IsTriangle(num6))
					{
						result = false;
					}
					if (new_tris != null)
					{
						new_tris[num++] = num6;
					}
				}
				IL_1DC:;
			}
			return result;
		}

		public bool AppendMesh(IMesh appendMesh, int appendGID = -1)
		{
			int[] array;
			return this.AppendMesh(appendMesh, out array, appendGID);
		}

		public bool AppendMesh(IMesh appendMesh, out int[] mapV, int appendGID = -1)
		{
			mapV = new int[appendMesh.MaxVertexID];
			foreach (int num in appendMesh.VertexIndices())
			{
				NewVertexInfo vertexAll = appendMesh.GetVertexAll(num);
				int num2 = this.Mesh.AppendVertex(vertexAll);
				mapV[num] = num2;
			}
			foreach (int i in appendMesh.TriangleIndices())
			{
				Index3i triangle = appendMesh.GetTriangle(i);
				triangle.a = mapV[triangle.a];
				triangle.b = mapV[triangle.b];
				triangle.c = mapV[triangle.c];
				int gid = appendMesh.GetTriangleGroup(i);
				if (appendGID >= 0)
				{
					gid = appendGID;
				}
				this.Mesh.AppendTriangle(triangle, gid);
			}
			return true;
		}

		public static DMesh3 Combine(params IMesh[] appendMeshes)
		{
			DMesh3 dmesh = new DMesh3(true, false, false, false);
			MeshEditor meshEditor = new MeshEditor(dmesh);
			foreach (IMesh appendMesh in appendMeshes)
			{
				meshEditor.AppendMesh(appendMesh, dmesh.AllocateTriangleGroup());
			}
			return dmesh;
		}

		public static void Append(DMesh3 appendTo, DMesh3 append)
		{
			new MeshEditor(appendTo).AppendMesh(append, appendTo.AllocateTriangleGroup());
		}

		public bool AppendMesh(IMesh appendMesh, IndexMap mergeMapV, out int[] mapV, int appendGID = -1)
		{
			mapV = new int[appendMesh.MaxVertexID];
			foreach (int num in appendMesh.VertexIndices())
			{
				if (mergeMapV.Contains(num))
				{
					mapV[num] = mergeMapV[num];
				}
				else
				{
					NewVertexInfo vertexAll = appendMesh.GetVertexAll(num);
					int num2 = this.Mesh.AppendVertex(vertexAll);
					mapV[num] = num2;
				}
			}
			foreach (int i in appendMesh.TriangleIndices())
			{
				Index3i triangle = appendMesh.GetTriangle(i);
				triangle.a = mapV[triangle.a];
				triangle.b = mapV[triangle.b];
				triangle.c = mapV[triangle.c];
				int gid = appendMesh.GetTriangleGroup(i);
				if (appendGID >= 0)
				{
					gid = appendGID;
				}
				this.Mesh.AppendTriangle(triangle, gid);
			}
			return true;
		}

		public void AppendBox(Frame3f frame, float size)
		{
			this.AppendBox(frame, size * Vector3f.One);
		}

		public void AppendBox(Frame3f frame, Vector3f size)
		{
			this.AppendBox(frame, size, Colorf.White);
		}

		public void AppendBox(Frame3f frame, Vector3f size, Colorf color)
		{
			TrivialBox3Generator trivialBox3Generator = new TrivialBox3Generator();
			trivialBox3Generator.Box = new Box3d(frame, size);
			trivialBox3Generator.NoSharedVertices = false;
			trivialBox3Generator.Generate();
			DMesh3 dmesh = new DMesh3(true, false, false, false);
			trivialBox3Generator.MakeMesh(dmesh);
			if (this.Mesh.HasVertexColors)
			{
				dmesh.EnableVertexColors(color);
			}
			this.AppendMesh(dmesh, this.Mesh.AllocateTriangleGroup());
		}

		public void AppendLine(Segment3d seg, float size)
		{
			Frame3f frame = new Frame3f(seg.Center);
			frame.AlignAxis(2, (Vector3f)seg.Direction);
			this.AppendBox(frame, new Vector3f((double)size, (double)size, seg.Extent));
		}

		public void AppendLine(Segment3d seg, float size, Colorf color)
		{
			Frame3f frame = new Frame3f(seg.Center);
			frame.AlignAxis(2, (Vector3f)seg.Direction);
			this.AppendBox(frame, new Vector3f((double)size, (double)size, seg.Extent), color);
		}

		public static void AppendBox(DMesh3 mesh, Vector3d pos, float size)
		{
			new MeshEditor(mesh).AppendBox(new Frame3f(pos), size);
		}

		public static void AppendBox(DMesh3 mesh, Vector3d pos, float size, Colorf color)
		{
			new MeshEditor(mesh).AppendBox(new Frame3f(pos), size * Vector3f.One, color);
		}

		public static void AppendBox(DMesh3 mesh, Vector3d pos, Vector3d normal, float size)
		{
			new MeshEditor(mesh).AppendBox(new Frame3f(pos, normal), size);
		}

		public static void AppendBox(DMesh3 mesh, Vector3d pos, Vector3d normal, float size, Colorf color)
		{
			new MeshEditor(mesh).AppendBox(new Frame3f(pos, normal), size * Vector3f.One, color);
		}

		public static void AppendBox(DMesh3 mesh, Frame3f frame, Vector3f size, Colorf color)
		{
			new MeshEditor(mesh).AppendBox(frame, size, color);
		}

		public static void AppendLine(DMesh3 mesh, Segment3d seg, float size)
		{
			Frame3f frame = new Frame3f(seg.Center);
			frame.AlignAxis(2, (Vector3f)seg.Direction);
			new MeshEditor(mesh).AppendBox(frame, new Vector3f((double)size, (double)size, seg.Extent));
		}

		public void AppendPathSolid(IEnumerable<Vector3d> vertices, double radius, Colorf color)
		{
			DMesh3 dmesh = new TubeGenerator
			{
				Vertices = new List<Vector3d>(vertices),
				Polygon = Polygon2d.MakeCircle(radius, 6, 0.0),
				NoSharedVertices = false
			}.Generate().MakeDMesh();
			if (this.Mesh.HasVertexColors)
			{
				dmesh.EnableVertexColors(color);
			}
			this.AppendMesh(dmesh, this.Mesh.AllocateTriangleGroup());
		}

		public bool RemoveAllBowtieVertices(bool bRepeatUntilClean)
		{
			int num = 0;
			do
			{
				List<int> list = new List<int>();
				foreach (int num2 in this.Mesh.VertexIndices())
				{
					if (this.Mesh.IsBowtieVertex(num2))
					{
						list.Add(num2);
					}
				}
				if (list.Count == 0)
				{
					break;
				}
				foreach (int vID in list)
				{
					this.Mesh.RemoveVertex(vID, true, false);
					num++;
				}
			}
			while (bRepeatUntilClean);
			return num > 0;
		}

		public int RemoveUnusedVertices()
		{
			int num = 0;
			int maxVertexID = this.Mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (this.Mesh.IsVertex(i) && this.Mesh.GetVtxEdgeCount(i) == 0)
				{
					this.Mesh.RemoveVertex(i, true, false);
					num++;
				}
			}
			return num;
		}

		public static int RemoveUnusedVertices(DMesh3 mesh)
		{
			return new MeshEditor(mesh).RemoveUnusedVertices();
		}

		public int RemoveSmallComponents(double min_volume, double min_area)
		{
			MeshConnectedComponents meshConnectedComponents = new MeshConnectedComponents(this.Mesh);
			meshConnectedComponents.FindConnectedT();
			if (meshConnectedComponents.Count == 1)
			{
				return 0;
			}
			int num = 0;
			foreach (MeshConnectedComponents.Component component in meshConnectedComponents.Components)
			{
				Vector2d vector2d = MeshMeasurements.VolumeArea(this.Mesh, component.Indices, new Func<int, Vector3d>(this.Mesh.GetVertex));
				if (vector2d.x < min_volume || vector2d.y < min_area)
				{
					MeshEditor.RemoveTriangles(this.Mesh, component.Indices, true);
					num++;
				}
			}
			return num;
		}

		public static int RemoveSmallComponents(DMesh3 mesh, double min_volume, double min_area)
		{
			return new MeshEditor(mesh).RemoveSmallComponents(min_volume, min_area);
		}

		private bool remove_triangles(int[] tri_list, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (this.Mesh.IsTriangle(tri_list[i]) && this.Mesh.RemoveTriangle(tri_list[i], false, false) != MeshResult.Ok)
				{
					return false;
				}
			}
			return true;
		}

		public DMesh3 Mesh;

		public enum DuplicateTriBehavior
		{
			AssertContinue,
			AssertAbort,
			UseExisting,
			Replace
		}
	}
}
