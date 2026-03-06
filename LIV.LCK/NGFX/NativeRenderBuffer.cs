using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Liv.NGFX
{
	public class NativeRenderBuffer : IDisposable
	{
		public NativeRenderBuffer(IntPtr ctx, RenderBuffer rb, int width, int height, int mips, GraphicsFormat format)
		{
			this.m_context = ctx;
			this.m_buffer = rb;
			this.m_mips = mips;
			this.m_id = NI.AllocResource(this.m_context);
			Handle<NativeRenderBuffer.RenderBufferCreateInfo> handle = new Handle<NativeRenderBuffer.RenderBufferCreateInfo>(new NativeRenderBuffer.RenderBufferCreateInfo(ctx, this.m_id, rb.GetNativeRenderBufferPtr(), width, height, mips, format));
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(NI.GetPluginEventFunction(), (int)NativeRenderBuffer.RenderBufferCreateInfo.eventType, handle.ptr());
			Graphics.ExecuteCommandBuffer(commandBuffer);
			this.m_valid = true;
		}

		public NativeRenderBuffer(IntPtr ctx, RenderBuffer rb, IntPtr texturePtr, int width, int height, int mips, GraphicsFormat format)
		{
			this.m_context = ctx;
			this.m_buffer = rb;
			this.m_mips = mips;
			this.m_id = NI.AllocResource(this.m_context);
			Handle<NativeRenderBuffer.RenderBufferCreateInfo> handle = new Handle<NativeRenderBuffer.RenderBufferCreateInfo>(new NativeRenderBuffer.RenderBufferCreateInfo(ctx, this.m_id, texturePtr, width, height, mips, format));
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(NI.GetPluginEventFunction(), (int)NativeRenderBuffer.RenderBufferCreateInfo.eventType, handle.ptr());
			Graphics.ExecuteCommandBuffer(commandBuffer);
			this.m_valid = true;
		}

		~NativeRenderBuffer()
		{
		}

		public static implicit operator RenderBuffer(NativeRenderBuffer o)
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

		public RenderBuffer buffer
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
			}
		}

		private RenderBuffer m_buffer;

		private uint m_id;

		private int m_mips;

		private bool m_valid;

		private IntPtr m_context = IntPtr.Zero;

		public enum Format
		{
			RGBA,
			Depth
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct RenderBufferCreateInfo
		{
			public RenderBufferCreateInfo(IntPtr ctx, uint id, IntPtr handle, int width, int height, int mips, GraphicsFormat format)
			{
				this.m_context = ctx;
				this.m_handle = handle;
				this.m_width = width;
				this.m_height = height;
				this.m_mips = mips;
				this.m_format = format;
				this.m_out_id = id;
			}

			public uint id()
			{
				return this.m_out_id;
			}

			public static EventType eventType = EventType.RenderBufferCreate;

			private IntPtr m_context;

			private IntPtr m_handle;

			private int m_width;

			private int m_height;

			private int m_mips;

			private GraphicsFormat m_format;

			private uint m_out_id;
		}
	}
}
