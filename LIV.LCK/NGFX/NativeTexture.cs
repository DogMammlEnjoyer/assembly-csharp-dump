using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Liv.NGFX
{
	public class NativeTexture : IDisposable
	{
		public NativeTexture(IntPtr ctx, int width, int height, NativeTexture.Format format)
		{
			this.m_context = ctx;
			this.m_texture = new Texture2D(width, height, this.FormatToUnity(format), false);
			this.m_id = NI.AllocResource(this.m_context);
			Handle<NativeTexture.TextureCreateInfo> handle = new Handle<NativeTexture.TextureCreateInfo>(new NativeTexture.TextureCreateInfo(ctx, this.m_id, this.m_texture.GetNativeTexturePtr(), width, height, format));
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(NI.GetPluginEventFunction(), (int)NativeTexture.TextureCreateInfo.eventType, handle.ptr());
			Graphics.ExecuteCommandBuffer(commandBuffer);
			this.m_valid = true;
		}

		~NativeTexture()
		{
		}

		public TextureFormat FormatToUnity(NativeTexture.Format fmt)
		{
			if (fmt == NativeTexture.Format.RGBA)
			{
				return TextureFormat.RGBA32;
			}
			if (fmt != NativeTexture.Format.Depth)
			{
				throw new NotSupportedException();
			}
			return TextureFormat.R16;
		}

		public static implicit operator Texture2D(NativeTexture o)
		{
			return o.m_texture;
		}

		public uint id
		{
			get
			{
				return this.m_id;
			}
		}

		public Texture2D texture
		{
			get
			{
				return this.m_texture;
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
				this.m_texture = null;
			}
		}

		private Texture2D m_texture;

		private uint m_id;

		private bool m_valid;

		private IntPtr m_context = IntPtr.Zero;

		public enum Format
		{
			RGBA,
			Depth
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct TextureCreateInfo
		{
			public TextureCreateInfo(IntPtr ctx, uint id, IntPtr handle, int width, int height, NativeTexture.Format format)
			{
				this.m_context = ctx;
				this.m_handle = handle;
				this.m_width = width;
				this.m_height = height;
				this.m_format = format;
				this.m_out_id = id;
			}

			public uint id()
			{
				return this.m_out_id;
			}

			public static EventType eventType = EventType.TextureCreate;

			private IntPtr m_context;

			private IntPtr m_handle;

			private int m_width;

			private int m_height;

			private NativeTexture.Format m_format;

			private uint m_out_id;
		}
	}
}
