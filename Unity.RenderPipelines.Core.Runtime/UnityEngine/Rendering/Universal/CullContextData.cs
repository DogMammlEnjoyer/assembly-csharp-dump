using System;

namespace UnityEngine.Rendering.Universal
{
	public class CullContextData : ContextItem
	{
		public override void Reset()
		{
			this.m_RenderContext = null;
		}

		public void SetRenderContext(in ScriptableRenderContext renderContext)
		{
			this.m_RenderContext = new ScriptableRenderContext?(renderContext);
		}

		public CullingResults Cull(ref ScriptableCullingParameters parameters)
		{
			if (this.m_RenderContext == null)
			{
				throw new InvalidOperationException("The ScriptableRenderContext member is not set.");
			}
			return this.m_RenderContext.Value.Cull(ref parameters);
		}

		public void CullShadowCasters(CullingResults cullingResults, ShadowCastersCullingInfos shadowCastersCullingInfos)
		{
			if (this.m_RenderContext == null)
			{
				throw new InvalidOperationException("The ScriptableRenderContext member is not set.");
			}
			this.m_RenderContext.Value.CullShadowCasters(cullingResults, shadowCastersCullingInfos);
		}

		internal ScriptableRenderContext? m_RenderContext;
	}
}
