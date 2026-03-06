using System;

namespace UnityEngine.Rendering.RadeonRays
{
	internal struct TopLevelAccelStruct : IDisposable
	{
		public void Dispose()
		{
			GraphicsBuffer graphicsBuffer = this.topLevelBvh;
			if (graphicsBuffer != null)
			{
				graphicsBuffer.Dispose();
			}
			GraphicsBuffer graphicsBuffer2 = this.instanceInfos;
			if (graphicsBuffer2 == null)
			{
				return;
			}
			graphicsBuffer2.Dispose();
		}

		public const GraphicsBuffer.Target topLevelBvhTarget = GraphicsBuffer.Target.Structured;

		public const GraphicsBuffer.Target instanceInfoTarget = GraphicsBuffer.Target.Structured;

		public GraphicsBuffer topLevelBvh;

		public GraphicsBuffer bottomLevelBvhs;

		public GraphicsBuffer instanceInfos;

		public uint instanceCount;
	}
}
