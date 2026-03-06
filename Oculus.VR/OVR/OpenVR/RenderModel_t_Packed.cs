using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RenderModel_t_Packed
	{
		public RenderModel_t_Packed(RenderModel_t unpacked)
		{
			this.rVertexData = unpacked.rVertexData;
			this.unVertexCount = unpacked.unVertexCount;
			this.rIndexData = unpacked.rIndexData;
			this.unTriangleCount = unpacked.unTriangleCount;
			this.diffuseTextureId = unpacked.diffuseTextureId;
		}

		public void Unpack(ref RenderModel_t unpacked)
		{
			unpacked.rVertexData = this.rVertexData;
			unpacked.unVertexCount = this.unVertexCount;
			unpacked.rIndexData = this.rIndexData;
			unpacked.unTriangleCount = this.unTriangleCount;
			unpacked.diffuseTextureId = this.diffuseTextureId;
		}

		public IntPtr rVertexData;

		public uint unVertexCount;

		public IntPtr rIndexData;

		public uint unTriangleCount;

		public int diffuseTextureId;
	}
}
