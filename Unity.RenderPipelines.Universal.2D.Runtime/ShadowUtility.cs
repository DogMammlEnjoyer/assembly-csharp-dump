using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal.UTess;
using UnityEngine.U2D;

namespace UnityEngine.Rendering.Universal
{
	[BurstCompile]
	internal class ShadowUtility
	{
		private unsafe static int GetNextShapeStart(int currentShape, int* inShapeStartingEdgePtr, int inShapeStartingEdgeLength, int maxValue)
		{
			if (currentShape + 1 >= inShapeStartingEdgeLength || inShapeStartingEdgePtr[currentShape + 1] < 0)
			{
				return maxValue;
			}
			return inShapeStartingEdgePtr[currentShape + 1];
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.CalculateProjectionInfo_000002F8$PostfixBurstDelegate))]
		internal static void CalculateProjectionInfo(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, ref NativeArray<Vector2> outProjectionInfo)
		{
			ShadowUtility.CalculateProjectionInfo_000002F8$BurstDirectCall.Invoke(ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, ref outProjectionInfo);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.CalculateVertices_000002F9$PostfixBurstDelegate))]
		internal static void CalculateVertices(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<Vector2> inEdgeOtherPoints, ref NativeArray<ShadowUtility.ShadowMeshVertex> outMeshVertices)
		{
			ShadowUtility.CalculateVertices_000002F9$BurstDirectCall.Invoke(ref inVertices, ref inEdges, ref inEdgeOtherPoints, ref outMeshVertices);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.CalculateTriangles_000002FA$PostfixBurstDelegate))]
		internal static void CalculateTriangles(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, ref NativeArray<int> outMeshIndices)
		{
			ShadowUtility.CalculateTriangles_000002FA$BurstDirectCall.Invoke(ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, ref outMeshIndices);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.CalculateLocalBounds_000002FB$PostfixBurstDelegate))]
		internal static void CalculateLocalBounds(ref NativeArray<Vector3> inVertices, out Bounds retBounds)
		{
			ShadowUtility.CalculateLocalBounds_000002FB$BurstDirectCall.Invoke(ref inVertices, out retBounds);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.GenerateInteriorMesh_000002FC$PostfixBurstDelegate))]
		private static void GenerateInteriorMesh(ref NativeArray<ShadowUtility.ShadowMeshVertex> inVertices, ref NativeArray<int> inIndices, ref NativeArray<ShadowEdge> inEdges, out NativeArray<ShadowUtility.ShadowMeshVertex> outVertices, out NativeArray<int> outIndices, out int outStartIndex, out int outIndexCount)
		{
			ShadowUtility.GenerateInteriorMesh_000002FC$BurstDirectCall.Invoke(ref inVertices, ref inIndices, ref inEdges, out outVertices, out outIndices, out outStartIndex, out outIndexCount);
		}

		public static Bounds GenerateShadowMesh(Mesh mesh, NativeArray<Vector3> inVertices, NativeArray<ShadowEdge> inEdges, NativeArray<int> inShapeStartingEdge, NativeArray<bool> inShapeIsClosedArray, bool allowContraction, bool fill, ShadowShape2D.OutlineTopology topology)
		{
			int length = inVertices.Length + 4 * inEdges.Length;
			int length2 = inEdges.Length * 3 * 3;
			NativeArray<Vector2> nativeArray = new NativeArray<Vector2>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray2 = new NativeArray<int>(length2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			NativeArray<ShadowUtility.ShadowMeshVertex> nativeArray3 = new NativeArray<ShadowUtility.ShadowMeshVertex>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			ShadowUtility.CalculateProjectionInfo(ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, ref nativeArray);
			ShadowUtility.CalculateVertices(ref inVertices, ref inEdges, ref nativeArray, ref nativeArray3);
			ShadowUtility.CalculateTriangles(ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, ref nativeArray2);
			int num = 0;
			int num2 = 0;
			NativeArray<ShadowUtility.ShadowMeshVertex> data;
			NativeArray<int> data2;
			if (fill)
			{
				ShadowUtility.GenerateInteriorMesh(ref nativeArray3, ref nativeArray2, ref inEdges, out data, out data2, out num, out num2);
				nativeArray3.Dispose();
				nativeArray2.Dispose();
			}
			else
			{
				data = nativeArray3;
				data2 = nativeArray2;
			}
			mesh.SetVertexBufferParams(data.Length, ShadowUtility.m_VertexLayout);
			mesh.SetVertexBufferData<ShadowUtility.ShadowMeshVertex>(data, 0, 0, data.Length, 0, MeshUpdateFlags.Default);
			mesh.SetIndexBufferParams(data2.Length, IndexFormat.UInt32);
			mesh.SetIndexBufferData<int>(data2, 0, 0, data2.Length, MeshUpdateFlags.Default);
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, data2.Length, MeshTopology.Triangles), MeshUpdateFlags.Default);
			mesh.subMeshCount = 1;
			nativeArray.Dispose();
			data.Dispose();
			data2.Dispose();
			Bounds result;
			ShadowUtility.CalculateLocalBounds(ref inVertices, out result);
			return result;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.CalculateEdgesFromLines_000002FE$PostfixBurstDelegate))]
		public static void CalculateEdgesFromLines(ref NativeArray<int> indices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge, out NativeArray<bool> outShapeIsClosedArray)
		{
			ShadowUtility.CalculateEdgesFromLines_000002FE$BurstDirectCall.Invoke(ref indices, out outEdges, out outShapeStartingEdge, out outShapeIsClosedArray);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.GetVertexReferenceStats_000002FF$PostfixBurstDelegate))]
		internal static void GetVertexReferenceStats(ref NativeArray<Vector3> vertices, ref NativeArray<ShadowEdge> edges, int vertexCount, out bool hasReusedVertices, out int newVertexCount, out NativeArray<ShadowUtility.RemappingInfo> remappingInfo)
		{
			ShadowUtility.GetVertexReferenceStats_000002FF$BurstDirectCall.Invoke(ref vertices, ref edges, vertexCount, out hasReusedVertices, out newVertexCount, out remappingInfo);
		}

		public static bool IsTriangleReversed(NativeArray<Vector3> vertices, int idx0, int idx1, int idx2)
		{
			Vector3 vector = vertices[idx0];
			Vector3 vector2 = vertices[idx1];
			Vector3 vector3 = vertices[idx2];
			return Mathf.Sign(vector.x * vector2.y + vector2.x * vector3.y + vector3.x * vector.y - (vector.y * vector2.x + vector2.y * vector3.x + vector3.y * vector.x)) >= 0f;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.CalculateEdgesFromTriangles_00000301$PostfixBurstDelegate))]
		public static void CalculateEdgesFromTriangles(ref NativeArray<Vector3> vertices, ref NativeArray<int> indices, bool duplicatesVertices, out NativeArray<Vector3> newVertices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge, out NativeArray<bool> outShapeIsClosedArray)
		{
			ShadowUtility.CalculateEdgesFromTriangles_00000301$BurstDirectCall.Invoke(ref vertices, ref indices, duplicatesVertices, out newVertices, out outEdges, out outShapeStartingEdge, out outShapeIsClosedArray);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.ReverseWindingOrder_00000302$PostfixBurstDelegate))]
		public static void ReverseWindingOrder(ref NativeArray<int> inShapeStartingEdge, ref NativeArray<ShadowEdge> inOutSortedEdges)
		{
			ShadowUtility.ReverseWindingOrder_00000302$BurstDirectCall.Invoke(ref inShapeStartingEdge, ref inOutSortedEdges);
		}

		private static int GetClosedPathCount(ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray)
		{
			int num = 0;
			int num2 = 0;
			while (num2 < inShapeStartingEdge.Length && inShapeStartingEdge[num2] >= 0)
			{
				num++;
				num2++;
			}
			return num;
		}

		private static void GetPathInfo(NativeArray<ShadowEdge> inEdges, NativeArray<int> inShapeStartingEdge, NativeArray<bool> inShapeIsClosedArray, out int closedPathArrayCount, out int closedPathsCount, out int openPathArrayCount, out int openPathsCount)
		{
			closedPathArrayCount = 0;
			openPathArrayCount = 0;
			closedPathsCount = 0;
			openPathsCount = 0;
			int num = 0;
			while (num < inShapeStartingEdge.Length && inShapeStartingEdge[num] >= 0)
			{
				int num2 = inShapeStartingEdge[num];
				int num3 = ((num < inShapeStartingEdge.Length - 1 && inShapeStartingEdge[num + 1] != -1) ? inShapeStartingEdge[num + 1] : inEdges.Length) - num2;
				if (inShapeIsClosedArray[num])
				{
					closedPathArrayCount += num3 + 1;
					closedPathsCount++;
				}
				else
				{
					openPathArrayCount += num3 + 1;
					openPathsCount++;
				}
				num++;
			}
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(ShadowUtility.ClipEdges_00000305$PostfixBurstDelegate))]
		public static void ClipEdges(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, float contractEdge, out NativeArray<Vector3> outVertices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge)
		{
			ShadowUtility.ClipEdges_00000305$BurstDirectCall.Invoke(ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, contractEdge, out outVertices, out outEdges, out outShapeStartingEdge);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void CalculateProjectionInfo$BurstManaged(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, ref NativeArray<Vector2> outProjectionInfo)
		{
			Vector3* buffer = (Vector3*)inVertices.m_Buffer;
			ShadowEdge* buffer2 = (ShadowEdge*)inEdges.m_Buffer;
			int* buffer3 = (int*)inShapeStartingEdge.m_Buffer;
			bool* buffer4 = (bool*)inShapeIsClosedArray.m_Buffer;
			Vector2* buffer5 = (Vector2*)outProjectionInfo.m_Buffer;
			Vector2 vector = default(Vector2);
			int length = inEdges.Length;
			int length2 = inShapeStartingEdge.Length;
			int length3 = inVertices.Length;
			int num = 0;
			int num2 = 0;
			int nextShapeStart = ShadowUtility.GetNextShapeStart(num, buffer3, length2, length);
			int num3 = nextShapeStart;
			for (int i = 0; i < length; i++)
			{
				if (i == nextShapeStart)
				{
					num++;
					num2 = nextShapeStart;
					nextShapeStart = ShadowUtility.GetNextShapeStart(num, buffer3, length2, length);
					num3 = nextShapeStart - num2;
				}
				int num4 = (i - num2 + 1) % num3 + num2;
				int num5 = (i - num2 + num3 - 1) % num3 + num2;
				int v = buffer2[i].v0;
				int v2 = buffer2[i].v1;
				int v3 = buffer2[num5].v0;
				int v4 = buffer2[num4].v1;
				vector.x = buffer[v].x;
				vector.y = buffer[v].y;
				Vector2 vector2 = vector;
				vector.x = buffer[v2].x;
				vector.y = buffer[v2].y;
				Vector2 vector3 = vector;
				vector.x = buffer[v3].x;
				vector.y = buffer[v3].y;
				vector.x = buffer[v4].x;
				vector.y = buffer[v4].y;
				buffer5[v] = vector3;
				int num6 = 4 * i + length3;
				buffer5[num6] = vector3;
				buffer5[num6 + 1] = vector2;
				buffer5[num6 + 2] = vector3;
				buffer5[num6 + 3] = vector3;
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void CalculateVertices$BurstManaged(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<Vector2> inEdgeOtherPoints, ref NativeArray<ShadowUtility.ShadowMeshVertex> outMeshVertices)
		{
			Vector3* buffer = (Vector3*)inVertices.m_Buffer;
			ShadowEdge* buffer2 = (ShadowEdge*)inEdges.m_Buffer;
			Vector2* buffer3 = (Vector2*)inEdgeOtherPoints.m_Buffer;
			ShadowUtility.ShadowMeshVertex* buffer4 = (ShadowUtility.ShadowMeshVertex*)outMeshVertices.m_Buffer;
			Vector2 vector = default(Vector2);
			int length = inEdges.Length;
			int length2 = inVertices.Length;
			for (int i = 0; i < length2; i++)
			{
				vector.x = buffer[i].x;
				vector.y = buffer[i].y;
				ShadowUtility.ShadowMeshVertex shadowMeshVertex = new ShadowUtility.ShadowMeshVertex(ShadowUtility.ProjectionType.ProjectionNone, vector, buffer3[i]);
				buffer4[i] = shadowMeshVertex;
			}
			for (int j = 0; j < length; j++)
			{
				int v = buffer2[j].v0;
				int v2 = buffer2[j].v1;
				vector.x = buffer[v].x;
				vector.y = buffer[v].y;
				Vector2 inEdgePosition = vector;
				vector.x = buffer[v2].x;
				vector.y = buffer[v2].y;
				Vector2 inEdgePosition2 = vector;
				int num = 4 * j + length2;
				ShadowUtility.ShadowMeshVertex shadowMeshVertex2 = new ShadowUtility.ShadowMeshVertex(ShadowUtility.ProjectionType.ProjectionHard, inEdgePosition, buffer3[num]);
				ShadowUtility.ShadowMeshVertex shadowMeshVertex3 = new ShadowUtility.ShadowMeshVertex(ShadowUtility.ProjectionType.ProjectionHard, inEdgePosition2, buffer3[num + 1]);
				ShadowUtility.ShadowMeshVertex shadowMeshVertex4 = new ShadowUtility.ShadowMeshVertex(ShadowUtility.ProjectionType.ProjectionSoftLeft, inEdgePosition, buffer3[num + 2]);
				ShadowUtility.ShadowMeshVertex shadowMeshVertex5 = new ShadowUtility.ShadowMeshVertex(ShadowUtility.ProjectionType.ProjectionSoftRight, inEdgePosition, buffer3[num + 3]);
				buffer4[num] = shadowMeshVertex2;
				buffer4[num + 1] = shadowMeshVertex3;
				buffer4[num + 2] = shadowMeshVertex4;
				buffer4[num + 3] = shadowMeshVertex5;
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void CalculateTriangles$BurstManaged(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, ref NativeArray<int> outMeshIndices)
		{
			ShadowEdge* buffer = (ShadowEdge*)inEdges.m_Buffer;
			int* buffer2 = (int*)inShapeStartingEdge.m_Buffer;
			int* buffer3 = (int*)outMeshIndices.m_Buffer;
			int length = inEdges.Length;
			int length2 = inShapeStartingEdge.Length;
			int length3 = inVertices.Length;
			int num = 0;
			for (int i = 0; i < length2; i++)
			{
				int num2 = buffer2[i];
				if (num2 < 0)
				{
					return;
				}
				int num3 = length;
				if (i + 1 < length2 && buffer2[i + 1] > -1)
				{
					num3 = buffer2[i + 1];
				}
				for (int j = num2; j < num3; j++)
				{
					int v = buffer[j].v0;
					int v2 = buffer[j].v1;
					int num4 = 4 * j + length3;
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)v);
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)num4);
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)(num4 + 1));
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)(num4 + 1));
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)v2);
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)v);
				}
				for (int k = num2; k < num3; k++)
				{
					int v3 = buffer[k].v0;
					ShadowEdge shadowEdge = buffer[k];
					int num5 = 4 * k + length3;
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)v3);
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)num5 + 2);
					buffer3[(IntPtr)(num++) * 4] = (int)((ushort)num5 + 3);
				}
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void CalculateLocalBounds$BurstManaged(ref NativeArray<Vector3> inVertices, out Bounds retBounds)
		{
			if (inVertices.Length <= 0)
			{
				retBounds = new Bounds(Vector3.zero, Vector3.zero);
				return;
			}
			Vector2 vector = Vector2.positiveInfinity;
			Vector2 vector2 = Vector2.negativeInfinity;
			Vector3* buffer = (Vector3*)inVertices.m_Buffer;
			int length = inVertices.Length;
			for (int i = 0; i < length; i++)
			{
				Vector2 rhs = new Vector2(buffer[i].x, buffer[i].y);
				vector = Vector2.Min(vector, rhs);
				vector2 = Vector2.Max(vector2, rhs);
			}
			retBounds = new Bounds
			{
				max = vector2,
				min = vector
			};
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GenerateInteriorMesh$BurstManaged(ref NativeArray<ShadowUtility.ShadowMeshVertex> inVertices, ref NativeArray<int> inIndices, ref NativeArray<ShadowEdge> inEdges, out NativeArray<ShadowUtility.ShadowMeshVertex> outVertices, out NativeArray<int> outIndices, out int outStartIndex, out int outIndexCount)
		{
			int length = inEdges.Length;
			NativeArray<int2> edges = new NativeArray<int2>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			NativeArray<float2> points = new NativeArray<float2>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			for (int i = 0; i < length; i++)
			{
				int2 @int = new int2(inEdges[i].v0, inEdges[i].v1);
				edges[i] = @int;
				int x = @int.x;
				points[x] = new float2(inVertices[x].position.x, inVertices[x].position.y);
			}
			NativeArray<int> nativeArray = new NativeArray<int>(points.Length * 8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			NativeArray<float2> nativeArray2 = new NativeArray<float2>(points.Length * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			NativeArray<int2> nativeArray3 = new NativeArray<int2>(edges.Length * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			ModuleHandle.Tessellate(Allocator.Persistent, points, edges, ref nativeArray2, ref num, ref nativeArray, ref num2, ref nativeArray3, ref num3);
			int length2 = inIndices.Length;
			int length3 = inVertices.Length;
			int length4 = num + inVertices.Length;
			int length5 = num2 + inIndices.Length;
			outVertices = new NativeArray<ShadowUtility.ShadowMeshVertex>(length4, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			outIndices = new NativeArray<int>(length5, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			for (int j = 0; j < inVertices.Length; j++)
			{
				outVertices[j] = inVertices[j];
			}
			for (int k = 0; k < num; k++)
			{
				float2 v = nativeArray2[k];
				ShadowUtility.ShadowMeshVertex value = new ShadowUtility.ShadowMeshVertex(ShadowUtility.ProjectionType.ProjectionNone, v, Vector2.zero);
				outVertices[k + length3] = value;
			}
			for (int l = 0; l < inIndices.Length; l++)
			{
				outIndices[l] = inIndices[l];
			}
			for (int m = 0; m < num2; m++)
			{
				outIndices[m + length2] = nativeArray[m] + length3;
			}
			outStartIndex = length2;
			outIndexCount = num2;
			edges.Dispose();
			points.Dispose();
			nativeArray.Dispose();
			nativeArray2.Dispose();
			nativeArray3.Dispose();
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void CalculateEdgesFromLines$BurstManaged(ref NativeArray<int> indices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge, out NativeArray<bool> outShapeIsClosedArray)
		{
			int num = indices.Length >> 1;
			NativeArray<int> nativeArray = new NativeArray<int>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			NativeArray<bool> nativeArray2 = new NativeArray<bool>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int* buffer = (int*)indices.m_Buffer;
			int* buffer2 = (int*)nativeArray.m_Buffer;
			bool* buffer3 = (bool*)nativeArray2.m_Buffer;
			int length = indices.Length;
			int num2 = 0;
			int num3 = *buffer;
			int num4 = *buffer;
			bool flag = false;
			*buffer2 = 0;
			for (int i = 0; i < length; i += 2)
			{
				if (flag)
				{
					num3 = buffer[i];
					buffer3[num2] = true;
					buffer2[(IntPtr)(++num2) * 4] = i >> 1;
					flag = false;
				}
				else if (buffer[i] != num4)
				{
					buffer3[num2] = false;
					buffer2[(IntPtr)(++num2) * 4] = i >> 1;
					num3 = buffer[i];
				}
				if (num3 == buffer[i + 1])
				{
					flag = true;
				}
				num4 = buffer[i + 1];
			}
			buffer3[num2++] = flag;
			outShapeStartingEdge = new NativeArray<int>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			outShapeIsClosedArray = new NativeArray<bool>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int* buffer4 = (int*)outShapeStartingEdge.m_Buffer;
			bool* buffer5 = (bool*)outShapeIsClosedArray.m_Buffer;
			for (int j = 0; j < num2; j++)
			{
				buffer4[j] = buffer2[j];
				buffer5[j] = buffer3[j];
			}
			nativeArray.Dispose();
			nativeArray2.Dispose();
			outEdges = new NativeArray<ShadowEdge>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			ShadowEdge* buffer6 = (ShadowEdge*)outEdges.m_Buffer;
			for (int k = 0; k < num; k++)
			{
				int num5 = k << 1;
				int indexA = buffer[num5];
				int indexB = buffer[num5 + 1];
				buffer6[k] = new ShadowEdge(indexA, indexB);
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void GetVertexReferenceStats$BurstManaged(ref NativeArray<Vector3> vertices, ref NativeArray<ShadowEdge> edges, int vertexCount, out bool hasReusedVertices, out int newVertexCount, out NativeArray<ShadowUtility.RemappingInfo> remappingInfo)
		{
			int length = edges.Length;
			newVertexCount = 0;
			hasReusedVertices = false;
			remappingInfo = new NativeArray<ShadowUtility.RemappingInfo>(vertexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			ShadowUtility.RemappingInfo* unsafePtr = (ShadowUtility.RemappingInfo*)remappingInfo.GetUnsafePtr<ShadowUtility.RemappingInfo>();
			ShadowEdge* unsafePtr2 = (ShadowEdge*)edges.GetUnsafePtr<ShadowEdge>();
			for (int i = 0; i < vertexCount; i++)
			{
				unsafePtr[i].Initialize();
			}
			for (int j = 0; j < length; j++)
			{
				int v = unsafePtr2[j].v0;
				unsafePtr[v].count = unsafePtr[v].count + 1;
				if (unsafePtr[v].count > 1)
				{
					hasReusedVertices = true;
				}
				newVertexCount++;
			}
			for (int k = 0; k < length; k++)
			{
				int v2 = unsafePtr2[k].v1;
				if (unsafePtr[v2].count == 0)
				{
					unsafePtr[v2].count = 1;
					newVertexCount++;
				}
			}
			int num = 0;
			for (int l = 0; l < vertexCount; l++)
			{
				if (unsafePtr[l].count > 0)
				{
					unsafePtr[l].index = num;
					num += unsafePtr[l].count;
				}
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void CalculateEdgesFromTriangles$BurstManaged(ref NativeArray<Vector3> vertices, ref NativeArray<int> indices, bool duplicatesVertices, out NativeArray<Vector3> newVertices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge, out NativeArray<bool> outShapeIsClosedArray)
		{
			Clipper2D.Solution solution = default(Clipper2D.Solution);
			Clipper2D.ExecuteArguments inExecuteArguments = new Clipper2D.ExecuteArguments(Clipper2D.InitOptions.ioDefault, Clipper2D.ClipType.ctUnion, Clipper2D.PolyFillType.pftEvenOdd, Clipper2D.PolyFillType.pftEvenOdd, false, false, false);
			int num = indices.Length / 3;
			NativeArray<Vector2> nativeArray = new NativeArray<Vector2>(indices.Length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray2 = new NativeArray<int>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			NativeArray<Clipper2D.PathArguments> nativeArray3 = new NativeArray<Clipper2D.PathArguments>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			Vector2* unsafePtr = (Vector2*)nativeArray.GetUnsafePtr<Vector2>();
			int* unsafePtr2 = (int*)nativeArray2.GetUnsafePtr<int>();
			Clipper2D.PathArguments* unsafePtr3 = (Clipper2D.PathArguments*)nativeArray3.GetUnsafePtr<Clipper2D.PathArguments>();
			Vector3* unsafePtr4 = (Vector3*)vertices.GetUnsafePtr<Vector3>();
			Clipper2D.PathArguments pathArguments = new Clipper2D.PathArguments(Clipper2D.PolyType.ptSubject, true);
			for (int i = 0; i < num; i++)
			{
				unsafePtr2[i] = 3;
				unsafePtr3[i] = pathArguments;
				int num2 = 3 * i;
				unsafePtr[num2] = unsafePtr4[indices[num2]];
				unsafePtr[num2 + 1] = unsafePtr4[indices[num2 + 1]];
				unsafePtr[num2 + 2] = unsafePtr4[indices[num2 + 2]];
			}
			Clipper2D.Execute(ref solution, nativeArray, nativeArray2, nativeArray3, inExecuteArguments, Allocator.Persistent, 65536, false);
			nativeArray.Dispose();
			nativeArray2.Dispose();
			nativeArray3.Dispose();
			int length = solution.points.Length;
			int length2 = solution.pathSizes.Length;
			newVertices = new NativeArray<Vector3>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			outEdges = new NativeArray<ShadowEdge>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			outShapeStartingEdge = new NativeArray<int>(length2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			outShapeIsClosedArray = new NativeArray<bool>(length2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int* unsafePtr5 = (int*)solution.pathSizes.GetUnsafePtr<int>();
			Vector2* unsafePtr6 = (Vector2*)solution.points.GetUnsafePtr<Vector2>();
			Vector3* unsafePtr7 = (Vector3*)newVertices.GetUnsafePtr<Vector3>();
			ShadowEdge* unsafePtr8 = (ShadowEdge*)outEdges.GetUnsafePtr<ShadowEdge>();
			int* unsafePtr9 = (int*)outShapeStartingEdge.GetUnsafePtr<int>();
			bool* unsafePtr10 = (bool*)outShapeIsClosedArray.GetUnsafePtr<bool>();
			int num3 = 0;
			for (int j = 0; j < length2; j++)
			{
				int num4 = num3;
				int num5 = unsafePtr5[j];
				unsafePtr9[j] = num3;
				num3 += num5;
				int indexA = num3 - 1;
				for (int k = num4; k < num3; k++)
				{
					unsafePtr7[k] = unsafePtr6[k];
					unsafePtr8[k] = new ShadowEdge(indexA, k);
					indexA = k;
				}
				unsafePtr10[j] = true;
			}
			solution.Dispose();
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ReverseWindingOrder$BurstManaged(ref NativeArray<int> inShapeStartingEdge, ref NativeArray<ShadowEdge> inOutSortedEdges)
		{
			for (int i = 0; i < inShapeStartingEdge.Length; i++)
			{
				int num = inShapeStartingEdge[i];
				if (num < 0)
				{
					return;
				}
				int num2 = inOutSortedEdges.Length;
				if (i + 1 < inShapeStartingEdge.Length && inShapeStartingEdge[i + 1] > -1)
				{
					num2 = inShapeStartingEdge[i + 1];
				}
				int num3 = num2 - num;
				for (int j = 0; j < num3 >> 1; j++)
				{
					int index = num + j;
					int index2 = num + num3 - 1 - j;
					ShadowEdge value = inOutSortedEdges[index];
					ShadowEdge value2 = inOutSortedEdges[index2];
					value.Reverse();
					value2.Reverse();
					inOutSortedEdges[index] = value2;
					inOutSortedEdges[index2] = value;
				}
				if ((num3 & 1) == 1)
				{
					int index3 = num + (num3 >> 1);
					ShadowEdge value3 = inOutSortedEdges[index3];
					value3.Reverse();
					inOutSortedEdges[index3] = value3;
				}
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void ClipEdges$BurstManaged(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, float contractEdge, out NativeArray<Vector3> outVertices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge)
		{
			Allocator allocator = Allocator.Persistent;
			int num = 65536;
			int length;
			int num2;
			int num3;
			int num4;
			ShadowUtility.GetPathInfo(inEdges, inShapeStartingEdge, inShapeIsClosedArray, out length, out num2, out num3, out num4);
			NativeArray<Clipper2D.PathArguments> nativeArray = new NativeArray<Clipper2D.PathArguments>(num2, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray2 = new NativeArray<int>(num2, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<Vector2> nativeArray3 = new NativeArray<Vector2>(length, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray4 = new NativeArray<int>(num4, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<Vector2> nativeArray5 = new NativeArray<Vector2>(num3, allocator, NativeArrayOptions.ClearMemory);
			Clipper2D.PathArguments* buffer = (Clipper2D.PathArguments*)nativeArray.m_Buffer;
			int* buffer2 = (int*)nativeArray2.m_Buffer;
			Vector2* buffer3 = (Vector2*)nativeArray3.m_Buffer;
			int* buffer4 = (int*)nativeArray4.m_Buffer;
			Vector2* buffer5 = (Vector2*)nativeArray5.m_Buffer;
			int* buffer6 = (int*)inShapeStartingEdge.m_Buffer;
			bool* buffer7 = (bool*)inShapeIsClosedArray.m_Buffer;
			Vector3* buffer8 = (Vector3*)inVertices.m_Buffer;
			ShadowEdge* buffer9 = (ShadowEdge*)inEdges.m_Buffer;
			int length2 = inEdges.Length;
			Vector2 vector = default(Vector2);
			Vector3 zero = Vector3.zero;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = num2 + num4;
			for (int i = 0; i < num9; i++)
			{
				int num10 = buffer6[i];
				int num11 = ((i + 1 < num9) ? buffer6[i + 1] : length2) - num10;
				if (buffer7[i])
				{
					buffer2[num6] = num11 + 1;
					buffer[num6] = new Clipper2D.PathArguments(Clipper2D.PolyType.ptSubject, true);
					num6++;
					for (int j = 0; j < num11; j++)
					{
						Vector3 vector2 = buffer8[buffer9[j + num10].v0];
						vector.x = vector2.x;
						vector.y = vector2.y;
						buffer3[(IntPtr)(num5++) * (IntPtr)sizeof(Vector2)] = vector;
					}
					buffer3[(IntPtr)(num5++) * (IntPtr)sizeof(Vector2)] = buffer8[buffer9[num11 + num10 - 1].v1];
				}
				else
				{
					buffer4[(IntPtr)(num8++) * 4] = num11 + 1;
					for (int k = 0; k < num11; k++)
					{
						Vector3 vector3 = buffer8[buffer9[k + num10].v0];
						vector.x = vector3.x;
						vector.y = vector3.y;
						buffer5[(IntPtr)(num7++) * (IntPtr)sizeof(Vector2)] = vector;
					}
					buffer5[(IntPtr)(num7++) * (IntPtr)sizeof(Vector2)] = buffer8[buffer9[num11 + num10 - 1].v1];
				}
			}
			NativeArray<Vector2> inPoints = nativeArray3;
			NativeArray<int> inPathSizes = nativeArray2;
			Clipper2D.Solution solution = default(Clipper2D.Solution);
			if (nativeArray2.Length > 1)
			{
				Clipper2D.ExecuteArguments inExecuteArguments = new Clipper2D.ExecuteArguments
				{
					clipType = Clipper2D.ClipType.ctUnion,
					clipFillType = Clipper2D.PolyFillType.pftEvenOdd,
					subjFillType = Clipper2D.PolyFillType.pftEvenOdd,
					strictlySimple = false,
					preserveColinear = false
				};
				Clipper2D.Execute(ref solution, nativeArray3, nativeArray2, nativeArray, inExecuteArguments, allocator, num, true);
				inPoints = solution.points;
				inPathSizes = solution.pathSizes;
			}
			ClipperOffset2D.Solution solution2 = default(ClipperOffset2D.Solution);
			NativeArray<ClipperOffset2D.PathArguments> inPathArguments = new NativeArray<ClipperOffset2D.PathArguments>(inPathSizes.Length, allocator, NativeArrayOptions.ClearMemory);
			ClipperOffset2D.Execute(ref solution2, inPoints, inPathSizes, inPathArguments, allocator, (double)(-(double)contractEdge), 2.0, 0.25, 0.0, (double)num, false);
			if (solution2.pathSizes.Length > 0 || num4 > 0)
			{
				int num12 = 0;
				int length3 = solution2.pathSizes.Length + num4;
				outVertices = new NativeArray<Vector3>(solution2.points.Length + num3, allocator, NativeArrayOptions.ClearMemory);
				outEdges = new NativeArray<ShadowEdge>(solution2.points.Length + num3, allocator, NativeArrayOptions.ClearMemory);
				outShapeStartingEdge = new NativeArray<int>(length3, allocator, NativeArrayOptions.ClearMemory);
				Vector3* buffer10 = (Vector3*)outVertices.m_Buffer;
				ShadowEdge* buffer11 = (ShadowEdge*)outEdges.m_Buffer;
				int* buffer12 = (int*)outShapeStartingEdge.m_Buffer;
				Vector2* buffer13 = (Vector2*)solution2.points.m_Buffer;
				int length4 = solution2.points.Length;
				int* buffer14 = (int*)solution2.pathSizes.m_Buffer;
				int length5 = solution2.pathSizes.Length;
				for (int l = 0; l < length4; l++)
				{
					zero.x = buffer13[l].x;
					zero.y = buffer13[l].y;
					buffer10[(IntPtr)(num12++) * (IntPtr)sizeof(Vector3)] = zero;
				}
				int num13 = 0;
				for (int m = 0; m < length5; m++)
				{
					int num14 = buffer14[m];
					int num15 = num13 + num14;
					buffer12[m] = num13;
					for (int n = 0; n < num14; n++)
					{
						ShadowEdge shadowEdge = new ShadowEdge(n + num13, (n + 1) % num14 + num13);
						buffer11[n + num13] = shadowEdge;
					}
					num13 = num15;
				}
				int num16 = length5;
				num13 = num12;
				for (int num17 = 0; num17 < nativeArray5.Length; num17++)
				{
					zero.x = buffer5[num17].x;
					zero.y = buffer5[num17].y;
					buffer10[(IntPtr)(num12++) * (IntPtr)sizeof(Vector3)] = zero;
				}
				for (int num18 = 0; num18 < num4; num18++)
				{
					int num19 = buffer4[num18];
					int num20 = num13 + num19;
					buffer12[num16 + num18] = num13;
					for (int num21 = 0; num21 < num19 - 1; num21++)
					{
						ShadowEdge shadowEdge2 = new ShadowEdge(num21 + num13, num21 + 1);
						buffer11[num21 + num13] = shadowEdge2;
					}
					num13 = num20;
				}
			}
			else
			{
				outVertices = new NativeArray<Vector3>(0, allocator, NativeArrayOptions.ClearMemory);
				outEdges = new NativeArray<ShadowEdge>(0, allocator, NativeArrayOptions.ClearMemory);
				outShapeStartingEdge = new NativeArray<int>(0, allocator, NativeArrayOptions.ClearMemory);
			}
			nativeArray2.Dispose();
			nativeArray3.Dispose();
			nativeArray4.Dispose();
			nativeArray5.Dispose();
			nativeArray.Dispose();
			inPathArguments.Dispose();
			solution.Dispose();
			solution2.Dispose();
		}

		internal const int k_AdditionalVerticesPerEdge = 4;

		internal const int k_VerticesPerTriangle = 3;

		internal const int k_TrianglesPerEdge = 3;

		internal const int k_MinimumEdges = 3;

		internal const int k_SafeSize = 40;

		private static VertexAttributeDescriptor[] m_VertexLayout = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, 0)
		};

		public enum ProjectionType
		{
			ProjectionNone = -1,
			ProjectionHard,
			ProjectionSoftLeft,
			ProjectionSoftRight = 3
		}

		internal struct ShadowMeshVertex
		{
			internal ShadowMeshVertex(ShadowUtility.ProjectionType inProjectionType, Vector2 inEdgePosition0, Vector2 inEdgePosition1)
			{
				this.position.x = inEdgePosition0.x;
				this.position.y = inEdgePosition0.y;
				this.position.z = 0f;
				this.tangent.x = (float)inProjectionType;
				this.tangent.y = 0f;
				this.tangent.z = inEdgePosition1.x;
				this.tangent.w = inEdgePosition1.y;
			}

			internal Vector3 position;

			internal Vector4 tangent;
		}

		internal struct RemappingInfo
		{
			public void Initialize()
			{
				this.count = 0;
				this.index = -1;
				this.v0Offset = 0;
				this.v1Offset = 0;
			}

			public int count;

			public int index;

			public int v0Offset;

			public int v1Offset;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateProjectionInfo_000002F8$PostfixBurstDelegate(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, ref NativeArray<Vector2> outProjectionInfo);

		internal static class CalculateProjectionInfo_000002F8$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.CalculateProjectionInfo_000002F8$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.CalculateProjectionInfo_000002F8$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.CalculateProjectionInfo_000002F8$PostfixBurstDelegate>(new ShadowUtility.CalculateProjectionInfo_000002F8$PostfixBurstDelegate(ShadowUtility.CalculateProjectionInfo)).Value;
				}
				A_0 = ShadowUtility.CalculateProjectionInfo_000002F8$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.CalculateProjectionInfo_000002F8$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, ref NativeArray<Vector2> outProjectionInfo)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.CalculateProjectionInfo_000002F8$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<System.Boolean>&,Unity.Collections.NativeArray`1<UnityEngine.Vector2>&), ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, ref outProjectionInfo, functionPointer);
						return;
					}
				}
				ShadowUtility.CalculateProjectionInfo$BurstManaged(ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, ref outProjectionInfo);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateVertices_000002F9$PostfixBurstDelegate(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<Vector2> inEdgeOtherPoints, ref NativeArray<ShadowUtility.ShadowMeshVertex> outMeshVertices);

		internal static class CalculateVertices_000002F9$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.CalculateVertices_000002F9$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.CalculateVertices_000002F9$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.CalculateVertices_000002F9$PostfixBurstDelegate>(new ShadowUtility.CalculateVertices_000002F9$PostfixBurstDelegate(ShadowUtility.CalculateVertices)).Value;
				}
				A_0 = ShadowUtility.CalculateVertices_000002F9$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.CalculateVertices_000002F9$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<Vector2> inEdgeOtherPoints, ref NativeArray<ShadowUtility.ShadowMeshVertex> outMeshVertices)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.CalculateVertices_000002F9$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,Unity.Collections.NativeArray`1<UnityEngine.Vector2>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowUtility/ShadowMeshVertex>&), ref inVertices, ref inEdges, ref inEdgeOtherPoints, ref outMeshVertices, functionPointer);
						return;
					}
				}
				ShadowUtility.CalculateVertices$BurstManaged(ref inVertices, ref inEdges, ref inEdgeOtherPoints, ref outMeshVertices);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateTriangles_000002FA$PostfixBurstDelegate(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, ref NativeArray<int> outMeshIndices);

		internal static class CalculateTriangles_000002FA$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.CalculateTriangles_000002FA$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.CalculateTriangles_000002FA$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.CalculateTriangles_000002FA$PostfixBurstDelegate>(new ShadowUtility.CalculateTriangles_000002FA$PostfixBurstDelegate(ShadowUtility.CalculateTriangles)).Value;
				}
				A_0 = ShadowUtility.CalculateTriangles_000002FA$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.CalculateTriangles_000002FA$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, ref NativeArray<int> outMeshIndices)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.CalculateTriangles_000002FA$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<System.Boolean>&,Unity.Collections.NativeArray`1<System.Int32>&), ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, ref outMeshIndices, functionPointer);
						return;
					}
				}
				ShadowUtility.CalculateTriangles$BurstManaged(ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, ref outMeshIndices);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateLocalBounds_000002FB$PostfixBurstDelegate(ref NativeArray<Vector3> inVertices, out Bounds retBounds);

		internal static class CalculateLocalBounds_000002FB$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.CalculateLocalBounds_000002FB$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.CalculateLocalBounds_000002FB$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.CalculateLocalBounds_000002FB$PostfixBurstDelegate>(new ShadowUtility.CalculateLocalBounds_000002FB$PostfixBurstDelegate(ShadowUtility.CalculateLocalBounds)).Value;
				}
				A_0 = ShadowUtility.CalculateLocalBounds_000002FB$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.CalculateLocalBounds_000002FB$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<Vector3> inVertices, out Bounds retBounds)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.CalculateLocalBounds_000002FB$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,UnityEngine.Bounds&), ref inVertices, ref retBounds, functionPointer);
						return;
					}
				}
				ShadowUtility.CalculateLocalBounds$BurstManaged(ref inVertices, out retBounds);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GenerateInteriorMesh_000002FC$PostfixBurstDelegate(ref NativeArray<ShadowUtility.ShadowMeshVertex> inVertices, ref NativeArray<int> inIndices, ref NativeArray<ShadowEdge> inEdges, out NativeArray<ShadowUtility.ShadowMeshVertex> outVertices, out NativeArray<int> outIndices, out int outStartIndex, out int outIndexCount);

		internal static class GenerateInteriorMesh_000002FC$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.GenerateInteriorMesh_000002FC$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.GenerateInteriorMesh_000002FC$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.GenerateInteriorMesh_000002FC$PostfixBurstDelegate>(new ShadowUtility.GenerateInteriorMesh_000002FC$PostfixBurstDelegate(ShadowUtility.GenerateInteriorMesh)).Value;
				}
				A_0 = ShadowUtility.GenerateInteriorMesh_000002FC$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.GenerateInteriorMesh_000002FC$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<ShadowUtility.ShadowMeshVertex> inVertices, ref NativeArray<int> inIndices, ref NativeArray<ShadowEdge> inEdges, out NativeArray<ShadowUtility.ShadowMeshVertex> outVertices, out NativeArray<int> outIndices, out int outStartIndex, out int outIndexCount)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.GenerateInteriorMesh_000002FC$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowUtility/ShadowMeshVertex>&,Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowUtility/ShadowMeshVertex>&,Unity.Collections.NativeArray`1<System.Int32>&,System.Int32&,System.Int32&), ref inVertices, ref inIndices, ref inEdges, ref outVertices, ref outIndices, ref outStartIndex, ref outIndexCount, functionPointer);
						return;
					}
				}
				ShadowUtility.GenerateInteriorMesh$BurstManaged(ref inVertices, ref inIndices, ref inEdges, out outVertices, out outIndices, out outStartIndex, out outIndexCount);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateEdgesFromLines_000002FE$PostfixBurstDelegate(ref NativeArray<int> indices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge, out NativeArray<bool> outShapeIsClosedArray);

		internal static class CalculateEdgesFromLines_000002FE$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.CalculateEdgesFromLines_000002FE$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.CalculateEdgesFromLines_000002FE$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.CalculateEdgesFromLines_000002FE$PostfixBurstDelegate>(new ShadowUtility.CalculateEdgesFromLines_000002FE$PostfixBurstDelegate(ShadowUtility.CalculateEdgesFromLines)).Value;
				}
				A_0 = ShadowUtility.CalculateEdgesFromLines_000002FE$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.CalculateEdgesFromLines_000002FE$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<int> indices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge, out NativeArray<bool> outShapeIsClosedArray)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.CalculateEdgesFromLines_000002FE$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<System.Boolean>&), ref indices, ref outEdges, ref outShapeStartingEdge, ref outShapeIsClosedArray, functionPointer);
						return;
					}
				}
				ShadowUtility.CalculateEdgesFromLines$BurstManaged(ref indices, out outEdges, out outShapeStartingEdge, out outShapeIsClosedArray);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetVertexReferenceStats_000002FF$PostfixBurstDelegate(ref NativeArray<Vector3> vertices, ref NativeArray<ShadowEdge> edges, int vertexCount, out bool hasReusedVertices, out int newVertexCount, out NativeArray<ShadowUtility.RemappingInfo> remappingInfo);

		internal static class GetVertexReferenceStats_000002FF$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.GetVertexReferenceStats_000002FF$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.GetVertexReferenceStats_000002FF$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.GetVertexReferenceStats_000002FF$PostfixBurstDelegate>(new ShadowUtility.GetVertexReferenceStats_000002FF$PostfixBurstDelegate(ShadowUtility.GetVertexReferenceStats)).Value;
				}
				A_0 = ShadowUtility.GetVertexReferenceStats_000002FF$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.GetVertexReferenceStats_000002FF$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<Vector3> vertices, ref NativeArray<ShadowEdge> edges, int vertexCount, out bool hasReusedVertices, out int newVertexCount, out NativeArray<ShadowUtility.RemappingInfo> remappingInfo)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.GetVertexReferenceStats_000002FF$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,System.Int32,System.Boolean&,System.Int32&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowUtility/RemappingInfo>&), ref vertices, ref edges, vertexCount, ref hasReusedVertices, ref newVertexCount, ref remappingInfo, functionPointer);
						return;
					}
				}
				ShadowUtility.GetVertexReferenceStats$BurstManaged(ref vertices, ref edges, vertexCount, out hasReusedVertices, out newVertexCount, out remappingInfo);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateEdgesFromTriangles_00000301$PostfixBurstDelegate(ref NativeArray<Vector3> vertices, ref NativeArray<int> indices, bool duplicatesVertices, out NativeArray<Vector3> newVertices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge, out NativeArray<bool> outShapeIsClosedArray);

		internal static class CalculateEdgesFromTriangles_00000301$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.CalculateEdgesFromTriangles_00000301$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.CalculateEdgesFromTriangles_00000301$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.CalculateEdgesFromTriangles_00000301$PostfixBurstDelegate>(new ShadowUtility.CalculateEdgesFromTriangles_00000301$PostfixBurstDelegate(ShadowUtility.CalculateEdgesFromTriangles)).Value;
				}
				A_0 = ShadowUtility.CalculateEdgesFromTriangles_00000301$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.CalculateEdgesFromTriangles_00000301$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<Vector3> vertices, ref NativeArray<int> indices, bool duplicatesVertices, out NativeArray<Vector3> newVertices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge, out NativeArray<bool> outShapeIsClosedArray)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.CalculateEdgesFromTriangles_00000301$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,Unity.Collections.NativeArray`1<System.Int32>&,System.Boolean,Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<System.Boolean>&), ref vertices, ref indices, duplicatesVertices, ref newVertices, ref outEdges, ref outShapeStartingEdge, ref outShapeIsClosedArray, functionPointer);
						return;
					}
				}
				ShadowUtility.CalculateEdgesFromTriangles$BurstManaged(ref vertices, ref indices, duplicatesVertices, out newVertices, out outEdges, out outShapeStartingEdge, out outShapeIsClosedArray);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ReverseWindingOrder_00000302$PostfixBurstDelegate(ref NativeArray<int> inShapeStartingEdge, ref NativeArray<ShadowEdge> inOutSortedEdges);

		internal static class ReverseWindingOrder_00000302$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.ReverseWindingOrder_00000302$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.ReverseWindingOrder_00000302$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.ReverseWindingOrder_00000302$PostfixBurstDelegate>(new ShadowUtility.ReverseWindingOrder_00000302$PostfixBurstDelegate(ShadowUtility.ReverseWindingOrder)).Value;
				}
				A_0 = ShadowUtility.ReverseWindingOrder_00000302$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.ReverseWindingOrder_00000302$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<int> inShapeStartingEdge, ref NativeArray<ShadowEdge> inOutSortedEdges)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.ReverseWindingOrder_00000302$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&), ref inShapeStartingEdge, ref inOutSortedEdges, functionPointer);
						return;
					}
				}
				ShadowUtility.ReverseWindingOrder$BurstManaged(ref inShapeStartingEdge, ref inOutSortedEdges);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ClipEdges_00000305$PostfixBurstDelegate(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, float contractEdge, out NativeArray<Vector3> outVertices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge);

		internal static class ClipEdges_00000305$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (ShadowUtility.ClipEdges_00000305$BurstDirectCall.Pointer == 0)
				{
					ShadowUtility.ClipEdges_00000305$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<ShadowUtility.ClipEdges_00000305$PostfixBurstDelegate>(new ShadowUtility.ClipEdges_00000305$PostfixBurstDelegate(ShadowUtility.ClipEdges)).Value;
				}
				A_0 = ShadowUtility.ClipEdges_00000305$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				ShadowUtility.ClipEdges_00000305$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref NativeArray<Vector3> inVertices, ref NativeArray<ShadowEdge> inEdges, ref NativeArray<int> inShapeStartingEdge, ref NativeArray<bool> inShapeIsClosedArray, float contractEdge, out NativeArray<Vector3> outVertices, out NativeArray<ShadowEdge> outEdges, out NativeArray<int> outShapeStartingEdge)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = ShadowUtility.ClipEdges_00000305$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeArray`1<System.Boolean>&,System.Single,Unity.Collections.NativeArray`1<UnityEngine.Vector3>&,Unity.Collections.NativeArray`1<UnityEngine.Rendering.Universal.ShadowEdge>&,Unity.Collections.NativeArray`1<System.Int32>&), ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, contractEdge, ref outVertices, ref outEdges, ref outShapeStartingEdge, functionPointer);
						return;
					}
				}
				ShadowUtility.ClipEdges$BurstManaged(ref inVertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, contractEdge, out outVertices, out outEdges, out outShapeStartingEdge);
			}

			private static IntPtr Pointer;
		}
	}
}
