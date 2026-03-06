using System;
using System.Collections.Generic;

namespace g3
{
	public static class MeshQueries
	{
		public static DistPoint3Triangle3 TriangleDistance(DMesh3 mesh, int ti, Vector3d point)
		{
			if (!mesh.IsTriangle(ti))
			{
				return null;
			}
			Triangle3d triangleIn = default(Triangle3d);
			mesh.GetTriVertices(ti, ref triangleIn.V0, ref triangleIn.V1, ref triangleIn.V2);
			DistPoint3Triangle3 distPoint3Triangle = new DistPoint3Triangle3(point, triangleIn);
			distPoint3Triangle.GetSquared();
			return distPoint3Triangle;
		}

		public static Frame3f NearestPointFrame(DMesh3 mesh, ISpatial spatial, Vector3d queryPoint, bool bForceFaceNormal = false)
		{
			int num = spatial.FindNearestTriangle(queryPoint, double.MaxValue);
			Vector3d triangleClosest = MeshQueries.TriangleDistance(mesh, num, queryPoint).TriangleClosest;
			if (mesh.HasVertexNormals && !bForceFaceNormal)
			{
				return MeshQueries.SurfaceFrame(mesh, num, triangleClosest, false);
			}
			return new Frame3f(triangleClosest, mesh.GetTriNormal(num));
		}

