using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class InvokeOnRenderObjectCallbackPass : ScriptableRenderPass
	{
		public InvokeOnRenderObjectCallbackPass(RenderPassEvent evt)
		{
			base.profilingSampler = new ProfilingSampler("Invoke OnRenderObject Callback");
			base.renderPassEvent = evt;
			base.useNativeRenderPass = false;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			renderingData.commandBuffer->InvokeOnRenderObjectCallbacks();
		}

		internal void Render(RenderGraph renderGraph, TextureHandle colorTarget, TextureHandle depthTarget)
		{
			InvokeOnRenderObjectCallbackPass.PassData passData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<InvokeOnRenderObjectCallbackPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\InvokeOnRenderObjectCallbackPass.cs", 36))
			{
				passData.colorTarget = colorTarget;
				unsafeRenderGraphBuilder.UseTexture(colorTarget, AccessFlags.Write);
				passData.depthTarget = depthTarget;
				unsafeRenderGraphBuilder.UseTexture(depthTarget, AccessFlags.Write);
				unsafeRenderGraphBuilder.AllowPassCulling(false);
				unsafeRenderGraphBuilder.SetRenderFunc<InvokeOnRenderObjectCallbackPass.PassData>(delegate(InvokeOnRenderObjectCallbackPass.PassData data, UnsafeGraphContext context)
				{
					context.cmd.SetRenderTarget(data.colorTarget, data.depthTarget);
					context.cmd.InvokeOnRenderObjectCallbacks();
				});
			}
		}

		private class PassData
		{
			internal TextureHandle colorTarget;

			internal TextureHandle depthTarget;
		}
	}
}
