using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace g3
{
	public abstract class DGraph
	{
		public DGraph()
		{
			this.vertex_edges = new DVector<List<int>>();
			this.vertices_refcount = new RefCountVector();
			this.edges = new DVector<int>();
			this.edges_refcount = new RefCountVector();
			this.max_group_id = 0;
		}

		protected void updateTimeStamp(bool bShapeChange)
		{
			this.timestamp++;
			if (bShapeChange)
			{
				this.shape_timestamp++;
			}
		}

		public int Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		public int ShapeTimestamp
		{
			get
			{
				return this.shape_timestamp;
			}
		}

		public int VertexCount
		{
			get
			{
				return this.vertices_refcount.count;
			}
		}

		public int EdgeCount
		{
			get
			{
				return this.edges_refcount.count;
			}
		}

		public int MaxVertexID
		{
			get
			{
				return this.vertices_refcount.max_index;
			}
		}

		public int MaxEdgeID
		{
			get
			{
				return this.edges_refcount.max_index;
			}
		}

		public int MaxGroupID
		{
			get
			{
				return this.max_group_id;
			}
		}

		public bool IsVertex(int vID)
		{
			return this.vertices_refcount.isValid(vID);
		}

		public bool IsEdge(int eID)
		{
			return this.edges_refcount.isValid(eID);
		}

		public ReadOnlyCollection<int> GetVtxEdges(int vID)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return null;
			}
			return this.vertex_edges[vID].AsReadOnly();
		}

		public int GetVtxEdgeCount(int vID)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return -1;
			}
			return this.vertex_edges[vID].Count;
		}

		public int GetMaxVtxEdgeCount()
		{
			int num = 0;
			foreach (object obj in this.vertices_refcount)
			{
				int i = (int)obj;
				num = Math.Max(num, this.vertex_edges[i].Count);
			}
			return num;
		}

		public int GetEdgeGroup(int eid)
		{
			if (!this.edges_refcount.isValid(eid))
			{
				return -1;
			}
			return this.edges[3 * eid + 2];
		}

		public void SetEdgeGroup(int eid, int group_id)
		{
			if (this.edges_refcount.isValid(eid))
			{
				this.edges[3 * eid + 2] = group_id;
				this.max_group_id = Math.Max(this.max_group_id, group_id + 1);
				this.updateTimeStamp(false);
			}
		}

		public int AllocateEdgeGroup()
		{
			int num = this.max_group_id;
			this.max_group_id = num + 1;
			return num;
		}

		public Index2i GetEdgeV(int eID)
		{
			if (!this.edges_refcount.isValid(eID))
			{
				return DGraph.InvalidEdgeV;
			}
			return new Index2i(this.edges[3 * eID], this.edges[3 * eID + 1]);
		}

		public Index3i GetEdge(int eID)
		{
			int num = 3 * eID;
			if (!this.edges_refcount.isValid(eID))
			{
				return DGraph.InvalidEdge3;
			}
			return new Index3i(this.edges[num], this.edges[num + 1], this.edges[num + 2]);
		}

		protected int append_vertex_internal()
		{
			int num = this.vertices_refcount.allocate();
			this.vertex_edges.insert(new List<int>(), num);
			this.updateTimeStamp(true);
			return num;
		}

		public int AppendEdge(int v0, int v1, int gid = -1)
		{
			return this.AppendEdge(new Index2i(v0, v1), gid);
		}

		public int AppendEdge(Index2i ev, int gid = -1)
		{
			if (!this.IsVertex(ev[0]) || !this.IsVertex(ev[1]))
			{
				return -1;
			}
			if (ev[0] == ev[1])
			{
				return -1;
			}
			if (this.FindEdge(ev[0], ev[1]) != -1)
			{
				return -2;
			}
			this.vertices_refcount.increment(ev[0], 1);
			this.vertices_refcount.increment(ev[1], 1);
			this.max_group_id = Math.Max(this.max_group_id, gid + 1);
			int result = this.add_edge(ev[0], ev[1], gid);
			this.updateTimeStamp(true);
			return result;
		}

		protected int add_edge(int a, int b, int gid)
		{
			if (b < a)
			{
				int num = b;
				b = a;
				a = num;
			}
			int num2 = this.edges_refcount.allocate();
			int num3 = 3 * num2;
			this.edges.insert(a, num3);
			this.edges.insert(b, num3 + 1);
			this.edges.insert(gid, num3 + 2);
			this.vertex_edges[a].Add(num2);
			this.vertex_edges[b].Add(num2);
			return num2;
		}

		public IEnumerable<int> VertexIndices()
		{
			foreach (object obj in this.vertices_refcount)
			{
				int num = (int)obj;
				yield return num;
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<int> EdgeIndices()
		{
			foreach (object obj in this.edges_refcount)
			{
				int num = (int)obj;
				yield return num;
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<Index3i> Edges()
		{
			foreach (object obj in this.edges_refcount)
			{
				int num = (int)obj;
				int num2 = 3 * num;
				yield return new Index3i(this.edges[num2], this.edges[num2 + 1], this.edges[num2 + 2]);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<int> VtxVerticesItr(int vID)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				List<int> vedges = this.vertex_edges[vID];
				int N = vedges.Count;
				int num;
				for (int i = 0; i < N; i = num)
				{
					yield return this.edge_other_v(vedges[i], vID);
					num = i + 1;
				}
				vedges = null;
			}
			yield break;
		}

		public IEnumerable<int> VtxEdgesItr(int vID)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				List<int> vedges = this.vertex_edges[vID];
				int N = vedges.Count;
				int num;
				for (int i = 0; i < N; i = num)
				{
					yield return vedges[i];
					num = i + 1;
				}
				vedges = null;
			}
			yield break;
		}

		public int FindEdge(int vA, int vB)
		{
			int vid = Math.Max(vA, vB);
			List<int> list = this.vertex_edges[Math.Min(vA, vB)];
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.edge_has_v(list[i], vid))
				{
					return list[i];
				}
			}
			return -1;
		}

		public MeshResult RemoveEdge(int eID, bool bRemoveIsolatedVertices)
		{
			if (!this.edges_refcount.isValid(eID))
			{
				return MeshResult.Failed_NotAnEdge;
			}
			int num = 3 * eID;
			Index2i index2i = new Index2i(this.edges[num], this.edges[num + 1]);
			this.vertex_edges[index2i.a].Remove(eID);
			this.vertex_edges[index2i.b].Remove(eID);
			this.edges_refcount.decrement(eID, 1);
			for (int i = 0; i < 2; i++)
			{
				int num2 = index2i[i];
				this.vertices_refcount.decrement(num2, 1);
				if (bRemoveIsolatedVertices && this.vertices_refcount.refCount(num2) == 1)
				{
					this.vertices_refcount.decrement(num2, 1);
					this.vertex_edges[num2] = null;
				}
			}
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public MeshResult RemoveVertex(int vid, bool bRemoveIsolatedVertices)
		{
			foreach (int eID in new List<int>(this.GetVtxEdges(vid)))
			{
				MeshResult meshResult = this.RemoveEdge(eID, bRemoveIsolatedVertices);
				if (meshResult != MeshResult.Ok)
				{
					return meshResult;
				}
			}
			return MeshResult.Ok;
		}

		public MeshResult SplitEdge(int vA, int vB, out DGraph.EdgeSplitInfo split)
		{
			int num = this.FindEdge(vA, vB);
			if (num == -1)
			{
				split = default(DGraph.EdgeSplitInfo);
				return MeshResult.Failed_NotAnEdge;
			}
			return this.SplitEdge(num, out split);
		}

		public MeshResult SplitEdge(int eab, out DGraph.EdgeSplitInfo split)
		{
			split = default(DGraph.EdgeSplitInfo);
			if (!this.IsEdge(eab))
			{
				return MeshResult.Failed_NotAnEdge;
			}
			int num = 3 * eab;
			int a = this.edges[num];
			int num2 = this.edges[num + 1];
			int gid = this.edges[num + 2];
			int num3 = this.append_new_split_vertex(a, num2);
			this.replace_edge_vertex(eab, num2, num3);
			this.vertex_edges[num2].Remove(eab);
			this.vertex_edges[num3].Add(eab);
			int eNewBN = this.add_edge(num3, num2, gid);
			this.vertices_refcount.increment(num3, 2);
			split.vNew = num3;
			split.eNewBN = eNewBN;
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		protected virtual int append_new_split_vertex(int a, int b)
		{
			throw new NotImplementedException("DGraph2.append_new_split_vertex");
		}

		public MeshResult CollapseEdge(int vKeep, int vRemove, out DGraph.EdgeCollapseInfo collapse)
		{
			bool flag = true;
			collapse = default(DGraph.EdgeCollapseInfo);
			if (!this.IsVertex(vKeep) || !this.IsVertex(vRemove))
			{
				return MeshResult.Failed_NotAnEdge;
			}
			int num = this.FindEdge(vRemove, vKeep);
			if (num == -1)
			{
				return MeshResult.Failed_NotAnEdge;
			}
			List<int> list = this.vertex_edges[vKeep];
			List<int> list2 = this.vertex_edges[vRemove];
			bool flag2 = false;
			while (!flag2)
			{
				flag2 = true;
				foreach (int eID in list2)
				{
					int num2 = this.edge_other_v(eID, vRemove);
					if (num2 != vKeep && this.FindEdge(vKeep, num2) != -1)
					{
						this.RemoveEdge(eID, flag);
						flag2 = false;
						break;
					}
				}
			}
			list.Remove(num);
			foreach (int num3 in list2)
			{
				if (this.edge_other_v(num3, vRemove) != vKeep)
				{
					this.replace_edge_vertex(num3, vRemove, vKeep);
					this.vertices_refcount.decrement(vRemove, 1);
					list.Add(num3);
					this.vertices_refcount.increment(vKeep, 1);
				}
			}
			this.edges_refcount.decrement(num, 1);
			this.vertices_refcount.decrement(vKeep, 1);
			this.vertices_refcount.decrement(vRemove, 1);
			if (flag)
			{
				this.vertices_refcount.decrement(vRemove, 1);
				this.vertex_edges[vRemove] = null;
			}
			list2.Clear();
			collapse.vKept = vKeep;
			collapse.vRemoved = vRemove;
			collapse.eCollapsed = num;
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		protected bool edge_has_v(int eid, int vid)
		{
			int num = 3 * eid;
			return this.edges[num] == vid || this.edges[num + 1] == vid;
		}

		protected int edge_other_v(int eID, int vID)
		{
			int num = 3 * eID;
			int num2 = this.edges[num];
			int num3 = this.edges[num + 1];
			if (num2 == vID)
			{
				return num3;
			}
			if (num3 != vID)
			{
				return -1;
			}
			return num2;
		}

		protected int replace_edge_vertex(int eID, int vOld, int vNew)
		{
			int num = 3 * eID;
			int num2 = this.edges[num];
			int num3 = this.edges[num + 1];
			if (num2 == vOld)
			{
				this.edges[num] = Math.Min(num3, vNew);
				this.edges[num + 1] = Math.Max(num3, vNew);
				return 0;
			}
			if (num3 == vOld)
			{
				this.edges[num] = Math.Min(num2, vNew);
				this.edges[num + 1] = Math.Max(num2, vNew);
				return 1;
			}
			return -1;
		}

		public bool IsCompact
		{
			get
			{
				return this.vertices_refcount.is_dense && this.edges_refcount.is_dense;
			}
		}

		public bool IsCompactV
		{
			get
			{
				return this.vertices_refcount.is_dense;
			}
		}

		public bool IsBoundaryVertex(int vID)
		{
			return this.vertices_refcount.isValid(vID) && this.vertex_edges[vID].Count == 1;
		}

		public bool IsJunctionVertex(int vID)
		{
			return this.vertices_refcount.isValid(vID) && this.vertex_edges[vID].Count > 2;
		}

		public bool IsRegularVertex(int vID)
		{
			return this.vertices_refcount.isValid(vID) && this.vertex_edges[vID].Count == 2;
		}

		public virtual bool CheckValidity(DGraph.FailMode eFailMode = DGraph.FailMode.Throw)
		{
			bool is_ok = true;
			Action<bool> action = delegate(bool b)
			{
				is_ok = (is_ok && b);
			};
			if (eFailMode == DGraph.FailMode.DebugAssert)
			{
				action = delegate(bool b)
				{
					is_ok = (is_ok && b);
				};
			}
			else if (eFailMode == DGraph.FailMode.gDevAssert)
			{
				action = delegate(bool b)
				{
					is_ok = (is_ok && b);
				};
			}
			else if (eFailMode == DGraph.FailMode.Throw)
			{
				action = delegate(bool b)
				{
					if (!b)
					{
						throw new Exception("DGraph3.CheckValidity: check failed");
					}
				};
			}
			foreach (int num in this.EdgeIndices())
			{
				action(this.IsEdge(num));
				action(this.edges_refcount.refCount(num) == 1);
				Index2i edgeV = this.GetEdgeV(num);
				action(this.IsVertex(edgeV[0]));
				action(this.IsVertex(edgeV[1]));
				action(edgeV[0] < edgeV[1]);
			}
			if (this.vertices_refcount.is_dense)
			{
				for (int i = 0; i < this.VertexCount; i++)
				{
					action(this.vertices_refcount.isValid(i));
				}
			}
			foreach (int num2 in this.VertexIndices())
			{
				action(this.IsVertex(num2));
				List<int> list = this.vertex_edges[num2];
				foreach (int num3 in list)
				{
					action(this.IsEdge(num3));
					action(this.edge_has_v(num3, num2));
					int num4 = this.edge_other_v(num3, num2);
					int num5 = this.FindEdge(num2, num4);
					action(num5 != -1);
					action(num5 == num3);
					num5 = this.FindEdge(num4, num2);
					action(num5 != -1);
					action(num5 == num3);
				}
				action(this.vertices_refcount.refCount(num2) == list.Count + 1);
			}
			this.subclass_validity_checks(action);
			return is_ok;
		}

		protected virtual void subclass_validity_checks(Action<bool> CheckOrFailF)
		{
		}

		[Conditional("DEBUG")]
		public void debug_check_is_vertex(int v)
		{
			if (!this.IsVertex(v))
			{
				throw new Exception("DGraph.debug_is_vertex - not a vertex!");
			}
		}

		[Conditional("DEBUG")]
		public void debug_check_is_edge(int e)
		{
			if (!this.IsEdge(e))
			{
				throw new Exception("DGraph.debug_is_edge - not an edge!");
			}
		}

		public const int InvalidID = -1;

		public const int DuplicateEdgeID = -2;

		public static readonly Index2i InvalidEdgeV = new Index2i(-1, -1);

		public static readonly Index3i InvalidEdge3 = new Index3i(-1, -1, -1);

		protected RefCountVector vertices_refcount;

		protected DVector<List<int>> vertex_edges;

		protected RefCountVector edges_refcount;

		protected DVector<int> edges;

		protected int timestamp;

		protected int shape_timestamp;

		protected int max_group_id;

		public struct EdgeSplitInfo
		{
			public int vNew;

			public int eNewBN;
		}

		public struct EdgeCollapseInfo
		{
			public int vKept;

			public int vRemoved;

			public int eCollapsed;
		}

		public enum FailMode
		{
			DebugAssert,
			gDevAssert,
			Throw,
			ReturnOnly
		}
	}
}