		public static double NearestPointDistance(DMesh3 mesh, ISpatial spatial, Vector3d queryPoint, double maxDist = 1.7976931348623157E+308)
		{
			int num = spatial.FindNearestTriangle(queryPoint, maxDist);
			if (num == -1)
			{
				return double.MaxValue;
			}
			Triangle3d triangle3d = default(Triangle3d);
			mesh.GetTriVertices(num, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			Vector3d vector3d;
			Vector3d vector3d2;
			return Math.Sqrt(DistPoint3Triangle3.DistanceSqr(ref queryPoint, ref triangle3d, out vector3d, out vector3d2));
		}

		public static DistTriangle3Triangle3 TriangleTriangleDistance(DMesh3 mesh1, int ti, DMesh3 mesh2, int tj, Func<Vector3d, Vector3d> TransformF = null)
		{
			if (!mesh1.IsTriangle(ti) || !mesh2.IsTriangle(tj))
			{
				return null;
			}
			Triangle3d triangle0in = default(Triangle3d);
			Triangle3d triangle3d = default(Triangle3d);
			mesh1.GetTriVertices(ti, ref triangle0in.V0, ref triangle0in.V1, ref triangle0in.V2);
			mesh2.GetTriVertices(tj, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			if (TransformF != null)
			{
				triangle3d.V0 = TransformF(triangle3d.V0);
				triangle3d.V1 = TransformF(triangle3d.V1);
				triangle3d.V2 = TransformF(triangle3d.V2);
			}
			DistTriangle3Triangle3 distTriangle3Triangle = new DistTriangle3Triangle3(triangle0in, triangle3d);
			distTriangle3Triangle.Compute();
			return distTriangle3Triangle;
		}

		public static IntrRay3Triangle3 TriangleIntersection(DMesh3 mesh, int ti, Ray3d ray)
		{
			if (!mesh.IsTriangle(ti))
			{
				return null;
			}
			Triangle3d t = default(Triangle3d);
			mesh.GetTriVertices(ti, ref t.V0, ref t.V1, ref t.V2);
			IntrRay3Triangle3 intrRay3Triangle = new IntrRay3Triangle3(ray, t);
			intrRay3Triangle.Find();
			return intrRay3Triangle;
		}

		public static IntrTriangle3Triangle3 TrianglesIntersection(DMesh3 mesh1, int ti, DMesh3 mesh2, int tj, Func<Vector3d, Vector3d> TransformF = null)
		{
			if (!mesh1.IsTriangle(ti) || !mesh2.IsTriangle(tj))
			{
				return null;
			}
			Triangle3d t = default(Triangle3d);
			Triangle3d triangle3d = default(Triangle3d);
			mesh1.GetTriVertices(ti, ref t.V0, ref t.V1, ref t.V2);
			mesh2.GetTriVertices(tj, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			if (TransformF != null)
			{
				triangle3d.V0 = TransformF(triangle3d.V0);
				triangle3d.V1 = TransformF(triangle3d.V1);
				triangle3d.V2 = TransformF(triangle3d.V2);
			}
			IntrTriangle3Triangle3 intrTriangle3Triangle = new IntrTriangle3Triangle3(t, triangle3d);
			intrTriangle3Triangle.Find();
			return intrTriangle3Triangle;
		}

		public static DistTriangle3Triangle3 TrianglesDistance(DMesh3 mesh1, int ti, DMesh3 mesh2, int tj, Func<Vector3d, Vector3d> TransformF = null)
		{
			if (!mesh1.IsTriangle(ti) || !mesh2.IsTriangle(tj))
			{
				return null;
			}
			Triangle3d triangle0in = default(Triangle3d);
			Triangle3d triangle3d = default(Triangle3d);
			mesh1.GetTriVertices(ti, ref triangle0in.V0, ref triangle0in.V1, ref triangle0in.V2);
			mesh2.GetTriVertices(tj, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			if (TransformF != null)
			{
				triangle3d.V0 = TransformF(triangle3d.V0);
				triangle3d.V1 = TransformF(triangle3d.V1);
				triangle3d.V2 = TransformF(triangle3d.V2);
			}
			DistTriangle3Triangle3 distTriangle3Triangle = new DistTriangle3Triangle3(triangle0in, triangle3d);
			distTriangle3Triangle.GetSquared();
			return distTriangle3Triangle;
		}

		public static bool RayHitPointFrame(DMesh3 mesh, ISpatial spatial, Ray3d ray, out Frame3f hitPosFrame, bool bForceFaceNormal = false)
		{
			hitPosFrame = default(Frame3f);
			int num = spatial.FindNearestHitTriangle(ray, double.MaxValue);
			if (num == -1)
			{
				return false;
			}
			IntrRay3Triangle3 intrRay3Triangle = MeshQueries.TriangleIntersection(mesh, num, ray);
			if (intrRay3Triangle.Result != IntersectionResult.Intersects)
			{
				return false;
			}
			Vector3d vector3d = ray.PointAt(intrRay3Triangle.RayParameter);
			if (mesh.HasVertexNormals && !bForceFaceNormal)
			{
				hitPosFrame = MeshQueries.SurfaceFrame(mesh, num, vector3d, false);
			}
			else
			{
				hitPosFrame = new Frame3f(vector3d, mesh.GetTriNormal(num));
			}
			return true;
		}

		public static Frame3f SurfaceFrame(DMesh3 mesh, int tID, Vector3d point, bool bForceFaceNormal = false)
		{
			if (!mesh.IsTriangle(tID))
			{
				throw new Exception("MeshQueries.SurfaceFrame: triangle " + tID.ToString() + " does not exist!");
			}
			Triangle3d triangle3d = default(Triangle3d);
			mesh.GetTriVertices(tID, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			Vector3d vector3d = triangle3d.BarycentricCoords(point);
			point = triangle3d.PointAt(vector3d);
			if (mesh.HasVertexNormals && !bForceFaceNormal)
			{
				Vector3d triBaryNormal = mesh.GetTriBaryNormal(tID, vector3d.x, vector3d.y, vector3d.z);
				return new Frame3f(point, triBaryNormal);
			}
			return new Frame3f(point, mesh.GetTriNormal(tID));
		}

		public static Vector3d BaryCoords(DMesh3 mesh, int tID, Vector3d point)
		{
			if (!mesh.IsTriangle(tID))
			{
				throw new Exception("MeshQueries.SurfaceFrame: triangle " + tID.ToString() + " does not exist!");
			}
			Triangle3d triangle3d = default(Triangle3d);
			mesh.GetTriVertices(tID, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			return triangle3d.BarycentricCoords(point);
		}

		public static double TriDistanceSqr(DMesh3 mesh, int ti, Vector3d point)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			mesh.GetTriVertices(ti, ref zero, ref zero2, ref zero3);
			Vector3d vector3d = zero - point;
			Vector3d vector3d2 = zero2 - zero;
			Vector3d vector3d3 = zero3 - zero;
			double lengthSquared = vector3d2.LengthSquared;
			double num = vector3d2.Dot(ref vector3d3);
			double lengthSquared2 = vector3d3.LengthSquared;
			double num2 = vector3d.Dot(ref vector3d2);
			double num3 = vector3d.Dot(ref vector3d3);
			double lengthSquared3 = vector3d.LengthSquared;
			double num4 = Math.Abs(lengthSquared * lengthSquared2 - num * num);
			double num5 = num * num3 - lengthSquared2 * num2;
			double num6 = num * num2 - lengthSquared * num3;
			double num7;
			if (num5 + num6 <= num4)
			{
				if (num5 < 0.0)
				{
					if (num6 < 0.0)
					{
						if (num2 < 0.0)
						{
							if (-num2 >= lengthSquared)
							{
								num7 = lengthSquared + 2.0 * num2 + lengthSquared3;
							}
							else
							{
								num5 = -num2 / lengthSquared;
								num7 = num2 * num5 + lengthSquared3;
							}
						}
						else if (num3 >= 0.0)
						{
							num7 = lengthSquared3;
						}
						else if (-num3 >= lengthSquared2)
						{
							num7 = lengthSquared2 + 2.0 * num3 + lengthSquared3;
						}
						else
						{
							num6 = -num3 / lengthSquared2;
							num7 = num3 * num6 + lengthSquared3;
						}
					}
					else if (num3 >= 0.0)
					{
						num7 = lengthSquared3;
					}
					else if (-num3 >= lengthSquared2)
					{
						num7 = lengthSquared2 + 2.0 * num3 + lengthSquared3;
					}
					else
					{
						num6 = -num3 / lengthSquared2;
						num7 = num3 * num6 + lengthSquared3;
					}
				}
				else if (num6 < 0.0)
				{
					if (num2 >= 0.0)
					{
						num7 = lengthSquared3;
					}
					else if (-num2 >= lengthSquared)
					{
						num7 = lengthSquared + 2.0 * num2 + lengthSquared3;
					}
					else
					{
						num5 = -num2 / lengthSquared;
						num7 = num2 * num5 + lengthSquared3;
					}
				}
				else
				{
					double num8 = 1.0 / num4;
					num5 *= num8;
					num6 *= num8;
					num7 = num5 * (lengthSquared * num5 + num * num6 + 2.0 * num2) + num6 * (num * num5 + lengthSquared2 * num6 + 2.0 * num3) + lengthSquared3;
				}
			}
			else if (num5 < 0.0)
			{
				double num9 = num + num2;
				double num10 = lengthSquared2 + num3;
				if (num10 > num9)
				{
					double num11 = num10 - num9;
					double num12 = lengthSquared - 2.0 * num + lengthSquared2;
					if (num11 >= num12)
					{
						num7 = lengthSquared + 2.0 * num2 + lengthSquared3;
					}
					else
					{
						num5 = num11 / num12;
						num6 = 1.0 - num5;
						num7 = num5 * (lengthSquared * num5 + num * num6 + 2.0 * num2) + num6 * (num * num5 + lengthSquared2 * num6 + 2.0 * num3) + lengthSquared3;
					}
				}
				else if (num10 <= 0.0)
				{
					num7 = lengthSquared2 + 2.0 * num3 + lengthSquared3;
				}
				else if (num3 >= 0.0)
				{
					num7 = lengthSquared3;
				}
				else
				{
					num6 = -num3 / lengthSquared2;
					num7 = num3 * num6 + lengthSquared3;
				}
			}
			else if (num6 < 0.0)
			{
				double num9 = num + num3;
				double num10 = lengthSquared + num2;
				if (num10 > num9)
				{
					double num11 = num10 - num9;
					double num12 = lengthSquared - 2.0 * num + lengthSquared2;
					if (num11 >= num12)
					{
						num7 = lengthSquared2 + 2.0 * num3 + lengthSquared3;
					}
					else
					{
						num6 = num11 / num12;
						num5 = 1.0 - num6;
						num7 = num5 * (lengthSquared * num5 + num * num6 + 2.0 * num2) + num6 * (num * num5 + lengthSquared2 * num6 + 2.0 * num3) + lengthSquared3;
					}
				}
				else if (num10 <= 0.0)
				{
					num7 = lengthSquared + 2.0 * num2 + lengthSquared3;
				}
				else if (num2 >= 0.0)
				{
					num7 = lengthSquared3;
				}
				else
				{
					num5 = -num2 / lengthSquared;
					num7 = num2 * num5 + lengthSquared3;
				}
			}
			else
			{
				double num11 = lengthSquared2 + num3 - num - num2;
				if (num11 <= 0.0)
				{
					num7 = lengthSquared2 + 2.0 * num3 + lengthSquared3;
				}
				else
				{
					double num12 = lengthSquared - 2.0 * num + lengthSquared2;
					if (num11 >= num12)
					{
						num7 = lengthSquared + 2.0 * num2 + lengthSquared3;
					}
					else
					{
						num5 = num11 / num12;
						num6 = 1.0 - num5;
						num7 = num5 * (lengthSquared * num5 + num * num6 + 2.0 * num2) + num6 * (num * num5 + lengthSquared2 * num6 + 2.0 * num3) + lengthSquared3;
					}
				}
			}
			if (num7 < 0.0)
			{
				num7 = 0.0;
			}
			return num7;
		}

		public static int FindNearestVertex_LinearSearch(DMesh3 mesh, Vector3d p)
		{
			int result = -1;
			double num = double.MaxValue;
			foreach (int num2 in mesh.VertexIndices())
			{
				double num3 = mesh.GetVertex(num2).DistanceSquared(p);
				if (num3 < num)
				{
					num = num3;
					result = num2;
				}
			}
			return result;
		}

		public static int FindNearestTriangle_LinearSearch(DMesh3 mesh, Vector3d p)
		{
			int result = -1;
			double num = double.MaxValue;
			foreach (int num2 in mesh.TriangleIndices())
			{
				double num3 = MeshQueries.TriDistanceSqr(mesh, num2, p);
				if (num3 < num)
				{
					num = num3;
					result = num2;
				}
			}
			return result;
		}

		public static int FindHitTriangle_LinearSearch(DMesh3 mesh, Ray3d ray)
		{
			int result = -1;
			double num = double.MaxValue;
			Triangle3d t = default(Triangle3d);
			foreach (int num2 in mesh.TriangleIndices())
			{
				mesh.GetTriVertices(num2, ref t.V0, ref t.V1, ref t.V2);
				IntrRay3Triangle3 intrRay3Triangle = new IntrRay3Triangle3(ray, t);
				if (intrRay3Triangle.Find() && intrRay3Triangle.RayParameter < num)
				{
					num = intrRay3Triangle.RayParameter;
					result = num2;
				}
			}
			return result;
		}

		public static Index2i FindIntersectingTriangles_LinearSearch(DMesh3 mesh1, DMesh3 mesh2)
		{
			foreach (int num in mesh1.TriangleIndices())
			{
				Vector3d zero = Vector3d.Zero;
				Vector3d zero2 = Vector3d.Zero;
				Vector3d zero3 = Vector3d.Zero;
				mesh1.GetTriVertices(num, ref zero, ref zero2, ref zero3);
				foreach (int num2 in mesh2.TriangleIndices())
				{
					Vector3d zero4 = Vector3d.Zero;
					Vector3d zero5 = Vector3d.Zero;
					Vector3d zero6 = Vector3d.Zero;
					mesh2.GetTriVertices(num2, ref zero4, ref zero5, ref zero6);
					if (new IntrTriangle3Triangle3(new Triangle3d(zero, zero2, zero3), new Triangle3d(zero4, zero5, zero6)).Test())
					{
						return new Index2i(num, num2);
					}
				}
			}
			return Index2i.Max;
		}

		public static Index2i FindNearestTriangles_LinearSearch(DMesh3 mesh1, DMesh3 mesh2, out double fNearestSqr)
		{
			Index2i result = Index2i.Max;
			fNearestSqr = double.MaxValue;
			foreach (int num in mesh1.TriangleIndices())
			{
				Vector3d zero = Vector3d.Zero;
				Vector3d zero2 = Vector3d.Zero;
				Vector3d zero3 = Vector3d.Zero;
				mesh1.GetTriVertices(num, ref zero, ref zero2, ref zero3);
				foreach (int num2 in mesh2.TriangleIndices())
				{
					Vector3d zero4 = Vector3d.Zero;
					Vector3d zero5 = Vector3d.Zero;
					Vector3d zero6 = Vector3d.Zero;
					mesh2.GetTriVertices(num2, ref zero4, ref zero5, ref zero6);
					DistTriangle3Triangle3 distTriangle3Triangle = new DistTriangle3Triangle3(new Triangle3d(zero, zero2, zero3), new Triangle3d(zero4, zero5, zero6));
					if (distTriangle3Triangle.GetSquared() < fNearestSqr)
					{
						fNearestSqr = distTriangle3Triangle.GetSquared();
						result = new Index2i(num, num2);
					}
				}
			}
			fNearestSqr = Math.Sqrt(fNearestSqr);
			return result;
		}

		public static void EdgeLengthStats(DMesh3 mesh, out double minEdgeLen, out double maxEdgeLen, out double avgEdgeLen, int samples = 0)
		{
			minEdgeLen = double.MaxValue;
			maxEdgeLen = double.MinValue;
			avgEdgeLen = 0.0;
			int num = 0;
			int maxEdgeID = mesh.MaxEdgeID;
			int num2 = (samples == 0) ? 1 : 31337;
			int num3 = (samples == 0) ? maxEdgeID : samples;
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			int num4 = 0;
			int num5 = 0;
			do
			{
				if (mesh.IsEdge(num4))
				{
					mesh.GetEdgeV(num4, ref zero, ref zero2);
					double num6 = zero.Distance(zero2);
					if (num6 < minEdgeLen)
					{
						minEdgeLen = num6;
					}
					if (num6 > maxEdgeLen)
					{
						maxEdgeLen = num6;
					}
					avgEdgeLen += num6;
					num++;
				}
				num4 = (num4 + num2) % maxEdgeID;
			}
			while (num4 != 0 && num5++ < num3);
			avgEdgeLen /= (double)num;
		}

		public static void EdgeLengthStatsFromEdges(DMesh3 mesh, IEnumerable<int> EdgeItr, out double minEdgeLen, out double maxEdgeLen, out double avgEdgeLen, int samples = 0)
		{
			minEdgeLen = double.MaxValue;
			maxEdgeLen = double.MinValue;
			avgEdgeLen = 0.0;
			int num = 0;
			int maxEdgeID = mesh.MaxEdgeID;
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			foreach (int eID in EdgeItr)
			{
				if (mesh.IsEdge(eID))
				{
					mesh.GetEdgeV(eID, ref zero, ref zero2);
					double num2 = zero.Distance(zero2);
					if (num2 < minEdgeLen)
					{
						minEdgeLen = num2;
					}
					if (num2 > maxEdgeLen)
					{
						maxEdgeLen = num2;
					}
					avgEdgeLen += num2;
					num++;
				}
			}
			avgEdgeLen /= (double)num;
		}
	}
}
