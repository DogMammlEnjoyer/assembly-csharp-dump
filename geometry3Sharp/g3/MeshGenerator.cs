using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace g3
{
	public abstract class MeshGenerator
	{
		public abstract MeshGenerator Generate();

		public virtual void MakeMesh(SimpleMesh m)
		{
			m.AppendVertices(this.vertices, this.WantNormals ? this.normals : null, null, this.WantUVs ? this.uv : null);
			m.AppendTriangles(this.triangles, null);
		}

		public virtual SimpleMesh MakeSimpleMesh()
		{
			SimpleMesh simpleMesh = new SimpleMesh();
			this.MakeMesh(simpleMesh);
			return simpleMesh;
		}

		public virtual void MakeMesh(DMesh3 m)
		{
			int count = this.vertices.Count;
			bool flag = this.WantNormals && this.normals != null && this.normals.Count == this.vertices.Count;
			if (flag)
			{
				m.EnableVertexNormals(Vector3f.AxisY);
			}
			bool flag2 = this.WantUVs && this.uv != null && this.uv.Count == this.vertices.Count;
			if (flag2)
			{
				m.EnableVertexUVs(Vector2f.Zero);
			}
			for (int i = 0; i < count; i++)
			{
				NewVertexInfo info = new NewVertexInfo
				{
					v = this.vertices[i]
				};
				if (flag)
				{
					info.bHaveN = true;
					info.n = this.normals[i];
				}
				if (flag2)
				{
					info.bHaveUV = true;
					info.uv = this.uv[i];
				}
				m.AppendVertex(info);
			}
			int count2 = this.triangles.Count;
			if (this.WantGroups && this.groups != null && this.groups.Length == count2)
			{
				m.EnableTriangleGroups(0);
				for (int j = 0; j < count2; j++)
				{
					m.AppendTriangle(this.triangles[j], this.groups[j]);
				}
				return;
			}
			for (int k = 0; k < count2; k++)
			{
				m.AppendTriangle(this.triangles[k], -1);
			}
		}

		public virtual DMesh3 MakeDMesh()
		{
			DMesh3 dmesh = new DMesh3(true, false, false, false);
			this.MakeMesh(dmesh);
			return dmesh;
		}

		public virtual void MakeMesh(NTMesh3 m)
		{
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				m.AppendVertex(this.vertices[i]);
			}
			int count2 = this.triangles.Count;
			if (this.WantGroups && this.groups != null && this.groups.Length == count2)
			{
				m.EnableTriangleGroups(0);
				for (int j = 0; j < count2; j++)
				{
					m.AppendTriangle(this.triangles[j], this.groups[j]);
				}
				return;
			}
			for (int k = 0; k < count2; k++)
			{
				m.AppendTriangle(this.triangles[k], -1);
			}
		}

		public virtual NTMesh3 MakeNTMesh()
		{
			NTMesh3 ntmesh = new NTMesh3(true, false, false);
			this.MakeMesh(ntmesh);
			return ntmesh;
		}

		protected void duplicate_vertex_span(int nStart, int nCount)
		{
			for (int i = 0; i < nCount; i++)
			{
				this.vertices[nStart + nCount + i] = this.vertices[nStart + i];
				this.normals[nStart + nCount + i] = this.normals[nStart + i];
				this.uv[nStart + nCount + i] = this.uv[nStart + i];
			}
		}

		protected void append_disc(int Slices, int nCenterV, int nRingStart, bool bClosed, bool bCycle, ref int tri_counter, int groupid = -1)
		{
			int num = nRingStart + Slices;
			for (int i = nRingStart; i < num - 1; i++)
			{
				if (groupid >= 0)
				{
					this.groups[tri_counter] = groupid;
				}
				IndexArray3i indexArray3i = this.triangles;
				int num2 = tri_counter;
				tri_counter = num2 + 1;
				indexArray3i.Set(num2, i, nCenterV, i + 1, bCycle);
			}
			if (bClosed)
			{
				if (groupid >= 0)
				{
					this.groups[tri_counter] = groupid;
				}
				IndexArray3i indexArray3i2 = this.triangles;
				int num2 = tri_counter;
				tri_counter = num2 + 1;
				indexArray3i2.Set(num2, num - 1, nCenterV, nRingStart, bCycle);
			}
		}

		protected void append_rectangle(int v0, int v1, int v2, int v3, bool bCycle, ref int tri_counter, int groupid = -1)
		{
			if (groupid >= 0)
			{
				this.groups[tri_counter] = groupid;
			}
			IndexArray3i indexArray3i = this.triangles;
			int num = tri_counter;
			tri_counter = num + 1;
			indexArray3i.Set(num, v0, v1, v2, bCycle);
			if (groupid >= 0)
			{
				this.groups[tri_counter] = groupid;
			}
			IndexArray3i indexArray3i2 = this.triangles;
			num = tri_counter;
			tri_counter = num + 1;
			indexArray3i2.Set(num, v0, v2, v3, bCycle);
		}

		protected void append_2d_disc_segment(int iCenter, int iEnd1, int iEnd2, int nSteps, bool bCycle, ref int vtx_counter, ref int tri_counter, int groupid = -1, double force_r = 0.0)
		{
			Vector3d vector3d = this.vertices[iCenter];
			Vector3d v = this.vertices[iEnd1];
			Vector3d v2 = this.vertices[iEnd2];
			Vector3d vector3d2 = v - vector3d;
			double num = vector3d2.Normalize(2.220446049250313E-16);
			if (force_r > 0.0)
			{
				num = force_r;
			}
			double num2 = Math.Atan2(vector3d2.z, vector3d2.x);
			Vector3d vector3d3 = v2 - vector3d;
			double num3 = vector3d3.Normalize(2.220446049250313E-16);
			if (force_r > 0.0)
			{
				num3 = force_r;
			}
			double num4 = Math.Atan2(vector3d3.z, vector3d3.x);
			if (num2 < 0.0)
			{
				num2 += 6.283185307179586;
			}
			if (num4 < 0.0)
			{
				num4 += 6.283185307179586;
			}
			if (num4 < num2)
			{
				num4 += 6.283185307179586;
			}
			int b = iEnd1;
			int num7;
			for (int i = 0; i < nSteps; i++)
			{
				double num5 = (double)(i + 1) / (double)(nSteps + 1);
				double num6 = (1.0 - num5) * num2 + num5 * num4;
				Vector3d vector3d4 = vector3d + new Vector3d(num * Math.Cos(num6), 0.0, num3 * Math.Sin(num6));
				this.vertices.Set(vtx_counter, vector3d4.x, vector3d4.y, vector3d4.z);
				if (groupid >= 0)
				{
					this.groups[tri_counter] = groupid;
				}
				IndexArray3i indexArray3i = this.triangles;
				num7 = tri_counter;
				tri_counter = num7 + 1;
				indexArray3i.Set(num7, iCenter, b, vtx_counter, bCycle);
				num7 = vtx_counter;
				vtx_counter = num7 + 1;
				b = num7;
			}
			if (groupid >= 0)
			{
				this.groups[tri_counter] = groupid;
			}
			IndexArray3i indexArray3i2 = this.triangles;
			num7 = tri_counter;
			tri_counter = num7 + 1;
			indexArray3i2.Set(num7, iCenter, b, iEnd2, bCycle);
		}

		protected Vector3f estimate_normal(int v0, int v1, int v2)
		{
			Vector3d v3 = this.vertices[v0];
			Vector3d v4 = this.vertices[v1];
			Vector3d v5 = this.vertices[v2];
			Vector3d normalized = (v4 - v3).Normalized;
			Vector3d normalized2 = (v5 - v3).Normalized;
			return new Vector3f(normalized.Cross(normalized2));
		}

		protected Vector3d bilerp(ref Vector3d v00, ref Vector3d v10, ref Vector3d v11, ref Vector3d v01, double tx, double ty)
		{
			Vector3d a = Vector3d.Lerp(ref v00, ref v01, ty);
			Vector3d b = Vector3d.Lerp(ref v10, ref v11, ty);
			return Vector3d.Lerp(a, b, tx);
		}

		protected Vector2d bilerp(ref Vector2d v00, ref Vector2d v10, ref Vector2d v11, ref Vector2d v01, double tx, double ty)
		{
			Vector2d a = Vector2d.Lerp(ref v00, ref v01, ty);
			Vector2d b = Vector2d.Lerp(ref v10, ref v11, ty);
			return Vector2d.Lerp(a, b, tx);
		}

		protected Vector2f bilerp(ref Vector2f v00, ref Vector2f v10, ref Vector2f v11, ref Vector2f v01, float tx, float ty)
		{
			Vector2f a = Vector2f.Lerp(ref v00, ref v01, ty);
			Vector2f b = Vector2f.Lerp(ref v10, ref v11, ty);
			return Vector2f.Lerp(a, b, tx);
		}

		protected Vector3i bilerp(ref Vector3i v00, ref Vector3i v10, ref Vector3i v11, ref Vector3i v01, double tx, double ty)
		{
			Vector3d a = Vector3d.Lerp((Vector3d)v00, (Vector3d)v01, ty);
			Vector3d b = Vector3d.Lerp((Vector3d)v10, (Vector3d)v11, ty);
			Vector3d vector3d = Vector3d.Lerp(a, b, tx);
			return new Vector3i((int)Math.Round(vector3d.x), (int)Math.Round(vector3d.y), (int)Math.Round(vector3d.z));
		}

		protected Vector3i lerp(ref Vector3i a, ref Vector3i b, double t)
		{
			Vector3d vector3d = Vector3d.Lerp((Vector3d)a, (Vector3d)b, t);
			return new Vector3i((int)Math.Round(vector3d.x), (int)Math.Round(vector3d.y), (int)Math.Round(vector3d.z));
		}

		private static Vector3[] ToUnityVector3(VectorArray3f a, bool bFlipLR = false)
		{
			Vector3[] array = new Vector3[a.Count];
			float num = (float)(bFlipLR ? -1 : 1);
			for (int i = 0; i < a.Count; i++)
			{
				array[i].x = a.array[3 * i];
				array[i].y = a.array[3 * i + 1];
				array[i].z = num * a.array[3 * i + 2];
			}
			return array;
		}

		private static Vector3[] ToUnityVector3(VectorArray3d a, bool bFlipLR = false)
		{
			Vector3[] array = new Vector3[a.Count];
			float num = (float)(bFlipLR ? -1 : 1);
			for (int i = 0; i < a.Count; i++)
			{
				array[i].x = (float)a.array[3 * i];
				array[i].y = (float)a.array[3 * i + 1];
				array[i].z = num * (float)a.array[3 * i + 2];
			}
			return array;
		}

		private static Vector2[] ToUnityVector2(VectorArray2f a)
		{
			Vector2[] array = new Vector2[a.Count];
			for (int i = 0; i < a.Count; i++)
			{
				array[i].x = a.array[2 * i];
				array[i].y = a.array[2 * i + 1];
			}
			return array;
		}

		public void MakeMesh(Mesh m, bool bRecalcNormals = false, bool bFlipLR = false)
		{
			m.vertices = MeshGenerator.ToUnityVector3(this.vertices, bFlipLR);
			if (this.uv != null && this.WantUVs)
			{
				m.uv = MeshGenerator.ToUnityVector2(this.uv);
			}
			if (this.normals != null && this.WantNormals)
			{
				m.normals = MeshGenerator.ToUnityVector3(this.normals, bFlipLR);
			}
			if (m.vertexCount > 64000 || this.triangles.Count > 64000)
			{
				m.indexFormat = IndexFormat.UInt32;
			}
			m.triangles = this.triangles.array;
			if (bRecalcNormals)
			{
				m.RecalculateNormals();
			}
		}

		public VectorArray3d vertices;

		public VectorArray2f uv;

		public VectorArray3f normals;

		public IndexArray3i triangles;

		public int[] groups;

		public bool WantUVs = true;

		public bool WantNormals = true;

		public bool WantGroups = true;

		public bool Clockwise;

		public struct CircularSection
		{
			public CircularSection(float r, float y)
			{
				this.Radius = r;
				this.SectionY = y;
			}

			public float Radius;

			public float SectionY;
		}
	}
}
