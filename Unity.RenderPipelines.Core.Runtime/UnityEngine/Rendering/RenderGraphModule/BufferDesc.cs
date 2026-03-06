using System;

namespace UnityEngine.Rendering.RenderGraphModule
{
	public struct BufferDesc
	{
		public BufferDesc(int count, int stride)
		{
			this = default(BufferDesc);
			this.count = count;
			this.stride = stride;
			this.target = GraphicsBuffer.Target.Structured;
			this.usageFlags = GraphicsBuffer.UsageFlags.None;
		}

		public BufferDesc(int count, int stride, GraphicsBuffer.Target target)
		{
			this = default(BufferDesc);
			this.count = count;
			this.stride = stride;
			this.target = target;
			this.usageFlags = GraphicsBuffer.UsageFlags.None;
		}

		public override int GetHashCode()
		{
			HashFNV1A32 hashFNV1A = HashFNV1A32.Create();
			hashFNV1A.Append(this.count);
			hashFNV1A.Append(this.stride);
			int num = (int)this.target;
			hashFNV1A.Append(num);
			num = (int)this.usageFlags;
			hashFNV1A.Append(num);
			return hashFNV1A.value;
		}

		public int count;

		public int stride;

		public string name;

		public GraphicsBuffer.Target target;

		public GraphicsBuffer.UsageFlags usageFlags;
	}
}
