using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace g3
{
	public class DMesh3 : IDeformableMesh, IMesh, IPointSet
	{
		public DMesh3(bool bWantNormals = true, bool bWantColors = false, bool bWantUVs = false, bool bWantTriGroups = false)
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
			if (bWantUVs)
			{
				this.uv = new DVector<float>();
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
		}

		public DMesh3(MeshComponents flags) : this((flags & MeshComponents.VertexNormals) > MeshComponents.None, (flags & MeshComponents.VertexColors) > MeshComponents.None, (flags & MeshComponents.VertexUVs) > MeshComponents.None, (flags & MeshComponents.FaceGroups) > MeshComponents.None)
		{
		}

		public DMesh3(DMesh3 copy, bool bCompact = false, bool bWantNormals = true, bool bWantColors = true, bool bWantUVs = true)
		{
			if (bCompact)
			{
				this.CompactCopy(copy, bWantNormals, bWantColors, bWantUVs);
				return;
			}
			this.Copy(copy, bWantNormals, bWantColors, bWantUVs);
		}

		public DMesh3(DMesh3 copy, bool bCompact, MeshComponents flags) : this(copy, bCompact, (flags & MeshComponents.VertexNormals) > MeshComponents.None, (flags & MeshComponents.VertexColors) > MeshComponents.None, (flags & MeshComponents.VertexUVs) > MeshComponents.None)
		{
		}

		public DMesh3(IMesh copy, MeshHints hints, bool bWantNormals = true, bool bWantColors = true, bool bWantUVs = true)
		{
			this.Copy(copy, hints, bWantNormals, bWantColors, bWantUVs);
		}

		public DMesh3(IMesh copy, MeshHints hints, MeshComponents flags) : this(copy, hints, (flags & MeshComponents.VertexNormals) > MeshComponents.None, (flags & MeshComponents.VertexColors) > MeshComponents.None, (flags & MeshComponents.VertexUVs) > MeshComponents.None)
		{
		}

		public DMesh3.CompactInfo CompactCopy(DMesh3 copy, bool bNormals = true, bool bColors = true, bool bUVs = true)
		{
			if (copy.IsCompact)
			{
				this.Copy(copy, bNormals, bColors, bUVs);
				return new DMesh3.CompactInfo
				{
					MapV = new IdentityIndexMap()
				};
			}
			this.vertices = new DVector<double>();
			this.vertex_edges = new SmallListSet();
			this.vertices_refcount = new RefCountVector();
			this.triangles = new DVector<int>();
			this.triangle_edges = new DVector<int>();
			this.triangles_refcount = new RefCountVector();
			this.edges = new DVector<int>();
			this.edges_refcount = new RefCountVector();
			this.max_group_id = 0;
			this.normals = ((bNormals && copy.normals != null) ? new DVector<float>() : null);
			this.colors = ((bColors && copy.colors != null) ? new DVector<float>() : null);
			this.uv = ((bUVs && copy.uv != null) ? new DVector<float>() : null);
			this.triangle_groups = ((copy.triangle_groups != null) ? new DVector<int>() : null);
			NewVertexInfo info = default(NewVertexInfo);
			int[] array = new int[copy.MaxVertexID];
			foreach (object obj in copy.vertices_refcount)
			{
				int num = (int)obj;
				copy.GetVertex(num, ref info, bNormals, bColors, bUVs);
				array[num] = this.AppendVertex(info);
			}
			foreach (object obj2 in copy.triangles_refcount)
			{
				int tID = (int)obj2;
				Index3i triangle = copy.GetTriangle(tID);
				triangle.a = array[triangle.a];
				triangle.b = array[triangle.b];
				triangle.c = array[triangle.c];
				int num2 = copy.HasTriangleGroups ? copy.GetTriangleGroup(tID) : -1;
				this.AppendTriangle(triangle, num2);
				this.max_group_id = Math.Max(this.max_group_id, num2 + 1);
			}
			if (copy.Metadata != null)
			{
				this.Metadata = copy.Metadata;
			}
			return new DMesh3.CompactInfo
			{
				MapV = new IndexMap(array, this.MaxVertexID)
			};
		}

		public void Copy(DMesh3 copy, bool bNormals = true, bool bColors = true, bool bUVs = true)
		{
			this.vertices = new DVector<double>(copy.vertices);
			this.normals = ((bNormals && copy.normals != null) ? new DVector<float>(copy.normals) : null);
			this.colors = ((bColors && copy.colors != null) ? new DVector<float>(copy.colors) : null);
			this.uv = ((bUVs && copy.uv != null) ? new DVector<float>(copy.uv) : null);
			this.vertices_refcount = new RefCountVector(copy.vertices_refcount);
			this.vertex_edges = new SmallListSet(copy.vertex_edges);
			this.triangles = new DVector<int>(copy.triangles);
			this.triangle_edges = new DVector<int>(copy.triangle_edges);
			this.triangles_refcount = new RefCountVector(copy.triangles_refcount);
			if (copy.triangle_groups != null)
			{
				this.triangle_groups = new DVector<int>(copy.triangle_groups);
			}
			if (copy.Metadata != null)
			{
				this.Metadata = copy.Metadata;
			}
			this.max_group_id = copy.max_group_id;
			this.edges = new DVector<int>(copy.edges);
			this.edges_refcount = new RefCountVector(copy.edges_refcount);
		}

		public DMesh3.CompactInfo Copy(IMesh copy, MeshHints hints, bool bNormals = true, bool bColors = true, bool bUVs = true)
		{
			this.vertices = new DVector<double>();
			this.vertex_edges = new SmallListSet();
			this.vertices_refcount = new RefCountVector();
			this.triangles = new DVector<int>();
			this.triangle_edges = new DVector<int>();
			this.triangles_refcount = new RefCountVector();
			this.edges = new DVector<int>();
			this.edges_refcount = new RefCountVector();
			this.max_group_id = 0;
			this.normals = ((bNormals && copy.HasVertexNormals) ? new DVector<float>() : null);
			this.colors = ((bColors && copy.HasVertexColors) ? new DVector<float>() : null);
			this.uv = ((bUVs && copy.HasVertexUVs) ? new DVector<float>() : null);
			this.triangle_groups = (copy.HasTriangleGroups ? new DVector<int>() : null);
			NewVertexInfo info = default(NewVertexInfo);
			int[] array = new int[copy.MaxVertexID];
			foreach (int num in copy.VertexIndices())
			{
				info = copy.GetVertexAll(num);
				array[num] = this.AppendVertex(info);
			}
			foreach (int i in copy.TriangleIndices())
			{
				Index3i triangle = copy.GetTriangle(i);
				triangle.a = array[triangle.a];
				triangle.b = array[triangle.b];
				triangle.c = array[triangle.c];
				int num2 = copy.HasTriangleGroups ? copy.GetTriangleGroup(i) : -1;
				this.AppendTriangle(triangle, num2);
				this.max_group_id = Math.Max(this.max_group_id, num2 + 1);
			}
			return new DMesh3.CompactInfo
			{
				MapV = new IndexMap(array, this.MaxVertexID)
			};
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
				return this.uv != null;
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
				if (this.uv != null)
				{
					meshComponents |= MeshComponents.VertexUVs;
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

		public Vector2f GetVertexUV(int vID)
		{
			if (this.uv == null)
			{
				return Vector2f.Zero;
			}
			int num = 2 * vID;
			return new Vector2f(this.uv[num], this.uv[num + 1]);
		}

		public void SetVertexUV(int vID, Vector2f vNewUV)
		{
			if (this.HasVertexUVs)
			{
				int num = 2 * vID;
				this.uv[num] = vNewUV.x;
				this.uv[num + 1] = vNewUV.y;
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
			if (this.HasVertexNormals && bWantNormals)
			{
				vinfo.bHaveN = true;
				vinfo.n.Set(this.normals[3 * vID], this.normals[3 * vID + 1], this.normals[3 * vID + 2]);
			}
			if (this.HasVertexColors && bWantColors)
			{
				vinfo.bHaveC = true;
				vinfo.c.Set(this.colors[3 * vID], this.colors[3 * vID + 1], this.colors[3 * vID + 2]);
			}
			if (this.HasVertexUVs && bWantUVs)
			{
				vinfo.bHaveUV = true;
				vinfo.uv.Set(this.uv[2 * vID], this.uv[2 * vID + 1]);
			}
			return true;
		}

		[Obsolete("GetVtxEdges will be removed in future, use VtxEdgesItr instead")]
		public ReadOnlyCollection<int> GetVtxEdges(int vID)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return null;
			}
			return this.vertex_edges_list(vID).AsReadOnly();
		}

		public int GetVtxEdgeCount(int vID)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return -1;
			}
			return this.vertex_edges.Count(vID);
		}

		[Obsolete("GetVtxEdgeValence will be removed in future, use GetVtxEdgeCount instead")]
		public int GetVtxEdgeValence(int vID)
		{
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

		public Frame3f GetVertexFrame(int vID, bool bFrameNormalY = false)
		{
			int num = 3 * vID;
			Vector3d vector3d = new Vector3d(this.vertices[num], this.vertices[num + 1], this.vertices[num + 2]);
			Vector3d vector3d2 = new Vector3d((double)this.normals[num], (double)this.normals[num + 1], (double)this.normals[num + 2]);
			int eID = this.vertex_edges.First(vID);
			int num2 = 3 * this.edge_other_v(eID, vID);
			Vector3d vector3d3 = new Vector3d(this.vertices[num2], this.vertices[num2 + 1], this.vertices[num2 + 2]) - vector3d;
			vector3d3.Normalize(2.220446049250313E-16);
			Vector3d v = vector3d2.Cross(vector3d3);
			vector3d3 = v.Cross(vector3d2);
			if (bFrameNormalY)
			{
				return new Frame3f((Vector3f)vector3d, (Vector3f)vector3d3, (Vector3f)vector3d2, (Vector3f)(-v));
			}
			return new Frame3f((Vector3f)vector3d, (Vector3f)vector3d3, (Vector3f)v, (Vector3f)vector3d2);
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

		public Index3i GetTriNeighbourTris(int tID)
		{
			if (this.triangles_refcount.isValid(tID))
			{
				int num = 3 * tID;
				Index3i zero = Index3i.Zero;
				for (int i = 0; i < 3; i++)
				{
					int num2 = 4 * this.triangle_edges[num + i];
					zero[i] = ((this.edges[num2 + 2] == tID) ? this.edges[num2 + 3] : this.edges[num2 + 2]);
				}
				return zero;
			}
			return DMesh3.InvalidTriangle;
		}

		public IEnumerable<int> TriTrianglesItr(int tID)
		{
			if (this.triangles_refcount.isValid(tID))
			{
				int tei = 3 * tID;
				int num3;
				for (int i = 0; i < 3; i = num3)
				{
					int num = 4 * this.triangle_edges[tei + i];
					int num2 = (this.edges[num + 2] == tID) ? this.edges[num + 3] : this.edges[num + 2];
					if (num2 != -1)
					{
						yield return num2;
					}
					num3 = i + 1;
				}
			}
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
			int result = this.max_group_id + 1;
			this.max_group_id = result;
			return result;
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

		public Vector3d GetTriBaryPoint(int tID, double bary0, double bary1, double bary2)
		{
			int num = 3 * this.triangles[3 * tID];
			int num2 = 3 * this.triangles[3 * tID + 1];
			int num3 = 3 * this.triangles[3 * tID + 2];
			return new Vector3d(bary0 * this.vertices[num] + bary1 * this.vertices[num2] + bary2 * this.vertices[num3], bary0 * this.vertices[num + 1] + bary1 * this.vertices[num2 + 1] + bary2 * this.vertices[num3 + 1], bary0 * this.vertices[num + 2] + bary1 * this.vertices[num2 + 2] + bary2 * this.vertices[num3 + 2]);
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

		public Vector3d GetTriBaryNormal(int tID, double bary0, double bary1, double bary2)
		{
			int num = 3 * this.triangles[3 * tID];
			int num2 = 3 * this.triangles[3 * tID + 1];
			int num3 = 3 * this.triangles[3 * tID + 2];
			Vector3d result = new Vector3d(bary0 * (double)this.normals[num] + bary1 * (double)this.normals[num2] + bary2 * (double)this.normals[num3], bary0 * (double)this.normals[num + 1] + bary1 * (double)this.normals[num2 + 1] + bary2 * (double)this.normals[num3 + 1], bary0 * (double)this.normals[num + 2] + bary1 * (double)this.normals[num2 + 2] + bary2 * (double)this.normals[num3 + 2]);
			result.Normalize(2.220446049250313E-16);
			return result;
		}

		public Vector3d GetTriCentroid(int tID)
		{
			int num = 3 * this.triangles[3 * tID];
			int num2 = 3 * this.triangles[3 * tID + 1];
			int num3 = 3 * this.triangles[3 * tID + 2];
			double num4 = 0.3333333333333333;
			return new Vector3d((this.vertices[num] + this.vertices[num2] + this.vertices[num3]) * num4, (this.vertices[num + 1] + this.vertices[num2 + 1] + this.vertices[num3 + 1]) * num4, (this.vertices[num + 2] + this.vertices[num2 + 2] + this.vertices[num3 + 2]) * num4);
		}

		public void GetTriBaryPoint(int tID, double bary0, double bary1, double bary2, out NewVertexInfo vinfo)
		{
			vinfo = default(NewVertexInfo);
			int num = 3 * this.triangles[3 * tID];
			int num2 = 3 * this.triangles[3 * tID + 1];
			int num3 = 3 * this.triangles[3 * tID + 2];
			vinfo.v = new Vector3d(bary0 * this.vertices[num] + bary1 * this.vertices[num2] + bary2 * this.vertices[num3], bary0 * this.vertices[num + 1] + bary1 * this.vertices[num2 + 1] + bary2 * this.vertices[num3 + 1], bary0 * this.vertices[num + 2] + bary1 * this.vertices[num2 + 2] + bary2 * this.vertices[num3 + 2]);
			vinfo.bHaveN = this.HasVertexNormals;
			if (vinfo.bHaveN)
			{
				vinfo.n = new Vector3f(bary0 * (double)this.normals[num] + bary1 * (double)this.normals[num2] + bary2 * (double)this.normals[num3], bary0 * (double)this.normals[num + 1] + bary1 * (double)this.normals[num2 + 1] + bary2 * (double)this.normals[num3 + 1], bary0 * (double)this.normals[num + 2] + bary1 * (double)this.normals[num2 + 2] + bary2 * (double)this.normals[num3 + 2]);
				vinfo.n.Normalize(1.1920929E-07f);
			}
			vinfo.bHaveC = this.HasVertexColors;
			if (vinfo.bHaveC)
			{
				vinfo.c = new Vector3f(bary0 * (double)this.colors[num] + bary1 * (double)this.colors[num2] + bary2 * (double)this.colors[num3], bary0 * (double)this.colors[num + 1] + bary1 * (double)this.colors[num2 + 1] + bary2 * (double)this.colors[num3 + 1], bary0 * (double)this.colors[num + 2] + bary1 * (double)this.colors[num2 + 2] + bary2 * (double)this.colors[num3 + 2]);
			}
			vinfo.bHaveUV = this.HasVertexUVs;
			if (vinfo.bHaveUV)
			{
				num = 2 * this.triangles[3 * tID];
				num2 = 2 * this.triangles[3 * tID + 1];
				num3 = 2 * this.triangles[3 * tID + 2];
				vinfo.uv = new Vector2f(bary0 * (double)this.uv[num] + bary1 * (double)this.uv[num2] + bary2 * (double)this.uv[num3], bary0 * (double)this.uv[num + 1] + bary1 * (double)this.uv[num2 + 1] + bary2 * (double)this.uv[num3 + 1]);
			}
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
			int num2 = 3 * this.triangles[num + nEdge % 3];
			int num3 = 3 * this.triangles[num + (nEdge + 1) % 3];
			int num4 = 3 * this.triangles[num + (nEdge + 2) % 3];
			Vector3d vector3d = new Vector3d(this.vertices[num2], this.vertices[num2 + 1], this.vertices[num2 + 2]);
			Vector3d vector3d2 = new Vector3d(this.vertices[num3], this.vertices[num3 + 1], this.vertices[num3 + 2]);
			Vector3d vector3d3 = new Vector3d(this.vertices[num4], this.vertices[num4 + 1], this.vertices[num4 + 2]);
			Vector3d vector3d4 = vector3d2 - vector3d;
			vector3d4.Normalize(2.220446049250313E-16);
			Vector3d v = vector3d3 - vector3d2;
			v.Normalize(2.220446049250313E-16);
			Vector3d v2 = vector3d4.Cross(v);
			v2.Normalize(2.220446049250313E-16);
			Vector3d v3 = v2.Cross(vector3d4);
			return new Frame3f((Vector3f)(vector3d + vector3d2 + vector3d3) / 3f, (Vector3f)vector3d4, (Vector3f)v3, (Vector3f)v2);
		}

		public double GetTriSolidAngle(int tID, ref Vector3d p)
		{
			int num = 3 * tID;
			int num2 = 3 * this.triangles[num];
			Vector3d vector3d = new Vector3d(this.vertices[num2] - p.x, this.vertices[num2 + 1] - p.y, this.vertices[num2 + 2] - p.z);
			int num3 = 3 * this.triangles[num + 1];
			Vector3d vector3d2 = new Vector3d(this.vertices[num3] - p.x, this.vertices[num3 + 1] - p.y, this.vertices[num3 + 2] - p.z);
			int num4 = 3 * this.triangles[num + 2];
			Vector3d vector3d3 = new Vector3d(this.vertices[num4] - p.x, this.vertices[num4 + 1] - p.y, this.vertices[num4 + 2] - p.z);
			double length = vector3d.Length;
			double length2 = vector3d2.Length;
			double length3 = vector3d3.Length;
			double x = length * length2 * length3 + vector3d.Dot(ref vector3d2) * length3 + vector3d2.Dot(ref vector3d3) * length + vector3d3.Dot(ref vector3d) * length2;
			double y = vector3d.x * (vector3d2.y * vector3d3.z - vector3d3.y * vector3d2.z) - vector3d.y * (vector3d2.x * vector3d3.z - vector3d3.x * vector3d2.z) + vector3d.z * (vector3d2.x * vector3d3.y - vector3d3.x * vector3d2.y);
			return 2.0 * Math.Atan2(y, x);
		}

		public double GetTriInternalAngleR(int tID, int i)
		{
			int num = 3 * tID;
			int num2 = 3 * this.triangles[num];
			Vector3d vector3d = new Vector3d(this.vertices[num2], this.vertices[num2 + 1], this.vertices[num2 + 2]);
			int num3 = 3 * this.triangles[num + 1];
			Vector3d vector3d2 = new Vector3d(this.vertices[num3], this.vertices[num3 + 1], this.vertices[num3 + 2]);
			int num4 = 3 * this.triangles[num + 2];
			Vector3d vector3d3 = new Vector3d(this.vertices[num4], this.vertices[num4 + 1], this.vertices[num4 + 2]);
			if (i == 0)
			{
				return (vector3d2 - vector3d).Normalized.AngleR((vector3d3 - vector3d).Normalized);
			}
			if (i == 1)
			{
				return (vector3d - vector3d2).Normalized.AngleR((vector3d3 - vector3d2).Normalized);
			}
			return (vector3d - vector3d3).Normalized.AngleR((vector3d2 - vector3d3).Normalized);
		}

		public Index2i GetEdgeV(int eID)
		{
			int num = 4 * eID;
			return new Index2i(this.edges[num], this.edges[num + 1]);
		}

		public bool GetEdgeV(int eID, ref Vector3d a, ref Vector3d b)
		{
			int num = 3 * this.edges[4 * eID];
			a.x = this.vertices[num];
			a.y = this.vertices[num + 1];
			a.z = this.vertices[num + 2];
			int num2 = 3 * this.edges[4 * eID + 1];
			b.x = this.vertices[num2];
			b.y = this.vertices[num2 + 1];
			b.z = this.vertices[num2 + 2];
			return true;
		}

		public Index2i GetEdgeT(int eID)
		{
			int num = 4 * eID;
			return new Index2i(this.edges[num + 2], this.edges[num + 3]);
		}

		public Index4i GetEdge(int eID)
		{
			int num = 4 * eID;
			return new Index4i(this.edges[num], this.edges[num + 1], this.edges[num + 2], this.edges[num + 3]);
		}

		public bool GetEdge(int eID, ref int a, ref int b, ref int t0, ref int t1)
		{
			int num = eID * 4;
			a = this.edges[num];
			b = this.edges[num + 1];
			t0 = this.edges[num + 2];
			t1 = this.edges[num + 3];
			return true;
		}

		public Index2i GetOrientedBoundaryEdgeV(int eID)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 4 * eID;
				if (this.edges[num + 3] == -1)
				{
					int a = this.edges[num];
					int b = this.edges[num + 1];
					int num2 = 3 * this.edges[num + 2];
					Index3i index3i = new Index3i(this.triangles[num2], this.triangles[num2 + 1], this.triangles[num2 + 2]);
					int num3 = IndexUtil.find_edge_index_in_tri(a, b, ref index3i);
					return new Index2i(index3i[num3], index3i[(num3 + 1) % 3]);
				}
			}
			return DMesh3.InvalidEdge;
		}

		public Vector3d GetEdgeNormal(int eID)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 4 * eID;
				Vector3d vector3d = this.GetTriNormal(this.edges[num + 2]);
				if (this.edges[num + 3] != -1)
				{
					vector3d += this.GetTriNormal(this.edges[num + 3]);
					vector3d.Normalize(2.220446049250313E-16);
				}
				return vector3d;
			}
			return Vector3d.Zero;
		}

		public Vector3d GetEdgePoint(int eID, double t)
		{
			if (this.edges_refcount.isValid(eID))
			{
				int num = 4 * eID;
				int num2 = 3 * this.edges[num];
				int num3 = 3 * this.edges[num + 1];
				double num4 = 1.0 - t;
				return new Vector3d(num4 * this.vertices[num2] + t * this.vertices[num3], num4 * this.vertices[num2 + 1] + t * this.vertices[num3 + 1], num4 * this.vertices[num2 + 2] + t * this.vertices[num3 + 2]);
			}
			return Vector3d.Zero;
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

		public int AppendVertex(ref NewVertexInfo info)
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
			if (this.uv != null)
			{
				Vector2f vector2f = info.bHaveUV ? info.uv : Vector2f.Zero;
				int num3 = 2 * num;
				this.uv.insert(vector2f[1], num3 + 1);
				this.uv.insert(vector2f[0], num3);
			}
			this.allocate_edges_list(num);
			this.updateTimeStamp(true);
			return num;
		}

		public int AppendVertex(NewVertexInfo info)
		{
			return this.AppendVertex(ref info);
		}

		public int AppendVertex(DMesh3 from, int fromVID)
		{
			int num = 3 * fromVID;
			int num2 = this.vertices_refcount.allocate();
			int num3 = 3 * num2;
			this.vertices.insert(from.vertices[num + 2], num3 + 2);
			this.vertices.insert(from.vertices[num + 1], num3 + 1);
			this.vertices.insert(from.vertices[num], num3);
			if (this.normals != null)
			{
				if (from.normals != null)
				{
					this.normals.insert(from.normals[num + 2], num3 + 2);
					this.normals.insert(from.normals[num + 1], num3 + 1);
					this.normals.insert(from.normals[num], num3);
				}
				else
				{
					this.normals.insert(0f, num3 + 2);
					this.normals.insert(1f, num3 + 1);
					this.normals.insert(0f, num3);
				}
			}
			if (this.colors != null)
			{
				if (from.colors != null)
				{
					this.colors.insert(from.colors[num + 2], num3 + 2);
					this.colors.insert(from.colors[num + 1], num3 + 1);
					this.colors.insert(from.colors[num], num3);
				}
				else
				{
					this.colors.insert(1f, num3 + 2);
					this.colors.insert(1f, num3 + 1);
					this.colors.insert(1f, num3);
				}
			}
			if (this.uv != null)
			{
				int num4 = 2 * num2;
				if (from.uv != null)
				{
					int num5 = 2 * fromVID;
					this.uv.insert(from.uv[num5 + 1], num4 + 1);
					this.uv.insert(from.uv[num5], num4);
				}
				else
				{
					this.uv.insert(0f, num4 + 1);
					this.uv.insert(0f, num4);
				}
			}
			this.allocate_edges_list(num2);
			this.updateTimeStamp(true);
			return num2;
		}

		public MeshResult InsertVertex(int vid, ref NewVertexInfo info, bool bUnsafe = false)
		{
			if (this.vertices_refcount.isValid(vid))
			{
				return MeshResult.Failed_VertexAlreadyExists;
			}
			if (!(bUnsafe ? this.vertices_refcount.allocate_at_unsafe(vid) : this.vertices_refcount.allocate_at(vid)))
			{
				return MeshResult.Failed_CannotAllocateVertex;
			}
			int num = 3 * vid;
			this.vertices.insert(info.v[2], num + 2);
			this.vertices.insert(info.v[1], num + 1);
			this.vertices.insert(info.v[0], num);
			if (this.normals != null)
			{
				Vector3f vector3f = info.bHaveN ? info.n : Vector3f.AxisY;
				this.normals.insert(vector3f[2], num + 2);
				this.normals.insert(vector3f[1], num + 1);
				this.normals.insert(vector3f[0], num);
			}
			if (this.colors != null)
			{
				Vector3f vector3f2 = info.bHaveC ? info.c : Vector3f.One;
				this.colors.insert(vector3f2[2], num + 2);
				this.colors.insert(vector3f2[1], num + 1);
				this.colors.insert(vector3f2[0], num);
			}
			if (this.uv != null)
			{
				Vector2f vector2f = info.bHaveUV ? info.uv : Vector2f.Zero;
				int num2 = 2 * vid;
				this.uv.insert(vector2f[1], num2 + 1);
				this.uv.insert(vector2f[0], num2);
			}
			this.allocate_edges_list(vid);
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public MeshResult InsertVertex(int vid, NewVertexInfo info)
		{
			return this.InsertVertex(vid, ref info, false);
		}

		public virtual void BeginUnsafeVerticesInsert()
		{
		}

		public virtual void EndUnsafeVerticesInsert()
		{
			this.vertices_refcount.rebuild_free_list();
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
			int num = this.find_edge(tv[0], tv[1]);
			int num2 = this.find_edge(tv[1], tv[2]);
			int num3 = this.find_edge(tv[2], tv[0]);
			if ((num != -1 && !this.IsBoundaryEdge(num)) || (num2 != -1 && !this.IsBoundaryEdge(num2)) || (num3 != -1 && !this.IsBoundaryEdge(num3)))
			{
				return -2;
			}
			int num4 = this.triangles_refcount.allocate();
			int num5 = 3 * num4;
			this.triangles.insert(tv[2], num5 + 2);
			this.triangles.insert(tv[1], num5 + 1);
			this.triangles.insert(tv[0], num5);
			if (this.triangle_groups != null)
			{
				this.triangle_groups.insert(gid, num4);
				this.max_group_id = Math.Max(this.max_group_id, gid + 1);
			}
			this.vertices_refcount.increment(tv[0], 1);
			this.vertices_refcount.increment(tv[1], 1);
			this.vertices_refcount.increment(tv[2], 1);
			this.add_tri_edge(num4, tv[0], tv[1], 0, num);
			this.add_tri_edge(num4, tv[1], tv[2], 1, num2);
			this.add_tri_edge(num4, tv[2], tv[0], 2, num3);
			this.updateTimeStamp(true);
			return num4;
		}

		private void add_tri_edge(int tid, int v0, int v1, int j, int eid)
		{
			if (eid != -1)
			{
				this.edges[4 * eid + 3] = tid;
				this.triangle_edges.insert(eid, 3 * tid + j);
				return;
			}
			this.triangle_edges.insert(this.add_edge(v0, v1, tid, -1), 3 * tid + j);
		}

		public MeshResult InsertTriangle(int tid, Index3i tv, int gid = -1, bool bUnsafe = false)
		{
			if (this.triangles_refcount.isValid(tid))
			{
				return MeshResult.Failed_TriangleAlreadyExists;
			}
			if (!this.IsVertex(tv[0]) || !this.IsVertex(tv[1]) || !this.IsVertex(tv[2]))
			{
				return MeshResult.Failed_NotAVertex;
			}
			if (tv[0] == tv[1] || tv[0] == tv[2] || tv[1] == tv[2])
			{
				return MeshResult.Failed_InvalidNeighbourhood;
			}
			int num = this.find_edge(tv[0], tv[1]);
			int num2 = this.find_edge(tv[1], tv[2]);
			int num3 = this.find_edge(tv[2], tv[0]);
			if ((num != -1 && !this.IsBoundaryEdge(num)) || (num2 != -1 && !this.IsBoundaryEdge(num2)) || (num3 != -1 && !this.IsBoundaryEdge(num3)))
			{
				return MeshResult.Failed_WouldCreateNonmanifoldEdge;
			}
			if (!(bUnsafe ? this.triangles_refcount.allocate_at_unsafe(tid) : this.triangles_refcount.allocate_at(tid)))
			{
				return MeshResult.Failed_CannotAllocateTriangle;
			}
			int num4 = 3 * tid;
			this.triangles.insert(tv[2], num4 + 2);
			this.triangles.insert(tv[1], num4 + 1);
			this.triangles.insert(tv[0], num4);
			if (this.triangle_groups != null)
			{
				this.triangle_groups.insert(gid, tid);
				this.max_group_id = Math.Max(this.max_group_id, gid + 1);
			}
			this.vertices_refcount.increment(tv[0], 1);
			this.vertices_refcount.increment(tv[1], 1);
			this.vertices_refcount.increment(tv[2], 1);
			this.add_tri_edge(tid, tv[0], tv[1], 0, num);
			this.add_tri_edge(tid, tv[1], tv[2], 1, num2);
			this.add_tri_edge(tid, tv[2], tv[0], 2, num3);
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public virtual void BeginUnsafeTrianglesInsert()
		{
		}

		public virtual void EndUnsafeTrianglesInsert()
		{
			this.triangles_refcount.rebuild_free_list();
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

		public void EnableVertexUVs(Vector2f initial_uv)
		{
			if (this.HasVertexUVs)
			{
				return;
			}
			this.uv = new DVector<float>();
			int maxVertexID = this.MaxVertexID;
			this.uv.resize(2 * maxVertexID);
			for (int i = 0; i < maxVertexID; i++)
			{
				int num = 2 * i;
				this.uv[num] = initial_uv.x;
				this.uv[num + 1] = initial_uv.y;
			}
		}

		public void DiscardVertexUVs()
		{
			this.uv = null;
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
				if (this.edges[4 * num + 3] == -1)
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

		public IEnumerable<Index4i> Edges()
		{
			foreach (object obj in this.edges_refcount)
			{
				int num = (int)obj;
				int num2 = 4 * num;
				yield return new Index4i(this.edges[num2], this.edges[num2 + 1], this.edges[num2 + 2], this.edges[num2 + 3]);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public int FindEdge(int vA, int vB)
		{
			return this.find_edge(vA, vB);
		}

		public int FindEdgeFromTri(int vA, int vB, int tID)
		{
			return this.find_edge_from_tri(vA, vB, tID);
		}

		public Index2i GetEdgeOpposingV(int eID)
		{
			int num = 4 * eID;
			int a = this.edges[num];
			int b = this.edges[num + 1];
			int ti = this.edges[num + 2];
			int num2 = this.edges[num + 3];
			int ii = IndexUtil.find_tri_other_vtx(a, b, this.triangles, ti);
			if (num2 != -1)
			{
				int jj = IndexUtil.find_tri_other_vtx(a, b, this.triangles, num2);
				return new Index2i(ii, jj);
			}
			return new Index2i(ii, -1);
		}

		public int FindTriangle(int a, int b, int c)
		{
			int num = this.find_edge(a, b);
			if (num == -1)
			{
				return -1;
			}
			int num2 = 4 * num;
			int num3 = 3 * this.edges[num2 + 2];
			if (this.triangles[num3] == c || this.triangles[num3 + 1] == c || this.triangles[num3 + 2] == c)
			{
				return this.edges[num2 + 2];
			}
			if (this.edges[num2 + 3] != -1)
			{
				num3 = 3 * this.edges[num2 + 3];
				if (this.triangles[num3] == c || this.triangles[num3 + 1] == c || this.triangles[num3 + 2] == c)
				{
					return this.edges[num2 + 3];
				}
			}
			return -1;
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

		public int VtxBoundaryEdges(int vID, ref int e0, ref int e1)
		{
			if (this.vertices_refcount.isValid(vID))
			{
				int num = 0;
				foreach (int num2 in this.vertex_edges.ValueItr(vID))
				{
					int num3 = 4 * num2;
					if (this.edges[num3 + 3] == -1)
					{
						if (num == 0)
						{
							e0 = num2;
						}
						else if (num == 1)
						{
							e1 = num2;
						}
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
					int num2 = 4 * num;
					if (this.edges[num2 + 3] == -1)
					{
						e[result++] = num;
					}
				}
				return result;
			}
			return -1;
		}

		public MeshResult GetVtxTriangles(int vID, List<int> vTriangles, bool bUseOrientation)
		{
			if (!this.IsVertex(vID))
			{
				return MeshResult.Failed_NotAVertex;
			}
			if (bUseOrientation)
			{
				using (IEnumerator<int> enumerator = this.vertex_edges.ValueItr(vID).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int num = enumerator.Current;
						int vB = this.edge_other_v(num, vID);
						int num2 = 4 * num;
						int num3 = this.edges[num2 + 2];
						if (this.tri_has_sequential_v(num3, vID, vB))
						{
							vTriangles.Add(num3);
						}
						int num4 = this.edges[num2 + 3];
						if (num4 != -1 && this.tri_has_sequential_v(num4, vID, vB))
						{
							vTriangles.Add(num4);
						}
					}
					return MeshResult.Ok;
				}
			}
			foreach (int num5 in this.vertex_edges.ValueItr(vID))
			{
				int num6 = 4 * num5;
				int item = this.edges[num6 + 2];
				if (!vTriangles.Contains(item))
				{
					vTriangles.Add(item);
				}
				int num7 = this.edges[num6 + 3];
				if (num7 != -1 && !vTriangles.Contains(num7))
				{
					vTriangles.Add(num7);
				}
			}
			return MeshResult.Ok;
		}

		public int GetVtxTriangleCount(int vID, bool bBruteForce = false)
		{
			if (bBruteForce)
			{
				List<int> list = new List<int>();
				if (this.GetVtxTriangles(vID, list, false) != MeshResult.Ok)
				{
					return -1;
				}
				return list.Count;
			}
			else
			{
				if (!this.IsVertex(vID))
				{
					return -1;
				}
				int num = 0;
				foreach (int num2 in this.vertex_edges.ValueItr(vID))
				{
					int vB = this.edge_other_v(num2, vID);
					int num3 = 4 * num2;
					int tID = this.edges[num3 + 2];
					if (this.tri_has_sequential_v(tID, vID, vB))
					{
						num++;
					}
					int num4 = this.edges[num3 + 3];
					if (num4 != -1 && this.tri_has_sequential_v(num4, vID, vB))
					{
						num++;
					}
				}
				return num;
			}
		}

		public IEnumerable<int> VtxTrianglesItr(int vID)
		{
			if (this.IsVertex(vID))
			{
				foreach (int num in this.vertex_edges.ValueItr(vID))
				{
					int vOther = this.edge_other_v(num, vID);
					int i = 4 * num;
					int num2 = this.edges[i + 2];
					if (this.tri_has_sequential_v(num2, vID, vOther))
					{
						yield return num2;
					}
					int num3 = this.edges[i + 3];
					if (num3 != -1 && this.tri_has_sequential_v(num3, vID, vOther))
					{
						yield return num3;
					}
				}
				IEnumerator<int> enumerator = null;
			}
			yield break;
			yield break;
		}

		public void GetVtxNbrhood(int eID, int vID, ref int vOther, ref int oppV1, ref int oppV2, ref int t1, ref int t2)
		{
			int num = 4 * eID;
			vOther = ((this.edges[num] == vID) ? this.edges[num + 1] : this.edges[num]);
			t1 = this.edges[num + 2];
			oppV1 = IndexUtil.find_tri_other_vtx(vID, vOther, this.triangles, t1);
			t2 = this.edges[num + 3];
			if (t2 != -1)
			{
				oppV2 = IndexUtil.find_tri_other_vtx(vID, vOther, this.triangles, t2);
				return;
			}
			t2 = -1;
		}

		public void VtxOneRingCentroid(int vID, ref Vector3d centroid)
		{
			centroid = Vector3d.Zero;
			if (this.vertices_refcount.isValid(vID))
			{
				int num = 0;
				foreach (int eID in this.vertex_edges.ValueItr(vID))
				{
					int num2 = 3 * this.edge_other_v(eID, vID);
					centroid.x += this.vertices[num2];
					centroid.y += this.vertices[num2 + 1];
					centroid.z += this.vertices[num2 + 2];
					num++;
				}
				if (num > 0)
				{
					double num3 = 1.0 / (double)num;
					centroid.x *= num3;
					centroid.y *= num3;
					centroid.z *= num3;
				}
			}
		}

		public bool tri_has_v(int tID, int vID)
		{
			int num = 3 * tID;
			return this.triangles[num] == vID || this.triangles[num + 1] == vID || this.triangles[num + 2] == vID;
		}

		public bool tri_is_boundary(int tID)
		{
			int num = 3 * tID;
			return this.IsBoundaryEdge(this.triangle_edges[num]) || this.IsBoundaryEdge(this.triangle_edges[num + 1]) || this.IsBoundaryEdge(this.triangle_edges[num + 2]);
		}

		public bool tri_has_neighbour_t(int tCheck, int tNbr)
		{
			int num = 3 * tCheck;
			return this.edge_has_t(this.triangle_edges[num], tNbr) || this.edge_has_t(this.triangle_edges[num + 1], tNbr) || this.edge_has_t(this.triangle_edges[num + 2], tNbr);
		}

		public bool tri_has_sequential_v(int tID, int vA, int vB)
		{
			int num = 3 * tID;
			int num2 = this.triangles[num];
			int num3 = this.triangles[num + 1];
			int num4 = this.triangles[num + 2];
			return (num2 == vA && num3 == vB) || (num3 == vA && num4 == vB) || (num4 == vA && num2 == vB);
		}

		public int find_tri_neighbour_edge(int tID, int vA, int vB)
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

		public int find_tri_neighbour_index(int tID, int vA, int vB)
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

		public bool IsBoundaryEdge(int eid)
		{
			return this.edges[4 * eid + 3] == -1;
		}

		[Obsolete("edge_is_boundary will be removed in future, use IsBoundaryEdge instead")]
		public bool edge_is_boundary(int eid)
		{
			return this.edges[4 * eid + 3] == -1;
		}

		public bool edge_has_v(int eid, int vid)
		{
			int num = 4 * eid;
			return this.edges[num] == vid || this.edges[num + 1] == vid;
		}

		public bool edge_has_t(int eid, int tid)
		{
			int num = 4 * eid;
			return this.edges[num + 2] == tid || this.edges[num + 3] == tid;
		}

		public int edge_other_v(int eID, int vID)
		{
			int num = 4 * eID;
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

		public int edge_other_t(int eID, int tid)
		{
			int num = 4 * eID;
			int num2 = this.edges[num + 2];
			int num3 = this.edges[num + 3];
			if (num2 == tid)
			{
				return num3;
			}
			if (num3 != tid)
			{
				return -1;
			}
			return num2;
		}

		[Obsolete("vertex_is_boundary will be removed in future, use IsBoundaryVertex instead")]
		public bool vertex_is_boundary(int vID)
		{
			return this.IsBoundaryVertex(vID);
		}

		public bool IsBoundaryVertex(int vID)
		{
			foreach (int num in this.vertex_edges.ValueItr(vID))
			{
				if (this.edges[4 * num + 3] == -1)
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
				if (this.edges[4 * num2 + 1] == num)
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

		public bool IsGroupBoundaryEdge(int eID)
		{
			if (!this.IsEdge(eID))
			{
				throw new Exception("DMesh3.IsGroupBoundaryEdge: " + eID.ToString() + " is not a valid edge");
			}
			if (this.triangle_groups == null)
			{
				throw new Exception("DMesh3.IsGroupBoundaryEdge: no triangle groups!");
			}
			int num = this.edges[4 * eID + 3];
			if (num == -1)
			{
				return false;
			}
			int num2 = this.triangle_groups[num];
			int i = this.edges[4 * eID + 2];
			int num3 = this.triangle_groups[i];
			return num2 != num3;
		}

		public bool IsGroupBoundaryVertex(int vID)
		{
			if (!this.IsVertex(vID))
			{
				throw new Exception("DMesh3.IsGroupBoundaryVertex: " + vID.ToString() + " is not a valid vertex");
			}
			if (this.triangle_groups == null)
			{
				throw new Exception("DMesh3.IsGroupBoundaryVertex: no triangle groups!");
			}
			int num = int.MinValue;
			foreach (int num2 in this.vertex_edges.ValueItr(vID))
			{
				int i = this.edges[4 * num2 + 2];
				int num3 = this.triangle_groups[i];
				if (num != num3)
				{
					if (num != -2147483648)
					{
						return true;
					}
					num = num3;
				}
				int num4 = this.edges[4 * num2 + 3];
				if (num4 != -1)
				{
					int num5 = this.triangle_groups[num4];
					if (num != num5)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsGroupJunctionVertex(int vID)
		{
			if (!this.IsVertex(vID))
			{
				throw new Exception("DMesh3.IsGroupJunctionVertex: " + vID.ToString() + " is not a valid vertex");
			}
			if (this.triangle_groups == null)
			{
				throw new Exception("DMesh3.IsGroupJunctionVertex: no triangle groups!");
			}
			Index2i max = Index2i.Max;
			foreach (int num in this.vertex_edges.ValueItr(vID))
			{
				Index2i index2i = new Index2i(this.edges[4 * num + 2], this.edges[4 * num + 3]);
				for (int i = 0; i < 2; i++)
				{
					if (index2i[i] != -1)
					{
						int num2 = this.triangle_groups[index2i[i]];
						if (num2 != max.a && num2 != max.b)
						{
							if (max.a != Index2i.Max.a && max.b != Index2i.Max.b)
							{
								return true;
							}
							if (max.a == Index2i.Max.a)
							{
								max.a = num2;
							}
							else
							{
								max.b = num2;
							}
						}
					}
				}
			}
			return false;
		}

		public bool GetVertexGroups(int vID, out Index4i groups)
		{
			groups = Index4i.Max;
			int num = 0;
			if (!this.IsVertex(vID))
			{
				throw new Exception("DMesh3.GetVertexGroups: " + vID.ToString() + " is not a valid vertex");
			}
			if (this.triangle_groups == null)
			{
				throw new Exception("DMesh3.GetVertexGroups: no triangle groups!");
			}
			foreach (int num2 in this.vertex_edges.ValueItr(vID))
			{
				int i = this.edges[4 * num2 + 2];
				int num3 = this.triangle_groups[i];
				if (!groups.Contains(num3))
				{
					groups[num++] = num3;
				}
				if (num == 4)
				{
					return false;
				}
				int num4 = this.edges[4 * num2 + 3];
				if (num4 != -1)
				{
					int num5 = this.triangle_groups[num4];
					if (!groups.Contains(num5))
					{
						groups[num++] = num5;
					}
					if (num == 4)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool GetAllVertexGroups(int vID, ref List<int> groups)
		{
			if (!this.IsVertex(vID))
			{
				throw new Exception("DMesh3.GetAllVertexGroups: " + vID.ToString() + " is not a valid vertex");
			}
			if (this.triangle_groups == null)
			{
				throw new Exception("DMesh3.GetAllVertexGroups: no triangle groups!");
			}
			foreach (int num in this.vertex_edges.ValueItr(vID))
			{
				int i = this.edges[4 * num + 2];
				int item = this.triangle_groups[i];
				if (!groups.Contains(item))
				{
					groups.Add(item);
				}
				int num2 = this.edges[4 * num + 3];
				if (num2 != -1)
				{
					int item2 = this.triangle_groups[num2];
					if (!groups.Contains(item2))
					{
						groups.Add(item2);
					}
				}
			}
			return true;
		}

		public List<int> GetAllVertexGroups(int vID)
		{
			List<int> result = new List<int>();
			this.GetAllVertexGroups(vID, ref result);
			return result;
		}

		public bool IsBowtieVertex(int vID)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				throw new Exception("DMesh3.IsBowtieVertex: " + vID.ToString() + " is not a valid vertex");
			}
			int num = this.vertex_edges.Count(vID);
			if (num == 0)
			{
				return false;
			}
			int num2 = -1;
			bool flag = false;
			foreach (int num3 in this.vertex_edges.ValueItr(vID))
			{
				if (this.edges[4 * num3 + 3] == -1)
				{
					flag = true;
					num2 = num3;
					break;
				}
			}
			if (num2 == -1)
			{
				num2 = this.vertex_edges.First(vID);
			}
			int num4 = this.edges[4 * num2 + 2];
			int num5 = num2;
			int num6 = 1;
			for (;;)
			{
				int num7 = 3 * num4;
				Index3i index3i = new Index3i(this.triangles[num7], this.triangles[num7 + 1], this.triangles[num7 + 2]);
				Index3i index3i2 = new Index3i(this.triangle_edges[num7], this.triangle_edges[num7 + 1], this.triangle_edges[num7 + 2]);
				int num8 = IndexUtil.find_tri_index(vID, ref index3i);
				int num9 = index3i2[num8];
				int num10 = index3i2[(num8 + 2) % 3];
				int num11 = (num9 == num5) ? num10 : num9;
				if (num11 == num2)
				{
					break;
				}
				Index2i edgeT = this.GetEdgeT(num11);
				int num12 = (edgeT.a == num4) ? edgeT.b : edgeT.a;
				if (num12 == -1)
				{
					break;
				}
				num5 = num11;
				num4 = num12;
				num6++;
			}
			return (flag ? (num - 1) : num) != num6;
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
						if (this.IsBoundaryEdge(eid))
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
				if (this.edges_refcount.isValid(i) && this.IsBoundaryEdge(i))
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
				if (this.cached_is_closed_timestamp != this.Timestamp)
				{
					this.cached_is_closed = this.IsClosed();
					this.cached_is_closed_timestamp = this.Timestamp;
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

		public bool IsCompactT
		{
			get
			{
				return this.triangles_refcount.is_dense;
			}
		}

		public double CompactMetric
		{
			get
			{
				return ((double)this.VertexCount / (double)this.MaxVertexID + (double)this.TriangleCount / (double)this.MaxTriangleID) * 0.5;
			}
		}

		public double WindingNumber(Vector3d v)
		{
			double num = 0.0;
			foreach (object obj in this.triangles_refcount)
			{
				int tID = (int)obj;
				num += this.GetTriSolidAngle(tID, ref v);
			}
			return num / 12.566370614359172;
		}

		public bool HasMetadata
		{
			get
			{
				return this.Metadata != null && this.Metadata.Keys.Count > 0;
			}
		}

		public void AttachMetadata(string key, object o)
		{
			if (this.Metadata == null)
			{
				this.Metadata = new Dictionary<string, object>();
			}
			this.Metadata.Add(key, o);
		}

		public object FindMetadata(string key)
		{
			if (this.Metadata == null)
			{
				return null;
			}
			object result = null;
			if (!this.Metadata.TryGetValue(key, out result))
			{
				return null;
			}
			return result;
		}

		public bool RemoveMetadata(string key)
		{
			return this.Metadata != null && this.Metadata.Remove(key);
		}

		public void ClearMetadata()
		{
			if (this.Metadata != null)
			{
				this.Metadata.Clear();
				this.Metadata = null;
			}
		}

		public DVector<double> VerticesBuffer
		{
			get
			{
				return this.vertices;
			}
			set
			{
				this.vertices = value;
			}
		}

		public RefCountVector VerticesRefCounts
		{
			get
			{
				return this.vertices_refcount;
			}
			set
			{
				this.vertices_refcount = value;
			}
		}

		public DVector<float> NormalsBuffer
		{
			get
			{
				return this.normals;
			}
			set
			{
				this.normals = value;
			}
		}

		public DVector<float> ColorsBuffer
		{
			get
			{
				return this.colors;
			}
			set
			{
				this.colors = value;
			}
		}

		public DVector<float> UVBuffer
		{
			get
			{
				return this.uv;
			}
			set
			{
				this.uv = value;
			}
		}

		public DVector<int> TrianglesBuffer
		{
			get
			{
				return this.triangles;
			}
			set
			{
				this.triangles = value;
			}
		}

		public RefCountVector TrianglesRefCounts
		{
			get
			{
				return this.triangles_refcount;
			}
			set
			{
				this.triangles_refcount = value;
			}
		}

		public DVector<int> GroupsBuffer
		{
			get
			{
				return this.triangle_groups;
			}
			set
			{
				this.triangle_groups = value;
			}
		}

		public DVector<int> EdgesBuffer
		{
			get
			{
				return this.edges;
			}
			set
			{
				this.edges = value;
			}
		}

		public RefCountVector EdgesRefCounts
		{
			get
			{
				return this.edges_refcount;
			}
			set
			{
				this.edges_refcount = value;
			}
		}

		public SmallListSet VertexEdges
		{
			get
			{
				return this.vertex_edges;
			}
			set
			{
				this.vertex_edges = value;
			}
		}

		public void RebuildFromEdgeRefcounts()
		{
			int num = this.vertices.Length / 3;
			int num2 = this.triangles.Length / 3;
			this.triangle_edges.resize(this.triangles.Length);
			this.triangles_refcount.RawRefCounts.resize(num2);
			this.vertex_edges.Resize(num);
			this.vertices_refcount.RawRefCounts.resize(num);
			int num3 = this.edges.Length / 4;
			for (int i = 0; i < num3; i++)
			{
				if (this.edges_refcount.isValid(i))
				{
					int num4 = this.edges[4 * i];
					int num5 = this.edges[4 * i + 1];
					int num6 = this.edges[4 * i + 2];
					int num7 = this.edges[4 * i + 3];
					if (!this.vertices_refcount.isValidUnsafe(num4))
					{
						this.allocate_edges_list(num4);
						this.vertices_refcount.set_Unsafe(num4, 1);
					}
					if (!this.vertices_refcount.isValidUnsafe(num5))
					{
						this.allocate_edges_list(num5);
						this.vertices_refcount.set_Unsafe(num5, 1);
					}
					this.triangles_refcount.set_Unsafe(num6, 1);
					Index3i triangle = this.GetTriangle(num6);
					int num8 = IndexUtil.find_edge_index_in_tri(num4, num5, ref triangle);
					this.triangle_edges[3 * num6 + num8] = i;
					if (num7 != -1)
					{
						this.triangles_refcount.set_Unsafe(num7, 1);
						Index3i triangle2 = this.GetTriangle(num7);
						int num9 = IndexUtil.find_edge_index_in_tri(num4, num5, ref triangle2);
						this.triangle_edges[3 * num7 + num9] = i;
					}
					this.vertex_edges.Insert(num4, i);
					this.vertex_edges.Insert(num5, i);
				}
			}
			bool hasTriangleGroups = this.HasTriangleGroups;
			this.max_group_id = 0;
			for (int j = 0; j < num2; j++)
			{
				if (this.triangles_refcount.isValid(j))
				{
					int index = this.triangles[3 * j];
					int index2 = this.triangles[3 * j + 1];
					int index3 = this.triangles[3 * j + 2];
					this.vertices_refcount.increment(index, 1);
					this.vertices_refcount.increment(index2, 1);
					this.vertices_refcount.increment(index3, 1);
					if (hasTriangleGroups)
					{
						this.max_group_id = Math.Max(this.max_group_id, this.triangle_groups[j]);
					}
				}
			}
			this.max_group_id++;
			this.vertices_refcount.rebuild_free_list();
			this.triangles_refcount.rebuild_free_list();
			this.edges_refcount.rebuild_free_list();
			this.updateTimeStamp(true);
		}

		public DMesh3.CompactInfo CompactInPlace(bool bComputeCompactInfo = false)
		{
			IndexMap indexMap = bComputeCompactInfo ? new IndexMap(this.MaxVertexID, this.VertexCount) : null;
			DMesh3.CompactInfo result = default(DMesh3.CompactInfo);
			result.MapV = indexMap;
			int num = this.MaxVertexID - 1;
			int i = 0;
			while (!this.vertices_refcount.isValidUnsafe(num))
			{
				num--;
			}
			while (this.vertices_refcount.isValidUnsafe(i))
			{
				i++;
			}
			DVector<short> rawRefCounts = this.vertices_refcount.RawRefCounts;
			while (i < num)
			{
				int num2 = i * 3;
				int num3 = num * 3;
				this.vertices[num2] = this.vertices[num3];
				this.vertices[num2 + 1] = this.vertices[num3 + 1];
				this.vertices[num2 + 2] = this.vertices[num3 + 2];
				if (this.normals != null)
				{
					this.normals[num2] = this.normals[num3];
					this.normals[num2 + 1] = this.normals[num3 + 1];
					this.normals[num2 + 2] = this.normals[num3 + 2];
				}
				if (this.colors != null)
				{
					this.colors[num2] = this.colors[num3];
					this.colors[num2 + 1] = this.colors[num3 + 1];
					this.colors[num2 + 2] = this.colors[num3 + 2];
				}
				if (this.uv != null)
				{
					int num4 = i * 2;
					int num5 = num * 2;
					this.uv[num4] = this.uv[num5];
					this.uv[num4 + 1] = this.uv[num5 + 1];
				}
				foreach (int num6 in this.vertex_edges.ValueItr(num))
				{
					this.replace_edge_vertex(num6, num, i);
					int tID = this.edges[4 * num6 + 2];
					this.replace_tri_vertex(tID, num, i);
					int num7 = this.edges[4 * num6 + 3];
					if (num7 != -1)
					{
						this.replace_tri_vertex(num7, num, i);
					}
				}
				rawRefCounts[i] = rawRefCounts[num];
				rawRefCounts[num] = RefCountVector.invalid;
				this.vertex_edges.Move(num, i);
				if (indexMap != null)
				{
					indexMap[num] = i;
				}
				num--;
				i++;
				while (!this.vertices_refcount.isValidUnsafe(num))
				{
					num--;
				}
				while (this.vertices_refcount.isValidUnsafe(i) && i < num)
				{
					i++;
				}
			}
			this.vertices_refcount.trim(this.VertexCount);
			this.vertices.resize(this.VertexCount * 3);
			if (this.normals != null)
			{
				this.normals.resize(this.VertexCount * 3);
			}
			if (this.colors != null)
			{
				this.colors.resize(this.VertexCount * 3);
			}
			if (this.uv != null)
			{
				this.uv.resize(this.VertexCount * 2);
			}
			int num8 = this.MaxTriangleID - 1;
			int j = 0;
			while (!this.triangles_refcount.isValidUnsafe(num8))
			{
				num8--;
			}
			while (this.triangles_refcount.isValidUnsafe(j))
			{
				j++;
			}
			DVector<short> rawRefCounts2 = this.triangles_refcount.RawRefCounts;
			while (j < num8)
			{
				int num9 = j * 3;
				int num10 = num8 * 3;
				for (int k = 0; k < 3; k++)
				{
					this.triangles[num9 + k] = this.triangles[num10 + k];
					this.triangle_edges[num9 + k] = this.triangle_edges[num10 + k];
				}
				if (this.triangle_groups != null)
				{
					this.triangle_groups[j] = this.triangle_groups[num8];
				}
				for (int l = 0; l < 3; l++)
				{
					int eID = this.triangle_edges[num9 + l];
					this.replace_edge_triangle(eID, num8, j);
				}
				rawRefCounts2[j] = rawRefCounts2[num8];
				rawRefCounts2[num8] = RefCountVector.invalid;
				num8--;
				j++;
				while (!this.triangles_refcount.isValidUnsafe(num8))
				{
					num8--;
				}
				while (this.triangles_refcount.isValidUnsafe(j) && j < num8)
				{
					j++;
				}
			}
			this.triangles_refcount.trim(this.TriangleCount);
			this.triangles.resize(this.TriangleCount * 3);
			this.triangle_edges.resize(this.TriangleCount * 3);
			if (this.triangle_groups != null)
			{
				this.triangle_groups.resize(this.TriangleCount);
			}
			int iLastE = this.MaxEdgeID - 1;
			int m = 0;
			while (!this.edges_refcount.isValidUnsafe(iLastE))
			{
				int iLastE2 = iLastE;
				iLastE = iLastE2 - 1;
			}
			while (this.edges_refcount.isValidUnsafe(m))
			{
				m++;
			}
			DVector<short> rawRefCounts3 = this.edges_refcount.RawRefCounts;
			Func<int, bool> <>9__0;
			Func<int, bool> <>9__1;
			while (m < iLastE)
			{
				int num11 = m * 4;
				int num12 = iLastE * 4;
				for (int n = 0; n < 4; n++)
				{
					this.edges[num11 + n] = this.edges[num12 + n];
				}
				int num13 = this.edges[num11];
				int num14 = this.edges[num11 + 1];
				SmallListSet smallListSet = this.vertex_edges;
				int list_index = num13;
				Func<int, bool> findF;
				if ((findF = <>9__0) == null)
				{
					findF = (<>9__0 = ((int eid) => eid == iLastE));
				}
				smallListSet.Replace(list_index, findF, m);
				SmallListSet smallListSet2 = this.vertex_edges;
				int list_index2 = num14;
				Func<int, bool> findF2;
				if ((findF2 = <>9__1) == null)
				{
					findF2 = (<>9__1 = ((int eid) => eid == iLastE));
				}
				smallListSet2.Replace(list_index2, findF2, m);
				this.replace_triangle_edge(this.edges[num11 + 2], iLastE, m);
				if (this.edges[num11 + 3] != -1)
				{
					this.replace_triangle_edge(this.edges[num11 + 3], iLastE, m);
				}
				rawRefCounts3[m] = rawRefCounts3[iLastE];
				rawRefCounts3[iLastE] = RefCountVector.invalid;
				int iLastE2 = iLastE;
				iLastE = iLastE2 - 1;
				m++;
				while (!this.edges_refcount.isValidUnsafe(iLastE))
				{
					iLastE2 = iLastE;
					iLastE = iLastE2 - 1;
				}
				while (this.edges_refcount.isValidUnsafe(m) && m < iLastE)
				{
					m++;
				}
			}
			this.edges_refcount.trim(this.EdgeCount);
			this.edges.resize(this.EdgeCount * 4);
			return result;
		}

		public static explicit operator Mesh(DMesh3 mesh)
		{
			if (!mesh.Clockwise)
			{
				mesh.ReverseOrientation(true);
			}
			Mesh mesh2 = new Mesh();
			mesh2.MarkDynamic();
			if (mesh.VertexCount > 64000 || mesh.TriangleCount > 64000)
			{
				mesh2.indexFormat = IndexFormat.UInt32;
			}
			Vector3[] array = new Vector3[mesh.VertexCount];
			Color[] array2 = new Color[mesh.VertexCount];
			Vector2[] array3 = new Vector2[mesh.VertexCount];
			Vector3[] array4 = new Vector3[mesh.VertexCount];
			for (int i = 0; i < mesh.VertexCount; i++)
			{
				if (mesh.IsVertex(i))
				{
					NewVertexInfo vertexAll = mesh.GetVertexAll(i);
					array[i] = (Vector3)vertexAll.v;
					if (vertexAll.bHaveC)
					{
						array2[i] = vertexAll.c;
					}
					if (vertexAll.bHaveUV)
					{
						array3[i] = vertexAll.uv;
					}
					if (vertexAll.bHaveN)
					{
						array4[i] = vertexAll.n;
					}
				}
			}
			mesh2.vertices = array;
			if (mesh.HasVertexColors)
			{
				mesh2.SetColors(array2);
			}
			if (mesh.HasVertexUVs)
			{
				mesh2.SetUVs(0, array3);
			}
			int[] array5 = new int[mesh.TriangleCount * 3];
			int num = 0;
			foreach (Index3i index3i in mesh.Triangles())
			{
				array5[num * 3] = index3i.a;
				array5[num * 3 + 1] = index3i.b;
				array5[num * 3 + 2] = index3i.c;
				num++;
			}
			mesh2.triangles = array5;
			if (mesh.HasVertexNormals)
			{
				mesh2.SetNormals(array4);
			}
			else
			{
				mesh2.RecalculateNormals();
			}
			mesh2.RecalculateBounds();
			mesh2.RecalculateTangents();
			return mesh2;
		}

		public static explicit operator DMesh3(Mesh mesh)
		{
			DMesh3 dmesh = new DMesh3(true, false, false, false);
			dmesh.Clockwise = true;
			foreach (Vector3 v in mesh.vertices)
			{
				dmesh.AppendVertex(v);
			}
			int[] array2 = mesh.triangles;
			for (int j = 0; j < array2.Length; j += 3)
			{
				dmesh.AppendTriangle(array2[j], array2[j + 1], array2[j + 2], -1);
			}
			dmesh.ReverseOrientation(true);
			return dmesh;
		}

		public void CalculateUVs()
		{
			this.EnableVertexUVs(Vector2f.Zero);
			OrthogonalPlaneFit3 orthogonalPlaneFit = new OrthogonalPlaneFit3(this.Vertices());
			Frame3f frame3f = new Frame3f(orthogonalPlaneFit.Origin, orthogonalPlaneFit.Normal);
			AxisAlignedBox3d cachedBounds = this.CachedBounds;
			AxisAlignedBox2d axisAlignedBox2d = default(AxisAlignedBox2d);
			for (int i = 0; i < 8; i++)
			{
				axisAlignedBox2d.Contain(frame3f.ToPlaneUV((Vector3f)cachedBounds.Corner(i), 3));
			}
			Vector2f vector2f = (Vector2f)axisAlignedBox2d.Min;
			float num = (float)axisAlignedBox2d.Width;
			float num2 = (float)axisAlignedBox2d.Height;
			for (int j = 0; j < this.VertexCount; j++)
			{
				Vector2f vector2f2 = frame3f.ToPlaneUV((Vector3f)this.GetVertex(j), 3);
				vector2f2.x = (vector2f2.x - vector2f.x) / num;
				vector2f2.y = (vector2f2.y - vector2f.y) / num2;
				this.SetVertexUV(j, vector2f2);
			}
		}

		public Task<int> CalculateUVsAsync()
		{
			TaskCompletionSource<int> tcs1 = new TaskCompletionSource<int>();
			Task<int> task = tcs1.Task;
			task.ConfigureAwait(false);
			Task.Factory.StartNew(delegate()
			{
				this.CalculateUVs();
				tcs1.SetResult(1);
			});
			return task;
		}

		public int[] Colorisation()
		{
			int[] array = new int[this.VertexCount];
			int[] array2 = new int[]
			{
				1,
				2,
				3,
				4,
				5,
				6
			};
			int[] array3 = new int[]
			{
				3,
				4,
				5,
				0,
				1,
				2
			};
			foreach (Index3i index3i in this.Triangles())
			{
				int[] array4 = new int[6];
				int[] array5 = index3i.array;
				foreach (int num in array5)
				{
					int num2 = array[num];
					if (num2 != 0)
					{
						for (int j = 0; j < 6; j++)
						{
							if (num2 == array2[j])
							{
								array4[j]++;
								break;
							}
						}
					}
				}
				if (array4.Max() > 1)
				{
					for (int k = 0; k < 6; k++)
					{
						if (array4[k] > 1)
						{
							foreach (int num3 in array5)
							{
								if (array[num3] == array2[k])
								{
									array[num3] = array2[array3[k]];
									break;
								}
							}
							array4[k]--;
							array4[array3[k]]++;
						}
					}
				}
				while (array4.Sum() < 3)
				{
					foreach (int num4 in array5)
					{
						if (array[num4] == 0)
						{
							for (int l = 0; l < 6; l++)
							{
								if (array4[l] == 0 && array4[array3[l]] <= 0)
								{
									array[num4] = array2[l];
									array4[l]++;
									break;
								}
							}
						}
					}
				}
			}
			return array;
		}

		public void Colorisation(out Vector2[] uv)
		{
			int[] array = this.Colorisation();
			uv = new Vector2[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case 1:
				case 4:
					uv[i] = new Vector2(1f, 0f);
					break;
				case 2:
				case 5:
					uv[i] = new Vector2(2f, 0f);
					break;
				case 3:
				case 6:
					uv[i] = new Vector2(3f, 0f);
					break;
				default:
					throw new Exception("Invalid Color in colorisation");
				}
			}
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

		public void debug_print_vertex(int v)
		{
			Console.WriteLine("Vertex " + v.ToString());
			List<int> list = new List<int>();
			this.GetVtxTriangles(v, list, false);
			Console.WriteLine(string.Format("  Tris {0}  Edges {1}  refcount {2}", list.Count, this.GetVtxEdgeCount(v), this.vertices_refcount.refCount(v)));
			foreach (int num in list)
			{
				Index3i triangle = this.GetTriangle(num);
				Index3i triEdges = this.GetTriEdges(num);
				Console.WriteLine(string.Format("  t{6} {0} {1} {2}   te {3} {4} {5}", new object[]
				{
					triangle[0],
					triangle[1],
					triangle[2],
					triEdges[0],
					triEdges[1],
					triEdges[2],
					num
				}));
			}
			foreach (int num2 in this.VtxEdgesItr(v))
			{
				Index2i edgeV = this.GetEdgeV(num2);
				Index2i edgeT = this.GetEdgeT(num2);
				Console.WriteLine(string.Format("  e{4} {0} {1} / {2} {3}", new object[]
				{
					edgeV[0],
					edgeV[1],
					edgeT[0],
					edgeT[1],
					num2
				}));
			}
		}

		public void debug_print_mesh()
		{
			for (int i = 0; i < this.vertices_refcount.max_index; i++)
			{
				if (!this.vertices_refcount.isValid(i))
				{
					Console.WriteLine(string.Format("v{0} : invalid", i));
				}
				else
				{
					this.debug_print_vertex(i);
				}
			}
		}

		public string MeshInfoString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Vertices  count {0} max {1} {2}", this.VertexCount, this.MaxVertexID, this.vertices_refcount.UsageStats);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("Triangles count {0} max {1} {2}", this.TriangleCount, this.MaxTriangleID, this.triangles_refcount.UsageStats);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("Edges     count {0} max {1} {2}", this.EdgeCount, this.MaxEdgeID, this.edges_refcount.UsageStats);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("Normals {0}  Colors {1}  UVs {2}  Groups {3}", new object[]
			{
				this.HasVertexNormals,
				this.HasVertexColors,
				this.HasVertexUVs,
				this.HasTriangleGroups
			});
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("Closed {0} Compact {1} timestamp {2} shape_timestamp {3}  MaxGroupID {4}", new object[]
			{
				this.CachedIsClosed,
				this.IsCompact,
				this.timestamp,
				this.shape_timestamp,
				this.max_group_id
			});
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("VertexEdges " + this.vertex_edges.MemoryUsage, Array.Empty<object>());
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		public bool IsSameMesh(DMesh3 m2, bool bCheckConnectivity, bool bCheckEdgeIDs = false, bool bCheckNormals = false, bool bCheckColors = false, bool bCheckUVs = false, bool bCheckGroups = false, float Epsilon = 1.1920929E-07f)
		{
			if (this.VertexCount != m2.VertexCount)
			{
				return false;
			}
			if (this.TriangleCount != m2.TriangleCount)
			{
				return false;
			}
			foreach (int vID in this.VertexIndices())
			{
				if (!m2.IsVertex(vID) || !this.GetVertex(vID).EpsilonEqual(m2.GetVertex(vID), (double)Epsilon))
				{
					return false;
				}
			}
			foreach (int tID in this.TriangleIndices())
			{
				if (!m2.IsTriangle(tID) || !this.GetTriangle(tID).Equals(m2.GetTriangle(tID)))
				{
					return false;
				}
			}
			if (bCheckConnectivity)
			{
				foreach (int eID in this.EdgeIndices())
				{
					Index4i edge = this.GetEdge(eID);
					int num = m2.FindEdge(edge.a, edge.b);
					if (num == -1)
					{
						return false;
					}
					Index4i edge2 = m2.GetEdge(num);
					if (Math.Min(edge.c, edge.d) != Math.Min(edge2.c, edge2.d) || Math.Max(edge.c, edge.d) != Math.Max(edge2.c, edge2.d))
					{
						return false;
					}
				}
			}
			if (bCheckEdgeIDs)
			{
				if (this.EdgeCount != m2.EdgeCount)
				{
					return false;
				}
				foreach (int eID2 in this.EdgeIndices())
				{
					if (!m2.IsEdge(eID2) || !this.GetEdge(eID2).Equals(m2.GetEdge(eID2)))
					{
						return false;
					}
				}
			}
			if (bCheckNormals)
			{
				if (this.HasVertexNormals != m2.HasVertexNormals)
				{
					return false;
				}
				if (this.HasVertexNormals)
				{
					foreach (int vID2 in this.VertexIndices())
					{
						if (!this.GetVertexNormal(vID2).EpsilonEqual(m2.GetVertexNormal(vID2), Epsilon))
						{
							return false;
						}
					}
				}
			}
			if (bCheckColors)
			{
				if (this.HasVertexColors != m2.HasVertexColors)
				{
					return false;
				}
				if (this.HasVertexColors)
				{
					foreach (int vID3 in this.VertexIndices())
					{
						if (!this.GetVertexColor(vID3).EpsilonEqual(m2.GetVertexColor(vID3), Epsilon))
						{
							return false;
						}
					}
				}
			}
			if (bCheckUVs)
			{
				if (this.HasVertexUVs != m2.HasVertexUVs)
				{
					return false;
				}
				if (this.HasVertexUVs)
				{
					foreach (int vID4 in this.VertexIndices())
					{
						if (!this.GetVertexUV(vID4).EpsilonEqual(m2.GetVertexUV(vID4), Epsilon))
						{
							return false;
						}
					}
				}
			}
			if (bCheckGroups)
			{
				if (this.HasTriangleGroups != m2.HasTriangleGroups)
				{
					return false;
				}
				if (this.HasTriangleGroups)
				{
					foreach (int tID2 in this.TriangleIndices())
					{
						if (this.GetTriangleGroup(tID2) != m2.GetTriangleGroup(tID2))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public bool CheckValidity(bool bAllowNonManifoldVertices = false, FailMode eFailMode = FailMode.Throw)
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
			if (this.uv != null)
			{
				action(this.uv.size / 2 == this.vertices.size / 3);
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
					int eID = triEdges[k];
					action(this.IsEdge(eID));
					int num2 = this.edge_other_t(eID, num);
					if (num2 == -1)
					{
						action(this.tri_is_boundary(num));
					}
					else
					{
						action(this.tri_has_neighbour_t(num2, num));
						int num3 = triangle[k];
						int num4 = triangle[(k + 1) % 3];
						Index2i edgeV = this.GetEdgeV(triEdges[k]);
						action(IndexUtil.same_pair_unordered(num3, num4, edgeV[0], edgeV[1]));
						int num5 = IndexUtil.find_tri_ordered_edge(num4, num3, this.GetTriangle(num2).array);
						action(num5 != -1);
					}
				}
			}
			foreach (int num6 in this.EdgeIndices())
			{
				action(this.IsEdge(num6));
				action(this.edges_refcount.refCount(num6) == 1);
				Index2i edgeV2 = this.GetEdgeV(num6);
				Index2i edgeT = this.GetEdgeT(num6);
				action(this.IsVertex(edgeV2[0]));
				action(this.IsVertex(edgeV2[1]));
				action(edgeT[0] != -1);
				action(edgeV2[0] < edgeV2[1]);
				action(this.IsTriangle(edgeT[0]));
				if (edgeT[1] != -1)
				{
					action(this.IsTriangle(edgeT[1]));
				}
			}
			if (this.vertices_refcount.is_dense)
			{
				for (int l = 0; l < this.vertices.Length / 3; l++)
				{
					action(this.vertices_refcount.isValid(l));
				}
			}
			foreach (int num7 in this.VertexIndices())
			{
				action(this.IsVertex(num7));
				Vector3d vertex = this.GetVertex(num7);
				action(!double.IsNaN(vertex.LengthSquared));
				action(!double.IsInfinity(vertex.LengthSquared));
				foreach (int num8 in this.vertex_edges.ValueItr(num7))
				{
					action(this.IsEdge(num8));
					action(this.edge_has_v(num8, num7));
					int num9 = this.edge_other_v(num8, num7);
					int num10 = this.find_edge(num7, num9);
					action(num10 != -1);
					action(num10 == num8);
					num10 = this.find_edge(num9, num7);
					action(num10 != -1);
					action(num10 == num8);
				}
				foreach (int num11 in this.VtxVerticesItr(num7))
				{
					action(this.IsVertex(num11));
					int eID2 = this.find_edge(num7, num11);
					action(this.IsEdge(eID2));
				}
				List<int> list = new List<int>();
				List<int> list2 = new List<int>();
				this.GetVtxTriangles(num7, list, false);
				this.GetVtxTriangles(num7, list2, true);
				action(list.Count == list2.Count);
				if (bAllowNonManifoldVertices)
				{
					action(list.Count <= this.GetVtxEdgeCount(num7));
				}
				else
				{
					action(list.Count == this.GetVtxEdgeCount(num7) || list.Count == this.GetVtxEdgeCount(num7) - 1);
				}
				action(this.vertices_refcount.refCount(num7) == list.Count + 1);
				action(array[num7] == list.Count);
				foreach (int tID in list)
				{
					action(this.tri_has_v(tID, num7));
				}
				List<int> list3 = new List<int>(list);
				foreach (int eID3 in this.vertex_edges.ValueItr(num7))
				{
					Index2i edgeT2 = this.GetEdgeT(eID3);
					action(list.Contains(edgeT2[0]));
					if (edgeT2[1] != -1)
					{
						action(list.Contains(edgeT2[1]));
					}
					list3.Remove(edgeT2[0]);
					if (edgeT2[1] != -1)
					{
						list3.Remove(edgeT2[1]);
					}
				}
				action(list3.Count == 0);
			}
			return is_ok;
		}

		public MeshResult ReverseTriOrientation(int tID)
		{
			if (!this.IsTriangle(tID))
			{
				return MeshResult.Failed_NotATriangle;
			}
			this.internal_reverse_tri_orientation(tID);
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		private void internal_reverse_tri_orientation(int tID)
		{
			Index3i triangle = this.GetTriangle(tID);
			this.set_triangle(tID, triangle[1], triangle[0], triangle[2]);
			Index3i triEdges = this.GetTriEdges(tID);
			this.set_triangle_edges(tID, triEdges[0], triEdges[2], triEdges[1]);
		}

		public void ReverseOrientation(bool bFlipNormals = true)
		{
			foreach (int tID in this.TriangleIndices())
			{
				this.internal_reverse_tri_orientation(tID);
			}
			if (bFlipNormals && this.HasVertexNormals)
			{
				foreach (int num in this.VertexIndices())
				{
					int num2 = 3 * num;
					this.normals[num2] = -this.normals[num2];
					this.normals[num2 + 1] = -this.normals[num2 + 1];
					this.normals[num2 + 2] = -this.normals[num2 + 2];
				}
			}
			this.updateTimeStamp(true);
			this.Clockwise = !this.Clockwise;
		}

		public MeshResult RemoveVertex(int vID, bool bRemoveAllTriangles = true, bool bPreserveManifold = false)
		{
			if (!this.vertices_refcount.isValid(vID))
			{
				return MeshResult.Failed_NotAVertex;
			}
			if (bRemoveAllTriangles)
			{
				if (bPreserveManifold)
				{
					foreach (int tID in this.VtxTrianglesItr(vID))
					{
						Index3i triangle = this.GetTriangle(tID);
						int num = IndexUtil.find_tri_index(vID, ref triangle);
						int num2 = triangle[(num + 1) % 3];
						int num3 = triangle[(num + 2) % 3];
						int eid = this.find_edge(num2, num3);
						if (!this.IsBoundaryEdge(eid) && (this.IsBoundaryVertex(num2) || this.IsBoundaryVertex(num3)))
						{
							return MeshResult.Failed_WouldCreateBowtie;
						}
					}
				}
				List<int> list = new List<int>();
				this.GetVtxTriangles(vID, list, true);
				foreach (int tID2 in list)
				{
					MeshResult meshResult = this.RemoveTriangle(tID2, false, bPreserveManifold);
					if (meshResult != MeshResult.Ok)
					{
						return meshResult;
					}
				}
			}
			if (this.vertices_refcount.refCount(vID) != 1)
			{
				throw new NotImplementedException("DMesh3.RemoveVertex: vertex is still referenced");
			}
			this.vertices_refcount.decrement(vID, 1);
			this.vertex_edges.Clear(vID);
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public MeshResult RemoveTriangle(int tID, bool bRemoveIsolatedVertices = true, bool bPreserveManifold = false)
		{
			if (!this.triangles_refcount.isValid(tID))
			{
				return MeshResult.Failed_NotATriangle;
			}
			Index3i triangle = this.GetTriangle(tID);
			Index3i triEdges = this.GetTriEdges(tID);
			if (bPreserveManifold)
			{
				for (int i = 0; i < 3; i++)
				{
					if (this.IsBoundaryVertex(triangle[i]) && !this.IsBoundaryEdge(triEdges[i]) && !this.IsBoundaryEdge(triEdges[(i + 2) % 3]))
					{
						return MeshResult.Failed_WouldCreateBowtie;
					}
				}
			}
			for (int j = 0; j < 3; j++)
			{
				int num = triEdges[j];
				this.replace_edge_triangle(num, tID, -1);
				if (this.edges[4 * num + 2] == -1)
				{
					int list_index = this.edges[4 * num];
					this.vertex_edges.Remove(list_index, num);
					int list_index2 = this.edges[4 * num + 1];
					this.vertex_edges.Remove(list_index2, num);
					this.edges_refcount.decrement(num, 1);
				}
			}
			this.triangles_refcount.decrement(tID, 1);
			for (int k = 0; k < 3; k++)
			{
				int num2 = triangle[k];
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

		public virtual MeshResult SetTriangle(int tID, Index3i newv, bool bRemoveIsolatedVertices = true)
		{
			Index3i triangle = this.GetTriangle(tID);
			Index3i triEdges = this.GetTriEdges(tID);
			if (triangle.a == newv.a && triangle.b == newv.b)
			{
				triEdges.a = -1;
			}
			if (triangle.b == newv.b && triangle.c == newv.c)
			{
				triEdges.b = -1;
			}
			if (triangle.c == newv.c && triangle.a == newv.a)
			{
				triEdges.c = -1;
			}
			if (!this.triangles_refcount.isValid(tID))
			{
				return MeshResult.Failed_NotATriangle;
			}
			if (!this.IsVertex(newv[0]) || !this.IsVertex(newv[1]) || !this.IsVertex(newv[2]))
			{
				return MeshResult.Failed_NotAVertex;
			}
			if (newv[0] == newv[1] || newv[0] == newv[2] || newv[1] == newv[2])
			{
				return MeshResult.Failed_BrokenTopology;
			}
			int num = this.find_edge(newv[0], newv[1]);
			int num2 = this.find_edge(newv[1], newv[2]);
			int num3 = this.find_edge(newv[2], newv[0]);
			if ((triEdges.a != -1 && num != -1 && !this.IsBoundaryEdge(num)) || (triEdges.b != -1 && num2 != -1 && !this.IsBoundaryEdge(num2)) || (triEdges.c != -1 && num3 != -1 && !this.IsBoundaryEdge(num3)))
			{
				return MeshResult.Failed_BrokenTopology;
			}
			for (int i = 0; i < 3; i++)
			{
				int num4 = triEdges[i];
				if (num4 != -1)
				{
					this.replace_edge_triangle(num4, tID, -1);
					if (this.edges[4 * num4 + 2] == -1)
					{
						int list_index = this.edges[4 * num4];
						this.vertex_edges.Remove(list_index, num4);
						int list_index2 = this.edges[4 * num4 + 1];
						this.vertex_edges.Remove(list_index2, num4);
						this.edges_refcount.decrement(num4, 1);
					}
				}
			}
			for (int j = 0; j < 3; j++)
			{
				int num5 = triangle[j];
				if (num5 != newv[j])
				{
					this.vertices_refcount.decrement(num5, 1);
					if (bRemoveIsolatedVertices && this.vertices_refcount.refCount(num5) == 1)
					{
						this.vertices_refcount.decrement(num5, 1);
						this.vertex_edges.Clear(num5);
					}
				}
			}
			int num6 = 3 * tID;
			for (int k = 0; k < 3; k++)
			{
				if (newv[k] != triangle[k])
				{
					this.triangles[num6 + k] = newv[k];
					this.vertices_refcount.increment(newv[k], 1);
				}
			}
			if (triEdges.a != -1)
			{
				this.add_tri_edge(tID, newv[0], newv[1], 0, num);
			}
			if (triEdges.b != -1)
			{
				this.add_tri_edge(tID, newv[1], newv[2], 1, num2);
			}
			if (triEdges.c != -1)
			{
				this.add_tri_edge(tID, newv[2], newv[0], 2, num3);
			}
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public MeshResult SplitEdge(int vA, int vB, out DMesh3.EdgeSplitInfo split)
		{
			int num = this.find_edge(vA, vB);
			if (num == -1)
			{
				split = default(DMesh3.EdgeSplitInfo);
				return MeshResult.Failed_NotAnEdge;
			}
			return this.SplitEdge(num, out split, 0.5);
		}

		public MeshResult SplitEdge(int eab, out DMesh3.EdgeSplitInfo split, double split_t = 0.5)
		{
			split = default(DMesh3.EdgeSplitInfo);
			if (!this.IsEdge(eab))
			{
				return MeshResult.Failed_NotAnEdge;
			}
			int num = 4 * eab;
			int num2 = this.edges[num];
			int num3 = this.edges[num + 1];
			int num4 = this.edges[num + 2];
			if (num4 == -1)
			{
				return MeshResult.Failed_BrokenTopology;
			}
			int[] array = this.GetTriangle(num4).array;
			int num5 = IndexUtil.orient_tri_edge_and_find_other_vtx(ref num2, ref num3, array);
			if (this.vertices_refcount.rawRefCount(num5) > 32764)
			{
				return MeshResult.Failed_HitValenceLimit;
			}
			if (num2 != this.edges[num])
			{
				split_t = 1.0 - split_t;
			}
			if (this.IsBoundaryEdge(eab))
			{
				Vector3d v = Vector3d.Lerp(this.GetVertex(num2), this.GetVertex(num3), split_t);
				int num6 = this.AppendVertex(v);
				if (this.HasVertexNormals)
				{
					this.SetVertexNormal(num6, Vector3f.Lerp(this.GetVertexNormal(num2), this.GetVertexNormal(num3), (float)split_t).Normalized);
				}
				if (this.HasVertexColors)
				{
					this.SetVertexColor(num6, Colorf.Lerp(this.GetVertexColor(num2), this.GetVertexColor(num3), (float)split_t));
				}
				if (this.HasVertexUVs)
				{
					this.SetVertexUV(num6, Vector2f.Lerp(this.GetVertexUV(num2), this.GetVertexUV(num3), (float)split_t));
				}
				int num7 = this.GetTriEdges(num4)[IndexUtil.find_edge_index_in_tri(num3, num5, array)];
				this.replace_tri_vertex(num4, num3, num6);
				int num8 = this.add_triangle_only(num6, num3, num5, -1, -1, -1);
				if (this.triangle_groups != null)
				{
					this.triangle_groups.insert(this.triangle_groups[num4], num8);
				}
				this.replace_edge_triangle(num7, num4, num8);
				this.replace_edge_vertex(eab, num3, num6);
				this.vertex_edges.Remove(num3, eab);
				this.vertex_edges.Insert(num6, eab);
				int num9 = this.add_edge(num6, num3, num8, -1);
				int num10 = this.add_edge(num6, num5, num4, num8);
				this.replace_triangle_edge(num4, num7, num10);
				this.set_triangle_edges(num8, num9, num7, num10);
				this.vertices_refcount.increment(num5, 1);
				this.vertices_refcount.increment(num6, 2);
				split.bIsBoundary = true;
				split.vNew = num6;
				split.eNewBN = num9;
				split.eNewCN = num10;
				split.eNewDN = -1;
				split.eNewT2 = num8;
				split.eNewT3 = -1;
				this.updateTimeStamp(true);
				return MeshResult.Ok;
			}
			int num11 = this.edges[num + 3];
			int[] array2 = this.GetTriangle(num11).array;
			int num12 = IndexUtil.find_tri_other_vtx(num2, num3, array2);
			if (this.vertices_refcount.rawRefCount(num12) > 32764)
			{
				return MeshResult.Failed_HitValenceLimit;
			}
			Vector3d v2 = Vector3d.Lerp(this.GetVertex(num2), this.GetVertex(num3), split_t);
			int num13 = this.AppendVertex(v2);
			if (this.HasVertexNormals)
			{
				this.SetVertexNormal(num13, Vector3f.Lerp(this.GetVertexNormal(num2), this.GetVertexNormal(num3), (float)split_t).Normalized);
			}
			if (this.HasVertexColors)
			{
				this.SetVertexColor(num13, Colorf.Lerp(this.GetVertexColor(num2), this.GetVertexColor(num3), (float)split_t));
			}
			if (this.HasVertexUVs)
			{
				this.SetVertexUV(num13, Vector2f.Lerp(this.GetVertexUV(num2), this.GetVertexUV(num3), (float)split_t));
			}
			int num14 = this.GetTriEdges(num4)[IndexUtil.find_edge_index_in_tri(num3, num5, array)];
			int num15 = this.GetTriEdges(num11)[IndexUtil.find_edge_index_in_tri(num12, num3, array2)];
			this.replace_tri_vertex(num4, num3, num13);
			this.replace_tri_vertex(num11, num3, num13);
			int num16 = this.add_triangle_only(num13, num3, num5, -1, -1, -1);
			int num17 = this.add_triangle_only(num13, num12, num3, -1, -1, -1);
			if (this.triangle_groups != null)
			{
				this.triangle_groups.insert(this.triangle_groups[num4], num16);
				this.triangle_groups.insert(this.triangle_groups[num11], num17);
			}
			this.replace_edge_triangle(num14, num4, num16);
			this.replace_edge_triangle(num15, num11, num17);
			this.replace_edge_vertex(eab, num3, num13);
			this.vertex_edges.Remove(num3, eab);
			this.vertex_edges.Insert(num13, eab);
			int num18 = this.add_edge(num13, num3, num16, num17);
			int num19 = this.add_edge(num13, num5, num4, num16);
			int num20 = this.add_edge(num12, num13, num11, num17);
			this.replace_triangle_edge(num4, num14, num19);
			this.replace_triangle_edge(num11, num15, num20);
			this.set_triangle_edges(num16, num18, num14, num19);
			this.set_triangle_edges(num17, num20, num15, num18);
			this.vertices_refcount.increment(num5, 1);
			this.vertices_refcount.increment(num12, 1);
			this.vertices_refcount.increment(num13, 4);
			split.bIsBoundary = false;
			split.vNew = num13;
			split.eNewBN = num18;
			split.eNewCN = num19;
			split.eNewDN = num20;
			split.eNewT2 = num16;
			split.eNewT3 = num17;
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public MeshResult FlipEdge(int vA, int vB, out DMesh3.EdgeFlipInfo flip)
		{
			int num = this.find_edge(vA, vB);
			if (num == -1)
			{
				flip = default(DMesh3.EdgeFlipInfo);
				return MeshResult.Failed_NotAnEdge;
			}
			return this.FlipEdge(num, out flip);
		}

		public MeshResult FlipEdge(int eab, out DMesh3.EdgeFlipInfo flip)
		{
			flip = default(DMesh3.EdgeFlipInfo);
			if (!this.IsEdge(eab))
			{
				return MeshResult.Failed_NotAnEdge;
			}
			if (this.IsBoundaryEdge(eab))
			{
				return MeshResult.Failed_IsBoundaryEdge;
			}
			int num = 4 * eab;
			int num2 = this.edges[num];
			int num3 = this.edges[num + 1];
			int num4 = this.edges[num + 2];
			int num5 = this.edges[num + 3];
			int[] array = this.GetTriangle(num4).array;
			int[] array2 = this.GetTriangle(num5).array;
			int num6 = IndexUtil.orient_tri_edge_and_find_other_vtx(ref num2, ref num3, array);
			int num7 = IndexUtil.find_tri_other_vtx(num2, num3, array2);
			if (num6 == -1 || num7 == -1)
			{
				return MeshResult.Failed_BrokenTopology;
			}
			if (this.find_edge(num6, num7) != -1)
			{
				return MeshResult.Failed_FlippedEdgeExists;
			}
			int e = this.find_tri_neighbour_edge(num4, num3, num6);
			int num8 = this.find_tri_neighbour_edge(num4, num6, num2);
			int e2 = this.find_tri_neighbour_edge(num5, num2, num7);
			int num9 = this.find_tri_neighbour_edge(num5, num7, num3);
			this.set_triangle(num4, num6, num7, num3);
			this.set_triangle(num5, num7, num6, num2);
			this.set_edge_vertices(eab, num6, num7);
			this.set_edge_triangles(eab, num4, num5);
			if (this.replace_edge_triangle(num8, num4, num5) == -1)
			{
				throw new ArgumentException("DMesh3.FlipEdge: first replace_edge_triangle failed");
			}
			if (this.replace_edge_triangle(num9, num5, num4) == -1)
			{
				throw new ArgumentException("DMesh3.FlipEdge: second replace_edge_triangle failed");
			}
			this.set_triangle_edges(num4, eab, num9, e);
			this.set_triangle_edges(num5, eab, num8, e2);
			if (!this.vertex_edges.Remove(num2, eab))
			{
				throw new ArgumentException("DMesh3.FlipEdge: first edge list remove failed");
			}
			if (!this.vertex_edges.Remove(num3, eab))
			{
				throw new ArgumentException("DMesh3.FlipEdge: second edge list remove failed");
			}
			this.vertices_refcount.decrement(num2, 1);
			this.vertices_refcount.decrement(num3, 1);
			if (!this.IsVertex(num2) || !this.IsVertex(num3))
			{
				throw new ArgumentException("DMesh3.FlipEdge: either a or b is not a vertex?");
			}
			this.vertex_edges.Insert(num6, eab);
			this.vertex_edges.Insert(num7, eab);
			this.vertices_refcount.increment(num6, 1);
			this.vertices_refcount.increment(num7, 1);
			flip.eID = eab;
			flip.v0 = num2;
			flip.v1 = num3;
			flip.ov0 = num6;
			flip.ov1 = num7;
			flip.t0 = num4;
			flip.t1 = num5;
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		private void debug_fail(string s)
		{
		}

		private void check_tri(int t)
		{
			Index3i triangle = this.GetTriangle(t);
			if (triangle[0] != triangle[1] && triangle[0] != triangle[2])
			{
				int num = triangle[1];
				int num2 = triangle[2];
			}
		}

		private void check_edge(int e)
		{
			int num = this.GetEdgeT(e)[0];
		}

		public MeshResult CollapseEdge(int vKeep, int vRemove, out DMesh3.EdgeCollapseInfo collapse)
		{
			collapse = default(DMesh3.EdgeCollapseInfo);
			if (!this.IsVertex(vKeep) || !this.IsVertex(vRemove))
			{
				return MeshResult.Failed_NotAnEdge;
			}
			int num = this.find_edge(vRemove, vKeep);
			if (num == -1)
			{
				return MeshResult.Failed_NotAnEdge;
			}
			int num2 = this.edges[4 * num + 2];
			if (num2 == -1)
			{
				return MeshResult.Failed_BrokenTopology;
			}
			Index3i triangle = this.GetTriangle(num2);
			int num3 = IndexUtil.find_tri_other_vtx(vRemove, vKeep, triangle);
			bool flag = false;
			int num4 = -1;
			int num5 = this.edges[4 * num + 3];
			if (num5 != -1)
			{
				Index3i triangle2 = this.GetTriangle(num5);
				num4 = IndexUtil.find_tri_other_vtx(vRemove, vKeep, triangle2);
				if (num3 == num4)
				{
					return MeshResult.Failed_FoundDuplicateTriangle;
				}
			}
			else
			{
				flag = true;
			}
			int num6 = this.vertex_edges.Count(vRemove);
			int num7 = -1;
			int num8 = -1;
			int num9 = -1;
			int num10 = -1;
			foreach (int num11 in this.vertex_edges.ValueItr(vRemove))
			{
				int num12 = this.edge_other_v(num11, vRemove);
				if (num12 == num3)
				{
					num7 = num11;
				}
				else if (num12 == num4)
				{
					num8 = num11;
				}
				else if (num12 != vKeep)
				{
					foreach (int eID in this.vertex_edges.ValueItr(vKeep))
					{
						if (this.edge_other_v(eID, vKeep) == num12)
						{
							return MeshResult.Failed_InvalidNeighbourhood;
						}
					}
				}
			}
			if (num6 == 3 && !flag)
			{
				int num13 = this.find_edge(num4, num3);
				int num14 = 4 * num13;
				if (num13 != -1 && this.edges[num14 + 3] != -1)
				{
					int tID = this.edges[num14 + 2];
					int tID2 = this.edges[num14 + 3];
					if ((this.tri_has_v(tID, vRemove) && this.tri_has_v(tID2, vKeep)) || (this.tri_has_v(tID, vKeep) && this.tri_has_v(tID2, vRemove)))
					{
						return MeshResult.Failed_CollapseTetrahedron;
					}
				}
			}
			else if (flag && this.IsBoundaryEdge(num7))
			{
				num9 = this.find_edge_from_tri(vKeep, num3, num2);
				if (this.IsBoundaryEdge(num9))
				{
					return MeshResult.Failed_CollapseTriangle;
				}
			}
			if (!flag && this.IsBoundaryVertex(vRemove) && this.IsBoundaryVertex(vKeep))
			{
				return MeshResult.Failed_InvalidNeighbourhood;
			}
			int num15 = -1;
			int num16 = -1;
			foreach (int num17 in this.vertex_edges.ValueItr(vRemove))
			{
				int num18 = this.edge_other_v(num17, vRemove);
				if (num18 == vKeep)
				{
					if (!this.vertex_edges.Remove(vKeep, num17))
					{
						this.debug_fail("remove case o == b");
					}
				}
				else if (num18 == num3)
				{
					if (!this.vertex_edges.Remove(num3, num17))
					{
						this.debug_fail("remove case o == c");
					}
					num16 = this.edge_other_t(num17, num2);
				}
				else if (num18 == num4)
				{
					if (!this.vertex_edges.Remove(num4, num17))
					{
						this.debug_fail("remove case o == c, step 1");
					}
					num15 = this.edge_other_t(num17, num5);
				}
				else
				{
					if (this.replace_edge_vertex(num17, vRemove, vKeep) == -1)
					{
						this.debug_fail("remove case else");
					}
					this.vertex_edges.Insert(vKeep, num17);
				}
				for (int i = 0; i < 2; i++)
				{
					int num19 = this.edges[4 * num17 + 2 + i];
					if (num19 != -1 && num19 != num2 && num19 != num5 && this.tri_has_v(num19, vRemove))
					{
						if (this.replace_tri_vertex(num19, vRemove, vKeep) == -1)
						{
							this.debug_fail("remove last check");
						}
						this.vertices_refcount.increment(vKeep, 1);
						this.vertices_refcount.decrement(vRemove, 1);
					}
				}
			}
			if (!flag)
			{
				this.vertex_edges.Clear(vRemove);
				this.vertices_refcount.decrement(vRemove, 3);
				this.triangles_refcount.decrement(num2, 1);
				this.triangles_refcount.decrement(num5, 1);
				this.vertices_refcount.decrement(num3, 1);
				this.vertices_refcount.decrement(num4, 1);
				this.vertices_refcount.decrement(vKeep, 2);
				this.edges_refcount.decrement(num8, 1);
				this.edges_refcount.decrement(num, 1);
				this.edges_refcount.decrement(num7, 1);
				num10 = this.find_edge_from_tri(vKeep, num4, num5);
				if (num9 == -1)
				{
					num9 = this.find_edge_from_tri(vKeep, num3, num2);
				}
				if (this.replace_edge_triangle(num10, num5, num15) == -1)
				{
					this.debug_fail("isboundary=false branch, ebd replace triangle");
				}
				if (this.replace_edge_triangle(num9, num2, num16) == -1)
				{
					this.debug_fail("isboundary=false branch, ebc replace triangle");
				}
				if (num15 != -1 && this.replace_triangle_edge(num15, num8, num10) == -1)
				{
					this.debug_fail("isboundary=false branch, ebd replace triangle");
				}
				if (num16 != -1 && this.replace_triangle_edge(num16, num7, num9) == -1)
				{
					this.debug_fail("isboundary=false branch, ebd replace triangle");
				}
			}
			else
			{
				this.vertex_edges.Clear(vRemove);
				this.vertices_refcount.decrement(vRemove, 2);
				this.triangles_refcount.decrement(num2, 1);
				this.vertices_refcount.decrement(num3, 1);
				this.vertices_refcount.decrement(vKeep, 1);
				this.edges_refcount.decrement(num, 1);
				this.edges_refcount.decrement(num7, 1);
				num9 = this.find_edge_from_tri(vKeep, num3, num2);
				if (this.replace_edge_triangle(num9, num2, num16) == -1)
				{
					this.debug_fail("isboundary=false branch, ebc replace triangle");
				}
				if (num16 != -1 && this.replace_triangle_edge(num16, num7, num9) == -1)
				{
					this.debug_fail("isboundary=true branch, ebd replace triangle");
				}
			}
			collapse.vKept = vKeep;
			collapse.vRemoved = vRemove;
			collapse.bIsBoundary = flag;
			collapse.eCollapsed = num;
			collapse.tRemoved0 = num2;
			collapse.tRemoved1 = num5;
			collapse.eRemoved0 = num7;
			collapse.eRemoved1 = num8;
			collapse.eKept0 = num9;
			collapse.eKept1 = num10;
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public MeshResult MergeEdges(int eKeep, int eDiscard, out DMesh3.MergeEdgesInfo merge_info)
		{
			merge_info = default(DMesh3.MergeEdgesInfo);
			if (!this.IsEdge(eKeep) || !this.IsEdge(eDiscard))
			{
				return MeshResult.Failed_NotAnEdge;
			}
			Index4i edge = this.GetEdge(eKeep);
			Index4i edge2 = this.GetEdge(eDiscard);
			if (edge.d != -1 || edge2.d != -1)
			{
				return MeshResult.Failed_NotABoundaryEdge;
			}
			int a = edge.a;
			int b = edge.b;
			int c = edge.c;
			int num = edge2.a;
			int num2 = edge2.b;
			int c2 = edge2.c;
			IndexUtil.orient_tri_edge(ref a, ref b, this.GetTriangle(c));
			IndexUtil.orient_tri_edge(ref num, ref num2, this.GetTriangle(c2));
			int num3 = num;
			num = num2;
			num2 = num3;
			Vector3d vertex = this.GetVertex(a);
			Vector3d vertex2 = this.GetVertex(b);
			Vector3d vertex3 = this.GetVertex(num);
			Vector3d vertex4 = this.GetVertex(num2);
			if (vertex.DistanceSquared(vertex3) + vertex2.DistanceSquared(vertex4) > vertex.DistanceSquared(vertex4) + vertex2.DistanceSquared(vertex3))
			{
				return MeshResult.Failed_SameOrientation;
			}
			merge_info.eKept = eKeep;
			merge_info.eRemoved = eDiscard;
			if (a != num && this.find_edge(a, num) != -1)
			{
				return MeshResult.Failed_InvalidNeighbourhood;
			}
			if (b != num2 && this.find_edge(b, num2) != -1)
			{
				return MeshResult.Failed_InvalidNeighbourhood;
			}
			if (a != num)
			{
				int num4 = (b == num2) ? b : -1;
				foreach (int num5 in this.VtxVerticesItr(num))
				{
					int eid;
					if (num5 != num4 && (eid = this.find_edge(a, num5)) != -1)
					{
						int eid2 = this.find_edge(num, num5);
						if (!this.IsBoundaryEdge(eid) || !this.IsBoundaryEdge(eid2))
						{
							return MeshResult.Failed_InvalidNeighbourhood;
						}
					}
				}
			}
			if (b != num2)
			{
				int num6 = (a == num) ? a : -1;
				foreach (int num7 in this.VtxVerticesItr(num2))
				{
					int eid3;
					if (num7 != num6 && (eid3 = this.find_edge(b, num7)) != -1)
					{
						int eid4 = this.find_edge(num2, num7);
						if (!this.IsBoundaryEdge(eid3) || !this.IsBoundaryEdge(eid4))
						{
							return MeshResult.Failed_InvalidNeighbourhood;
						}
					}
				}
			}
			if (a != num)
			{
				foreach (int num8 in this.vertex_edges.ValueItr(num))
				{
					if (num8 != eDiscard)
					{
						this.replace_edge_vertex(num8, num, a);
						short num9 = 0;
						if (this.replace_tri_vertex(this.edges[4 * num8 + 2], num, a) >= 0)
						{
							num9 += 1;
						}
						if (this.edges[4 * num8 + 3] != -1 && this.replace_tri_vertex(this.edges[4 * num8 + 3], num, a) >= 0)
						{
							num9 += 1;
						}
						this.vertex_edges.Insert(a, num8);
						if (num9 > 0)
						{
							this.vertices_refcount.increment(a, num9);
							this.vertices_refcount.decrement(num, num9);
						}
					}
				}
				this.vertex_edges.Clear(num);
				this.vertices_refcount.decrement(num, 1);
				merge_info.vRemoved[0] = num;
			}
			else
			{
				this.vertex_edges.Remove(a, eDiscard);
				merge_info.vRemoved[0] = -1;
			}
			merge_info.vKept[0] = a;
			if (num2 != b)
			{
				foreach (int num10 in this.vertex_edges.ValueItr(num2))
				{
					if (num10 != eDiscard)
					{
						this.replace_edge_vertex(num10, num2, b);
						short num11 = 0;
						if (this.replace_tri_vertex(this.edges[4 * num10 + 2], num2, b) >= 0)
						{
							num11 += 1;
						}
						if (this.edges[4 * num10 + 3] != -1 && this.replace_tri_vertex(this.edges[4 * num10 + 3], num2, b) >= 0)
						{
							num11 += 1;
						}
						this.vertex_edges.Insert(b, num10);
						if (num11 > 0)
						{
							this.vertices_refcount.increment(b, num11);
							this.vertices_refcount.decrement(num2, num11);
						}
					}
				}
				this.vertex_edges.Clear(num2);
				this.vertices_refcount.decrement(num2, 1);
				merge_info.vRemoved[1] = num2;
			}
			else
			{
				this.vertex_edges.Remove(b, eDiscard);
				merge_info.vRemoved[1] = -1;
			}
			merge_info.vKept[1] = b;
			this.replace_triangle_edge(c2, eDiscard, eKeep);
			this.edges_refcount.decrement(eDiscard, 1);
			this.set_edge_triangles(eKeep, c, c2);
			merge_info.eRemovedExtra = new Vector2i(-1, -1);
			merge_info.eKeptExtra = merge_info.eRemovedExtra;
			for (int i = 0; i < 2; i++)
			{
				int num12 = a;
				int num13 = num;
				if (i == 1)
				{
					num12 = b;
					num13 = num2;
				}
				if (num12 != num13)
				{
					List<int> list = this.vertex_edges_list(num12);
					int count = list.Count;
					bool flag = false;
					int num14 = 0;
					while (num14 < count && !flag)
					{
						int num15 = list[num14];
						if (this.IsBoundaryEdge(num15))
						{
							int num16 = this.edge_other_v(num15, num12);
							for (int j = num14 + 1; j < count; j++)
							{
								int num17 = list[j];
								int num18 = this.edge_other_v(num17, num12);
								if (num16 == num18 && this.IsBoundaryEdge(num17))
								{
									int t = this.edges[4 * num15 + 2];
									int num19 = this.edges[4 * num17 + 2];
									this.replace_triangle_edge(num19, num17, num15);
									this.set_edge_triangles(num15, t, num19);
									this.vertex_edges.Remove(num12, num17);
									this.vertex_edges.Remove(num16, num17);
									this.edges_refcount.decrement(num17, 1);
									merge_info.eRemovedExtra[i] = num17;
									merge_info.eKeptExtra[i] = num15;
									flag = true;
									break;
								}
							}
						}
						num14++;
					}
				}
			}
			this.updateTimeStamp(true);
			return MeshResult.Ok;
		}

		public virtual MeshResult PokeTriangle(int tid, out DMesh3.PokeTriangleInfo result)
		{
			return this.PokeTriangle(tid, Vector3d.One / 3.0, out result);
		}

		public virtual MeshResult PokeTriangle(int tid, Vector3d baryCoordinates, out DMesh3.PokeTriangleInfo result)
		{
			result = default(DMesh3.PokeTriangleInfo);
			if (!this.IsTriangle(tid))
			{
				return MeshResult.Failed_NotATriangle;
			}
			Index3i triangle = this.GetTriangle(tid);
			Index3i triEdges = this.GetTriEdges(tid);
			NewVertexInfo info;
			this.GetTriBaryPoint(tid, baryCoordinates.x, baryCoordinates.y, baryCoordinates.z, out info);
			int num = this.AppendVertex(info);
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
			this.set_edge_triangles(num2, tid, num6);
			this.set_edge_triangles(num3, tid, num5);
			this.set_edge_triangles(num4, num5, num6);
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
			int num3 = 4 * num2;
			this.edges.insert(vA, num3);
			this.edges.insert(vB, num3 + 1);
			this.edges.insert(tA, num3 + 2);
			this.edges.insert(tB, num3 + 3);
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

		private void allocate_edges_list(int vid)
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

		private List<int> vertex_vertices_list(int vid)
		{
			List<int> list = new List<int>();
			foreach (int eID in this.vertex_edges.ValueItr(vid))
			{
				list.Add(this.edge_other_v(eID, vid));
			}
			return list;
		}

		private void set_edge_vertices(int eID, int a, int b)
		{
			int num = 4 * eID;
			this.edges[num] = Math.Min(a, b);
			this.edges[num + 1] = Math.Max(a, b);
		}

		private void set_edge_triangles(int eID, int t0, int t1)
		{
			int num = 4 * eID;
			this.edges[num + 2] = t0;
			this.edges[num + 3] = t1;
		}

		private int replace_edge_vertex(int eID, int vOld, int vNew)
		{
			int num = 4 * eID;
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

		private int replace_edge_triangle(int eID, int tOld, int tNew)
		{
			int num = 4 * eID;
			int num2 = this.edges[num + 2];
			int num3 = this.edges[num + 3];
			if (num2 == tOld)
			{
				if (tNew == -1)
				{
					this.edges[num + 2] = num3;
					this.edges[num + 3] = -1;
				}
				else
				{
					this.edges[num + 2] = tNew;
				}
				return 0;
			}
			if (num3 == tOld)
			{
				this.edges[num + 3] = tNew;
				return 1;
			}
			return -1;
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

		public const int InvalidID = -1;

		public const int NonManifoldID = -2;

		public static readonly Vector3d InvalidVertex = new Vector3d(double.MaxValue, 0.0, 0.0);

		public static readonly Index3i InvalidTriangle = new Index3i(-1, -1, -1);

		public static readonly Index2i InvalidEdge = new Index2i(-1, -1);

		private RefCountVector vertices_refcount;

		private DVector<double> vertices;

		private DVector<float> normals;

		private DVector<float> colors;

		private DVector<float> uv;

		private SmallListSet vertex_edges;

		private RefCountVector triangles_refcount;

		private DVector<int> triangles;

		private DVector<int> triangle_edges;

		private DVector<int> triangle_groups;

		private RefCountVector edges_refcount;

		private DVector<int> edges;

		private int timestamp;

		private int shape_timestamp;

		private int max_group_id;

		public bool Clockwise;

		private Dictionary<string, object> Metadata;

		private AxisAlignedBox3d cached_bounds;

		private int cached_bounds_timestamp = -1;

		private bool cached_is_closed;

		private int cached_is_closed_timestamp = -1;

		public struct CompactInfo
		{
			public IIndexMap MapV;
		}

		public struct EdgeSplitInfo
		{
			public bool bIsBoundary;

			public int vNew;

			public int eNewBN;

			public int eNewCN;

			public int eNewDN;

			public int eNewT2;

			public int eNewT3;
		}

		public struct EdgeFlipInfo
		{
			public int eID;

			public int v0;

			public int v1;

			public int ov0;

			public int ov1;

			public int t0;

			public int t1;
		}

		public struct EdgeCollapseInfo
		{
			public int vKept;

			public int vRemoved;

			public bool bIsBoundary;

			public int eCollapsed;

			public int tRemoved0;

			public int tRemoved1;

			public int eRemoved0;

			public int eRemoved1;

			public int eKept0;

			public int eKept1;
		}

		public struct MergeEdgesInfo
		{
			public int eKept;

			public int eRemoved;

			public Vector2i vKept;

			public Vector2i vRemoved;

			public Vector2i eRemovedExtra;

			public Vector2i eKeptExtra;
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
