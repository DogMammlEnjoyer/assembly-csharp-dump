using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal static class RayTracingHelper
	{
		public static GraphicsBuffer CreateDispatchDimensionBuffer()
		{
			return new GraphicsBuffer(GraphicsBuffer.Target.CopySource | GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.IndirectArguments, 6, 4);
		}

		public static GraphicsBuffer CreateScratchBufferForBuildAndDispatch(IRayTracingAccelStruct accelStruct, IRayTracingShader shader, uint dispatchWidth, uint dispatchHeight, uint dispatchDepth)
		{
			ulong num = Math.Max(accelStruct.GetBuildScratchBufferRequiredSizeInBytes(), shader.GetTraceScratchBufferRequiredSizeInBytes(dispatchWidth, dispatchHeight, dispatchDepth));
			if (num == 0UL)
			{
				return null;
			}
			return new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)(num / 4UL), 4);
		}

		public static GraphicsBuffer CreateScratchBufferForBuild(IRayTracingAccelStruct accelStruct)
		{
			ulong buildScratchBufferRequiredSizeInBytes = accelStruct.GetBuildScratchBufferRequiredSizeInBytes();
			return new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)(buildScratchBufferRequiredSizeInBytes / 4UL), 4);
		}

		public static void ResizeScratchBufferForTrace(IRayTracingShader shader, uint dispatchWidth, uint dispatchHeight, uint dispatchDepth, ref GraphicsBuffer scratchBuffer)
		{
			ulong traceScratchBufferRequiredSizeInBytes = shader.GetTraceScratchBufferRequiredSizeInBytes(dispatchWidth, dispatchHeight, dispatchDepth);
			if (traceScratchBufferRequiredSizeInBytes == 0UL)
			{
				return;
			}
			if (scratchBuffer == null || (long)(scratchBuffer.count * scratchBuffer.stride) < (long)traceScratchBufferRequiredSizeInBytes)
			{
				GraphicsBuffer graphicsBuffer = scratchBuffer;
				if (graphicsBuffer != null)
				{
					graphicsBuffer.Dispose();
				}
				scratchBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)(traceScratchBufferRequiredSizeInBytes / 4UL), 4);
			}
		}

		public static void ResizeScratchBufferForBuild(IRayTracingAccelStruct accelStruct, ref GraphicsBuffer scratchBuffer)
		{
			ulong buildScratchBufferRequiredSizeInBytes = accelStruct.GetBuildScratchBufferRequiredSizeInBytes();
			if (buildScratchBufferRequiredSizeInBytes == 0UL)
			{
				return;
			}
			if (scratchBuffer == null || (long)(scratchBuffer.count * scratchBuffer.stride) < (long)buildScratchBufferRequiredSizeInBytes)
			{
				GraphicsBuffer graphicsBuffer = scratchBuffer;
				if (graphicsBuffer != null)
				{
					graphicsBuffer.Dispose();
				}
				scratchBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)(buildScratchBufferRequiredSizeInBytes / 4UL), 4);
			}
		}

		public const GraphicsBuffer.Target ScratchBufferTarget = GraphicsBuffer.Target.Structured;

		public static readonly uint k_DimensionByteOffset = 0U;

		public static readonly uint k_GroupSizeByteOffset = 12U;
	}
}
