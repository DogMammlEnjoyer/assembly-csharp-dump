using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal static class RenderingLayerUtils
	{
		public static void CombineRendererEvents(bool isDeferred, int msaaSampleCount, RenderingLayerUtils.Event rendererEvent, ref RenderingLayerUtils.Event combinedEvent)
		{
			if (msaaSampleCount > 1 && !isDeferred)
			{
				combinedEvent = RenderingLayerUtils.Event.DepthNormalPrePass;
				return;
			}
			combinedEvent = RenderingLayerUtils.Combine(combinedEvent, rendererEvent);
		}

		public static bool RequireRenderingLayers(UniversalRenderer universalRenderer, List<ScriptableRendererFeature> rendererFeatures, int msaaSampleCount, out RenderingLayerUtils.Event combinedEvent, out RenderingLayerUtils.MaskSize combinedMaskSize)
		{
			RenderingMode renderingModeActual = universalRenderer.renderingModeActual;
			bool accurateGbufferNormals = universalRenderer.accurateGbufferNormals;
			return RenderingLayerUtils.RequireRenderingLayers(rendererFeatures, renderingModeActual, accurateGbufferNormals, msaaSampleCount, out combinedEvent, out combinedMaskSize);
		}

		internal static bool RequireRenderingLayers(List<ScriptableRendererFeature> rendererFeatures, RenderingMode renderingMode, bool accurateGbufferNormals, int msaaSampleCount, out RenderingLayerUtils.Event combinedEvent, out RenderingLayerUtils.MaskSize combinedMaskSize)
		{
			combinedEvent = RenderingLayerUtils.Event.Opaque;
			combinedMaskSize = RenderingLayerUtils.MaskSize.Bits8;
			bool isDeferred = renderingMode == RenderingMode.Deferred || renderingMode == RenderingMode.DeferredPlus;
			bool flag = false;
			foreach (ScriptableRendererFeature scriptableRendererFeature in rendererFeatures)
			{
				if (scriptableRendererFeature.isActive)
				{
					RenderingLayerUtils.Event b;
					RenderingLayerUtils.MaskSize b2;
					flag |= scriptableRendererFeature.RequireRenderingLayers(isDeferred, accurateGbufferNormals, out b, out b2);
					combinedEvent = RenderingLayerUtils.Combine(combinedEvent, b);
					combinedMaskSize = RenderingLayerUtils.Combine(combinedMaskSize, b2);
				}
			}
			if (msaaSampleCount > 1 && combinedEvent == RenderingLayerUtils.Event.Opaque)
			{
				combinedEvent = RenderingLayerUtils.Event.DepthNormalPrePass;
			}
			if (RenderPipelineGlobalSettings<UniversalRenderPipelineGlobalSettings, UniversalRenderPipeline>.instance)
			{
				RenderingLayerUtils.MaskSize maskSize = RenderingLayerUtils.GetMaskSize(RenderingLayerMask.GetRenderingLayerCount());
				combinedMaskSize = RenderingLayerUtils.Combine(combinedMaskSize, maskSize);
			}
			return flag;
		}

		public static void SetupProperties(CommandBuffer cmd, RenderingLayerUtils.MaskSize maskSize)
		{
			RenderingLayerUtils.SetupProperties(CommandBufferHelpers.GetRasterCommandBuffer(cmd), maskSize);
		}

		internal static void SetupProperties(RasterCommandBuffer cmd, RenderingLayerUtils.MaskSize maskSize)
		{
			int bits = RenderingLayerUtils.GetBits(maskSize);
			uint value = (bits != 32) ? ((1U << bits) - 1U) : uint.MaxValue;
			cmd.SetGlobalInt(ShaderPropertyId.renderingLayerMaxInt, (int)value);
		}

		public static GraphicsFormat GetFormat(RenderingLayerUtils.MaskSize maskSize)
		{
			switch (maskSize)
			{
			case RenderingLayerUtils.MaskSize.Bits8:
				return GraphicsFormat.R8_UInt;
			case RenderingLayerUtils.MaskSize.Bits16:
				return GraphicsFormat.R16_UInt;
			case RenderingLayerUtils.MaskSize.Bits24:
			case RenderingLayerUtils.MaskSize.Bits32:
				return GraphicsFormat.R32_UInt;
			default:
				throw new NotImplementedException();
			}
		}

		public static uint ToValidRenderingLayers(uint renderingLayers)
		{
			if (RenderPipelineGlobalSettings<UniversalRenderPipelineGlobalSettings, UniversalRenderPipeline>.instance)
			{
				return RenderingLayerMask.GetDefinedRenderingLayersCombinedMaskValue() & renderingLayers;
			}
			return renderingLayers;
		}

		private static RenderingLayerUtils.MaskSize GetMaskSize(int bits)
		{
			switch ((bits + 7) / 8)
			{
			case 0:
				return RenderingLayerUtils.MaskSize.Bits8;
			case 1:
				return RenderingLayerUtils.MaskSize.Bits8;
			case 2:
				return RenderingLayerUtils.MaskSize.Bits16;
			case 3:
				return RenderingLayerUtils.MaskSize.Bits24;
			case 4:
				return RenderingLayerUtils.MaskSize.Bits32;
			default:
				return RenderingLayerUtils.MaskSize.Bits32;
			}
		}

		private static int GetBits(RenderingLayerUtils.MaskSize maskSize)
		{
			switch (maskSize)
			{
			case RenderingLayerUtils.MaskSize.Bits8:
				return 8;
			case RenderingLayerUtils.MaskSize.Bits16:
				return 16;
			case RenderingLayerUtils.MaskSize.Bits24:
				return 24;
			case RenderingLayerUtils.MaskSize.Bits32:
				return 32;
			default:
				throw new NotImplementedException();
			}
		}

		private static RenderingLayerUtils.Event Combine(RenderingLayerUtils.Event a, RenderingLayerUtils.Event b)
		{
			return (RenderingLayerUtils.Event)Mathf.Min((int)a, (int)b);
		}

		private static RenderingLayerUtils.MaskSize Combine(RenderingLayerUtils.MaskSize a, RenderingLayerUtils.MaskSize b)
		{
			return (RenderingLayerUtils.MaskSize)Mathf.Max((int)a, (int)b);
		}

		public enum Event
		{
			DepthNormalPrePass,
			Opaque
		}

		public enum MaskSize
		{
			Bits8,
			Bits16,
			Bits24,
			Bits32
		}
	}
}
