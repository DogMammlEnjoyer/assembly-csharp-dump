using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal class IRenderGraphResource
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void Reset(IRenderGraphResourcePool _ = null)
		{
			this.imported = false;
			this.shared = false;
			this.sharedExplicitRelease = false;
			this.cachedHash = -1;
			this.transientPassIndex = -1;
			this.sharedResourceLastFrameUsed = -1;
			this.requestFallBack = false;
			this.forceRelease = false;
			this.writeCount = 0U;
			this.readCount = 0U;
			this.version = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual string GetName()
		{
			return "";
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual bool IsCreated()
		{
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void IncrementWriteCount()
		{
			this.writeCount += 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void IncrementReadCount()
		{
			this.readCount += 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual int NewVersion()
		{
			this.version++;
			return this.version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual bool NeedsFallBack()
		{
			return this.requestFallBack && this.writeCount == 0U;
		}

		public virtual void CreatePooledGraphicsResource()
		{
		}

		public virtual void CreateGraphicsResource()
		{
		}

		public virtual void UpdateGraphicsResource()
		{
		}

		public virtual void ReleasePooledGraphicsResource(int frameIndex)
		{
		}

		public virtual void ReleaseGraphicsResource()
		{
		}

		public virtual void LogCreation(RenderGraphLogger logger)
		{
		}

		public virtual void LogRelease(RenderGraphLogger logger)
		{
		}

		public virtual int GetSortIndex()
		{
			return 0;
		}

		public virtual int GetDescHashCode()
		{
			return 0;
		}

		public bool imported;

		public bool shared;

		public bool sharedExplicitRelease;

		public bool requestFallBack;

		public bool forceRelease;

		public uint writeCount;

		public uint readCount;

		public int cachedHash;

		public int transientPassIndex;

		public int sharedResourceLastFrameUsed;

		public int version;
	}
}
