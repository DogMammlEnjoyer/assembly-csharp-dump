using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Liv.NGFX
{
	public class NativeGraphicsBuffer<T> : IDisposable
	{
		public NativeGraphicsBuffer(IntPtr ctx, int count, GraphicsBuffer.Target target)
		{
			this.m_context = ctx;
			this.m_count = count;
			int stride = Marshal.SizeOf(typeof(T));
			this.m_buffer = new GraphicsBuffer(target, count, stride);
			this.m_buffer.GetNativeBufferPtr();
			this.m_id = NI.AllocResource(this.m_context);
			this.m_buffer.name = "NativeGraphicsBuffer " + this.m_id.ToString();
			Handle<NativeGraphicsBuffer<T>.BufferCreateInfo> handle = new Handle<NativeGraphicsBuffer<T>.BufferCreateInfo>(new NativeGraphicsBuffer<T>.BufferCreateInfo(ctx, this.m_id, this.m_buffer.GetNativeBufferPtr(), count, stride, target));
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(NI.GetPluginEventFunction(), (int)NativeGraphicsBuffer<T>.BufferCreateInfo.eventType, handle.ptr());
			Graphics.ExecuteCommandBuffer(commandBuffer);
			this.m_valid = true;
		}

		public NativeGraphicsBuffer(IntPtr ctx, int count, IntPtr nativeBuffer, GraphicsBuffer.Target target)
		{
			this.m_context = ctx;
			this.m_count = count;
			int stride = Marshal.SizeOf(typeof(T));
			this.m_id = NI.AllocResource(this.m_context);
			Handle<NativeGraphicsBuffer<T>.BufferCreateInfo> handle = new Handle<NativeGraphicsBuffer<T>.BufferCreateInfo>(new NativeGraphicsBuffer<T>.BufferCreateInfo(ctx, this.m_id, nativeBuffer, count, stride, target));
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(NI.GetPluginEventFunction(), (int)NativeGraphicsBuffer<T>.BufferCreateInfo.eventType, handle.ptr());
			Graphics.ExecuteCommandBuffer(commandBuffer);
			this.m_valid = true;
		}

		public NativeGraphicsBuffer(IntPtr ctx, GraphicsBuffer buffer, GraphicsBuffer.Target target)
		{
			this.m_context = ctx;
			this.m_count = buffer.count;
			int stride = buffer.stride;
			this.m_buffer = buffer;
			this.m_buffer.GetNativeBufferPtr();
			this.m_id = NI.AllocResource(this.m_context);
			Handle<NativeGraphicsBuffer<T>.BufferCreateInfo> handle = new Handle<NativeGraphicsBuffer<T>.BufferCreateInfo>(new NativeGraphicsBuffer<T>.BufferCreateInfo(ctx, this.m_id, this.m_buffer.GetNativeBufferPtr(), this.count, stride, target));
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(NI.GetPluginEventFunction(), (int)NativeGraphicsBuffer<T>.BufferCreateInfo.eventType, handle.ptr());
			Graphics.ExecuteCommandBuffer(commandBuffer);
			this.m_valid = true;
		}

		~NativeGraphicsBuffer()
		{
		}

		public void BufferCopy(NativeGraphicsBuffer<T> dst, uint size)
		{
			Handle<NativeGraphicsBuffer<T>.BufferCopyInfo> handle = new Handle<NativeGraphicsBuffer<T>.BufferCopyInfo>(new NativeGraphicsBuffer<T>.BufferCopyInfo(this.m_context, this.m_id, dst.m_id, size));
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(NI.GetPluginEventFunction(), (int)NativeGraphicsBuffer<T>.BufferCopyInfo.eventType, handle.ptr());
			Graphics.ExecuteCommandBuffer(commandBuffer);
		}

		public static implicit operator GraphicsBuffer(NativeGraphicsBuffer<T> o)
		{
			return o.m_buffer;
		}

		public uint id
		{
			get
			{
				return this.m_id;
			}
		}

		public int count
		{
			get
			{
				return this.m_count;
			}
		}

		public GraphicsBuffer buffer
		{
			get
			{
				return this.m_buffer;
			}
		}

		public void Dispose()
		{
			if (this.m_valid)
			{
				Handle<ResourceDestroyInfo> handle = new Handle<ResourceDestroyInfo>(new ResourceDestroyInfo(this.m_context, this.m_id));
				CommandBuffer commandBuffer = new CommandBuffer();
				commandBuffer.IssuePluginEventAndData(NI.GetPluginEventFunction(), (int)ResourceDestroyInfo.eventType, handle.ptr());
				Graphics.ExecuteCommandBuffer(commandBuffer);
				this.m_valid = false;
				this.m_buffer.Dispose();
				this.m_buffer = null;
			}
		}

		private GraphicsBuffer m_buffer;

		private uint m_id;

		private int m_count;

		private bool m_valid;

		private IntPtr m_context = IntPtr.Zero;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BufferCreateInfo
		{
			public BufferCreateInfo(IntPtr ctx, uint id, IntPtr handle, int count, int stride, GraphicsBuffer.Target target)
			{
				this.m_context = ctx;
				this.m_handle = handle;
				this.m_count = count;
				this.m_stride = stride;
				this.m_target = target;
				this.m_out_id = id;
			}

			public uint id()
			{
				return this.m_out_id;
			}

			public static EventType eventType;

			private IntPtr m_context;

			private IntPtr m_handle;

			private int m_count;

			private int m_stride;

			private GraphicsBuffer.Target m_target;

			private uint m_out_id;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BufferCopyInfo
		{
			public BufferCopyInfo(IntPtr ctx, uint src, uint dst, uint size)
			{
				this.m_context = ctx;
				this.m_src = src;
				this.m_dst = dst;
				this.m_size = size;
			}

			public static EventType eventType = EventType.GraphicsBufferCopy;

			private IntPtr m_context;

			private uint m_src;

			private uint m_dst;

			private uint m_size;
		}
	}
}
