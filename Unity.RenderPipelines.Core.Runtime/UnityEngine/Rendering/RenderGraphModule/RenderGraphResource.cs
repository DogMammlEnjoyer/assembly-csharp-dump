using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("Resource ({GetType().Name}:{GetName()})")]
	internal abstract class RenderGraphResource<DescType, ResType> : IRenderGraphResource where DescType : struct where ResType : class
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Reset(IRenderGraphResourcePool pool = null)
		{
			base.Reset(null);
			this.m_Pool = (pool as RenderGraphResourcePool<ResType>);
			this.graphicsResource = default(ResType);
			this.validDesc = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsCreated()
		{
			return this.graphicsResource != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ReleaseGraphicsResource()
		{
			this.graphicsResource = default(ResType);
		}

		public override void CreatePooledGraphicsResource()
		{
			int descHashCode = this.GetDescHashCode();
			if (this.graphicsResource != null)
			{
				throw new InvalidOperationException("RenderGraphResource: Trying to create an already created resource (" + this.GetName() + "). Resource was probably declared for writing more than once in the same pass.");
			}
			if (!this.m_Pool.TryGetResource(descHashCode, out this.graphicsResource))
			{
				this.CreateGraphicsResource();
			}
			else
			{
				this.UpdateGraphicsResource();
			}
			this.cachedHash = descHashCode;
		}

		public override void ReleasePooledGraphicsResource(int frameIndex)
		{
			if (this.graphicsResource == null)
			{
				throw new InvalidOperationException("RenderGraphResource: Tried to release a resource (" + this.GetName() + ") that was never created. Check that there is at least one pass writing to it first.");
			}
			if (this.m_Pool != null)
			{
				this.m_Pool.ReleaseResource(this.cachedHash, this.graphicsResource, frameIndex);
			}
			this.Reset(null);
		}

		public DescType desc;

		public bool validDesc;

		public ResType graphicsResource;

		protected RenderGraphResourcePool<ResType> m_Pool;
	}
}
