using System;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal readonly struct TextureAccess
	{
		public TextureAccess(TextureHandle handle, AccessFlags flags, int mipLevel, int depthSlice)
		{
			this.textureHandle = handle;
			this.flags = flags;
			this.mipLevel = mipLevel;
			this.depthSlice = depthSlice;
		}

		public TextureAccess(TextureAccess access, TextureHandle handle)
		{
			this.textureHandle = handle;
			this.flags = access.flags;
			this.mipLevel = access.mipLevel;
			this.depthSlice = access.depthSlice;
		}

		public readonly TextureHandle textureHandle;

		public readonly int mipLevel;

		public readonly int depthSlice;

		public readonly AccessFlags flags;
	}
}
