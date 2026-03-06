using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal struct ResourceUnversionedData
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetName(CompilerContextData ctx, ResourceHandle h)
		{
			return ctx.GetResourceName(h);
		}

		public ResourceUnversionedData(IRenderGraphResource rll, ref RenderTargetInfo info, ref TextureDesc desc, bool isResourceShared)
		{
			this.isImported = rll.imported;
			this.isShared = isResourceShared;
			this.tag = 0;
			this.firstUsePassID = -1;
			this.lastUsePassID = -1;
			this.lastWritePassID = -1;
			this.memoryLess = false;
			this.width = info.width;
			this.height = info.height;
			this.volumeDepth = info.volumeDepth;
			this.msaaSamples = info.msaaSamples;
			this.latestVersionNumber = rll.version;
			this.clear = desc.clearBuffer;
			this.discard = desc.discardBuffer;
			this.bindMS = info.bindMS;
		}

		public ResourceUnversionedData(IRenderGraphResource rll, ref BufferDesc _, bool isResourceShared)
		{
			this.isImported = rll.imported;
			this.isShared = isResourceShared;
			this.tag = 0;
			this.firstUsePassID = -1;
			this.lastUsePassID = -1;
			this.lastWritePassID = -1;
			this.memoryLess = false;
			this.width = -1;
			this.height = -1;
			this.volumeDepth = -1;
			this.msaaSamples = -1;
			this.latestVersionNumber = rll.version;
			this.clear = false;
			this.discard = false;
			this.bindMS = false;
		}

		public ResourceUnversionedData(IRenderGraphResource rll, ref RayTracingAccelerationStructureDesc _, bool isResourceShared)
		{
			this.isImported = rll.imported;
			this.isShared = isResourceShared;
			this.tag = 0;
			this.firstUsePassID = -1;
			this.lastUsePassID = -1;
			this.lastWritePassID = -1;
			this.memoryLess = false;
			this.width = -1;
			this.height = -1;
			this.volumeDepth = -1;
			this.msaaSamples = -1;
			this.latestVersionNumber = rll.version;
			this.clear = false;
			this.discard = false;
			this.bindMS = false;
		}

		public void InitializeNullResource()
		{
			this.firstUsePassID = -1;
			this.lastUsePassID = -1;
			this.lastWritePassID = -1;
		}

		public readonly bool isImported;

		public bool isShared;

		public int tag;

		public int lastUsePassID;

		public int lastWritePassID;

		public int firstUsePassID;

		public bool memoryLess;

		public readonly int width;

		public readonly int height;

		public readonly int volumeDepth;

		public readonly int msaaSamples;

		public int latestVersionNumber;

		public readonly bool clear;

		public readonly bool discard;

		public readonly bool bindMS;
	}
}
