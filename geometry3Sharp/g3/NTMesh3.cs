using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace g3
{
	public class NTMesh3 : IDeformableMesh, IMesh, IPointSet
	{
		public NTMesh3(bool bWantNormals = true, bool bWantColors = false, bool bWantTriGroups = false)
		{
			this.allocate(bWantNormals, bWantColors, bWantTriGroups);
		}

		public NTMesh3(MeshComponents flags) : this((flags & MeshComponents.VertexNormals) > MeshComponents.None, (flags & MeshComponents.VertexColors) > MeshComponents.None, (flags & MeshComponents.FaceGroups) > MeshComponents.None)
		{
		}

		private void allocate(bool bWantNormals, bool bWantColors, bool bWantTriGroups)
		{
			this.vertices = new DVector<double>();
			if (bWantNormals)
			{
				this.normals = new DVector<float>();
			}
			if (bWantColors)
			{
				this.colors = new DVector<float>();
			}
			this.vertex_edges = new SmallListSet();
			this.vertices_refcount = new RefCountVector();
			this.triangles = new DVector<int>();
			this.triangle_edges = new DVector<int>();
			this.triangles_refcount = new RefCountVector();
			if (bWantTriGroups)
			{
				this.triangle_groups = new DVector<int>();
			}
			this.max_group_id = 0;
			this.edges = new DVector<int>();
			this.edges_refcount = new RefCountVector();
			this.edge_triangles = new SmallListSet();
		}

		public NTMesh3(NTMesh3 copy)
		{
			this.Copy(copy, true, true);
		}

		public void Copy(NTMesh3 copy, bool bNormals = true, bool bColors = true)
		{
			this.vertices = new DVector<double>(copy.vertices);
			this.normals = ((bNormals && copy.normals != null) ? new DVector<float>(copy.normals) : null);
			this.colors = ((bColors && copy.colors != null) ? new DVector<float>(copy.colors) : null);
			this.vertices_refcount = new RefCountVector(copy.vertices_refcount);
			this.vertex_edges = new SmallListSet(copy.vertex_edges);
			this.triangles = new DVector<int>(copy.triangles);
			this.triangle_edges = new DVector<int>(copy.triangle_edges);
			this.triangles_refcount = new RefCountVector(copy.triangles_refcount);
			if (copy.triangle_groups != null)
			{
				this.triangle_groups = new DVector<int>(copy.triangle_groups);
			}
			this.max_group_id = copy.max_group_id;
			this.edges = new DVector<int>(copy.edges);
			this.edges_refcount = new RefCountVector(copy.edges_refcount);
			this.edge_triangles = new SmallListSet(copy.edge_triangles);
		}

		public NTMesh3(DMesh3 copy)
		{
			this.allocate(copy.HasVertexNormals, copy.HasVertexColors, copy.HasTriangleGroups);
			int[] array = new int[copy.MaxVertexID];
			foreach (int num in copy.VertexIndices())
			{
				array[num] = this.AppendVertex(copy.GetVertex(num));
			}
			foreach (Index3i index3i in copy.Triangles())
			{
				this.AppendTriangle(array[index3i.a], array[index3i.b], array[index3i.c], -1);
			}
		}

		private void updateTimeStamp(bool bShapeChange)
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

		public int TriangleCount
		{
			get
			{
				return this.triangles_refcount.count;
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

		public int MaxTriangleID
		{
			get
			{
				return this.triangles_refcount.max_index;
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

		public bool HasVertexColors
		{
			get
			{
				return this.colors != null;
			}
		}

		public bool HasVertexNormals
		{
			get
			{
				return this.normals != null;
			}
		}

		public bool HasVertexUVs
		{
			get
			{
				return false;
			}
		}

		public bool HasTriangleGroups
		{
			get
			{
				return this.triangle_groups != null;
			}
		}

		public MeshComponents Components
		{
			get
			{
				MeshComponents meshComponents = MeshComponents.None;
				if (this.normals != null)
				{
					meshComponents |= MeshComponents.VertexNormals;
				}
				if (this.colors != null)
				{
					meshComponents |= MeshComponents.VertexColors;
				}
				if (this.triangle_groups != null)
				{
					meshComponents |= MeshComponents.FaceGroups;
				}
				return meshComponents;
			}
		}

		public bool IsVertex(int vID)
		{
			return this.vertices_refcount.isValid(vID);
		}

		public bool IsTriangle(int tID)
		{
			return this.triangles_refcount.isValid(tID);
		}

		public bool IsEdge(int eID)
		{
			return this.edges_refcount.isValid(eID);
		}

		public Vector3d GetVertex(int vID)
		{
			int num = 3 * vID;
			return new Vector3d(this.vertices[num], this.vertices[num + 1], this.vertices[num + 2]);
		}

		public Vector3f GetVertexf(int vID)
		{
			int num = 3 * vID;
			return new Vector3f((float)this.vertices[num], (float)this.vertices[num + 1], (float)this.vertices[num + 2]);
		}

		public void SetVertex(int vID, Vector3d vNewPos)
		{
			int num = 3 * vID;
			this.vertices[num] = vNewPos.x;
			this.vertices[num + 1] = vNewPos.y;
			this.vertices[num + 2] = vNewPos.z;
			this.updateTimeStamp(true);
		}

		public Vector3f GetVertexNormal(int vID)
		{
			if (this.normals == null)
			{
				return Vector3f.AxisY;
			}
			int num = 3 * vID;
			return new Vector3f(this.normals[num], this.normals[num + 1], this.normals[num + 2]);
		}

		public Vector2f GetVertexUV(int i)
		{
			return Vector2f.Zero;
		}

		public void SetVertexUV(int vID, Vector2f UV)
		{
		}

		public void SetVertexNormal(int vID, Vector3f vNewNormal)
		{
			if (this.HasVertexNormals)
			{
				int num = 3 * vID;
				this.normals[num] = vNewNormal.x;
				this.normals[num + 1] = vNewNormal.y;
				this.normals[num + 2] = vNewNormal.z;
				this.updateTimeStamp(false);
			}
		}

		public Vector3f GetVertexColor(int vID)
		{
			if (this.colors == null)
			{
				return Vector3f.One;
			}
			int num = 3 * vID;
			return new Vector3f(this.colors[num], this.colors[num + 1], this.colors[num + 2]);
		}

		public void SetVertexColor(int vID, Vector3f vNewColor)
		{
			if (this.HasVertexColors)
			{
				int num = 3 * vID;
				this.colors[num] = vNewColor.x;
				this.colors[num + 1] = vNewColor.y;
				this.colors[num + 2] = vNewColor.z;
				this.updateTimeStamp(false);
			}
		}

		public bool GetVertex(int vID, ref NewVertexInfo vinfo, bool bWantNormals, bool bWantColors, bool bWantUVs)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return false;
			}
			vinfo.v.Set(this.vertices[3 * vID], this.vertices[3 * vID + 1], this.vertices[3 * vID + 2]);
			vinfo.bHaveN = (vinfo.bHaveUV = (vinfo.bHaveC = false));
			if (this.HasVertexColors && bWantNormals)
			{
				vinfo.bHaveN = true;
				vinfo.n.Set(this.normals[3 * vID], this.normals[3 * vID + 1], this.normals[3 * vID + 2]);
			}
			if (this.HasVertexColors && bWantColors)
			{
				vinfo.bHaveC = true;
				vinfo.c.Set(this.colors[3 * vID], this.colors[3 * vID + 1], this.colors[3 * vID + 2]);
			}
			return true;
		}

		public int GetVtxEdgeCount(int vID)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return -1;
			}
			return this.vertex_edges.Count(vID);
		}

		public int GetMaxVtxEdgeCount()
		{
			int num = 0;
			foreach (object obj in this.vertices_refcount)
			{
				int list_index = (int)obj;
				num = Math.Max(num, this.vertex_edges.Count(list_index));
			}
			return num;
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
			result.bHaveUV = false;
			return result;
		}

		public Index3i GetTriangle(int tID)
		{
			int num = 3 * tID;
			return new Index3i(this.triangles[num], this.triangles[num + 1], this.triangles[num + 2]);
		}

		public Index3i GetTriEdges(int tID)
		{
			int num = 3 * tID;
			return new Index3i(this.triangle_edges[num], this.triangle_edges[num + 1], this.triangle_edges[num + 2]);
		}

		public int GetTriEdge(int tid, int j)
		{
			return this.triangle_edges[3 * tid + j];
		}

		public IEnumerable<int> TriTrianglesItr(int tID)
		{
			if (this.triangles_refcount.isValid(tID))
			{
				int tei = 3 * tID;
				int num2;
				for (int i = 0; i < 3; i = num2)
				{
					int list_index = this.triangle_edges[tei + i];
					foreach (int num in this.edge_triangles.ValueItr(list_index))
					{
						if (num != tID)
						{
							yield return num;
						}
					}
					IEnumerator<int> enumerator = null;
					num2 = i + 1;
				}
			}
			yield break;
			yield break;
		}

		public int GetTriangleGroup(int tID)
		{
			if (this.triangle_groups == null)
			{
				return -1;
			}
			if (!this.triangles_refcount.isValid(tID))
			{
				return 0;
			}
			return this.triangle_groups[tID];
		}

		public void SetTriangleGroup(int tid, int group_id)
		{
			if (this.triangle_groups != null)
			{
				this.triangle_groups[tid] = group_id;
				this.max_group_id = Math.Max(this.max_group_id, group_id + 1);
				this.updateTimeStamp(false);
			}
		}

		public int AllocateTriangleGroup()
		{
			int num = this.max_group_id;
			this.max_group_id = num + 1;
			return num;
		}

		public void GetTriVertices(int tID, ref Vector3d v0, ref Vector3d v1, ref Vector3d v2)
		{
			int num = 3 * this.triangles[3 * tID];
			v0.x = this.vertices[num];
			v0.y = this.vertices[num + 1];
			v0.z = this.vertices[num + 2];
			int num2 = 3 * this.triangles[3 * tID + 1];
			v1.x = this.vertices[num2];
			v1.y = this.vertices[num2 + 1];
			v1.z = this.vertices[num2 + 2];
			int num3 = 3 * this.triangles[3 * tID + 2];
			v2.x = this.vertices[num3];
			v2.y = this.vertices[num3 + 1];
			v2.z = this.vertices[num3 + 2];
		}

		public Vector3d GetTriVertex(int tid, int j)
		{
			int num = this.triangles[3 * tid + j];
			return new Vector3d(this.vertices[3 * num], this.vertices[3 * num + 1], this.vertices[3 * num + 2]);
		}

		public Vector3d GetTriNormal(int tID)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			this.GetTriVertices(tID, ref zero, ref zero2, ref zero3);
			return MathUtil.Normal(ref zero, ref zero2, ref zero3);
		}

		public double GetTriArea(int tID)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			this.GetTriVertices(tID, ref zero, ref zero2, ref zero3);
			return MathUtil.Area(ref zero, ref zero2, ref zero3);
		}

		public void GetTriInfo(int tID, out Vector3d normal, out double fArea, out Vector3d vCentroid)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			this.GetTriVertices(tID, ref zero, ref zero2, ref zero3);
			vCentroid = 0.3333333333333333 * (zero + zero2 + zero3);
			normal = MathUtil.FastNormalArea(ref zero, ref zero2, ref zero3, out fArea);
		}

		public AxisAlignedBox3d GetTriBounds(int tID)
		{
			int num = 3 * this.triangles[3 * tID];
			double num2 = this.vertices[num];
			double num3 = this.vertices[num + 1];
			double num4 = this.vertices[num + 2];
			double num5 = num2;
			double num6 = num2;
			double num7 = num3;
			double num8 = num3;
			double num9 = num4;
			double num10 = num4;
			for (int i = 1; i < 3; i++)
			{
				num = 3 * this.triangles[3 * tID + i];
				num2 = this.vertices[num];
				num3 = this.vertices[num + 1];
				num4 = this.vertices[num + 2];
				if (num2 < num5)
				{
					num5 = num2;
				}
				else if (num2 > num6)
				{
					num6 = num2;
				}
				if (num3 < num7)
				{
					num7 = num3;
				}
				else if (num3 > num8)
				{
					num8 = num3;
				}
				if (num4 < num9)
				{
					num9 = num4;
				}
				else if (num4 > num10)
				{
					num10 = num4;
				}
			}
			return new AxisAlignedBox3d(num5, num7, num9, num6, num8, num10);
		}

		public Frame3f GetTriFrame(int tID, int nEdge = 0)
		{
			int num = 3 * tID;
			int num2 = this.triangles[num + nEdge % 3];
			int num3 = this.triangles[num + (nEdge + 1) % 3];
			int num4 = this.triangles[num + (nEdge + 2) % 3];
			Vector3d vector3d = new Vector3d(this.vertices[3 * num2], this.vertices[3 * num2 + 1], this.vertices[3 * num2 + 2]);
			Vector3d vector3d2 = new Vector3d(this.vertices[3 * num3], this.vertices[3 * num3 + 1], this.vertices[3 * num3 + 2]);
			Vector3d v = new Vector3d(this.vertices[3 * num4], this.vertices[3 * num4 + 1], this.vertices[3 * num4 + 2]);
			Vector3f x = (Vector3f)(vector3d2 - vector3d).Normalized;
			Vector3f vector3f = (Vector3f)MathUtil.Normal(ref vector3d, ref vector3d2, ref v);
			Vector3f y = x.Cross(vector3f);
			return new Frame3f((Vector3f)(vector3d + vector3d2 + v) / 3f, x, y, vector3f);
		}

		public Index2i GetEdgeV(int eID)
		{
			int num = 2 * eID;
			return new Index2i(this.edges[num], this.edges[num + 1]);
		}

		public bool GetEdgeV(int eID, ref Vector3d a, ref Vector3d b)
		{
			int num = 3 * this.edges[2 * eID];
			a.x = this.vertices[num];
			a.y = this.vertices[num + 1];
			a.z = this.vertices[num + 2];
			int num2 = 3 * this.edges[2 * eID + 1];
			b.x = this.vertices[num2];
			b.y = this.vertices[num2 + 1];
			b.z = this.vertices[num2 + 2];
			return true;
		}

		public IEnumerable<int> EdgeTrianglesItr(int eID)
		{
			return this.edge_triangles.ValueItr(eID);
		}

		public int EdgeTrianglesCount(int eID)
		{
			return this.edge_triangles.Count(eID);
		}

		public Index2i GetOrientedBoundaryEdgeV(int eID)
		{
			if (this.edges_refcount.isValid(eID) && this.edge_is_boundary(eID))
			{
				int num = 2 * eID;
				int a = this.edges[num];
				int b = this.edges[num + 1];
				int num2 = this.edge_triangles.First(eID);
				Index3i index3i = new Index3i(this.triangles[num2], this.triangles[num2 + 1], this.triangles[num2 + 2]);
				int num3 = IndexUtil.find_edge_index_in_tri(a, b, ref index3i);
				return new Index2i(index3i[num3], index3i[(num3 + 1) % 3]);
			}
			return NTMesh3.InvalidEdge;
		}

		public int AppendVertex(Vector3d v)
		{
			return this.AppendVertex(new NewVertexInfo
			{
				v = v,
				bHaveC = false,
				bHaveUV = false,
				bHaveN = false
			});
		}

		public int AppendVertex(NewVertexInfo info)
		{
			int num = this.vertices_refcount.allocate();
			int num2 = 3 * num;
			this.vertices.insert(info.v[2], num2 + 2);
			this.vertices.insert(info.v[1], num2 + 1);
			this.vertices.insert(info.v[0], num2);
			if (this.normals != null)
			{
				Vector3f vector3f = info.bHaveN ? info.n : Vector3f.AxisY;
				this.normals.insert(vector3f[2], num2 + 2);
				this.normals.insert(vector3f[1], num2 + 1);
				this.normals.insert(vector3f[0], num2);
			}
			if (this.colors != null)
			{
				Vector3f vector3f2 = info.bHaveC ? info.c : Vector3f.One;
				this.colors.insert(vector3f2[2], num2 + 2);
				this.colors.insert(vector3f2[1], num2 + 1);
				this.colors.insert(vector3f2[0], num2);
			}
			this.allocate_vertex_edges_list(num);
			this.updateTimeStamp(true);
			return num;
		}

		public int AppendTriangle(int v0, int v1, int v2, int gid = -1)
		{
			return this.AppendTriangle(new Index3i(v0, v1, v2), gid);
		}

		public int AppendTriangle(Index3i tv, int gid = -1)
		{
			if (!this.IsVertex(tv[0]) || !this.IsVertex(tv[1]) || !this.IsVertex(tv[2]))
			{
				return -1;
			}
			if (tv[0] == tv[1] || tv[0] == tv[2] || tv[1] == tv[2])
			{
				return -1;
			}
			int eid = this.find_edge(tv[0], tv[1]);
			int eid2 = this.find_edge(tv[1], tv[2]);
			int eid3 = this.find_edge(tv[2], tv[0]);
			int num = this.triangles_refcount.allocate();
			int num2 = 3 * num;
			this.triangles.insert(tv[2], num2 + 2);
			this.triangles.insert(tv[1], num2 + 1);
			this.triangles.insert(tv[0], num2);
			if (this.triangle_groups != null)
			{
				this.triangle_groups.insert(gid, num);
				this.max_group_id = Math.Max(this.max_group_id, gid + 1);
			}
			this.vertices_refcount.increment(tv[0], 1);
			this.vertices_refcount.increment(tv[1], 1);
			this.vertices_refcount.increment(tv[2], 1);
			this.add_tri_edge(num, tv[0], tv[1], 0, eid);
			this.add_tri_edge(num, tv[1], tv[2], 1, eid2);
			this.add_tri_edge(num, tv[2], tv[0], 2, eid3);
			this.updateTimeStamp(true);
			return num;
		}

		private void add_tri_edge(int tid, int v0, int v1, int j, int eid)
		{
			if (eid != -1)
			{
				this.edge_triangles.Insert(eid, tid);
				this.triangle_edges.insert(eid, 3 * tid + j);
				return;
			}
			eid = this.add_edge(v0, v1, tid, -1);
			this.triangle_edges.insert(eid, 3 * tid + j);
		}

		public void EnableVertexNormals(Vector3f initial_normal)
		{
			if (this.HasVertexNormals)
			{
				return;
			}
			this.normals = new DVector<float>();
			int maxVertexID = this.MaxVertexID;
			this.normals.resize(3 * maxVertexID);
			for (int i = 0; i < maxVertexID; i++)
			{
				int num = 3 * i;
				this.normals[num] = initial_normal.x;
				this.normals[num + 1] = initial_normal.y;
				this.normals[num + 2] = initial_normal.z;
			}
		}

		public void DiscardVertexNormals()
		{
			this.normals = null;
		}

		public void EnableVertexColors(Vector3f initial_color)
		{
			if (this.HasVertexColors)
			{
				return;
			}
			this.colors = new DVector<float>();
			int maxVertexID = this.MaxVertexID;
			this.colors.resize(3 * maxVertexID);
			for (int i = 0; i < maxVertexID; i++)
			{
				int num = 3 * i;
				this.colors[num] = initial_color.x;
				this.colors[num + 1] = initial_color.y;
				this.colors[num + 2] = initial_color.z;
			}
		}

		public void DiscardVertexColors()
		{
			this.colors = null;
		}

		public void EnableTriangleGroups(int initial_group = 0)
		{
			if (this.HasTriangleGroups)
			{
				return;
			}
			this.triangle_groups = new DVector<int>();
			int maxTriangleID = this.MaxTriangleID;
			this.triangle_groups.resize(maxTriangleID);
			for (int i = 0; i < maxTriangleID; i++)
			{
				this.triangle_groups[i] = initial_group;
			}
			this.max_group_id = 0;
		}

		public void DiscardTriangleGroups()
		{
			this.triangle_groups = null;
			this.max_group_id = 0;
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

		public IEnumerable<int> TriangleIndices()
		{
			foreach (object obj in this.triangles_refcount)
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

		public IEnumerable<int> BoundaryEdgeIndices()
		{
			foreach (object obj in this.edges_refcount)
			{
				int num = (int)obj;
				if (this.edge_triangles.Count(num) == 1)
				{
					yield return num;
				}
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<Vector3d> Vertices()
		{
			foreach (object obj in this.vertices_refcount)
			{
				int num = (int)obj;
				int num2 = 3 * num;
				yield return new Vector3d(this.vertices[num2], this.vertices[num2 + 1], this.vertices[num2 + 2]);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<Index3i> Triangles()
		{
			foreach (object obj in this.triangles_refcount)
			{
				int num = (int)obj;
				int num2 = 3 * num;
				yield return new Index3i(this.triangles[num2], this.triangles[num2 + 1], this.triangles[num2 + 2]);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public int FindEdge(int vA, int vB)
		{
			return this.find_edge(vA, vB);
		}

		public int FindEdgeFromTri(int vA, int vB, int t)
		{
			return this.find_edge_from_tri(vA, vB, t);
		}

		public IEnumerable<int> VtxVerticesItr(int vID)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				foreach (int eID in this.vertex_edges.ValueItr(vID))
				{
					yield return this.edge_other_v(eID, vID);
				}
				IEnumerator<int> enumerator = null;
			}
			yield break;
			yield break;
		}

		public IEnumerable<int> VtxEdgesItr(int vID)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				return this.vertex_edges.ValueItr(vID);
			}
			return Enumerable.Empty<int>();
		}

		public int VtxBoundaryEdges(int vID)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				int num = 0;
				foreach (int list_index in this.vertex_edges.ValueItr(vID))
				{
					if (this.edge_triangles.Count(list_index) == 1)
					{
						num++;
					}
				}
				return num;
			}
			return -1;
		}

		public int VtxAllBoundaryEdges(int vID, int[] e)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				int result = 0;
				foreach (int num in this.vertex_edges.ValueItr(vID))
				{
					if (this.edge_triangles.Count(num) == 1)
					{
						e[result++] = num;
					}
				}
				return result;
			}
			return -1;
		}

		public MeshResult GetVtxTriangles(int vID, List<int> vTriangles)
		{
			if (!this.IsVertex(vID))
			{
				return MeshResult.Failed_NotAVertex;
			}
			vTriangles.Clear();
			foreach (int list_index in this.vertex_edges.ValueItr(vID))
			{
				foreach (int item in this.edge_triangles.ValueItr(list_index))
				{
					if (!vTriangles.Contains(item))
					{
						vTriangles.Add(item);
					}
				}
			}
			return MeshResult.Ok;
		}

		public int GetVtxTriangleCount(int vID, bool bBruteForce = false)
		{
			List<int> list = new List<int>();
			if (this.GetVtxTriangles(vID, list) != MeshResult.Ok)
			{
				return -1;
			}
			return list.Count;
		}

		public IEnumerable<int> VtxTrianglesItr(int vID)
		{
			if (this.IsVertex(vID))
			{
				List<int> list = new List<int>();
				this.GetVtxTriangles(vID, list);
				foreach (int num in list)
				{
					yield return num;
				}
				List<int>.Enumerator enumerator = default(List<int>.Enumerator);
			}
			yield break;
			yield break;
		}

		protected bool tri_has_v(int tID, int vID)
		{
			int num = 3 * tID;
			return this.triangles[num] == vID || this.triangles[num + 1] == vID || this.triangles[num + 2] == vID;
		}

		protected bool tri_is_boundary(int tID)
		{
			int num = 3 * tID;
			return this.edge_is_boundary(this.triangle_edges[num]) || this.edge_is_boundary(this.triangle_edges[num + 1]) || this.edge_is_boundary(this.triangle_edges[num + 2]);
		}

		protected bool tri_has_neighbour_t(int tCheck, int tNbr)
		{
			int num = 3 * tCheck;
			return this.edge_has_t(this.triangle_edges[num], tNbr) || this.edge_has_t(this.triangle_edges[num + 1], tNbr) || this.edge_has_t(this.triangle_edges[num + 2], tNbr);
		}

		protected bool tri_has_sequential_v(int tID, int vA, int vB)
		{
			int num = 3 * tID;
			int num2 = this.triangles[num];
			int num3 = this.triangles[num + 1];
			int num4 = this.triangles[num + 2];
			return (num2 == vA && num3 == vB) || (num3 == vA && num4 == vB) || (num4 == vA && num2 == vB);
		}

		protected int find_tri_neighbour_edge(int tID, int vA, int vB)
		{
			int num = 3 * tID;
			int num2 = this.triangles[num];
			int num3 = this.triangles[num + 1];
			if (IndexUtil.same_pair_unordered(num2, num3, vA, vB))
			{
				return this.triangle_edges[3 * tID];
			}
			int num4 = this.triangles[num + 2];
			if (IndexUtil.same_pair_unordered(num3, num4, vA, vB))
			{
				return this.triangle_edges[3 * tID + 1];
			}
			if (IndexUtil.same_pair_unordered(num4, num2, vA, vB))
			{
				return this.triangle_edges[3 * tID + 2];
			}
			return -1;
		}

		protected int find_tri_neighbour_index(int tID, int vA, int vB)
		{
			int num = 3 * tID;
			int num2 = this.triangles[num];
			int num3 = this.triangles[num + 1];
			if (IndexUtil.same_pair_unordered(num2, num3, vA, vB))
			{
				return 0;
			}
			int num4 = this.triangles[num + 2];
			if (IndexUtil.same_pair_unordered(num3, num4, vA, vB))
			{
				return 1;
			}
			if (IndexUtil.same_pair_unordered(num4, num2, vA, vB))
			{
				return 2;
			}
			return -1;
		}

		public bool IsNonManifoldEdge(int eid)
		{
			return this.edge_triangles.Count(eid) > 2;
		}

		public bool IsBoundaryEdge(int eid)
		{
			return this.edge_triangles.Count(eid) == 1;
		}

		protected bool edge_is_boundary(int eid)
		{
			return this.edge_triangles.Count(eid) == 1;
		}

		protected bool edge_has_v(int eid, int vid)
		{
			int num = 2 * eid;
			return this.edges[num] == vid || this.edges[num + 1] == vid;
		}

		protected bool edge_has_t(int eid, int tid)
		{
			return this.edge_triangles.Contains(eid, tid);
		}

		protected int edge_other_v(int eID, int vID)
		{
			int num = 2 * eID;
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

		public bool vertex_is_boundary(int vID)
		{
			return this.IsBoundaryVertex(vID);
		}

		public bool IsBoundaryVertex(int vID)
		{
			foreach (int list_index in this.vertex_edges.ValueItr(vID))
			{
				if (this.edge_triangles.Count(list_index) == 1)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsBoundaryTriangle(int tID)
		{
			int num = 3 * tID;
			return this.IsBoundaryEdge(this.triangle_edges[num]) || this.IsBoundaryEdge(this.triangle_edges[num + 1]) || this.IsBoundaryEdge(this.triangle_edges[num + 2]);
		}

		private int find_edge(int vA, int vB)
		{
			int num = Math.Max(vA, vB);
			int list_index = Math.Min(vA, vB);
			foreach (int num2 in this.vertex_edges.ValueItr(list_index))
			{
				if (this.edges[2 * num2 + 1] == num)
				{
					return num2;
				}
			}
			return -1;
		}

		private int find_edge_from_tri(int vA, int vB, int tID)
		{
			int num = 3 * tID;
			int num2 = this.triangles[num];
			int num3 = this.triangles[num + 1];
			if (IndexUtil.same_pair_unordered(vA, vB, num2, num3))
			{
				return this.triangle_edges[num];
			}
			int num4 = this.triangles[num + 2];
			if (IndexUtil.same_pair_unordered(vA, vB, num3, num4))
			{
				return this.triangle_edges[num + 1];
			}
			if (IndexUtil.same_pair_unordered(vA, vB, num4, num2))
			{
				return this.triangle_edges[num + 2];
			}
			return -1;
		}

		public bool IsBowtieVertex(int vID)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				int vtxTriangleCount = this.GetVtxTriangleCount(vID, false);
				int vtxEdgeCount = this.GetVtxEdgeCount(vID);
				return vtxTriangleCount != vtxEdgeCount && vtxTriangleCount != vtxEdgeCount - 1;
			}
			throw new Exception("NTMesh3.IsBowtieVertex: " + vID.ToString() + " is not a valid vertex");
		}

		public AxisAlignedBox3d GetBounds()
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			using (IEnumerator enumerator = this.vertices_refcount.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					int num4 = (int)enumerator.Current;
					num = this.vertices[3 * num4];
					num2 = this.vertices[3 * num4 + 1];
					num3 = this.vertices[3 * num4 + 2];
				}
			}
			double num5 = num;
			double num6 = num;
			double num7 = num2;
			double num8 = num2;
			double num9 = num3;
			double num10 = num3;
			foreach (object obj in this.vertices_refcount)
			{
				int num11 = (int)obj;
				num = this.vertices[3 * num11];
				num2 = this.vertices[3 * num11 + 1];
				num3 = this.vertices[3 * num11 + 2];
				if (num < num5)
				{
					num5 = num;
				}
				else if (num > num6)
				{
					num6 = num;
				}
				if (num2 < num7)
				{
					num7 = num2;
				}
				else if (num2 > num8)
				{
					num8 = num2;
				}
				if (num3 < num9)
				{
					num9 = num3;
				}
				else if (num3 > num10)
				{
					num10 = num3;
				}
			}
			return new AxisAlignedBox3d(num5, num7, num9, num6, num8, num10);
		}

		public AxisAlignedBox3d CachedBounds
		{
			get
			{
				if (this.cached_bounds_timestamp != this.Timestamp)
				{
					this.cached_bounds = this.GetBounds();
					this.cached_bounds_timestamp = this.Timestamp;
				}
				return this.cached_bounds;
			}
		}

		public bool IsClosed()
		{
			if (this.TriangleCount == 0)
			{
				return false;
			}
			if (this.MaxEdgeID / this.EdgeCount > 5)
			{
				using (IEnumerator enumerator = this.edges_refcount.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						int eid = (int)obj;
						if (this.edge_is_boundary(eid))
						{
							return false;
						}
					}
					return true;
				}
			}
			int maxEdgeID = this.MaxEdgeID;
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (this.edges_refcount.isValid(i) && this.edge_is_boundary(i))
				{
					return false;
				}
			}
			return true;
		}

		public bool CachedIsClosed
		{
			get
			{
				if (this.cached_is_closed_timstamp != this.Timestamp)
				{
					this.cached_is_closed = this.IsClosed();
					this.cached_is_closed_timstamp = this.Timestamp;
				}
				return this.cached_is_closed;
			}
		}

		public bool IsCompact
		{
			get
			{
				return this.vertices_refcount.is_dense && this.edges_refcount.is_dense && this.triangles_refcount.is_dense;
			}
		}

		public bool IsCompactV
		{
			get
			{
				return this.vertices_refcount.is_dense;
			}
		}

		private void set_triangle(int tid, int v0, int v1, int v2)
		{
			int num = 3 * tid;
			this.triangles[num] = v0;
			this.triangles[num + 1] = v1;
			this.triangles[num + 2] = v2;
		}

		private void set_triangle_edges(int tid, int e0, int e1, int e2)
		{
			int num = 3 * tid;
			this.triangle_edges[num] = e0;
			this.triangle_edges[num + 1] = e1;
			this.triangle_edges[num + 2] = e2;
		}

		private int add_edge(int vA, int vB, int tA, int tB = -1)
		{
			if (vB < vA)
			{
				int num = vB;
				vB = vA;
				vA = num;
			}
			int num2 = this.edges_refcount.allocate();
			this.allocate_edge_triangles_list(num2);
			int num3 = 2 * num2;
			this.edges.insert(vA, num3);
			this.edges.insert(vB, num3 + 1);
			if (tA != -1)
			{
				this.edge_triangles.Insert(num2, tA);
			}
			if (tB != -1)
			{
				this.edge_triangles.Insert(num2, tB);
			}
			this.vertex_edges.Insert(vA, num2);
			this.vertex_edges.Insert(vB, num2);
			return num2;
		}

		private int replace_tri_vertex(int tID, int vOld, int vNew)
		{
			int num = 3 * tID;
			if (this.triangles[num] == vOld)
			{
				this.triangles[num] = vNew;
				return 0;
			}
			if (this.triangles[num + 1] == vOld)
			{
				this.triangles[num + 1] = vNew;
				return 1;
			}
			if (this.triangles[num + 2] == vOld)
			{
				this.triangles[num + 2] = vNew;
				return 2;
			}
			return -1;
		}

		private int add_triangle_only(int a, int b, int c, int e0, int e1, int e2)
		{
			int num = this.triangles_refcount.allocate();
			int num2 = 3 * num;
			this.triangles.insert(c, num2 + 2);
			this.triangles.insert(b, num2 + 1);
			this.triangles.insert(a, num2);
			this.triangle_edges.insert(e2, num2 + 2);
			this.triangle_edges.insert(e1, num2 + 1);
			this.triangle_edges.insert(e0, num2);
			return num;
		}

		private void allocate_vertex_edges_list(int vid)
		{
			if (vid < this.vertex_edges.Size)
			{
				this.vertex_edges.Clear(vid);
			}
			this.vertex_edges.AllocateAt(vid);
		}

		private List<int> vertex_edges_list(int vid)
		{
			return new List<int>(this.vertex_edges.ValueItr(vid));
		}

		private void allocate_edge_triangles_list(int eid)
		{
			if (eid < this.edge_triangles.Size)
			{
				this.edge_triangles.Clear(eid);
			}
			this.edge_triangles.AllocateAt(eid);
		}

		private void set_edge_vertices(int eID, int a, int b)
		{
			int num = 2 * eID;
			this.edges[num] = Math.Min(a, b);
			this.edges[num + 1] = Math.Max(a, b);
		}

		private int replace_edge_vertex(int eID, int vOld, int vNew)
		{
			int num = 2 * eID;
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

		private bool replace_edge_triangle(int eID, int tOld, int tNew)
		{
			bool result = this.edge_triangles.Remove(eID, tOld);
			this.edge_triangles.Insert(eID, tNew);
			return result;
		}

		private void add_edge_triangle(int eID, int tID)
		{
			this.edge_triangles.Insert(eID, tID);
		}

		private bool remove_edge_triangle(int eID, int tID)
		{
			return this.edge_triangles.Remove(eID, tID);
		}

		private int replace_triangle_edge(int tID, int eOld, int eNew)
		{
			int num = 3 * tID;
			if (this.triangle_edges[num] == eOld)
			{
				this.triangle_edges[num] = eNew;
				return 0;
			}
			if (this.triangle_edges[num + 1] == eOld)
			{
				this.triangle_edges[num + 1] = eNew;
				return 1;
			}
			if (this.triangle_edges[num + 2] == eOld)
			{
				this.triangle_edges[num + 2] = eNew;
				return 2;
			}
			return -1;
		}

		public MeshResult RemoveTriangle(int tID, bool bRemoveIsolatedVertices = true)
		{
			if (!this.triangles_refcount.isValid(tID))
			{
				return MeshResult.Failed_NotATriangle;
			}
			Index3i triangle = this.GetTriangle(tID);
			Index3i triEdges = this.GetTriEdges(tID);
			for (int i = 0; i < 3; i++)
			{
				int num = triEdges[i];
				this.remove_edge_triangle(num, tID);
				if (this.edge_triangles.Count(num) == 0)
				{
					int list_index = this.edges[2 * num];
					this.vertex_edges.Remove(list_index, num);
					int list_index2 = this.edges[2 * num + 1];
					this.vertex_edges.Remove(list_index2, num);
					this.edges_refcount.decrement(num, 1);
				}
			}
			this.triangles_refcount.decrement(tID, 1);
			for (int j = 0; j < 3; j++)
			{
				int num2 = triangle[j];
				this.vertices_refcount.decrement(num2, 1);
				if (bRemoveIsolatedVertices && this.vertices_refcount.refCount(num2) == 1)
				{
					this.vertices_refcount.decrement(num2, 1);
					this.vertex_edges.Clear(num2);
				}
			}
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public MeshResult SplitEdge(int vA, int vB, out NTMesh3.EdgeSplitInfo split)
		{
			int num = this.find_edge(vA, vB);
			if (num == -1)
			{
				split = default(NTMesh3.EdgeSplitInfo);
				return MeshResult.Failed_NotAnEdge;
			}
			return this.SplitEdge(num, out split);
		}

		public MeshResult SplitEdge(int eab, out NTMesh3.EdgeSplitInfo split)
		{
			split = default(NTMesh3.EdgeSplitInfo);
			if (!this.IsEdge(eab))
			{
				return MeshResult.Failed_NotAnEdge;
			}
			int num = 2 * eab;
			int num2 = this.edges[num];
			int num3 = this.edges[num + 1];
			List<int> list = new List<int>(this.edge_triangles.ValueItr(eab));
			if (list.Count < 1)
			{
				return MeshResult.Failed_BrokenTopology;
			}
			Vector3d v = 0.5 * (this.GetVertex(num2) + this.GetVertex(num3));
			int num4 = this.AppendVertex(v);
			if (this.HasVertexNormals)
			{
				this.SetVertexNormal(num4, (this.GetVertexNormal(num2) + this.GetVertexNormal(num3)).Normalized);
			}
			if (this.HasVertexColors)
			{
				this.SetVertexColor(num4, 0.5f * (this.GetVertexColor(num2) + this.GetVertexColor(num3)));
			}
			this.replace_edge_vertex(eab, num3, num4);
			this.vertex_edges.Remove(num3, eab);
			this.vertex_edges.Insert(num4, eab);
			int num5 = this.add_edge(num4, num3, -1, -1);
			this.vertices_refcount.increment(num4, (short)list.Count);
			split.NewEdges = new List<int>();
			split.eNewBN = num5;
			foreach (int num6 in list)
			{
				Index3i triangle = this.GetTriangle(num6);
				Index3i triEdges = this.GetTriEdges(num6);
				int num7 = IndexUtil.find_tri_other_vtx(num2, num3, triangle);
				this.replace_tri_vertex(num6, num3, num4);
				int num8 = triEdges[IndexUtil.find_edge_index_in_tri(num3, num7, ref triangle)];
				bool flag = IndexUtil.is_ordered(num2, num3, ref triangle);
				int num9 = flag ? this.add_triangle_only(num4, num3, num7, -1, -1, -1) : this.add_triangle_only(num3, num4, num7, -1, -1, -1);
				if (this.triangle_groups != null)
				{
					this.triangle_groups.insert(this.triangle_groups[num6], num9);
				}
				this.replace_edge_triangle(num8, num6, num9);
				this.add_edge_triangle(num5, num9);
				int num10 = this.add_edge(num7, num4, num6, num9);
				split.NewEdges.Add(num10);
				this.replace_triangle_edge(num6, num8, num10);
				if (flag)
				{
					this.set_triangle_edges(num9, num5, num8, num10);
				}
				else
				{
					this.set_triangle_edges(num9, num5, num10, num8);
				}
				this.vertices_refcount.increment(num7, 1);
				this.vertices_refcount.increment(num4, 1);
			}
			split.bIsBoundary = (list.Count == 1);
			split.vNew = num4;
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public virtual MeshResult PokeTriangle(int tid, out NTMesh3.PokeTriangleInfo result)
		{
			return this.PokeTriangle(tid, Vector3d.One / 3.0, out result);
		}

		public virtual MeshResult PokeTriangle(int tid, Vector3d baryCoordinates, out NTMesh3.PokeTriangleInfo result)
		{
			result = default(NTMesh3.PokeTriangleInfo);
			if (!this.IsTriangle(tid))
			{
				return MeshResult.Failed_NotATriangle;
			}
			Index3i triangle = this.GetTriangle(tid);
			Index3i triEdges = this.GetTriEdges(tid);
			Vector3d v = (this.GetVertex(triangle.a) + this.GetVertex(triangle.b) + this.GetVertex(triangle.c)) / 3.0;
			int num = this.AppendVertex(v);
			int num2 = this.add_edge(triangle.a, num, -1, -1);
			int num3 = this.add_edge(triangle.b, num, -1, -1);
			int num4 = this.add_edge(triangle.c, num, -1, -1);
			this.vertices_refcount.increment(triangle.a, 1);
			this.vertices_refcount.increment(triangle.b, 1);
			this.vertices_refcount.increment(triangle.c, 1);
			this.vertices_refcount.increment(num, 3);
			this.set_triangle(tid, triangle.a, triangle.b, num);
			this.set_triangle_edges(tid, triEdges.a, num3, num2);
			int num5 = this.add_triangle_only(triangle.b, triangle.c, num, triEdges.b, num4, num3);
			int num6 = this.add_triangle_only(triangle.c, triangle.a, num, triEdges.c, num2, num4);
			this.replace_edge_triangle(triEdges.b, tid, num5);
			this.replace_edge_triangle(triEdges.c, tid, num6);
			this.add_edge_triangle(num2, tid);
			this.add_edge_triangle(num2, num6);
			this.add_edge_triangle(num3, tid);
			this.add_edge_triangle(num3, num5);
			this.add_edge_triangle(num4, num5);
			this.add_edge_triangle(num4, num6);
			if (this.HasTriangleGroups)
			{
				int value = this.triangle_groups[tid];
				this.triangle_groups.insert(value, num5);
				this.triangle_groups.insert(value, num6);
			}
			result.new_vid = num;
			result.new_t1 = num5;
			result.new_t2 = num6;
			result.new_edges = new Index3i(num2, num3, num4);
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public DMesh3 Deconstruct()
		{
			DMesh3 dmesh = new DMesh3(true, false, false, false);
			foreach (Index3i index3i in this.Triangles())
			{
				dmesh.AppendTriangle(dmesh.AppendVertex(this.GetVertex(index3i.a)), dmesh.AppendVertex(this.GetVertex(index3i.b)), dmesh.AppendVertex(this.GetVertex(index3i.c)), -1);
			}
			return dmesh;
		}

		[Conditional("DEBUG")]
		public void debug_check_is_vertex(int v)
		{
			if (!this.IsVertex(v))
			{
				throw new Exception("DMesh3.debug_is_vertex - not a vertex!");
			}
		}

		[Conditional("DEBUG")]
		public void debug_check_is_triangle(int t)
		{
			if (!this.IsTriangle(t))
			{
				throw new Exception("DMesh3.debug_is_triangle - not a triangle!");
			}
		}

		[Conditional("DEBUG")]
		public void debug_check_is_edge(int e)
		{
			if (!this.IsEdge(e))
			{
				throw new Exception("DMesh3.debug_is_edge - not an edge!");
			}
		}

		public bool CheckValidity(FailMode eFailMode = FailMode.Throw)
		{
			int[] array = new int[this.MaxVertexID];
			bool is_ok = true;
			Action<bool> action = delegate(bool b)
			{
				is_ok = (is_ok && b);
			};
			if (eFailMode == FailMode.DebugAssert)
			{
				action = delegate(bool b)
				{
					is_ok = (is_ok && b);
				};
			}
			else if (eFailMode == FailMode.gDevAssert)
			{
				action = delegate(bool b)
				{
					is_ok = (is_ok && b);
				};
			}
			else if (eFailMode == FailMode.Throw)
			{
				action = delegate(bool b)
				{
					if (!b)
					{
						throw new Exception("DMesh3.CheckValidity: check failed");
					}
				};
			}
			if (this.normals != null)
			{
				action(this.normals.size == this.vertices.size);
			}
			if (this.colors != null)
			{
				action(this.colors.size == this.vertices.size);
			}
			if (this.triangle_groups != null)
			{
				action(this.triangle_groups.size == this.triangles.size / 3);
			}
			foreach (int num in this.TriangleIndices())
			{
				action(this.IsTriangle(num));
				action(this.triangles_refcount.refCount(num) == 1);
				Index3i triangle = this.GetTriangle(num);
				for (int i = 0; i < 3; i++)
				{
					action(this.IsVertex(triangle[i]));
					array[triangle[i]]++;
				}
				Index3i index3i = default(Index3i);
				for (int j = 0; j < 3; j++)
				{
					int vA = triangle[j];
					int vB = triangle[(j + 1) % 3];
					index3i[j] = this.FindEdge(vA, vB);
					action(index3i[j] != -1);
					action(this.edge_has_t(index3i[j], num));
					action(index3i[j] == this.FindEdgeFromTri(vA, vB, num));
				}
				action(index3i[0] != index3i[1] && index3i[0] != index3i[2] && index3i[1] != index3i[2]);
				Index3i triEdges = this.GetTriEdges(num);
				for (int k = 0; k < 3; k++)
				{
					int num2 = triEdges[k];
					action(this.IsEdge(num2));
					if (this.edge_is_boundary(num2))
					{
						action(this.tri_is_boundary(num));
					}
					else
					{
						bool obj = false;
						foreach (int num3 in this.EdgeTrianglesItr(num2))
						{
							if (num3 != num)
							{
								action(this.tri_has_neighbour_t(num3, num));
							}
							else
							{
								obj = true;
							}
						}
						action(obj);
						int a = triangle[k];
						int a2 = triangle[(k + 1) % 3];
						Index2i edgeV = this.GetEdgeV(triEdges[k]);
						action(IndexUtil.same_pair_unordered(a, a2, edgeV[0], edgeV[1]));
					}
				}
			}
			foreach (int num4 in this.EdgeIndices())
			{
				action(this.IsEdge(num4));
				action(this.edges_refcount.refCount(num4) == 1);
				Index2i edgeV2 = this.GetEdgeV(num4);
				action(this.IsVertex(edgeV2[0]));
				action(this.IsVertex(edgeV2[1]));
				action(edgeV2[0] < edgeV2[1]);
				foreach (int tID in this.EdgeTrianglesItr(num4))
				{
					action(this.IsTriangle(tID));
				}
			}
			if (this.vertices_refcount.is_dense)
			{
				for (int l = 0; l < this.vertices.Length / 3; l++)
				{
					action(this.vertices_refcount.isValid(l));
				}
			}
			foreach (int num5 in this.VertexIndices())
			{
				action(this.IsVertex(num5));
				Vector3d vertex = this.GetVertex(num5);
				action(!double.IsNaN(vertex.LengthSquared));
				action(!double.IsInfinity(vertex.LengthSquared));
				foreach (int num6 in this.vertex_edges.ValueItr(num5))
				{
					action(this.IsEdge(num6));
					action(this.edge_has_v(num6, num5));
					int num7 = this.edge_other_v(num6, num5);
					int num8 = this.find_edge(num5, num7);
					action(num8 != -1);
					action(num8 == num6);
					num8 = this.find_edge(num7, num5);
					action(num8 != -1);
					action(num8 == num6);
				}
				foreach (int num9 in this.VtxVerticesItr(num5))
				{
					action(this.IsVertex(num9));
					int eID = this.find_edge(num5, num9);
					action(this.IsEdge(eID));
				}
				List<int> list = new List<int>();
				this.GetVtxTriangles(num5, list);
				action(this.vertices_refcount.refCount(num5) == list.Count + 1);
				action(array[num5] == list.Count);
				foreach (int tID2 in list)
				{
					action(this.tri_has_v(tID2, num5));
				}
				List<int> list2 = new List<int>(list);
				foreach (int eID2 in this.vertex_edges.ValueItr(num5))
				{
					foreach (int item in this.EdgeTrianglesItr(eID2))
					{
						action(list.Contains(item));
						list2.Remove(item);
					}
				}
				action(list2.Count == 0);
			}
			return is_ok;
		}

		public const int InvalidID = -1;

		public const int NonManifoldID = -2;

		public static readonly Vector3d InvalidVertex = new Vector3d(double.MaxValue, 0.0, 0.0);

		public static readonly Index3i InvalidTriangle = new Index3i(-1, -1, -1);

		public static readonly Index2i InvalidEdge = new Index2i(-1, -1);

		private RefCountVector vertices_refcount;

		private DVector<double> vertices;

		private DVector<float> normals;

		private DVector<float> colors;

		private SmallListSet vertex_edges;

		private RefCountVector triangles_refcount;

		private DVector<int> triangles;

		private DVector<int> triangle_edges;

		private DVector<int> triangle_groups;

		private RefCountVector edges_refcount;

		private DVector<int> edges;

		private SmallListSet edge_triangles;

		private int timestamp;

		private int shape_timestamp;

		private int max_group_id;

		private AxisAlignedBox3d cached_bounds;

		private int cached_bounds_timestamp = -1;

		private bool cached_is_closed;

		private int cached_is_closed_timstamp = -1;

		public struct EdgeSplitInfo
		{
			public bool bIsBoundary;

			public int vNew;

			public int eNewBN;

			public List<int> NewEdges;
		}

		public struct PokeTriangleInfo
		{
			public int new_vid;

			public int new_t1;

			public int new_t2;

			public Index3i new_edges;
		}
	}
}
