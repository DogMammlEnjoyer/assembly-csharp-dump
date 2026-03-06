using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal.UTess
{
	internal struct PlanarGraph
	{
		internal static void RemoveDuplicateEdges(ref NativeArray<int2> edges, ref int edgeCount, NativeArray<int> duplicates, int duplicateCount)
		{
			if (duplicateCount == 0)
			{
				for (int i = 0; i < edgeCount; i++)
				{
					int2 value = edges[i];
					value.x = math.min(edges[i].x, edges[i].y);
					value.y = math.max(edges[i].x, edges[i].y);
					edges[i] = value;
				}
			}
			else
			{
				for (int j = 0; j < edgeCount; j++)
				{
					int2 @int = edges[j];
					int x = duplicates[@int.x];
					int y = duplicates[@int.y];
					@int.x = math.min(x, y);
					@int.y = math.max(x, y);
					edges[j] = @int;
				}
			}
			ModuleHandle.InsertionSort<int2, TessEdgeCompare>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int2>(edges), 0, edgeCount - 1, default(TessEdgeCompare));
			int num = 1;
			for (int k = 1; k < edgeCount; k++)
			{
				int2 int2 = edges[k - 1];
				int2 int3 = edges[k];
				if ((int3.x != int2.x || int3.y != int2.y) && int3.x != int3.y)
				{
					edges[num++] = int3;
				}
			}
			edgeCount = num;
		}

		internal static bool CheckCollinear(double2 a0, double2 a1, double2 b0, double2 b1)
		{
			double num = (a1.y - a0.y) / (a1.x - a0.x);
			double num2 = (b0.y - a0.y) / (b0.x - a0.x);
			double num3 = (b1.y - a0.y) / (b1.x - a0.x);
			return (!math.isinf(num) || !math.isinf(num2) || !math.isinf(num3)) && math.abs(num - num2) > PlanarGraph.kEpsilon && math.abs(num - num3) > PlanarGraph.kEpsilon;
		}

		internal static bool LineLineIntersection(double2 a0, double2 a1, double2 b0, double2 b1)
		{
			double num = ModuleHandle.OrientFastDouble(a0, b0, b1);
			double num2 = ModuleHandle.OrientFastDouble(a1, b0, b1);
			if ((num > PlanarGraph.kEpsilon && num2 > PlanarGraph.kEpsilon) || (num < -PlanarGraph.kEpsilon && num2 < -PlanarGraph.kEpsilon))
			{
				return false;
			}
			double num3 = ModuleHandle.OrientFastDouble(b0, a0, a1);
			double num4 = ModuleHandle.OrientFastDouble(b1, a0, a1);
			return (num3 <= PlanarGraph.kEpsilon || num4 <= PlanarGraph.kEpsilon) && (num3 >= -PlanarGraph.kEpsilon || num4 >= -PlanarGraph.kEpsilon) && (math.abs(num) >= PlanarGraph.kEpsilon || math.abs(num2) >= PlanarGraph.kEpsilon || math.abs(num3) >= PlanarGraph.kEpsilon || math.abs(num4) >= PlanarGraph.kEpsilon || PlanarGraph.CheckCollinear(a0, a1, b0, b1));
		}

		internal static bool LineLineIntersection(double2 p1, double2 p2, double2 p3, double2 p4, ref double2 result)
		{
			double num = p2.x - p1.x;
			double num2 = p2.y - p1.y;
			double num3 = p4.x - p3.x;
			double num4 = p4.y - p3.y;
			double num5 = num * num4 - num2 * num3;
			if (math.abs(num5) < PlanarGraph.kEpsilon)
			{
				return false;
			}
			double num6 = p3.x - p1.x;
			double num7 = p3.y - p1.y;
			double num8 = (num6 * num4 - num7 * num3) / num5;
			if (num8 >= -PlanarGraph.kEpsilon && num8 <= 1.0 + PlanarGraph.kEpsilon)
			{
				result.x = p1.x + num8 * num;
				result.y = p1.y + num8 * num2;
				return true;
			}
			return false;
		}

		internal static bool CalculateEdgeIntersections(NativeArray<int2> edges, int edgeCount, NativeArray<double2> points, int pointCount, ref NativeArray<int2> results, ref NativeArray<double2> intersects, ref int resultCount)
		{
			resultCount = 0;
			for (int i = 0; i < edgeCount; i++)
			{
				for (int j = i + 1; j < edgeCount; j++)
				{
					int2 @int = edges[i];
					int2 int2 = edges[j];
					if (@int.x != int2.x && @int.x != int2.y && @int.y != int2.x && @int.y != int2.y)
					{
						double2 @double = points[@int.x];
						double2 double2 = points[@int.y];
						double2 double3 = points[int2.x];
						double2 double4 = points[int2.y];
						double2 zero = double2.zero;
						if (PlanarGraph.LineLineIntersection(@double, double2, double3, double4) && PlanarGraph.LineLineIntersection(@double, double2, double3, double4, ref zero))
						{
							if (resultCount >= intersects.Length)
							{
								return false;
							}
							intersects[resultCount] = zero;
							int num = resultCount;
							resultCount = num + 1;
							results[num] = new int2(i, j);
						}
					}
				}
			}
			if (resultCount > edgeCount * PlanarGraph.kMaxIntersectionTolerance)
			{
				return false;
			}
			IntersectionCompare comp = default(IntersectionCompare);
			comp.edges = edges;
			comp.points = points;
			ModuleHandle.InsertionSort<int2, IntersectionCompare>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int2>(results), 0, resultCount - 1, comp);
			return true;
		}

		internal static bool CalculateTJunctions(NativeArray<int2> edges, int edgeCount, NativeArray<double2> points, int pointCount, NativeArray<int2> results, ref int resultCount)
		{
			resultCount = 0;
			for (int i = 0; i < edgeCount; i++)
			{
				for (int j = 0; j < pointCount; j++)
				{
					int2 @int = edges[i];
					if (@int.x != j && @int.y != j)
					{
						double2 a = points[@int.x];
						double2 a2 = points[@int.y];
						double2 b = points[j];
						double2 b2 = points[j];
						if (PlanarGraph.LineLineIntersection(a, a2, b, b2))
						{
							if (resultCount >= results.Length)
							{
								return false;
							}
							int num = resultCount;
							resultCount = num + 1;
							results[num] = new int2(i, j);
						}
					}
				}
			}
			return true;
		}

		internal static bool CutEdges(ref NativeArray<double2> points, ref int pointCount, ref NativeArray<int2> edges, ref int edgeCount, ref NativeArray<int2> tJunctions, ref int tJunctionCount, NativeArray<int2> intersections, NativeArray<double2> intersects, int intersectionCount)
		{
			for (int i = 0; i < intersectionCount; i++)
			{
				int2 @int = intersections[i];
				int x = @int.x;
				int y = @int.y;
				int2 zero = int2.zero;
				zero.x = x;
				zero.y = pointCount;
				int num = tJunctionCount;
				tJunctionCount = num + 1;
				tJunctions[num] = zero;
				int2 zero2 = int2.zero;
				zero2.x = y;
				zero2.y = pointCount;
				num = tJunctionCount;
				tJunctionCount = num + 1;
				tJunctions[num] = zero2;
				if (pointCount >= points.Length)
				{
					return false;
				}
				num = pointCount;
				pointCount = num + 1;
				points[num] = intersects[i];
			}
			ModuleHandle.InsertionSort<int2, TessJunctionCompare>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int2>(tJunctions), 0, tJunctionCount - 1, default(TessJunctionCompare));
			for (int j = tJunctionCount - 1; j >= 0; j--)
			{
				int2 int2 = tJunctions[j];
				int x2 = int2.x;
				int2 int3 = edges[x2];
				int num2 = int3.x;
				int num3 = int3.y;
				double2 @double = points[num2];
				double2 double2 = points[num3];
				if (@double.x - double2.x < 0.0 || (@double.x == double2.x && @double.y - double2.y < 0.0))
				{
					int num4 = num2;
					num2 = num3;
					num3 = num4;
				}
				int3.x = num2;
				int x3 = int3.y = int2.y;
				edges[x2] = int3;
				int num;
				while (j > 0 && tJunctions[j - 1].x == x2)
				{
					int y2 = tJunctions[--j].y;
					int2 value = default(int2);
					value.x = x3;
					value.y = y2;
					num = edgeCount;
					edgeCount = num + 1;
					edges[num] = value;
					x3 = y2;
				}
				int2 value2 = default(int2);
				value2.x = x3;
				value2.y = num3;
				num = edgeCount;
				edgeCount = num + 1;
				edges[num] = value2;
			}
			return true;
		}

		internal static void RemoveDuplicatePoints(ref NativeArray<double2> points, ref int pointCount, ref NativeArray<int> duplicates, ref int duplicateCount, Allocator allocator)
		{
			TessLink link = TessLink.CreateLink(pointCount, allocator);
			for (int i = 0; i < pointCount; i++)
			{
				for (int j = i + 1; j < pointCount; j++)
				{
					if (math.distance(points[i], points[j]) < PlanarGraph.kEpsilon)
					{
						link.Link(i, j);
					}
				}
			}
			duplicateCount = 0;
			for (int k = 0; k < pointCount; k++)
			{
				int num = link.Find(k);
				if (num != k)
				{
					duplicateCount++;
					points[num] = math.min(points[k], points[num]);
				}
			}
			if (duplicateCount != 0)
			{
				int num2 = pointCount;
				pointCount = 0;
				for (int l = 0; l < num2; l++)
				{
					if (link.Find(l) == l)
					{
						duplicates[l] = pointCount;
						int num3 = pointCount;
						pointCount = num3 + 1;
						points[num3] = points[l];
					}
					else
					{
						duplicates[l] = -1;
					}
				}
				for (int m = 0; m < num2; m++)
				{
					if (duplicates[m] < 0)
					{
						duplicates[m] = duplicates[link.Find(m)];
					}
				}
			}
			TessLink.DestroyLink(link);
		}

		internal static bool Validate(Allocator allocator, NativeArray<float2> inputPoints, int pointCount, NativeArray<int2> inputEdges, int edgeCount, ref NativeArray<float2> outputPoints, ref int outputPointCount, ref NativeArray<int2> outputEdges, ref int outputEdgeCount)
		{
			float num = 10000f;
			int num2 = edgeCount;
			bool flag = true;
			bool flag2 = false;
			NativeArray<int> duplicates = new NativeArray<int>(ModuleHandle.kMaxEdgeCount, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int2> nativeArray = new NativeArray<int2>(ModuleHandle.kMaxEdgeCount, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int2> results = new NativeArray<int2>(ModuleHandle.kMaxEdgeCount, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int2> intersections = new NativeArray<int2>(ModuleHandle.kMaxEdgeCount, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<double2> points = new NativeArray<double2>(pointCount * 8, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<double2> intersects = new NativeArray<double2>(pointCount * 8, allocator, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < pointCount; i++)
			{
				points[i] = inputPoints[i] * num;
			}
			ModuleHandle.Copy<int2>(inputEdges, nativeArray, edgeCount);
			PlanarGraph.RemoveDuplicateEdges(ref nativeArray, ref edgeCount, duplicates, 0);
			while (flag && --num2 > 0)
			{
				int num3 = 0;
				flag2 = PlanarGraph.CalculateEdgeIntersections(nativeArray, edgeCount, points, pointCount, ref intersections, ref intersects, ref num3);
				if (!flag2)
				{
					break;
				}
				int num4 = 0;
				flag2 = PlanarGraph.CalculateTJunctions(nativeArray, edgeCount, points, pointCount, results, ref num4);
				if (!flag2)
				{
					break;
				}
				flag2 = PlanarGraph.CutEdges(ref points, ref pointCount, ref nativeArray, ref edgeCount, ref results, ref num4, intersections, intersects, num3);
				if (!flag2)
				{
					break;
				}
				int duplicateCount = 0;
				PlanarGraph.RemoveDuplicatePoints(ref points, ref pointCount, ref duplicates, ref duplicateCount, allocator);
				PlanarGraph.RemoveDuplicateEdges(ref nativeArray, ref edgeCount, duplicates, duplicateCount);
				flag = (num3 != 0 || num4 != 0);
			}
			if (flag2)
			{
				outputEdgeCount = edgeCount;
				outputPointCount = pointCount;
				ModuleHandle.Copy<int2>(nativeArray, outputEdges, edgeCount);
				for (int j = 0; j < pointCount; j++)
				{
					outputPoints[j] = new float2((float)(points[j].x / (double)num), (float)(points[j].y / (double)num));
				}
			}
			nativeArray.Dispose();
			points.Dispose();
			intersects.Dispose();
			duplicates.Dispose();
			results.Dispose();
			intersections.Dispose();
			return flag2 && num2 > 0;
		}

		private static readonly double kEpsilon = 1E-05;

		private static readonly int kMaxIntersectionTolerance = 4;
	}
}
