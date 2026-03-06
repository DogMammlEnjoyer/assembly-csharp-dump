using System;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal struct RendererListResource
	{
		internal RendererListResource(in RendererListParams desc)
		{
			this.desc = desc;
			this.rendererList = default(RendererList);
		}

		public RendererListParams desc;

		public RendererList rendererList;
	}
}
