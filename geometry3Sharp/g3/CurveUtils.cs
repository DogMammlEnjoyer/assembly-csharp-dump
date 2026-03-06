using System;
using System.Collections.Generic;

namespace g3
{
	public class CurveUtils
	{
		public static Vector3d GetTangent(List<Vector3d> vertices, int i, bool bLoop = false)
		{
			if (bLoop)
			{
				int count = vertices.Count;
				if (i == 0)
				{
					return (vertices[1] - vertices[count - 1]).Normalized;
				}
				return (vertices[(i + 1) % count] - vertices[i - 1]).Normalized;
			}
			else
			{
				if (i == 0)
				{
					return (vertices[1] - vertices[0]).Normalized;
				}
				if (i == vertices.Count - 1)
				{
					return (vertices[vertices.Count - 1] - vertices[vertices.Count - 2]).Normalized;
				}
				return (vertices[i + 1] - vertices[i - 1]).Normalized;
			}
		}

		public static double ArcLength(List<Vector3d> vertices, bool bLoop = false)
		{
			double num = 0.0;
			int count = vertices.Count;
			for (int i = 1; i < count; i++)
			{
				num += vertices[i].Distance(vertices[i - 1]);
			}
			if (bLoop)
			{
				num += vertices[count - 1].Distance(vertices[0]);
			}
			return num;
		}

		public static double ArcLength(Vector3d[] vertices, bool bLoop = false)
		{
			double num = 0.0;
			for (int i = 1; i < vertices.Length; i++)
			{
				num += vertices[i].Distance(vertices[i - 1]);
			}
			if (bLoop)
			{
				num += vertices[vertices.Length - 1].Distance(vertices[0]);
			}
			return num;
		}

		public static double ArcLength(IEnumerable<Vector3d> vertices)
		{
			double num = 0.0;
			Vector3d v = Vector3f.Zero;
			int num2 = 0;
			foreach (Vector3d vector3d in vertices)
			{
				if (num2++ > 0)
				{
					num += (vector3d - v).Length;
				}
				v = vector3d;
			}
			return num;
		}

		public static int FindNearestIndex(ISampledCurve3d c, Vector3d v)
		{
			int result = -1;
			double num = double.MaxValue;
			int vertexCount = c.VertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				double lengthSquared = (c.GetVertex(i) - v).LengthSquared;
				if (lengthSquared < num)
				{
					num = lengthSquared;
					result = i;
				}
			}
			return result;
		}

		public static bool FindClosestRayIntersection(ISampledCurve3d c, double segRadius, Ray3d ray, out double minRayT)
		{
			minRayT = double.MaxValue;
			int num = -1;
			int segmentCount = c.SegmentCount;
			for (int i = 0; i < segmentCount; i++)
			{
				Segment3d segment = c.GetSegment(i);
				double num2;
				double num3;
				double num4;
				if (RayIntersection.SphereSigned(ref ray.Origin, ref ray.Direction, ref segment.Center, segment.Extent + segRadius, out num2) && DistRay3Segment3.SquaredDistance(ref ray, ref segment, out num3, out num4) < segRadius * segRadius && num3 < minRayT)
				{
					minRayT = num3;
					num = i;
				}
			}
			return num >= 0;
		}

		public static void InPlaceSmooth(IList<Vector3d> vertices, double alpha, int nIterations, bool bClosed)
		{
			CurveUtils.InPlaceSmooth(vertices, 0, vertices.Count, alpha, nIterations, bClosed);
		}

		public static void InPlaceSmooth(IList<Vector3d> vertices, int iStart, int iEnd, double alpha, int nIterations, bool bClosed)
		{
			int count = vertices.Count;
			if (bClosed)
			{
				for (int i = 0; i < nIterations; i++)
				{
					for (int j = iStart; j < iEnd; j++)
					{
						int index = j % count;
						int index2 = (j == 0) ? (count - 1) : (j - 1);
						int index3 = (j + 1) % count;
						Vector3d v = vertices[index2];
						Vector3d v2 = vertices[index3];
						Vector3d v3 = (v + v2) * 0.5;
						vertices[index] = (1.0 - alpha) * vertices[index] + alpha * v3;
					}
				}
				return;
			}
			for (int k = 0; k < nIterations; k++)
			{
				for (int l = iStart; l <= iEnd; l++)
				{
					if (l != 0 && l < count - 1)
					{
						Vector3d v4 = vertices[l - 1];
						Vector3d v5 = vertices[l + 1];
						Vector3d v6 = (v4 + v5) * 0.5;
						vertices[l] = (1.0 - alpha) * vertices[l] + alpha * v6;
					}
				}
			}
		}

		public static void IterativeSmooth(IList<Vector3d> vertices, double alpha, int nIterations, bool bClosed)
		{
			CurveUtils.IterativeSmooth(vertices, 0, vertices.Count, alpha, nIterations, bClosed, null);
		}

		public static void IterativeSmooth(IList<Vector3d> vertices, int iStart, int iEnd, double alpha, int nIterations, bool bClosed, Vector3d[] buffer = null)
		{
			int count = vertices.Count;
			if (buffer == null || buffer.Length < count)
			{
				buffer = new Vector3d[count];
			}
			if (bClosed)
			{
				for (int i = 0; i < nIterations; i++)
				{
					for (int j = iStart; j < iEnd; j++)
					{
						int num = j % count;
						int index = (j == 0) ? (count - 1) : (j - 1);
						int index2 = (j + 1) % count;
						Vector3d v = vertices[index];
						Vector3d v2 = vertices[index2];
						Vector3d v3 = (v + v2) * 0.5;
						buffer[num] = (1.0 - alpha) * vertices[num] + alpha * v3;
					}
					for (int k = iStart; k < iEnd; k++)
					{
						int num2 = k % count;
						vertices[num2] = buffer[num2];
					}
				}
				return;
			}
			for (int l = 0; l < nIterations; l++)
			{
				for (int m = iStart; m <= iEnd; m++)
				{
					if (m != 0 && m < count - 1)
					{
						Vector3d v4 = vertices[m - 1];
						Vector3d v5 = vertices[m + 1];
						Vector3d v6 = (v4 + v5) * 0.5;
						buffer[m] = (1.0 - alpha) * vertices[m] + alpha * v6;
					}
				}
				for (int n = iStart; n < iEnd; n++)
				{
					int num3 = n % count;
					vertices[num3] = buffer[num3];
				}
			}
		}
	}
}
