using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class MeshMeshCut
	{
		public void Compute()
		{
			double cellSize = this.Target.CachedBounds.MaxDim / 64.0;
			this.PointHash = new PointHashGrid3d<int>(cellSize, -1);
			foreach (int num in this.Target.VertexIndices())
			{
				Vector3d vertex = this.Target.GetVertex(num);
				int num2 = this.find_existing_vertex(vertex);
				if (num2 != -1)
				{
					Console.WriteLine("VERTEX {0} IS DUPLICATE OF {1}!", num, num2);
				}
				this.PointHash.InsertPointUnsafe(num, vertex);
			}
			this.initialize();
			this.find_segments();
			this.insert_face_vertices();
			this.insert_edge_vertices();
			this.connect_edges();
			foreach (MeshMeshCut.SegmentVtx segmentVtx in this.SegVertices)
			{
				this.SegmentInsertVertices.Add(segmentVtx.vtx_id);
			}
		}

		public void RemoveContained()
		{
			DMeshAABBTree3 spatial = new DMeshAABBTree3(this.CutMesh, true);
			spatial.WindingNumber(Vector3d.Zero);
			SafeListBuilder<int> removeT = new SafeListBuilder<int>();
			gParallel.ForEach<int>(this.Target.TriangleIndices(), delegate(int tid)
			{
				Vector3d triCentroid = this.Target.GetTriCentroid(tid);
				if (spatial.WindingNumber(triCentroid) > 0.9)
				{
					removeT.SafeAdd(tid);
				}
			});
			MeshEditor.RemoveTriangles(this.Target, removeT.Result, true);
			this.CutVertices = new List<int>();
			foreach (int num in this.SegmentInsertVertices)
			{
				if (this.Target.IsVertex(num))
				{
					this.CutVertices.Add(num);
				}
			}
		}

		public void AppendSegments(double r)
		{
			foreach (MeshMeshCut.IntersectSegment intersectSegment in this.Segments)
			{
				Segment3d seg = new Segment3d(intersectSegment.v0.v, intersectSegment.v1.v);
				if (this.Target.FindEdge(intersectSegment.v0.vtx_id, intersectSegment.v1.vtx_id) == -1)
				{
					MeshEditor.AppendLine(this.Target, seg, (float)r);
				}
			}
		}

		public void ColorFaces()
		{
			int num = 1;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (int key in this.SubFaces.Keys)
			{
				dictionary[key] = num++;
			}
			this.Target.EnableTriangleGroups(0);
			foreach (int num2 in this.Target.TriangleIndices())
			{
				if (this.ParentFaces.ContainsKey(num2))
				{
					this.Target.SetTriangleGroup(num2, dictionary[this.ParentFaces[num2]]);
				}
				else if (this.SubFaces.ContainsKey(num2))
				{
					this.Target.SetTriangleGroup(num2, dictionary[num2]);
				}
			}
		}

		private void initialize()
		{
			this.BaseFaceCentroids = new Vector3d[this.Target.MaxTriangleID];
			this.BaseFaceNormals = new Vector3d[this.Target.MaxTriangleID];
			double num = 0.0;
			foreach (int num2 in this.Target.TriangleIndices())
			{
				this.Target.GetTriInfo(num2, out this.BaseFaceNormals[num2], out num, out this.BaseFaceCentroids[num2]);
			}
			this.SegVertices = new List<MeshMeshCut.SegmentVtx>();
			this.EdgeVertices = new Dictionary<int, List<MeshMeshCut.SegmentVtx>>();
			this.FaceVertices = new Dictionary<int, List<MeshMeshCut.SegmentVtx>>();
			this.SubFaces = new Dictionary<int, HashSet<int>>();
			this.ParentFaces = new Dictionary<int, int>();
			this.SegmentInsertVertices = new HashSet<int>();
			this.VIDToSegVtxMap = new Dictionary<int, MeshMeshCut.SegmentVtx>();
		}

		private void find_segments()
		{
			Dictionary<Vector3d, MeshMeshCut.SegmentVtx> dictionary = new Dictionary<Vector3d, MeshMeshCut.SegmentVtx>();
			DMeshAABBTree3 dmeshAABBTree = new DMeshAABBTree3(this.Target, true);
			DMeshAABBTree3 otherTree = new DMeshAABBTree3(this.CutMesh, true);
			DMeshAABBTree3.IntersectionsQueryResult intersectionsQueryResult = dmeshAABBTree.FindAllIntersections(otherTree, null);
			this.Segments = new MeshMeshCut.IntersectSegment[intersectionsQueryResult.Segments.Count];
			for (int i = 0; i < this.Segments.Length; i++)
			{
				DMeshAABBTree3.SegmentIntersection segmentIntersection = intersectionsQueryResult.Segments[i];
				Vector3dTuple2 vector3dTuple = new Vector3dTuple2(segmentIntersection.point0, segmentIntersection.point1);
				MeshMeshCut.IntersectSegment intersectSegment = new MeshMeshCut.IntersectSegment
				{
					base_tid = segmentIntersection.t0
				};
				this.Segments[i] = intersectSegment;
				for (int j = 0; j < 2; j++)
				{
					Vector3d vector3d = vector3dTuple[j];
					MeshMeshCut.SegmentVtx segmentVtx;
					if (dictionary.TryGetValue(vector3d, out segmentVtx))
					{
						intersectSegment[j] = segmentVtx;
					}
					else
					{
						segmentVtx = new MeshMeshCut.SegmentVtx
						{
							v = vector3d
						};
						this.SegVertices.Add(segmentVtx);
						dictionary[vector3d] = segmentVtx;
						intersectSegment[j] = segmentVtx;
						int num = this.find_existing_vertex(segmentIntersection.point0);
						if (num >= 0)
						{
							segmentVtx.initial_type = (segmentVtx.type = 0);
							segmentVtx.elem_id = num;
							segmentVtx.vtx_id = num;
							this.VIDToSegVtxMap[segmentVtx.vtx_id] = segmentVtx;
						}
						else
						{
							Triangle3d triangle3d = default(Triangle3d);
							this.Target.GetTriVertices(segmentIntersection.t0, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
							Index3i triangle = this.Target.GetTriangle(segmentIntersection.t0);
							int num2 = this.on_edge(ref triangle3d, ref vector3d);
							if (num2 >= 0)
							{
								segmentVtx.initial_type = (segmentVtx.type = 1);
								segmentVtx.elem_id = this.Target.FindEdge(triangle[num2], triangle[(num2 + 1) % 3]);
								this.add_edge_vtx(segmentVtx.elem_id, segmentVtx);
							}
							else
							{
								segmentVtx.initial_type = (segmentVtx.type = 2);
								segmentVtx.elem_id = segmentIntersection.t0;
								this.add_face_vtx(segmentVtx.elem_id, segmentVtx);
							}
						}
					}
				}
			}
		}

		private void insert_face_vertices()
		{
			while (this.FaceVertices.Count > 0)
			{
				KeyValuePair<int, List<MeshMeshCut.SegmentVtx>> keyValuePair = this.FaceVertices.First<KeyValuePair<int, List<MeshMeshCut.SegmentVtx>>>();
				int key = keyValuePair.Key;
				List<MeshMeshCut.SegmentVtx> value = keyValuePair.Value;
				MeshMeshCut.SegmentVtx segmentVtx = value[value.Count - 1];
				value.RemoveAt(value.Count - 1);
				DMesh3.PokeTriangleInfo pokeTriangleInfo;
				if (this.Target.PokeTriangle(key, out pokeTriangleInfo) != MeshResult.Ok)
				{
					throw new Exception("shit");
				}
				int new_vid = pokeTriangleInfo.new_vid;
				this.Target.SetVertex(new_vid, segmentVtx.v);
				segmentVtx.vtx_id = new_vid;
				this.VIDToSegVtxMap[segmentVtx.vtx_id] = segmentVtx;
				this.PointHash.InsertPoint(segmentVtx.vtx_id, segmentVtx.v);
				this.FaceVertices.Remove(key);
				Index3i new_edges = pokeTriangleInfo.new_edges;
				Index3i pokeTris = new Index3i(key, pokeTriangleInfo.new_t1, pokeTriangleInfo.new_t2);
				foreach (MeshMeshCut.SegmentVtx segmentVtx2 in value)
				{
					this.update_from_poke(segmentVtx2, new_edges, pokeTris);
					if (segmentVtx2.type == 1)
					{
						this.add_edge_vtx(segmentVtx2.elem_id, segmentVtx2);
					}
					else if (segmentVtx2.type == 2)
					{
						this.add_face_vtx(segmentVtx2.elem_id, segmentVtx2);
					}
				}
				this.add_poke_subfaces(key, ref pokeTriangleInfo);
			}
		}

		private void update_from_poke(MeshMeshCut.SegmentVtx sv, Index3i pokeEdges, Index3i pokeTris)
		{
			int num = this.find_existing_vertex(sv.v);
			if (num >= 0)
			{
				sv.type = 0;
				sv.elem_id = num;
				sv.vtx_id = num;
				this.VIDToSegVtxMap[sv.vtx_id] = sv;
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				if (this.is_on_edge(pokeEdges[i], sv.v))
				{
					sv.type = 1;
					sv.elem_id = pokeEdges[i];
					return;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				if (this.is_in_triangle(pokeTris[j], sv.v))
				{
					sv.type = 2;
					sv.elem_id = pokeTris[j];
					return;
				}
			}
			Console.WriteLine("unsorted vertex!");
			sv.elem_id = pokeTris.a;
		}

		private void insert_edge_vertices()
		{
			while (this.EdgeVertices.Count > 0)
			{
				KeyValuePair<int, List<MeshMeshCut.SegmentVtx>> keyValuePair = this.EdgeVertices.First<KeyValuePair<int, List<MeshMeshCut.SegmentVtx>>>();
				int key = keyValuePair.Key;
				List<MeshMeshCut.SegmentVtx> value = keyValuePair.Value;
				MeshMeshCut.SegmentVtx segmentVtx = value[value.Count - 1];
				value.RemoveAt(value.Count - 1);
				Index2i edgeT = this.Target.GetEdgeT(key);
				DMesh3.EdgeSplitInfo edgeSplitInfo;
				if (this.Target.SplitEdge(key, out edgeSplitInfo, 0.5) != MeshResult.Ok)
				{
					throw new Exception("insert_edge_vertices: split failed!");
				}
				int vNew = edgeSplitInfo.vNew;
				Index2i splitEdges = new Index2i(key, edgeSplitInfo.eNewBN);
				this.Target.SetVertex(vNew, segmentVtx.v);
				segmentVtx.vtx_id = vNew;
				this.VIDToSegVtxMap[segmentVtx.vtx_id] = segmentVtx;
				this.PointHash.InsertPoint(segmentVtx.vtx_id, segmentVtx.v);
				this.EdgeVertices.Remove(key);
				foreach (MeshMeshCut.SegmentVtx segmentVtx2 in value)
				{
					this.update_from_split(segmentVtx2, splitEdges);
					if (segmentVtx2.type == 1)
					{
						this.add_edge_vtx(segmentVtx2.elem_id, segmentVtx2);
					}
				}
				this.add_split_subfaces(edgeT, ref edgeSplitInfo);
			}
		}

		private void update_from_split(MeshMeshCut.SegmentVtx sv, Index2i splitEdges)
		{
			int num = this.find_existing_vertex(sv.v);
			if (num >= 0)
			{
				sv.type = 0;
				sv.elem_id = num;
				sv.vtx_id = num;
				this.VIDToSegVtxMap[sv.vtx_id] = sv;
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (this.is_on_edge(splitEdges[i], sv.v))
				{
					sv.type = 1;
					sv.elem_id = splitEdges[i];
					return;
				}
			}
			throw new Exception("update_from_split: unsortable vertex?");
		}

		private void connect_edges()
		{
			int num = this.Segments.Length;
			for (int i = 0; i < num; i++)
			{
				MeshMeshCut.IntersectSegment intersectSegment = this.Segments[i];
				if (intersectSegment.v0 != intersectSegment.v1 && intersectSegment.v0.vtx_id != intersectSegment.v1.vtx_id)
				{
					int vtx_id = intersectSegment.v0.vtx_id;
					int vtx_id2 = intersectSegment.v1.vtx_id;
					if (vtx_id == -1 || vtx_id2 == -1)
					{
						throw new Exception("segment vertex is not defined?");
					}
					if (this.Target.FindEdge(vtx_id, vtx_id2) == -1)
					{
						try
						{
							this.insert_segment(intersectSegment);
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		private void insert_segment(MeshMeshCut.IntersectSegment seg)
		{
			List<int> regionTris = this.get_all_baseface_tris(seg.base_tid);
			RegionOperator regionOperator = new RegionOperator(this.Target, regionTris, null);
			Vector3d vector3d = this.BaseFaceNormals[seg.base_tid];
			Vector3d c = this.BaseFaceCentroids[seg.base_tid];
			Vector3d e0;
			Vector3d e1;
			Vector3d.MakePerpVectors(ref vector3d, out e0, out e1);
			DMesh3 subMesh = regionOperator.Region.SubMesh;
			MeshTransforms.PerVertexTransform(subMesh, delegate(Vector3d v)
			{
				v -= c;
				return new Vector3d(v.Dot(e0), v.Dot(e1), 0.0);
			});
			Vector3d v5 = seg.v0.v;
			Vector3d v2 = seg.v1.v;
			v5 -= c;
			v2 -= c;
			Vector2d v3 = new Vector2d(v5.Dot(e0), v5.Dot(e1));
			Vector2d v4 = new Vector2d(v2.Dot(e0), v2.Dot(e1));
			PolyLine2d polyLine2d = new PolyLine2d();
			polyLine2d.AppendVertex(v3);
			polyLine2d.AppendVertex(v4);
			MeshInsertUVPolyCurve meshInsertUVPolyCurve = new MeshInsertUVPolyCurve(subMesh, polyLine2d);
			meshInsertUVPolyCurve.Apply();
			MeshVertexSelection meshVertexSelection = new MeshVertexSelection(subMesh);
			meshVertexSelection.SelectEdgeVertices(meshInsertUVPolyCurve.OnCutEdges);
			MeshTransforms.PerVertexTransform(subMesh, (Vector3d v) => c + v.x * e0 + v.y * e1);
			regionOperator.BackPropropagate(true);
			foreach (int index in meshVertexSelection)
			{
				this.SegmentInsertVertices.Add(regionOperator.ReinsertSubToBaseMapV[index]);
			}
			this.add_regionop_subfaces(seg.base_tid, regionOperator);
		}

		private void add_edge_vtx(int eid, MeshMeshCut.SegmentVtx vtx)
		{
			List<MeshMeshCut.SegmentVtx> list;
			if (this.EdgeVertices.TryGetValue(eid, out list))
			{
				list.Add(vtx);
				return;
			}
			list = new List<MeshMeshCut.SegmentVtx>
			{
				vtx
			};
			this.EdgeVertices[eid] = list;
		}

		private void add_face_vtx(int tid, MeshMeshCut.SegmentVtx vtx)
		{
			List<MeshMeshCut.SegmentVtx> list;
			if (this.FaceVertices.TryGetValue(tid, out list))
			{
				list.Add(vtx);
				return;
			}
			list = new List<MeshMeshCut.SegmentVtx>
			{
				vtx
			};
			this.FaceVertices[tid] = list;
		}

		private void add_poke_subfaces(int tid, ref DMesh3.PokeTriangleInfo pokeInfo)
		{
			int num = this.get_parent(tid);
			HashSet<int> subfaces = this.get_subfaces(num);
			if (tid != num)
			{
				this.add_subface(subfaces, num, tid);
			}
			this.add_subface(subfaces, num, pokeInfo.new_t1);
			this.add_subface(subfaces, num, pokeInfo.new_t2);
		}

		private void add_split_subfaces(Index2i origTris, ref DMesh3.EdgeSplitInfo splitInfo)
		{
			int num = this.get_parent(origTris.a);
			HashSet<int> subfaces = this.get_subfaces(num);
			if (origTris.a != num)
			{
				this.add_subface(subfaces, num, origTris.a);
			}
			this.add_subface(subfaces, num, splitInfo.eNewT2);
			if (origTris.b != -1)
			{
				int num2 = this.get_parent(origTris.b);
				HashSet<int> subfaces2 = this.get_subfaces(num2);
				if (origTris.b != num2)
				{
					this.add_subface(subfaces2, num2, origTris.b);
				}
				this.add_subface(subfaces2, num2, splitInfo.eNewT3);
			}
		}

		private void add_regionop_subfaces(int parent, RegionOperator op)
		{
			HashSet<int> subfaces = this.get_subfaces(parent);
			foreach (int num in op.CurrentBaseTriangles)
			{
				if (num != parent)
				{
					this.add_subface(subfaces, parent, num);
				}
			}
		}

		private int get_parent(int tid)
		{
			int result;
			if (!this.ParentFaces.TryGetValue(tid, out result))
			{
				result = tid;
			}
			return result;
		}

		private HashSet<int> get_subfaces(int parent)
		{
			HashSet<int> hashSet;
			if (!this.SubFaces.TryGetValue(parent, out hashSet))
			{
				hashSet = new HashSet<int>();
				this.SubFaces[parent] = hashSet;
			}
			return hashSet;
		}

		private void add_subface(HashSet<int> subfaces, int parent, int tid)
		{
			subfaces.Add(tid);
			this.ParentFaces[tid] = parent;
		}

		private List<int> get_all_baseface_tris(int base_tid)
		{
			return new List<int>(this.get_subfaces(base_tid))
			{
				base_tid
			};
		}

		private bool is_inserted_free_edge(int eid)
		{
			Index2i edgeT = this.Target.GetEdgeT(eid);
			if (this.get_parent(edgeT.a) != this.get_parent(edgeT.b))
			{
				return false;
			}
			throw new Exception("not done yet!");
		}

		protected int on_edge(ref Triangle3d tri, ref Vector3d v)
		{
			Segment3d segment3d = new Segment3d(tri.V0, tri.V1);
			if (segment3d.DistanceSquared(v) < this.VertexSnapTol * this.VertexSnapTol)
			{
				return 0;
			}
			Segment3d segment3d2 = new Segment3d(tri.V1, tri.V2);
			if (segment3d2.DistanceSquared(v) < this.VertexSnapTol * this.VertexSnapTol)
			{
				return 1;
			}
			Segment3d segment3d3 = new Segment3d(tri.V2, tri.V0);
			if (segment3d3.DistanceSquared(v) < this.VertexSnapTol * this.VertexSnapTol)
			{
				return 2;
			}
			return -1;
		}

		protected int on_edge_eid(int tid, Vector3d v)
		{
			Index3i triangle = this.Target.GetTriangle(tid);
			Triangle3d triangle3d = default(Triangle3d);
			this.Target.GetTriVertices(tid, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			int num = this.on_edge(ref triangle3d, ref v);
			if (num < 0)
			{
				return -1;
			}
			return this.Target.FindEdge(triangle[num], triangle[(num + 1) % 3]);
		}

		protected bool is_on_edge(int eid, Vector3d v)
		{
			Index2i edgeV = this.Target.GetEdgeV(eid);
			Segment3d segment3d = new Segment3d(this.Target.GetVertex(edgeV.a), this.Target.GetVertex(edgeV.b));
			return segment3d.DistanceSquared(v) < this.VertexSnapTol * this.VertexSnapTol;
		}

		protected bool is_in_triangle(int tid, Vector3d v)
		{
			Triangle3d triangle3d = default(Triangle3d);
			this.Target.GetTriVertices(tid, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			Vector3d vector3d = triangle3d.BarycentricCoords(v);
			return vector3d.x >= 0.0 && vector3d.y >= 0.0 && vector3d.z >= 0.0 && vector3d.x < 1.0 && vector3d.y <= 1.0 && vector3d.z <= 1.0;
		}

		protected int find_existing_vertex(Vector3d pt)
		{
			return this.find_nearest_vertex(pt, this.VertexSnapTol, -1);
		}

		protected int find_nearest_vertex(Vector3d pt, double searchRadius, int ignore_vid = -1)
		{
			KeyValuePair<int, double> keyValuePair = (ignore_vid == -1) ? this.PointHash.FindNearestInRadius(pt, searchRadius, (int b) => pt.DistanceSquared(this.Target.GetVertex(b)), null) : this.PointHash.FindNearestInRadius(pt, searchRadius, (int b) => pt.DistanceSquared(this.Target.GetVertex(b)), (int vid) => vid == ignore_vid);
			if (keyValuePair.Key == this.PointHash.InvalidValue)
			{
				return -1;
			}
			return keyValuePair.Key;
		}

		public DMesh3 Target;

		public DMesh3 CutMesh;

		private PointHashGrid3d<int> PointHash;

		public double VertexSnapTol = 1E-05;

		public List<int> CutVertices;

		private List<MeshMeshCut.SegmentVtx> SegVertices;

		private Dictionary<int, MeshMeshCut.SegmentVtx> VIDToSegVtxMap;

		private Dictionary<int, List<MeshMeshCut.SegmentVtx>> FaceVertices;

		private Dictionary<int, List<MeshMeshCut.SegmentVtx>> EdgeVertices;

		private MeshMeshCut.IntersectSegment[] Segments;

		private Vector3d[] BaseFaceCentroids;

		private Vector3d[] BaseFaceNormals;

		private Dictionary<int, HashSet<int>> SubFaces;

		private Dictionary<int, int> ParentFaces;

		private HashSet<int> SegmentInsertVertices;

		private class SegmentVtx
		{
			public Vector3d v;

			public int type = -1;

			public int initial_type = -1;

			public int vtx_id = -1;

			public int elem_id = -1;
		}

		private class IntersectSegment
		{
			public MeshMeshCut.SegmentVtx this[int key]
			{
				get
				{
					if (key != 0)
					{
						return this.v1;
					}
					return this.v0;
				}
				set
				{
					if (key == 0)
					{
						this.v0 = value;
						return;
					}
					this.v1 = value;
				}
			}

			public int base_tid;

			public MeshMeshCut.SegmentVtx v0;

			public MeshMeshCut.SegmentVtx v1;
		}
	}
}
