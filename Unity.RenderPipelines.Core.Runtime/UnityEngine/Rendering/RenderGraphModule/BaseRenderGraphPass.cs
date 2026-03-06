using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("RenderPass: {name} (Index:{index} Async:{enableAsyncCompute})")]
	internal abstract class BaseRenderGraphPass<PassData, TRenderGraphContext> : RenderGraphPass where PassData : class, new()
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Initialize(int passIndex, PassData passData, string passName, RenderGraphPassType passType, ProfilingSampler sampler)
		{
			base.Clear();
			base.index = passIndex;
			this.data = passData;
			base.name = passName;
			base.type = passType;
			base.customSampler = sampler;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Release(RenderGraphObjectPool pool)
		{
			pool.Release<PassData>(this.data);
			this.data = default(PassData);
			this.renderFunc = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool HasRenderFunc()
		{
			return this.renderFunc != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetRenderFuncHash()
		{
			if (this.renderFunc == null)
			{
				return 0;
			}
			return DelegateHashCodeUtils.GetFuncHashCode(this.renderFunc);
		}

		internal PassData data;

		internal BaseRenderFunc<PassData, TRenderGraphContext> renderFunc;
	}
}
