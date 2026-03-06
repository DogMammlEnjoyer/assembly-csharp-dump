using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RenderModel_TextureMap_t_Packed
	{
		public RenderModel_TextureMap_t_Packed(RenderModel_TextureMap_t unpacked)
		{
			this.unWidth = unpacked.unWidth;
			this.unHeight = unpacked.unHeight;
			this.rubTextureMapData = unpacked.rubTextureMapData;
			this.format = unpacked.format;
		}

		public void Unpack(ref RenderModel_TextureMap_t unpacked)
		{
			unpacked.unWidth = this.unWidth;
			unpacked.unHeight = this.unHeight;
			unpacked.rubTextureMapData = this.rubTextureMapData;
			unpacked.format = this.format;
		}

		public ushort unWidth;

		public ushort unHeight;

		public IntPtr rubTextureMapData;

		public EVRRenderModelTextureFormat format;
	}
}
