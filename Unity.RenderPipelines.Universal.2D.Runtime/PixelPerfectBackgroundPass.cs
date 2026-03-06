using System;

namespace UnityEngine.Rendering.Universal
{
	internal class PixelPerfectBackgroundPass : ScriptableRenderPass
	{
		public PixelPerfectBackgroundPass(RenderPassEvent evt)
		{
			base.renderPassEvent = evt;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = *renderingData.commandBuffer;
			using (new ProfilingScope(cmd, PixelPerfectBackgroundPass.m_ProfilingScope))
			{
				CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.Color, Color.black);
			}
		}

		private static readonly ProfilingSampler m_ProfilingScope = new ProfilingSampler("Pixel Perfect Background Pass");
	}
}
