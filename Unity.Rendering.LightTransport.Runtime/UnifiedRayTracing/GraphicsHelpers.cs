using System;
using Unity.Mathematics;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal static class GraphicsHelpers
	{
		public static void CopyBuffer(ComputeShader copyShader, CommandBuffer cmd, GraphicsBuffer src, int srcOffsetInDWords, GraphicsBuffer dst, int dstOffsetInDwords, int sizeInDWords)
		{
			int i = sizeInDWords;
			cmd.SetComputeBufferParam(copyShader, 0, "_SrcBuffer", src);
			cmd.SetComputeBufferParam(copyShader, 0, "_DstBuffer", dst);
			while (i > 0)
			{
				int num = math.min(i, 134215680);
				cmd.SetComputeIntParam(copyShader, "_SrcOffset", srcOffsetInDWords);
				cmd.SetComputeIntParam(copyShader, "_DstOffset", dstOffsetInDwords);
				cmd.SetComputeIntParam(copyShader, "_Size", num);
				cmd.DispatchCompute(copyShader, 0, GraphicsHelpers.DivUp(num, 2048), 1, 1);
				i -= num;
				srcOffsetInDWords += num;
				dstOffsetInDwords += num;
			}
		}

		public static void CopyBuffer(ComputeShader copyShader, GraphicsBuffer src, int srcOffsetInDWords, GraphicsBuffer dst, int dstOffsetInDwords, int sizeInDwords)
		{
			CommandBuffer commandBuffer = new CommandBuffer();
			GraphicsHelpers.CopyBuffer(copyShader, commandBuffer, src, srcOffsetInDWords, dst, dstOffsetInDwords, sizeInDwords);
			Graphics.ExecuteCommandBuffer(commandBuffer);
		}

		public static void ReallocateBuffer(ComputeShader copyShader, int oldCapacity, int newCapacity, int elementSizeInBytes, ref GraphicsBuffer buffer)
		{
			int stride = buffer.stride;
			GraphicsBuffer graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, newCapacity * elementSizeInBytes / stride, stride);
			GraphicsHelpers.CopyBuffer(copyShader, buffer, 0, graphicsBuffer, 0, oldCapacity * elementSizeInBytes / 4);
			buffer.Dispose();
			buffer = graphicsBuffer;
		}

		public static int DivUp(int x, int y)
		{
			return (x + y - 1) / y;
		}

		public static int DivUp(int x, uint y)
		{
			return (x + (int)y - 1) / (int)y;
		}

		public static uint DivUp(uint x, uint y)
		{
			return (x + y - 1U) / y;
		}

		public static uint3 DivUp(uint3 x, uint3 y)
		{
			return (x + y - 1U) / y;
		}

		public static void Flush(CommandBuffer cmd)
		{
			Graphics.ExecuteCommandBuffer(cmd);
			cmd.Clear();
			GL.Flush();
		}

		public const int MaxGraphicsBufferSizeInBytes = 2147483647;
	}
}
