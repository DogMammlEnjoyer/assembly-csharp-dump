using System;
using System.Collections.Generic;

namespace g3
{
	public static class MeshUtil
	{
		public static Vector3d UniformSmooth(DMesh3 mesh, int vID, double t)
		{
			Vector3d vertex = mesh.GetVertex(vID);
			Vector3d zero = Vector3d.Zero;
			mesh.VtxOneRingCentroid(vID, ref zero);
			double num = 1.0 - t;
			vertex.x = num * vertex.x + t * zero.x;
			vertex.y = num * vertex.y + t * zero.y;
			vertex.z = num * vertex.z + t * zero.z;
			return vertex;
		}

		public static Vector3d MeanValueSmooth(DMesh3 mesh, int vID, double t)
		{
			Vector3d vertex = mesh.GetVertex(vID);
			Vector3d v = MeshWeights.MeanValueCentroid(mesh, vID);
			return (1.0 - t) * vertex + t * v;
		}

		public static Vector3d CotanSmooth(DMesh3 mesh, int vID, double t)
		{
			Vector3d vertex = mesh.GetVertex(vID);
			Vector3d v = MeshWeights.CotanCentroid(mesh, vID);
			return (1.0 - t) * vertex + t * v;
		}

		public static void ScaleMesh(DMesh3 mesh, Frame3f f, Vector3f vScale)
		{
			foreach (int vID in mesh.VertexIndices())
			{
				Vector3f vector3f = (Vector3f)mesh.GetVertex(vID);
				Vector3f vector3f2 = f.ToFrameP(ref vector3f) * vScale;
				Vector3d vNewPos = f.FromFrameP(ref vector3f2);
				mesh.SetVertex(vID, vNewPos);
			}
		}

		public static double OpeningAngleD(DMesh3 mesh, int eid)
		{
			Index2i edgeT = mesh.GetEdgeT(eid);
			if (edgeT[1] == -1)
			{
				return double.MaxValue;
			}
			Vector3d triNormal = mesh.GetTriNormal(edgeT[0]);
			Vector3d triNormal2 = mesh.GetTriNormal(edgeT[1]);
			return Vector3d.AngleD(triNormal, triNormal2);
		}

		public static double DiscreteGaussCurvature(DMesh3 mesh, int vid)
		{
			double num = 0.0;
			foreach (int tID in mesh.VtxTrianglesItr(vid))
			{
				Index3i triangle = mesh.GetTriangle(tID);
				int i = IndexUtil.find_tri_index(vid, ref triangle);
				num += mesh.GetTriInternalAngleR(tID, i);
			}
			return num - 6.283185307179586;
		}

