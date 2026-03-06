using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal.UTess
{
	internal struct ModuleHandle
	{
		internal static void Copy<T>(NativeArray<T> src, int srcIndex, NativeArray<T> dst, int dstIndex, int length) where T : struct
		{
			NativeArray<T>.Copy(src, srcIndex, dst, dstIndex, length);
		}

		internal static void Copy<T>(NativeArray<T> src, NativeArray<T> dst, int length) where T : struct
		{
			ModuleHandle.Copy<T>(src, 0, dst, 0, length);
		}

		internal unsafe static void InsertionSort<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
		{
			for (int i = lo; i < hi; i++)
			{
				int num = i;
				T t = UnsafeUtility.ReadArrayElement<T>(array, i + 1);
				while (num >= lo && comp.Compare(t, UnsafeUtility.ReadArrayElement<T>(array, num)) < 0)
				{
					UnsafeUtility.WriteArrayElement<T>(array, num + 1, UnsafeUtility.ReadArrayElement<T>(array, num));
					num--;
				}
				UnsafeUtility.WriteArrayElement<T>(array, num + 1, t);
			}
		}

		internal static int GetLower<T, U, X>(NativeArray<T> values, int count, U check, X condition) where T : struct where U : struct where X : ICondition2<T, U>
		{
			int i = 0;
			int num = count - 1;
			int result = i - 1;
			while (i <= num)
			{
				int num2 = i + num >> 1;
				float num3 = 0f;
				if (condition.Test(values[num2], check, ref num3))
				{
					result = num2;
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return result;
		}

		internal static int GetUpper<T, U, X>(NativeArray<T> values, int count, U check, X condition) where T : struct where U : struct where X : ICondition2<T, U>
		{
			int i = 0;
			int num = count - 1;
			int result = num + 1;
			while (i <= num)
			{
				int num2 = i + num >> 1;
				float num3 = 0f;
				if (condition.Test(values[num2], check, ref num3))
				{
					result = num2;
					num = num2 - 1;
				}
				else
				{
					i = num2 + 1;
				}
			}
			return result;
		}

		internal static int GetEqual<T, U, X>(NativeArray<T> values, int count, U check, X condition) where T : struct where U : struct where X : ICondition2<T, U>
		{
			int i = 0;
			int num = count - 1;
			while (i <= num)
			{
				int num2 = i + num >> 1;
				float num3 = 0f;
				condition.Test(values[num2], check, ref num3);
				if (num3 == 0f)
				{
					return num2;
				}
				if (num3 <= 0f)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return -1;
		}

		internal static float OrientFast(float2 a, float2 b, float2 c)
		{
			float num = 1.110223E-16f;
			float num2 = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
			if (math.abs(num2) < num)
			{
				return 0f;
			}
			return num2;
		}

		internal static double OrientFastDouble(double2 a, double2 b, double2 c)
		{
			double num = 1.1102230246251565E-16;
			double num2 = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
			if (math.abs(num2) < num)
			{
				return 0.0;
			}
			return num2;
		}

		internal static UCircle CircumCircle(UTriangle tri)
		{
			float num = tri.va.x * tri.va.x;
			float num2 = tri.vb.x * tri.vb.x;
			float num3 = tri.vc.x * tri.vc.x;
			float num4 = tri.va.y * tri.va.y;
			float num5 = tri.vb.y * tri.vb.y;
			float num6 = tri.vc.y * tri.vc.y;
			float num7 = 2f * ((tri.vb.x - tri.va.x) * (tri.vc.y - tri.va.y) - (tri.vb.y - tri.va.y) * (tri.vc.x - tri.va.x));
			float num8 = ((tri.vc.y - tri.va.y) * (num2 - num + num5 - num4) + (tri.va.y - tri.vb.y) * (num3 - num + num6 - num4)) / num7;
			float num9 = ((tri.va.x - tri.vc.x) * (num2 - num + num5 - num4) + (tri.vb.x - tri.va.x) * (num3 - num + num6 - num4)) / num7;
			float num10 = tri.va.x - num8;
			float num11 = tri.va.y - num9;
			return new UCircle
			{
				center = new float2(num8, num9),
				radius = math.sqrt(num10 * num10 + num11 * num11)
			};
		}

		internal static bool IsInsideCircle(UCircle c, float2 v)
		{
			return math.distance(v, c.center) < c.radius;
		}

		internal static float TriangleArea(float2 va, float2 vb, float2 vc)
		{
			float3 lhs = new float3(va.x, va.y, 0f);
			float3 rhs = new float3(vb.x, vb.y, 0f);
			float3 rhs2 = new float3(vc.x, vc.y, 0f);
			return math.abs(math.cross(lhs - rhs, lhs - rhs2).z) * 0.5f;
		}

		internal static float Sign(float2 p1, float2 p2, float2 p3)
		{
			return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
		}

		internal static bool IsInsideTriangle(float2 pt, float2 v1, float2 v2, float2 v3)
		{
			float num = ModuleHandle.Sign(pt, v1, v2);
			float num2 = ModuleHandle.Sign(pt, v2, v3);
			float num3 = ModuleHandle.Sign(pt, v3, v1);
			bool flag = num < 0f || num2 < 0f || num3 < 0f;
			bool flag2 = num > 0f || num2 > 0f || num3 > 0f;
			return !flag || !flag2;
		}

		internal static bool IsInsideTriangleApproximate(float2 pt, float2 v1, float2 v2, float2 v3)
		{
			float num = ModuleHandle.TriangleArea(v1, v2, v3);
			float num2 = ModuleHandle.TriangleArea(pt, v1, v2);
			float num3 = ModuleHandle.TriangleArea(pt, v2, v3);
			float num4 = ModuleHandle.TriangleArea(pt, v3, v1);
			float num5 = 1.110223E-16f;
			return Mathf.Abs(num - (num2 + num3 + num4)) < num5;
		}

		internal static bool IsInsideCircle(float2 a, float2 b, float2 c, float2 p)
		{
			float num = math.dot(a, a);
			float num2 = math.dot(b, b);
			float num3 = math.dot(c, c);
			float x = a.x;
			float y = a.y;
			float x2 = b.x;
			float y2 = b.y;
			float x3 = c.x;
			float y3 = c.y;
			float num4 = (num * (y3 - y2) + num2 * (y - y3) + num3 * (y2 - y)) / (x * (y3 - y2) + x2 * (y - y3) + x3 * (y2 - y));
			float num5 = (num * (x3 - x2) + num2 * (x - x3) + num3 * (x2 - x)) / (y * (x3 - x2) + y2 * (x - x3) + y3 * (x2 - x));
			float2 y4 = new float2
			{
				x = num4 / 2f,
				y = num5 / 2f
			};
			float num6 = math.distance(a, y4);
			float num7 = math.distance(p, y4);
			return num6 - num7 > 1E-05f;
		}

		internal static void BuildTriangles(NativeArray<float2> vertices, int vertexCount, NativeArray<int> indices, int indexCount, ref NativeArray<UTriangle> triangles, ref int triangleCount, ref float maxArea, ref float avgArea, ref float minArea)
		{
			for (int i = 0; i < indexCount; i += 3)
			{
				UTriangle utriangle = default(UTriangle);
				int index = indices[i];
				int index2 = indices[i + 1];
				int index3 = indices[i + 2];
				utriangle.va = vertices[index];
				utriangle.vb = vertices[index2];
				utriangle.vc = vertices[index3];
				utriangle.c = ModuleHandle.CircumCircle(utriangle);
				utriangle.area = ModuleHandle.TriangleArea(utriangle.va, utriangle.vb, utriangle.vc);
				maxArea = math.max(utriangle.area, maxArea);
				minArea = math.min(utriangle.area, minArea);
				avgArea += utriangle.area;
				int num = triangleCount;
				triangleCount = num + 1;
				triangles[num] = utriangle;
			}
			avgArea /= (float)triangleCount;
		}

		internal static void BuildTriangles(NativeArray<float2> vertices, int vertexCount, NativeArray<int> indices, int indexCount, ref NativeArray<UTriangle> triangles, ref int triangleCount, ref float maxArea, ref float avgArea, ref float minArea, ref float maxEdge, ref float avgEdge, ref float minEdge)
		{
			for (int i = 0; i < indexCount; i += 3)
			{
				UTriangle utriangle = default(UTriangle);
				int index = indices[i];
				int index2 = indices[i + 1];
				int index3 = indices[i + 2];
				utriangle.va = vertices[index];
				utriangle.vb = vertices[index2];
				utriangle.vc = vertices[index3];
				utriangle.c = ModuleHandle.CircumCircle(utriangle);
				utriangle.area = ModuleHandle.TriangleArea(utriangle.va, utriangle.vb, utriangle.vc);
				maxArea = math.max(utriangle.area, maxArea);
				minArea = math.min(utriangle.area, minArea);
				avgArea += utriangle.area;
				float num = math.distance(utriangle.va, utriangle.vb);
				float num2 = math.distance(utriangle.vb, utriangle.vc);
				float num3 = math.distance(utriangle.vc, utriangle.va);
				maxEdge = math.max(num, maxEdge);
				maxEdge = math.max(num2, maxEdge);
				maxEdge = math.max(num3, maxEdge);
				minEdge = math.min(num, minEdge);
				minEdge = math.min(num2, minEdge);
				minEdge = math.min(num3, minEdge);
				avgEdge += num;
				avgEdge += num2;
				avgEdge += num3;
				int num4 = triangleCount;
				triangleCount = num4 + 1;
				triangles[num4] = utriangle;
			}
			avgArea /= (float)triangleCount;
			avgEdge /= (float)indexCount;
		}

		internal static void BuildTrianglesAndEdges(NativeArray<float2> vertices, int vertexCount, NativeArray<int> indices, int indexCount, ref NativeArray<UTriangle> triangles, ref int triangleCount, ref NativeArray<int4> delaEdges, ref int delaEdgeCount, ref float maxArea, ref float avgArea, ref float minArea)
		{
			for (int i = 0; i < indexCount; i += 3)
			{
				UTriangle utriangle = default(UTriangle);
				int num = indices[i];
				int num2 = indices[i + 1];
				int num3 = indices[i + 2];
				utriangle.va = vertices[num];
				utriangle.vb = vertices[num2];
				utriangle.vc = vertices[num3];
				utriangle.c = ModuleHandle.CircumCircle(utriangle);
				utriangle.area = ModuleHandle.TriangleArea(utriangle.va, utriangle.vb, utriangle.vc);
				maxArea = math.max(utriangle.area, maxArea);
				minArea = math.min(utriangle.area, minArea);
				avgArea += utriangle.area;
				utriangle.indices = new int3(num, num2, num3);
				int num4 = delaEdgeCount;
				delaEdgeCount = num4 + 1;
				delaEdges[num4] = new int4(math.min(num, num2), math.max(num, num2), triangleCount, -1);
				num4 = delaEdgeCount;
				delaEdgeCount = num4 + 1;
				delaEdges[num4] = new int4(math.min(num2, num3), math.max(num2, num3), triangleCount, -1);
				num4 = delaEdgeCount;
				delaEdgeCount = num4 + 1;
				delaEdges[num4] = new int4(math.min(num3, num), math.max(num3, num), triangleCount, -1);
				num4 = triangleCount;
				triangleCount = num4 + 1;
				triangles[num4] = utriangle;
			}
			avgArea /= (float)triangleCount;
		}

		private static void CopyGraph(NativeArray<float2> srcPoints, int srcPointCount, ref NativeArray<float2> dstPoints, ref int dstPointCount, NativeArray<int2> srcEdges, int srcEdgeCount, ref NativeArray<int2> dstEdges, ref int dstEdgeCount)
		{
			dstEdgeCount = srcEdgeCount;
			dstPointCount = srcPointCount;
			ModuleHandle.Copy<int2>(srcEdges, dstEdges, srcEdgeCount);
			ModuleHandle.Copy<float2>(srcPoints, dstPoints, srcPointCount);
		}

		private static void CopyGeometry(NativeArray<int> srcIndices, int srcIndexCount, ref NativeArray<int> dstIndices, ref int dstIndexCount, NativeArray<float2> srcVertices, int srcVertexCount, ref NativeArray<float2> dstVertices, ref int dstVertexCount)
		{
			dstIndexCount = srcIndexCount;
			dstVertexCount = srcVertexCount;
			ModuleHandle.Copy<int>(srcIndices, dstIndices, srcIndexCount);
			ModuleHandle.Copy<float2>(srcVertices, dstVertices, srcVertexCount);
		}

		private static void TransferOutput(NativeArray<int2> srcEdges, int srcEdgeCount, ref NativeArray<int2> dstEdges, ref int dstEdgeCount, NativeArray<int> srcIndices, int srcIndexCount, ref NativeArray<int> dstIndices, ref int dstIndexCount, NativeArray<float2> srcVertices, int srcVertexCount, ref NativeArray<float2> dstVertices, ref int dstVertexCount)
		{
			dstEdgeCount = srcEdgeCount;
			dstIndexCount = srcIndexCount;
			dstVertexCount = srcVertexCount;
			ModuleHandle.Copy<int2>(srcEdges, dstEdges, srcEdgeCount);
			ModuleHandle.Copy<int>(srcIndices, dstIndices, srcIndexCount);
			ModuleHandle.Copy<float2>(srcVertices, dstVertices, srcVertexCount);
		}

		private static void GraphConditioner(NativeArray<float2> points, ref NativeArray<float2> pgPoints, ref int pgPointCount, ref NativeArray<int2> pgEdges, ref int pgEdgeCount, bool resetTopology)
		{
			float2 @float = new float2(float.PositiveInfinity, float.PositiveInfinity);
			float2 float2 = float2.zero;
			for (int i = 0; i < points.Length; i++)
			{
				@float = math.min(points[i], @float);
				float2 = math.max(points[i], float2);
			}
			float2 float3 = (float2 - @float) * 0.5f;
			float num = 0.0001f;
			pgPointCount = (resetTopology ? 0 : pgPointCount);
			int num2 = pgPointCount;
			int num3 = pgPointCount;
			pgPointCount = num3 + 1;
			pgPoints[num3] = new float2(@float.x, @float.y);
			num3 = pgPointCount;
			pgPointCount = num3 + 1;
			pgPoints[num3] = new float2(@float.x - num, @float.y + float3.y);
			num3 = pgPointCount;
			pgPointCount = num3 + 1;
			pgPoints[num3] = new float2(@float.x, float2.y);
			num3 = pgPointCount;
			pgPointCount = num3 + 1;
			pgPoints[num3] = new float2(@float.x + float3.x, float2.y + num);
			num3 = pgPointCount;
			pgPointCount = num3 + 1;
			pgPoints[num3] = new float2(float2.x, float2.y);
			num3 = pgPointCount;
			pgPointCount = num3 + 1;
			pgPoints[num3] = new float2(float2.x + num, @float.y + float3.y);
			num3 = pgPointCount;
			pgPointCount = num3 + 1;
			pgPoints[num3] = new float2(float2.x, @float.y);
			num3 = pgPointCount;
			pgPointCount = num3 + 1;
			pgPoints[num3] = new float2(@float.x + float3.x, @float.y - num);
			pgEdgeCount = 8;
			pgEdges[0] = new int2(num2, num2 + 1);
			pgEdges[1] = new int2(num2 + 1, num2 + 2);
			pgEdges[2] = new int2(num2 + 2, num2 + 3);
			pgEdges[3] = new int2(num2 + 3, num2 + 4);
			pgEdges[4] = new int2(num2 + 4, num2 + 5);
			pgEdges[5] = new int2(num2 + 5, num2 + 6);
			pgEdges[6] = new int2(num2 + 6, num2 + 7);
			pgEdges[7] = new int2(num2 + 7, num2);
		}

		private static void Reorder(int startVertexCount, int index, ref NativeArray<int> indices, ref int indexCount, ref NativeArray<float2> vertices, ref int vertexCount)
		{
			bool flag = false;
			for (int i = 0; i < indexCount; i++)
			{
				if (indices[i] == index)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				vertexCount--;
				vertices[index] = vertices[vertexCount];
				for (int j = 0; j < indexCount; j++)
				{
					if (indices[j] == vertexCount)
					{
						indices[j] = index;
					}
				}
			}
		}

		internal static void VertexCleanupConditioner(int startVertexCount, ref NativeArray<int> indices, ref int indexCount, ref NativeArray<float2> vertices, ref int vertexCount)
		{
			for (int i = startVertexCount; i < vertexCount; i++)
			{
				ModuleHandle.Reorder(startVertexCount, i, ref indices, ref indexCount, ref vertices, ref vertexCount);
			}
		}

		public static float4 ConvexQuad(Allocator allocator, NativeArray<float2> points, NativeArray<int2> edges, ref NativeArray<float2> outVertices, ref int outVertexCount, ref NativeArray<int> outIndices, ref int outIndexCount, ref NativeArray<int2> outEdges, ref int outEdgeCount)
		{
			float4 zero = float4.zero;
			outEdgeCount = 0;
			outIndexCount = 0;
			outVertexCount = 0;
			if (points.Length < 3 || points.Length >= ModuleHandle.kMaxVertexCount)
			{
				return zero;
			}
			int pgEdgeCount = 0;
			int pgPointCount = 0;
			NativeArray<int2> pgEdges = new NativeArray<int2>(ModuleHandle.kMaxEdgeCount, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<float2> pgPoints = new NativeArray<float2>(ModuleHandle.kMaxVertexCount, allocator, NativeArrayOptions.ClearMemory);
			ModuleHandle.GraphConditioner(points, ref pgPoints, ref pgPointCount, ref pgEdges, ref pgEdgeCount, true);
			Tessellator.Tessellate(allocator, pgPoints, pgPointCount, pgEdges, pgEdgeCount, ref outVertices, ref outVertexCount, ref outIndices, ref outIndexCount);
			pgPoints.Dispose();
			pgEdges.Dispose();
			return zero;
		}

		public static float4 Tessellate(Allocator allocator, NativeArray<float2> points, NativeArray<int2> edges, ref NativeArray<float2> outVertices, ref int outVertexCount, ref NativeArray<int> outIndices, ref int outIndexCount, ref NativeArray<int2> outEdges, ref int outEdgeCount)
		{
			float4 zero = float4.zero;
			outEdgeCount = 0;
			outIndexCount = 0;
			outVertexCount = 0;
			if (points.Length < 3 || points.Length >= ModuleHandle.kMaxVertexCount)
			{
				return zero;
			}
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			int num2 = 0;
			NativeArray<int2> nativeArray = new NativeArray<int2>(edges.Length * 8, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<float2> pgPoints = new NativeArray<float2>(points.Length * 4, allocator, NativeArrayOptions.ClearMemory);
			if (edges.Length != 0)
			{
				flag = PlanarGraph.Validate(allocator, points, points.Length, edges, edges.Length, ref pgPoints, ref num2, ref nativeArray, ref num);
			}
			if (!flag)
			{
				outEdgeCount = edges.Length;
				outVertexCount = points.Length;
				ModuleHandle.Copy<int2>(edges, outEdges, edges.Length);
				ModuleHandle.Copy<float2>(points, outVertices, points.Length);
			}
			if (num2 > 2 && num > 2)
			{
				NativeArray<int> srcIndices = new NativeArray<int>(num2 * 8, allocator, NativeArrayOptions.ClearMemory);
				NativeArray<float2> srcVertices = new NativeArray<float2>(num2 * 4, allocator, NativeArrayOptions.ClearMemory);
				int srcIndexCount = 0;
				int srcVertexCount = 0;
				flag = Tessellator.Tessellate(allocator, pgPoints, num2, nativeArray, num, ref srcVertices, ref srcVertexCount, ref srcIndices, ref srcIndexCount);
				if (flag)
				{
					ModuleHandle.TransferOutput(nativeArray, num, ref outEdges, ref outEdgeCount, srcIndices, srcIndexCount, ref outIndices, ref outIndexCount, srcVertices, srcVertexCount, ref outVertices, ref outVertexCount);
					if (flag2)
					{
						outEdgeCount = 0;
					}
				}
				srcVertices.Dispose();
				srcIndices.Dispose();
			}
			pgPoints.Dispose();
			nativeArray.Dispose();
			return zero;
		}

		internal static readonly int kMaxArea = 65536;

		internal static readonly int kMaxEdgeCount = 65536;

		internal static readonly int kMaxIndexCount = 65536;

		internal static readonly int kMaxVertexCount = 65536;

		internal static readonly int kMaxTriangleCount = ModuleHandle.kMaxIndexCount / 3;

		internal static readonly int kMaxRefineIterations = 48;

		internal static readonly int kMaxSmoothenIterations = 256;

		internal static readonly float kIncrementAreaFactor = 1.2f;
	}
}
