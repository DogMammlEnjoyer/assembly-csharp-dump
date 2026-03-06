using System;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal struct RendererListLegacyResource
	{
		internal RendererListLegacyResource(in bool active = false)
		{
			this.rendererList = default(RendererList);
			this.isActive = active;
		}

		public RendererList rendererList;

		public bool isActive;
	}
}