		public static bool CheckIfCollapseCreatesFlip(DMesh3 mesh, int edgeID, Vector3d newv)
		{
			Index4i edge = mesh.GetEdge(edgeID);
			int c = edge.c;
			int d = edge.d;
			for (int i = 0; i < 2; i++)
			{
				int num = edge[i];
				int num2 = edge[(i + 1) % 2];
				foreach (int num3 in mesh.VtxTrianglesItr(num))
				{
					if (num3 != c && num3 != d)
					{
						Index3i triangle = mesh.GetTriangle(num3);
						if (triangle.a == num2 || triangle.b == num2 || triangle.c == num2)
						{
							return true;
						}
						Vector3d vertex = mesh.GetVertex(triangle.a);
						Vector3d vertex2 = mesh.GetVertex(triangle.b);
						Vector3d vertex3 = mesh.GetVertex(triangle.c);
						Vector3d vector3d = (vertex2 - vertex).Cross(vertex3 - vertex);
						double num4;
						if (triangle.a == num)
						{
							Vector3d v = (vertex2 - newv).Cross(vertex3 - newv);
							num4 = vector3d.Dot(v);
						}
						else if (triangle.b == num)
						{
							Vector3d v2 = (newv - vertex).Cross(vertex3 - vertex);
							num4 = vector3d.Dot(v2);
						}
						else
						{
							if (triangle.c != num)
							{
								throw new Exception("should never be here!");
							}
							Vector3d v3 = (vertex2 - vertex).Cross(newv - vertex);
							num4 = vector3d.Dot(v3);
						}
						if (num4 <= 0.0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool CheckIfEdgeFlipCreatesFlip(DMesh3 mesh, int eID, double flip_dot_tol = 0.0)
		{
			Index4i edge = mesh.GetEdge(eID);
			Index2i edgeOpposingV = mesh.GetEdgeOpposingV(eID);
			int a = edge.a;
			int b = edge.b;
			int a2 = edgeOpposingV.a;
			int b2 = edgeOpposingV.b;
			int c = edge.c;
			Vector3d vertex = mesh.GetVertex(a2);
			Vector3d vertex2 = mesh.GetVertex(b2);
			Index3i triangle = mesh.GetTriangle(c);
			int vID = a;
			int vID2 = b;
			IndexUtil.orient_tri_edge(ref vID, ref vID2, ref triangle);
			Vector3d vertex3 = mesh.GetVertex(vID);
			Vector3d vertex4 = mesh.GetVertex(vID2);
			Vector3d vector3d = MathUtil.FastNormalDirection(ref vertex3, ref vertex4, ref vertex);
			Vector3d vector3d2 = MathUtil.FastNormalDirection(ref vertex4, ref vertex3, ref vertex2);
			Vector3d vector3d3 = MathUtil.FastNormalDirection(ref vertex, ref vertex2, ref vertex4);
			if (MeshUtil.edge_flip_metric(ref vector3d, ref vector3d3, flip_dot_tol) <= flip_dot_tol || MeshUtil.edge_flip_metric(ref vector3d2, ref vector3d3, flip_dot_tol) <= flip_dot_tol)
			{
				return true;
			}
			Vector3d vector3d4 = MathUtil.FastNormalDirection(ref vertex2, ref vertex, ref vertex3);
			return MeshUtil.edge_flip_metric(ref vector3d, ref vector3d4, flip_dot_tol) <= flip_dot_tol || MeshUtil.edge_flip_metric(ref vector3d2, ref vector3d4, flip_dot_tol) <= flip_dot_tol;
		}

		private static double edge_flip_metric(ref Vector3d n0, ref Vector3d n1, double flip_dot_tol)
		{
			if (flip_dot_tol != 0.0)
			{
				return n0.Normalized.Dot(n1.Normalized);
			}
			return n0.Dot(n1);
		}

		public static void GetEdgeFlipTris(DMesh3 mesh, int eID, out Index3i orig_t0, out Index3i orig_t1, out Index3i flip_t0, out Index3i flip_t1)
		{
			Index4i edge = mesh.GetEdge(eID);
			Index2i edgeOpposingV = mesh.GetEdgeOpposingV(eID);
			int a = edge.a;
			int b = edge.b;
			int a2 = edgeOpposingV.a;
			int b2 = edgeOpposingV.b;
			int c = edge.c;
			Index3i triangle = mesh.GetTriangle(c);
			int num = a;
			int num2 = b;
			IndexUtil.orient_tri_edge(ref num, ref num2, ref triangle);
			orig_t0 = new Index3i(num, num2, a2);
			orig_t1 = new Index3i(num2, num, b2);
			flip_t0 = new Index3i(a2, b2, num2);
			flip_t1 = new Index3i(b2, a2, num);
		}

		public static void GetEdgeFlipNormals(DMesh3 mesh, int eID, out Vector3d n1, out Vector3d n2, out Vector3d on1, out Vector3d on2)
		{
			Index4i edge = mesh.GetEdge(eID);
			Index2i edgeOpposingV = mesh.GetEdgeOpposingV(eID);
			int a = edge.a;
			int b = edge.b;
			int a2 = edgeOpposingV.a;
			int b2 = edgeOpposingV.b;
			int c = edge.c;
			Vector3d vertex = mesh.GetVertex(a2);
			Vector3d vertex2 = mesh.GetVertex(b2);
			Index3i triangle = mesh.GetTriangle(c);
			int vID = a;
			int vID2 = b;
			IndexUtil.orient_tri_edge(ref vID, ref vID2, ref triangle);
			Vector3d vertex3 = mesh.GetVertex(vID);
			Vector3d vertex4 = mesh.GetVertex(vID2);
			n1 = MathUtil.Normal(ref vertex3, ref vertex4, ref vertex);
			n2 = MathUtil.Normal(ref vertex4, ref vertex3, ref vertex2);
			on1 = MathUtil.Normal(ref vertex, ref vertex2, ref vertex4);
			on2 = MathUtil.Normal(ref vertex2, ref vertex, ref vertex3);
		}

		public static DCurve3 ExtractLoopV(IMesh mesh, IEnumerable<int> vertices)
		{
			DCurve3 dcurve = new DCurve3();
			foreach (int i in vertices)
			{
				dcurve.AppendVertex(mesh.GetVertex(i));
			}
			dcurve.Closed = true;
			return dcurve;
		}

		public static DCurve3 ExtractLoopV(IMesh mesh, int[] vertices)
		{
			DCurve3 dcurve = new DCurve3();
			for (int i = 0; i < vertices.Length; i++)
			{
				dcurve.AppendVertex(mesh.GetVertex(vertices[i]));
			}
			dcurve.Closed = true;
			return dcurve;
		}
	}
}
