using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace UnityEngine.Rendering.RadeonRays
{
	internal class RadeonRaysAPI : IDisposable
	{
		public RadeonRaysAPI(RadeonRaysShaders shaders)
		{
			this.buildBvh = new HlbvhBuilder(shaders);
			this.buildTopLevelBvh = new HlbvhTopLevelBuilder(shaders);
			this.restructureBvh = new RestructureBvh(shaders);
		}

		public void Dispose()
		{
			this.restructureBvh.Dispose();
		}

		public static int BvhInternalNodeSizeInDwords()
		{
			return Marshal.SizeOf<BvhNode>() / 4;
		}

		public static int BvhInternalNodeSizeInBytes()
		{
			return Marshal.SizeOf<BvhNode>();
		}

		public static int BvhLeafNodeSizeInBytes()
		{
			return Marshal.SizeOf<uint4>();
		}

		public static int BvhLeafNodeSizeInDwords()
		{
			return Marshal.SizeOf<uint4>() / 4;
		}

		public void BuildMeshAccelStruct(CommandBuffer cmd, MeshBuildInfo buildInfo, BuildFlags buildFlags, GraphicsBuffer scratchBuffer, in BottomLevelLevelAccelStruct result)
		{
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
			{
				buildFlags |= BuildFlags.PreferFastBuild;
			}
			this.buildBvh.Execute(cmd, buildInfo.vertices, buildInfo.verticesStartOffset, buildInfo.vertexStride, buildInfo.triangleIndices, buildInfo.indicesStartOffset, buildInfo.baseIndex, buildInfo.indexFormat, buildInfo.triangleCount, scratchBuffer, result);
			if ((buildFlags & BuildFlags.PreferFastBuild) == BuildFlags.None)
			{
				this.restructureBvh.Execute(cmd, buildInfo.vertices, buildInfo.verticesStartOffset, buildInfo.vertexStride, buildInfo.triangleCount, scratchBuffer, result);
			}
		}

		public MeshBuildMemoryRequirements GetMeshBuildMemoryRequirements(MeshBuildInfo buildInfo, BuildFlags buildFlags)
		{
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
			{
				buildFlags |= BuildFlags.PreferFastBuild;
			}
			MeshBuildMemoryRequirements meshBuildMemoryRequirements = default(MeshBuildMemoryRequirements);
			meshBuildMemoryRequirements.bvhSizeInDwords = (ulong)this.buildBvh.GetResultDataSizeInDwords(buildInfo.triangleCount);
			meshBuildMemoryRequirements.bvhLeavesSizeInDwords = (ulong)buildInfo.triangleCount * (ulong)((long)RadeonRaysAPI.BvhLeafNodeSizeInDwords());
			meshBuildMemoryRequirements.buildScratchSizeInDwords = (ulong)this.buildBvh.GetScratchDataSizeInDwords(buildInfo.triangleCount);
			ulong y = ((buildFlags & BuildFlags.PreferFastBuild) == BuildFlags.None) ? this.restructureBvh.GetScratchDataSizeInDwords(buildInfo.triangleCount) : 0UL;
			meshBuildMemoryRequirements.buildScratchSizeInDwords = math.max(meshBuildMemoryRequirements.buildScratchSizeInDwords, y);
			return meshBuildMemoryRequirements;
		}

		public TopLevelAccelStruct BuildSceneAccelStruct(CommandBuffer cmd, GraphicsBuffer meshAccelStructsBuffer, Instance[] instances, GraphicsBuffer scratchBuffer)
		{
			TopLevelAccelStruct topLevelAccelStruct = default(TopLevelAccelStruct);
			if (instances.Length == 0)
			{
				this.buildTopLevelBvh.CreateEmpty(ref topLevelAccelStruct);
				return topLevelAccelStruct;
			}
			this.buildTopLevelBvh.AllocateResultBuffers((uint)instances.Length, ref topLevelAccelStruct);
			InstanceInfo[] array = new InstanceInfo[instances.Length];
			uint num = 0U;
			while ((ulong)num < (ulong)((long)instances.Length))
			{
				array[(int)num] = new InstanceInfo
				{
					blasOffset = (int)instances[(int)num].meshAccelStructOffset,
					instanceMask = (int)instances[(int)num].instanceMask,
					vertexOffset = (int)instances[(int)num].vertexOffset,
					indexOffset = (int)instances[(int)num].meshAccelStructLeavesOffset,
					localToWorldTransform = instances[(int)num].localToWorldTransform,
					triangleCullingEnabled = (instances[(int)num].triangleCullingEnabled ? 1 : 0),
					invertTriangleCulling = (instances[(int)num].invertTriangleCulling ? 1 : 0),
					userInstanceID = instances[(int)num].userInstanceID
				};
				num += 1U;
			}
			topLevelAccelStruct.instanceInfos.SetData(array);
			topLevelAccelStruct.bottomLevelBvhs = meshAccelStructsBuffer;
			topLevelAccelStruct.instanceCount = (uint)instances.Length;
			this.buildTopLevelBvh.Execute(cmd, scratchBuffer, ref topLevelAccelStruct);
			return topLevelAccelStruct;
		}

		public TopLevelAccelStruct CreateSceneAccelStructBuffers(GraphicsBuffer meshAccelStructsBuffer, uint tlasSizeInDwords, Instance[] instances)
		{
			TopLevelAccelStruct topLevelAccelStruct = default(TopLevelAccelStruct);
			if (instances.Length == 0)
			{
				this.buildTopLevelBvh.CreateEmpty(ref topLevelAccelStruct);
				return topLevelAccelStruct;
			}
			InstanceInfo[] array = new InstanceInfo[instances.Length];
			uint num = 0U;
			while ((ulong)num < (ulong)((long)instances.Length))
			{
				array[(int)num] = new InstanceInfo
				{
					blasOffset = (int)instances[(int)num].meshAccelStructOffset,
					instanceMask = (int)instances[(int)num].instanceMask,
					vertexOffset = (int)instances[(int)num].vertexOffset,
					indexOffset = (int)instances[(int)num].meshAccelStructLeavesOffset,
					localToWorldTransform = instances[(int)num].localToWorldTransform,
					triangleCullingEnabled = (instances[(int)num].triangleCullingEnabled ? 1 : 0),
					invertTriangleCulling = (instances[(int)num].invertTriangleCulling ? 1 : 0),
					userInstanceID = instances[(int)num].userInstanceID,
					worldToLocalTransform = instances[(int)num].localToWorldTransform.Inverse()
				};
				num += 1U;
			}
			topLevelAccelStruct.instanceInfos = new GraphicsBuffer(GraphicsBuffer.Target.Structured, instances.Length, Marshal.SizeOf<InstanceInfo>());
			topLevelAccelStruct.instanceInfos.SetData(array);
			topLevelAccelStruct.bottomLevelBvhs = meshAccelStructsBuffer;
			topLevelAccelStruct.topLevelBvh = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)(tlasSizeInDwords / (uint)RadeonRaysAPI.BvhInternalNodeSizeInDwords()), Marshal.SizeOf<BvhNode>());
			topLevelAccelStruct.instanceCount = (uint)instances.Length;
			return topLevelAccelStruct;
		}

		public SceneBuildMemoryRequirements GetSceneBuildMemoryRequirements(uint instanceCount)
		{
			return new SceneBuildMemoryRequirements
			{
				buildScratchSizeInDwords = this.buildTopLevelBvh.GetScratchDataSizeInDwords(instanceCount)
			};
		}

		public SceneMemoryRequirements GetSceneMemoryRequirements(MeshBuildInfo[] buildInfos, BuildFlags buildFlags)
		{
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
			{
				buildFlags |= BuildFlags.PreferFastBuild;
			}
			SceneMemoryRequirements sceneMemoryRequirements = new SceneMemoryRequirements();
			sceneMemoryRequirements.buildScratchSizeInDwords = 0UL;
			sceneMemoryRequirements.bottomLevelBvhSizeInNodes = new ulong[buildInfos.Length];
			sceneMemoryRequirements.bottomLevelBvhOffsetInNodes = new uint[buildInfos.Length];
			sceneMemoryRequirements.bottomLevelBvhLeavesSizeInNodes = new ulong[buildInfos.Length];
			sceneMemoryRequirements.bottomLevelBvhLeavesOffsetInNodes = new uint[buildInfos.Length];
			int num = 0;
			uint num2 = 0U;
			uint num3 = 0U;
			foreach (MeshBuildInfo buildInfo in buildInfos)
			{
				MeshBuildMemoryRequirements meshBuildMemoryRequirements = this.GetMeshBuildMemoryRequirements(buildInfo, buildFlags);
				sceneMemoryRequirements.buildScratchSizeInDwords = math.max(sceneMemoryRequirements.buildScratchSizeInDwords, meshBuildMemoryRequirements.buildScratchSizeInDwords);
				sceneMemoryRequirements.bottomLevelBvhSizeInNodes[num] = meshBuildMemoryRequirements.bvhSizeInDwords / (ulong)((long)RadeonRaysAPI.BvhInternalNodeSizeInDwords());
				sceneMemoryRequirements.bottomLevelBvhOffsetInNodes[num] = num2;
				sceneMemoryRequirements.bottomLevelBvhLeavesSizeInNodes[num] = meshBuildMemoryRequirements.bvhLeavesSizeInDwords / (ulong)((long)RadeonRaysAPI.BvhLeafNodeSizeInDwords());
				sceneMemoryRequirements.bottomLevelBvhLeavesOffsetInNodes[num] = num3;
				num2 += (uint)(meshBuildMemoryRequirements.bvhSizeInDwords / (ulong)((long)RadeonRaysAPI.BvhInternalNodeSizeInDwords()));
				num3 += (uint)(meshBuildMemoryRequirements.bvhLeavesSizeInDwords / (ulong)((long)RadeonRaysAPI.BvhLeafNodeSizeInDwords()));
				num++;
			}
			sceneMemoryRequirements.totalBottomLevelBvhSizeInNodes = (ulong)num2;
			sceneMemoryRequirements.totalBottomLevelBvhLeavesSizeInNodes = (ulong)num3;
			ulong scratchDataSizeInDwords = this.buildTopLevelBvh.GetScratchDataSizeInDwords((uint)buildInfos.Length);
			sceneMemoryRequirements.buildScratchSizeInDwords = math.max(sceneMemoryRequirements.buildScratchSizeInDwords, scratchDataSizeInDwords);
			return sceneMemoryRequirements;
		}

		public static ulong GetTraceMemoryRequirements(uint rayCount)
		{
			return (ulong)(64U * rayCount);
		}

		private readonly HlbvhBuilder buildBvh;

		private readonly HlbvhTopLevelBuilder buildTopLevelBvh;

		private readonly RestructureBvh restructureBvh;

		public const GraphicsBuffer.Target BufferTarget = GraphicsBuffer.Target.Structured;
	}
}
