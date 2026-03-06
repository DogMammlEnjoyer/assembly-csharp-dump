using System;

namespace UnityEngine.Rendering.Universal
{
	public struct RenderingData
	{
		internal RenderingData(ContextContainer frameData)
		{
			this.frameData = frameData;
			this.cameraData = new CameraData(frameData);
			this.lightData = new LightData(frameData);
			this.shadowData = new ShadowData(frameData);
			this.postProcessingData = new PostProcessingData(frameData);
		}

		internal UniversalRenderingData universalRenderingData
		{
			get
			{
				return this.frameData.Get<UniversalRenderingData>();
			}
		}

		internal ref CommandBuffer commandBuffer
		{
			get
			{
				UniversalRenderingData universalRenderingData = this.frameData.Get<UniversalRenderingData>();
				if (universalRenderingData.m_CommandBuffer == null)
				{
					Debug.LogError("RenderingData.commandBuffer is null. RenderGraph does not support this property. Please use the command buffer provided by the RenderGraphContext.");
				}
				return ref universalRenderingData.m_CommandBuffer;
			}
		}

		public ref CullingResults cullResults
		{
			get
			{
				return ref this.frameData.Get<UniversalRenderingData>().cullResults;
			}
		}

		public ref bool supportsDynamicBatching
		{
			get
			{
				return ref this.frameData.Get<UniversalRenderingData>().supportsDynamicBatching;
			}
		}

		public ref PerObjectData perObjectData
		{
			get
			{
				return ref this.frameData.Get<UniversalRenderingData>().perObjectData;
			}
		}

		public ref bool postProcessingEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalPostProcessingData>().isEnabled;
			}
		}

		internal ContextContainer frameData;

		public CameraData cameraData;

		public LightData lightData;

		public ShadowData shadowData;

		public PostProcessingData postProcessingData;
	}
}
