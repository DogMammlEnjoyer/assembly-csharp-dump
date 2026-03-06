using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Rendering.RadeonRays
{
	internal class BvhCheck
	{
		public static BvhCheck.VertexBuffers Convert(MeshBuildInfo info)
		{
			return new BvhCheck.VertexBuffers
			{
				vertices = info.vertices,
				indices = info.triangleIndices,
				vertexBufferOffset = (uint)info.verticesStartOffset,
				vertexCount = info.vertexCount,
				vertexStride = info.vertexStride,
				indexBufferOffset = (uint)info.indicesStartOffset,
				indexCount = info.triangleCount * 3U,
				indexFormat = info.indexFormat
			};
		}

		public static double SurfaceArea(AABB aabb)
		{
			float3 @float = aabb.Max - aabb.Min;
			return (double)(2f * (@float.x * @float.y + @float.x * @float.z + @float.z * @float.y));
		}

		public static double NodeSahCost(uint nodeAddr, AABB nodeAabb, AABB parentAabb)
		{
			double num = (double)(BvhCheck.IsLeafNode(nodeAddr) ? BvhCheck.GetLeafNodePrimCount(nodeAddr) : 1.2f);
			double num2 = BvhCheck.SurfaceArea(nodeAabb);
			double num3 = BvhCheck.SurfaceArea(parentAabb);
			return num * num2 / num3;
		}

		public static double CheckConsistency(BvhCheck.VertexBuffers bvhVertexBuffers, BottomLevelLevelAccelStruct bvh, uint primitiveCount)
		{
			return BvhCheck.CheckConsistency(bvhVertexBuffers, bvh.bvh, bvh.bvhOffset, bvh.bvhLeaves, bvh.bvhLeavesOffset, primitiveCount);
		}

		public static double CheckConsistency(GraphicsBuffer bvhBuffer, uint bvhBufferOffset, uint primitiveCount)
		{
			return BvhCheck.CheckConsistency(null, bvhBuffer, bvhBufferOffset, null, 0U, primitiveCount);
		}

		private static double CheckConsistency(BvhCheck.VertexBuffers bvhVertexBuffers, GraphicsBuffer bvhBuffer, uint bvhBufferOffset, GraphicsBuffer bvhLeavesBuffer, uint bvhLeavesBufferOffset, uint primitiveCount)
		{
			BvhHeader[] array = new BvhHeader[1];
			bvhBuffer.GetData(array, 0, (int)bvhBufferOffset, 1);
			return BvhCheck.CheckConsistency(bvhVertexBuffers, bvhBuffer, bvhBufferOffset + 1U, bvhLeavesBuffer, bvhLeavesBufferOffset, array[0], primitiveCount);
		}

		public static int ExtractBits(uint value, int startBit, int count)
		{
			return (int)((1U << count) - 1U << startBit & value) >> startBit;
		}

		public static bool IsLeafNode(uint nodeAddr)
		{
			return ((ulong)nodeAddr & 18446744071562067968UL) > 0UL;
		}

		public static uint GetLeafNodeFirstPrim(uint nodeAddr)
		{
			return nodeAddr & 536870911U;
		}

		public static uint GetLeafNodePrimCount(uint nodeAddr)
		{
			return (uint)(BvhCheck.ExtractBits(nodeAddr, 29, 2) + 1);
		}

		private static double CheckConsistency(BvhCheck.VertexBuffers bvhVertexBuffers, GraphicsBuffer bvhBuffer, uint bvhBufferOffset, GraphicsBuffer bvhLeavesBuffer, uint bvhLeavesBufferOffset, BvhHeader header, uint primitiveCount)
		{
			uint leafNodeCount = header.leafNodeCount;
			uint root = header.root;
			uint bvhNodeCount = HlbvhBuilder.GetBvhNodeCount(leafNodeCount);
			bool flag = bvhVertexBuffers == null;
			BvhNode[] array = new BvhNode[bvhNodeCount];
			bvhBuffer.GetData(array, 0, (int)bvhBufferOffset, (int)bvhNodeCount);
			BvhCheck.VertexBuffersCPU bvhVertexBuffers2 = null;
			uint4[] array2 = null;
			if (!flag)
			{
				bvhVertexBuffers2 = BvhCheck.DownloadVertexData(bvhVertexBuffers);
				array2 = new uint4[primitiveCount];
				bvhLeavesBuffer.GetData(array2, 0, (int)bvhLeavesBufferOffset, (int)primitiveCount);
			}
			uint num = 0U;
			AABB aabb = BvhCheck.GetAabb(bvhVertexBuffers2, array, array2, root, flag);
			double num2 = 0.0;
			Queue<ValueTuple<uint, uint>> queue = new Queue<ValueTuple<uint, uint>>();
			queue.Enqueue(new ValueTuple<uint, uint>(root, uint.MaxValue));
			while (queue.Count != 0)
			{
				uint item = queue.Dequeue().Item1;
				AABB aabb2 = BvhCheck.GetAabb(bvhVertexBuffers2, array, array2, item, flag);
				num2 += BvhCheck.NodeSahCost(item, aabb2, aabb);
				if (flag)
				{
					BvhCheck.IsLeafNode(item);
				}
				if (BvhCheck.IsLeafNode(item))
				{
					num += (flag ? 1U : BvhCheck.GetLeafNodePrimCount(item));
				}
				else
				{
					BvhNode bvhNode = array[(int)item];
					AABB aabb3 = BvhCheck.GetAabb(bvhVertexBuffers2, array, array2, bvhNode.child0, flag);
					AABB aabb4 = BvhCheck.GetAabb(bvhVertexBuffers2, array, array2, bvhNode.child1, flag);
					aabb2.Contains(aabb3);
					aabb2.Contains(aabb4);
					queue.Enqueue(new ValueTuple<uint, uint>(bvhNode.child0, item));
					queue.Enqueue(new ValueTuple<uint, uint>(bvhNode.child1, item));
				}
			}
			return num2;
		}

		private static uint3 GetFaceIndices(uint[] indices, uint triangleIdx)
		{
			return new uint3(indices[(int)(3U * triangleIdx)], indices[(int)(3U * triangleIdx + 1U)], indices[(int)(3U * triangleIdx + 2U)]);
		}

		private static float3 GetVertex(float[] vertices, uint stride, uint idx)
		{
			uint num = idx * stride;
			return new float3(vertices[(int)num], vertices[(int)(num + 1U)], vertices[(int)(num + 2U)]);
		}

		private static BvhCheck.Triangle GetTriangle(float[] vertices, uint stride, uint3 idx)
		{
			BvhCheck.Triangle result;
			result.v0 = BvhCheck.GetVertex(vertices, stride, idx.x);
			result.v1 = BvhCheck.GetVertex(vertices, stride, idx.y);
			result.v2 = BvhCheck.GetVertex(vertices, stride, idx.z);
			return result;
		}

		private static BvhCheck.VertexBuffersCPU DownloadVertexData(BvhCheck.VertexBuffers vertexBuffers)
		{
			BvhCheck.VertexBuffersCPU vertexBuffersCPU = new BvhCheck.VertexBuffersCPU();
			vertexBuffersCPU.vertices = new float[vertexBuffers.vertexCount * vertexBuffers.vertexStride];
			vertexBuffersCPU.indices = new uint[vertexBuffers.indexCount];
			vertexBuffersCPU.vertexStride = vertexBuffers.vertexStride;
			if (vertexBuffers.indexFormat == IndexFormat.Int32)
			{
				vertexBuffers.indices.GetData(vertexBuffersCPU.indices, 0, (int)vertexBuffers.indexBufferOffset, (int)vertexBuffers.indexCount);
			}
			else
			{
				ushort[] array = new ushort[vertexBuffers.indexCount];
				vertexBuffers.indices.GetData(array, 0, (int)vertexBuffers.indexBufferOffset, (int)vertexBuffers.indexCount);
				int num = 0;
				while ((long)num < (long)((ulong)vertexBuffers.indexCount))
				{
					vertexBuffersCPU.indices[num] = (uint)array[num];
					num++;
				}
			}
			vertexBuffers.vertices.GetData(vertexBuffersCPU.vertices, 0, (int)vertexBuffers.vertexBufferOffset, (int)(vertexBuffers.vertexCount * vertexBuffers.vertexStride));
			return vertexBuffersCPU;
		}

		private static AABB GetAabb(BvhCheck.VertexBuffersCPU bvhVertexBuffers, BvhNode[] bvhNodes, uint4[] bvhLeafNodes, uint nodeAddr, bool isTopLevel)
		{
			AABB aabb = new AABB();
			if (!BvhCheck.IsLeafNode(nodeAddr))
			{
				BvhNode bvhNode = bvhNodes[(int)nodeAddr];
				AABB aabb2 = new AABB(bvhNode.aabb0_min, bvhNode.aabb0_max);
				aabb.Encapsulate(aabb2);
				AABB aabb3 = new AABB(bvhNode.aabb1_min, bvhNode.aabb1_max);
				aabb.Encapsulate(aabb3);
			}
			else if (!isTopLevel)
			{
				int leafNodeFirstPrim = (int)BvhCheck.GetLeafNodeFirstPrim(nodeAddr);
				int leafNodePrimCount = (int)BvhCheck.GetLeafNodePrimCount(nodeAddr);
				for (int i = 0; i < leafNodePrimCount; i++)
				{
					uint num = (uint)(i + leafNodeFirstPrim);
					uint3 xyz = bvhLeafNodes[(int)num].xyz;
					BvhCheck.GetFaceIndices(bvhVertexBuffers.indices, bvhLeafNodes[(int)num].w);
					BvhCheck.Triangle triangle = BvhCheck.GetTriangle(bvhVertexBuffers.vertices, bvhVertexBuffers.vertexStride, xyz);
					aabb.Encapsulate(triangle.v0);
					aabb.Encapsulate(triangle.v1);
					aabb.Encapsulate(triangle.v2);
				}
			}
			return aabb;
		}

		private const uint kInvalidID = 4294967295U;

		public class VertexBuffers
		{
			public GraphicsBuffer vertices;

			public GraphicsBuffer indices;

			public uint vertexBufferOffset;

			public uint vertexCount;

			public uint vertexStride = 3U;

			public uint indexBufferOffset;

			public IndexFormat indexFormat;

			public uint indexCount;
		}

		private sealed class VertexBuffersCPU
		{
			public float[] vertices;

			public uint[] indices;

			public uint vertexStride;
		}

		private struct Triangle
		{
			public float3 v0;

			public float3 v1;

			public float3 v2;
		}
	}
}
