using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RenderModel_TextureMap_t_Packed
	{
		public RenderModel_TextureMap_t_Packed(RenderModel_TextureMap_t unpacked)
		{
			this.unWidth = unpacked.unWidth;
			this.unHeight = unpacked.unHeight;
			this.rubTextureMapData = unpacked.rubTextureMapData;
		}

		public void Unpack(ref RenderModel_TextureMap_t unpacked)
		{
			unpacked.unWidth = this.unWidth;
			unpacked.unHeight = this.unHeight;
			unpacked.rubTextureMapData = this.rubTextureMapData;
		}

		public ushort unWidth;

		public ushort unHeight;

		public IntPtr rubTextureMapData;
	}
}
