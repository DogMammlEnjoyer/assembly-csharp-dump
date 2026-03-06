using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class GeometryUtils
	{
		public static bool FindClosestEdge(List<Vector3> vertices, Vector3 point, out Vector3 vertexA, out Vector3 vertexB)
		{
			int count = vertices.Count;
			if (count < 1)
			{
				vertexA = Vector3.zero;
				vertexB = Vector3.zero;
				return false;
			}
			float num = float.MaxValue;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			for (int i = 0; i < count; i++)
			{
				Vector3 vector3 = vertices[i];
				Vector3 vector4 = vertices[(i + 1) % vertices.Count];
				Vector3 b = GeometryUtils.ClosestPointOnLineSegment(point, vector3, vector4);
				float num2 = Vector3.SqrMagnitude(point - b);
				if (num2 < num)
				{
					num = num2;
					vector = vector3;
					vector2 = vector4;
				}
			}
			vertexA = vector;
			vertexB = vector2;
			return true;
		}

		public static Vector3 PointOnOppositeSideOfPolygon(List<Vector3> vertices, Vector3 point)
		{
			int count = vertices.Count;
			if (count < 3)
			{
				return Vector3.zero;
			}
			Vector3 vector = vertices[0];
			Vector3 a = vertices[1];
			Vector3 a2 = vertices[2];
			Vector3 normalized = Vector3.Cross(a - vector, a2 - vector).normalized;
			Vector3 vector2 = Vector3.zero;
			foreach (Vector3 b in vertices)
			{
				vector2 += b;
			}
			vector2 *= 1f / (float)count;
			Vector3 a3 = Vector3.ProjectOnPlane(point - vector2, normalized);
			int num = count - 1;
			for (int i = 0; i < count; i++)
			{
				Vector3 vector3 = vertices[i];
				Vector3 vector4 = ((i == num) ? vector : vertices[i + 1]) - vector3;
				float num2;
				float num3;
				GeometryUtils.ClosestTimesOnTwoLines(vector3, vector4, vector2, -a3 * 100f, out num2, out num3, double.Epsilon);
				if (num3 >= 0f && num2 >= 0f && num2 <= 1f)
				{
					return vector3 + vector4 * num2;
				}
			}
			return Vector3.zero;
		}

		public static void TriangulatePolygon(List<int> indices, int vertCount, bool reverse = false)
		{
			vertCount -= 2;
			indices.EnsureCapacity(vertCount * 3);
			if (reverse)
			{
				for (int i = 0; i < vertCount; i++)
				{
					indices.Add(0);
					indices.Add(i + 2);
					indices.Add(i + 1);
				}
				return;
			}
			for (int j = 0; j < vertCount; j++)
			{
				indices.Add(0);
				indices.Add(j + 1);
				indices.Add(j + 2);
			}
		}

		public static bool ClosestTimesOnTwoLines(Vector3 positionA, Vector3 velocityA, Vector3 positionB, Vector3 velocityB, out float s, out float t, double parallelTest = 5E-324)
		{
			double num = (double)Vector3.Dot(velocityA, velocityA);
			double num2 = (double)Vector3.Dot(velocityA, velocityB);
			double num3 = (double)Vector3.Dot(velocityB, velocityB);
			double num4 = num * num3 - num2 * num2;
			if (Math.Abs(num4) < parallelTest)
			{
				s = 0f;
				t = 0f;
				return false;
			}
			Vector3 rhs = positionA - positionB;
			float num5 = Vector3.Dot(velocityA, rhs);
			float num6 = Vector3.Dot(velocityB, rhs);
			s = (float)((num2 * (double)num6 - (double)num5 * num3) / num4);
			t = (float)((num * (double)num6 - (double)num5 * num2) / num4);
			return true;
		}

		public static bool ClosestTimesOnTwoLinesXZ(Vector3 positionA, Vector3 velocityA, Vector3 positionB, Vector3 velocityB, out float s, out float t, double parallelTest = 5E-324)
		{
			double num = (double)(velocityA.x * velocityA.x + velocityA.z * velocityA.z);
			double num2 = (double)(velocityA.x * velocityB.x + velocityA.z * velocityB.z);
			double num3 = (double)(velocityB.x * velocityB.x + velocityB.z * velocityB.z);
			double num4 = num * num3 - num2 * num2;
			if (Math.Abs(num4) < parallelTest)
			{
				s = 0f;
				t = 0f;
				return false;
			}
			Vector3 vector = positionA - positionB;
			float num5 = velocityA.x * vector.x + velocityA.z * vector.z;
			float num6 = velocityB.x * vector.x + velocityB.z * vector.z;
			s = (float)((num2 * (double)num6 - (double)num5 * num3) / num4);
			t = (float)((num * (double)num6 - (double)num5 * num2) / num4);
			return true;
		}

		public static bool ClosestPointsOnTwoLineSegments(Vector3 a, Vector3 aLineVector, Vector3 b, Vector3 bLineVector, out Vector3 resultA, out Vector3 resultB, double parallelTest = 5E-324)
		{
			float num;
			float num2;
			bool flag = !GeometryUtils.ClosestTimesOnTwoLines(a, aLineVector, b, bLineVector, out num, out num2, parallelTest);
			if (num > 0f && num <= 1f && num2 > 0f && num2 <= 1f)
			{
				resultA = a + aLineVector * num;
				resultB = b + bLineVector * num2;
			}
			else
			{
				Vector3 vector = b + bLineVector;
				Vector3 vector2 = a + aLineVector;
				Vector3 vector3 = GeometryUtils.ClosestPointOnLineSegment(a, b, vector);
				Vector3 vector4 = GeometryUtils.ClosestPointOnLineSegment(vector2, b, vector);
				float num3 = Vector3.Distance(a, vector3);
				resultA = a;
				resultB = vector3;
				float num4 = Vector3.Distance(vector2, vector4);
				if (num4 < num3)
				{
					resultA = vector2;
					resultB = vector4;
					num3 = num4;
				}
				Vector3 vector5 = GeometryUtils.ClosestPointOnLineSegment(b, a, vector2);
				num4 = Vector3.Distance(b, vector5);
				if (num4 < num3)
				{
					resultA = vector5;
					resultB = b;
					num3 = num4;
				}
				Vector3 vector6 = GeometryUtils.ClosestPointOnLineSegment(vector, a, vector2);
				num4 = Vector3.Distance(vector, vector6);
				if (num4 < num3)
				{
					resultA = vector6;
					resultB = vector;
				}
				if (flag)
				{
					if (Vector3.Dot(aLineVector, bLineVector) > 0f)
					{
						num2 = Vector3.Dot(vector - a, aLineVector.normalized) * 0.5f;
						Vector3 vector7 = a + aLineVector.normalized * num2;
						Vector3 vector8 = vector + bLineVector.normalized * -num2;
						if (num2 > 0f && num2 < aLineVector.magnitude)
						{
							resultA = vector7;
							resultB = vector8;
						}
					}
					else
					{
						num2 = Vector3.Dot(vector2 - vector, aLineVector.normalized) * 0.5f;
						Vector3 vector9 = vector2 + aLineVector.normalized * -num2;
						Vector3 vector10 = vector + bLineVector.normalized * -num2;
						if (num2 > 0f && num2 < aLineVector.magnitude)
						{
							resultA = vector9;
							resultB = vector10;
						}
					}
				}
			}
			return flag;
		}

		public static Vector3 ClosestPointOnLineSegment(Vector3 point, Vector3 a, Vector3 b)
		{
			Vector3 vector = b - a;
			Vector3 normalized = vector.normalized;
			float num = Vector3.Dot(point - a, normalized);
			if (num < 0f)
			{
				return a;
			}
			if (num * num > vector.sqrMagnitude)
			{
				return b;
			}
			return a + num * normalized;
		}

		public static void ClosestPolygonApproach(List<Vector3> verticesA, List<Vector3> verticesB, out Vector3 pointA, out Vector3 pointB, float parallelTest = 0f)
		{
			pointA = default(Vector3);
			pointB = default(Vector3);
			float num = float.MaxValue;
			int count = verticesA.Count;
			int count2 = verticesB.Count;
			int num2 = count - 1;
			int num3 = count2 - 1;
			Vector3 vector = verticesA[0];
			Vector3 vector2 = verticesB[0];
			for (int i = 0; i < count; i++)
			{
				Vector3 vector3 = verticesA[i];
				Vector3 aLineVector = ((i == num2) ? vector : verticesA[i + 1]) - vector3;
				for (int j = 0; j < count2; j++)
				{
					Vector3 b = verticesB[j];
					Vector3 bLineVector = ((j == num3) ? vector2 : verticesB[j + 1]) - b;
					Vector3 vector4;
					Vector3 vector5;
					bool flag = GeometryUtils.ClosestPointsOnTwoLineSegments(vector3, aLineVector, b, bLineVector, out vector4, out vector5, (double)parallelTest);
					float num4 = Vector3.Distance(vector4, vector5);
					if (flag)
					{
						if (num4 - num < parallelTest)
						{
							num = num4 - parallelTest;
							pointA = vector4;
							pointB = vector5;
						}
					}
					else if (num4 < num)
					{
						num = num4;
						pointA = vector4;
						pointB = vector5;
					}
				}
			}
		}

		public static bool PointInPolygon(Vector3 testPoint, List<Vector3> vertices)
		{
			if (vertices.Count < 3)
			{
				return false;
			}
			int num = 0;
			int i = 0;
			Vector3 vector = vertices[vertices.Count - 1];
			vector.x -= testPoint.x;
			vector.z -= testPoint.z;
			bool flag = false;
			if (!MathUtility.ApproximatelyZero(vector.z))
			{
				flag = (vector.z < 0f);
			}
			else
			{
				for (int j = vertices.Count - 2; j >= 0; j--)
				{
					float num2 = vertices[j].z;
					num2 -= testPoint.z;
					if (!MathUtility.ApproximatelyZero(num2))
					{
						flag = (num2 < 0f);
						break;
					}
				}
			}
			while (i < vertices.Count)
			{
				Vector3 vector2 = vertices[i];
				vector2.x -= testPoint.x;
				vector2.z -= testPoint.z;
				Vector3 vector3 = vector2 - vector;
				float sqrMagnitude = vector3.sqrMagnitude;
				if (MathUtility.ApproximatelyZero(vector3.x * vector2.z - vector3.z * vector2.x) && vector.sqrMagnitude <= sqrMagnitude && vector2.sqrMagnitude <= sqrMagnitude)
				{
					return true;
				}
				if (!MathUtility.ApproximatelyZero(vector2.z))
				{
					bool flag2 = vector2.z < 0f;
					if (flag2 != flag)
					{
						flag = flag2;
						if ((vector.x * vector2.z - vector.z * vector2.x) / -(vector.z - vector2.z) > 0f)
						{
							num++;
						}
					}
				}
				vector = vector2;
				i++;
			}
			return num % 2 > 0;
		}

		public static bool PointInPolygon3D(Vector3 testPoint, List<Vector3> vertices)
		{
			if (vertices.Count < 3)
			{
				return false;
			}
			double num = 0.0;
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector3 lhs = vertices[i] - testPoint;
				Vector3 rhs = vertices[(i + 1) % vertices.Count] - testPoint;
				float num2 = lhs.sqrMagnitude * rhs.sqrMagnitude;
				if (num2 <= MathUtility.EpsilonScaled)
				{
					return true;
				}
				double num3 = Math.Acos((double)(Vector3.Dot(lhs, rhs) / Mathf.Sqrt(num2)));
				num += num3;
			}
			return Mathf.Abs((float)num - 6.2831855f) < 0.01f;
		}

		public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			float d = -Vector3.Dot(planeNormal.normalized, point - planePoint);
			return point + planeNormal.normalized * d;
		}

		public static bool ConvexHull2D(List<Vector3> points, List<Vector3> hull)
		{
			if (points.Count < 3)
			{
				return false;
			}
			GeometryUtils.k_HullIndices.Clear();
			int count = points.Count;
			int num = 0;
			for (int i = 1; i < count; i++)
			{
				Vector3 vector = points[i];
				float x = vector.x;
				float z = vector.z;
				Vector3 vector2 = points[num];
				float x2 = vector2.x;
				float z2 = vector2.z;
				if (x < x2 || (MathUtility.Approximately(x, x2) && z < z2))
				{
					num = i;
				}
			}
			int num2 = num;
			do
			{
				Vector3 vector3 = points[num2];
				hull.Add(vector3);
				GeometryUtils.k_HullIndices.Add(num2);
				int num3 = 0;
				Vector3 vector4 = points[num3];
				for (int j = 1; j < count; j++)
				{
					if (j != num2 && (!GeometryUtils.k_HullIndices.Contains(j) || j == num))
					{
						Vector3 vector5 = points[j];
						Vector3 vector6 = vector4 - vector3;
						Vector3 vector7 = vector5 - vector3;
						float num4 = vector6.z * vector7.x - vector6.x * vector7.z;
						bool flag = num4 < 0f;
						if ((flag ? (-num4) : num4) < MathUtility.EpsilonScaled)
						{
							if (Vector3.SqrMagnitude(vector3 - vector4) < Vector3.SqrMagnitude(vector3 - vector5))
							{
								num3 = j;
								vector4 = points[num3];
							}
						}
						else if (flag)
						{
							num3 = j;
							vector4 = points[num3];
						}
					}
				}
				num2 = num3;
			}
			while (num2 != num);
			return true;
		}

		public static Vector3 PolygonCentroid2D(List<Vector3> vertices)
		{
			int count = vertices.Count;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			int i;
			double num4;
			double num5;
			double num6;
			double num7;
			double num8;
			for (i = 0; i < count - 1; i++)
			{
				Vector3 vector = vertices[i];
				num4 = (double)vector.x;
				num5 = (double)vector.z;
				Vector3 vector2 = vertices[i + 1];
				num6 = (double)vector2.x;
				num7 = (double)vector2.z;
				num8 = num4 * num7 - num6 * num5;
				num += num8;
				num2 += (num4 + num6) * num8;
				num3 += (num5 + num7) * num8;
			}
			Vector3 vector3 = vertices[i];
			num4 = (double)vector3.x;
			num5 = (double)vector3.z;
			Vector3 vector4 = vertices[0];
			num6 = (double)vector4.x;
			num7 = (double)vector4.z;
			num8 = num4 * num7 - num6 * num5;
			num += num8;
			num2 += (num4 + num6) * num8;
			num3 += (num5 + num7) * num8;
			num *= 0.5;
			double num9 = 6.0 * num;
			num2 /= num9;
			num3 /= num9;
			return new Vector3((float)num2, 0f, (float)num3);
		}

		public static Vector2 OrientedMinimumBoundingBox2D(List<Vector3> convexHull, Vector3[] boundingBox)
		{
			Vector3 vector = new Vector3(0f, 0f, 1f);
			Vector3 vector2 = new Vector3(0f, 0f, -1f);
			Vector3 vector3 = new Vector3(1f, 0f, 0f);
			Vector3 vector4 = new Vector3(-1f, 0f, 0f);
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			int index = 0;
			int index2 = 0;
			int index3 = 0;
			int index4 = 0;
			int count = convexHull.Count;
			for (int i = 0; i < count; i++)
			{
				Vector3 vector5 = convexHull[i];
				float x = vector5.x;
				if (x < num)
				{
					num = x;
					index = i;
				}
				if (x > num3)
				{
					num3 = x;
					index2 = i;
				}
				float z = vector5.z;
				if (z < num2)
				{
					num2 = z;
					index4 = i;
				}
				if (z > num4)
				{
					num4 = z;
					index3 = i;
				}
			}
			GeometryUtils.k_HullEdgeDirections.Clear();
			int num5 = count - 1;
			for (int j = 0; j < num5; j++)
			{
				Vector3 item = convexHull[j + 1] - convexHull[j];
				item.Normalize();
				GeometryUtils.k_HullEdgeDirections.Add(item);
			}
			Vector3 item2 = convexHull[0] - convexHull[num5];
			item2.Normalize();
			GeometryUtils.k_HullEdgeDirections.Add(item2);
			double num6 = double.MaxValue;
			for (int k = 0; k < count; k++)
			{
				Vector3 vector6 = GeometryUtils.k_HullEdgeDirections[index];
				Vector3 vector7 = GeometryUtils.k_HullEdgeDirections[index2];
				Vector3 vector8 = GeometryUtils.k_HullEdgeDirections[index3];
				Vector3 vector9 = GeometryUtils.k_HullEdgeDirections[index4];
				double num7 = Math.Acos((double)(vector.x * vector6.x + vector.z * vector6.z));
				double num8 = Math.Acos((double)(vector2.x * vector7.x + vector2.z * vector7.z));
				double num9 = Math.Acos((double)(vector3.x * vector8.x + vector3.z * vector8.z));
				double num10 = Math.Acos((double)(vector4.x * vector9.x + vector4.z * vector9.z));
				int num11 = 0;
				double num12 = num7;
				if (num8 < num12)
				{
					num12 = num8;
					num11 = 1;
				}
				if (num9 < num12)
				{
					num12 = num9;
					num11 = 2;
				}
				if (num10 < num12)
				{
					num11 = 3;
				}
				Vector3 vector10;
				Vector3 vector11;
				Vector3 vector12;
				Vector3 vector13;
				switch (num11)
				{
				case 0:
					GeometryUtils.RotateCalipers(vector6, convexHull, ref index, out index3, out index2, out index4, out vector, out vector3, out vector2, out vector4, out vector10, out vector11, out vector12, out vector13);
					break;
				case 1:
					GeometryUtils.RotateCalipers(vector7, convexHull, ref index2, out index4, out index, out index3, out vector2, out vector4, out vector, out vector3, out vector12, out vector13, out vector10, out vector11);
					break;
				case 2:
					GeometryUtils.RotateCalipers(vector8, convexHull, ref index3, out index2, out index4, out index, out vector3, out vector2, out vector4, out vector, out vector11, out vector12, out vector13, out vector10);
					break;
				default:
					GeometryUtils.RotateCalipers(vector9, convexHull, ref index4, out index, out index3, out index2, out vector4, out vector, out vector3, out vector2, out vector13, out vector10, out vector11, out vector12);
					break;
				}
				float sqrMagnitude = (vector10 - vector11).sqrMagnitude;
				float sqrMagnitude2 = (vector10 - vector13).sqrMagnitude;
				float num13 = sqrMagnitude * sqrMagnitude2;
				if ((double)num13 < num6)
				{
					num6 = (double)num13;
					boundingBox[0] = vector13;
					boundingBox[1] = vector12;
					boundingBox[2] = vector11;
					boundingBox[3] = vector10;
				}
			}
			Vector3 a = boundingBox[0];
			float x2 = Vector3.Distance(a, boundingBox[3]);
			float y = Vector3.Distance(a, boundingBox[1]);
			return new Vector2(x2, y);
		}

		private static void RotateCalipers(Vector3 alignEdge, List<Vector3> vertices, ref int indexA, out int indexB, out int indexC, out int indexD, out Vector3 caliperA, out Vector3 caliperB, out Vector3 caliperC, out Vector3 caliperD, out Vector3 caliperAEndCorner, out Vector3 caliperBEndCorner, out Vector3 caliperCEndCorner, out Vector3 caliperDEndCorner)
		{
			int count = vertices.Count;
			caliperA = alignEdge;
			caliperB = new Vector3(caliperA.z, 0f, -caliperA.x);
			caliperC = -caliperA;
			caliperD = -caliperB;
			indexA = (indexA + 1) % count;
			Vector3 vector = vertices[indexA];
			indexB = indexA;
			float num = 0f;
			for (;;)
			{
				int num2 = (indexB + 1) % count;
				float num3;
				float num4;
				GeometryUtils.ClosestTimesOnTwoLinesXZ(vector, caliperA, vertices[num2], caliperD, out num3, out num4, double.Epsilon);
				if (num3 <= num)
				{
					break;
				}
				num = num3;
				indexB = num2;
			}
			caliperAEndCorner = vector + caliperA * num;
			Vector3 vector2 = vertices[indexB];
			indexC = indexB;
			num = 0f;
			for (;;)
			{
				int num5 = (indexC + 1) % count;
				float num4;
				float num6;
				GeometryUtils.ClosestTimesOnTwoLinesXZ(vector2, caliperB, vertices[num5], caliperA, out num6, out num4, double.Epsilon);
				if (num6 <= num)
				{
					break;
				}
				num = num6;
				indexC = num5;
			}
			caliperBEndCorner = vector2 + caliperB * num;
			Vector3 vector3 = vertices[indexC];
			indexD = indexC;
			num = 0f;
			for (;;)
			{
				int num7 = (indexD + 1) % count;
				float num4;
				float num8;
				GeometryUtils.ClosestTimesOnTwoLinesXZ(vector3, caliperC, vertices[num7], caliperB, out num8, out num4, double.Epsilon);
				if (num8 <= num)
				{
					break;
				}
				num = num8;
				indexD = num7;
			}
			caliperCEndCorner = vector3 + caliperC * num;
			caliperDEndCorner = caliperCEndCorner + caliperAEndCorner - caliperBEndCorner;
		}

		public static Quaternion RotationForBox(Vector3[] vertices)
		{
			Vector3 b = vertices[0];
			Vector3 toDirection = vertices[3] - b;
			return Quaternion.FromToRotation(Vector3.right, toDirection);
		}

		public static float ConvexPolygonArea(List<Vector3> vertices)
		{
			int count = vertices.Count;
			if (count < 3)
			{
				return 0f;
			}
			Vector3 vector = vertices[0];
			int num = count - 1;
			Vector3 vector2 = vertices[num];
			float num2 = vector2.x * vector.z - vector.x * vector2.z;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector3 = vertices[i];
				Vector3 vector4 = vertices[i + 1];
				num2 += vector3.x * vector4.z - vector4.x * vector3.z;
			}
			return Math.Abs(num2 * 0.5f);
		}

		public static bool PolygonInPolygon(List<Vector3> polygonA, List<Vector3> polygonB)
		{
			if (polygonA.Count < 1)
			{
				return false;
			}
			using (List<Vector3>.Enumerator enumerator = polygonA.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!GeometryUtils.PointInPolygon3D(enumerator.Current, polygonB))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool PolygonsWithinRange(List<Vector3> polygonA, List<Vector3> polygonB, float maxDistance)
		{
			return GeometryUtils.PolygonsWithinSqRange(polygonA, polygonB, maxDistance * maxDistance);
		}

		public static bool PolygonsWithinSqRange(List<Vector3> polygonA, List<Vector3> polygonB, float maxSqDistance)
		{
			Vector3 b;
			Vector3 a;
			GeometryUtils.ClosestPolygonApproach(polygonA, polygonB, out b, out a, 0f);
			return Vector3.SqrMagnitude(a - b) <= maxSqDistance || GeometryUtils.PolygonInPolygon(polygonA, polygonB) || GeometryUtils.PolygonInPolygon(polygonB, polygonA);
		}

		public static bool PointOnPolygonBoundsXZ(Vector3 testPoint, List<Vector3> vertices, float epsilon = 1E-45f)
		{
			int count = vertices.Count;
			if (count < 2)
			{
				return false;
			}
			Vector3 lineStart = vertices[count - 1];
			foreach (Vector3 vector in vertices)
			{
				if (GeometryUtils.PointOnLineSegmentXZ(testPoint, lineStart, vector, epsilon))
				{
					return true;
				}
				lineStart = vector;
			}
			return false;
		}

		public static bool PointOnLineSegmentXZ(Vector3 testPoint, Vector3 lineStart, Vector3 lineEnd, float epsilon = 1E-45f)
		{
			Vector3 vector = lineEnd - lineStart;
			Vector3 vector2 = testPoint - lineStart;
			float num = vector.z * vector2.x - vector.x * vector2.z;
			if (((num >= 0f) ? num : (-num)) >= epsilon)
			{
				return false;
			}
			float num2 = vector.x * vector2.x + vector.z * vector2.z;
			float num3 = vector.x * vector.x + vector.z * vector.z;
			return num2 >= -epsilon && num2 <= num3 + epsilon;
		}

		private static Quaternion NormalizeRotationKeepingUp(Quaternion rot)
		{
			Vector3 normalized = (rot * GeometryUtils.k_Up).normalized;
			Vector3 forward;
			if (Mathf.Abs(normalized.y) > 0.95f)
			{
				forward = Vector3.Cross(GeometryUtils.k_Forward, normalized);
			}
			else
			{
				Vector3 rhs = Vector3.Cross(normalized, GeometryUtils.k_Up);
				forward = Vector3.Cross(normalized, rhs);
			}
			return Quaternion.LookRotation(forward, normalized);
		}

		public static Pose PolygonUVPoseFromPlanePose(Pose pose)
		{
			return new Pose(GeometryUtils.k_Zero, GeometryUtils.NormalizeRotationKeepingUp(pose.rotation));
		}

		public static Vector2 PolygonVertexToUV(Vector3 vertexPos, Pose planePose, Pose uvPose)
		{
			Vector3 a = planePose.position + planePose.rotation * vertexPos;
			Vector3 vector = Quaternion.Inverse(uvPose.rotation) * (a - uvPose.position);
			vector = GeometryUtils.k_VerticalCorrection * vector;
			return new Vector2(vector.x, vector.z);
		}

		private const float k_TwoPi = 6.2831855f;

		private static readonly Vector3 k_Up = Vector3.up;

		private static readonly Vector3 k_Forward = Vector3.forward;

		private static readonly Vector3 k_Zero = Vector3.zero;

		private static readonly Quaternion k_VerticalCorrection = Quaternion.AngleAxis(180f, GeometryUtils.k_Up);

		private const float k_MostlyVertical = 0.95f;

		private static readonly List<Vector3> k_HullEdgeDirections = new List<Vector3>();

		private static readonly HashSet<int> k_HullIndices = new HashSet<int>();
	}
}
