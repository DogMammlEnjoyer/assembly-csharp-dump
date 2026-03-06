using System;
using System.Runtime.CompilerServices;
using UnityEngine.VFX;

namespace UnityEngine.Rendering
{
	public struct CommandBufferHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RasterCommandBuffer GetRasterCommandBuffer(CommandBuffer baseBuffer)
		{
			CommandBufferHelpers.rasterCmd.m_WrappedCommandBuffer = baseBuffer;
			return CommandBufferHelpers.rasterCmd;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ComputeCommandBuffer GetComputeCommandBuffer(CommandBuffer baseBuffer)
		{
			CommandBufferHelpers.computeCmd.m_WrappedCommandBuffer = baseBuffer;
			return CommandBufferHelpers.computeCmd;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UnsafeCommandBuffer GetUnsafeCommandBuffer(CommandBuffer baseBuffer)
		{
			CommandBufferHelpers.unsafeCmd.m_WrappedCommandBuffer = baseBuffer;
			return CommandBufferHelpers.unsafeCmd;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CommandBuffer GetNativeCommandBuffer(UnsafeCommandBuffer baseBuffer)
		{
			return baseBuffer.m_WrappedCommandBuffer;
		}

		public static void VFXManager_ProcessCameraCommand(Camera cam, UnsafeCommandBuffer cmd, VFXCameraXRSettings camXRSettings, CullingResults results)
		{
			VFXManager.ProcessCameraCommand(cam, cmd.m_WrappedCommandBuffer, camXRSettings, results);
		}

		internal static RasterCommandBuffer rasterCmd = new RasterCommandBuffer(null, null, false);

		internal static ComputeCommandBuffer computeCmd = new ComputeCommandBuffer(null, null, false);

		internal static UnsafeCommandBuffer unsafeCmd = new UnsafeCommandBuffer(null, null, false);
	}
}
