using System;

namespace UnityEngine.Rendering
{
	public static class CommandBufferPool
	{
		public static CommandBuffer Get()
		{
			CommandBuffer commandBuffer = CommandBufferPool.s_BufferPool.Get();
			commandBuffer.name = "";
			return commandBuffer;
		}

		public static CommandBuffer Get(string name)
		{
			CommandBuffer commandBuffer = CommandBufferPool.s_BufferPool.Get();
			commandBuffer.name = name;
			return commandBuffer;
		}

		public static void Release(CommandBuffer buffer)
		{
			CommandBufferPool.s_BufferPool.Release(buffer);
		}

		private static ObjectPool<CommandBuffer> s_BufferPool = new ObjectPool<CommandBuffer>(null, delegate(CommandBuffer x)
		{
			x.Clear();
		}, true);
	}
}
