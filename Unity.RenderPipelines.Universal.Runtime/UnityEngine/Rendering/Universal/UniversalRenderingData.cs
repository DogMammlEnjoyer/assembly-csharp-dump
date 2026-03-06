using System;

namespace UnityEngine.Rendering.Universal
{
	public class UniversalRenderingData : ContextItem
	{
		internal CommandBuffer commandBuffer
		{
			get
			{
				if (this.m_CommandBuffer == null)
				{
					Debug.LogError("UniversalRenderingData.commandBuffer is null. RenderGraph does not support this property. Please use the command buffer provided by the RenderGraphContext.");
				}
				return this.m_CommandBuffer;
			}
		}

		public RenderingMode renderingMode { get; internal set; }

		public LayerMask prepassLayerMask { get; internal set; }

		public LayerMask opaqueLayerMask { get; internal set; }

		public LayerMask transparentLayerMask { get; internal set; }

		public bool stencilLodCrossFadeEnabled { get; internal set; }

		public override void Reset()
		{
			this.m_CommandBuffer = null;
			this.cullResults = default(CullingResults);
			this.supportsDynamicBatching = false;
			this.perObjectData = PerObjectData.None;
			this.renderingMode = RenderingMode.Forward;
			this.stencilLodCrossFadeEnabled = false;
			this.prepassLayerMask = -1;
			this.opaqueLayerMask = -1;
			this.transparentLayerMask = -1;
		}

		internal CommandBuffer m_CommandBuffer;

		public CullingResults cullResults;

		public bool supportsDynamicBatching;

		public PerObjectData perObjectData;
	}
}
