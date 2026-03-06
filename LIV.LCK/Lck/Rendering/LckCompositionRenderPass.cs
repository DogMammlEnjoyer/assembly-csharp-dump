using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Liv.Lck.Rendering
{
	public class LckCompositionRenderPass : ScriptableRenderPass
	{
		public void Setup(Material mat, Texture overlayTexture)
		{
			this._blitMaterial = mat;
			this._overlayTexture = overlayTexture;
			base.requiresIntermediateTexture = true;
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			if (universalResourceData.isActiveTargetBackBuffer)
			{
				return;
			}
			if (this._overlayTexture == null)
			{
				return;
			}
			this._blitMaterial.SetTexture(LckCompositionRenderPass.OverlayTexID, this._overlayTexture);
			TextureHandle activeColorTexture = universalResourceData.activeColorTexture;
			TextureDesc textureDesc = renderGraph.GetTextureDesc(activeColorTexture);
			textureDesc.name = "CameraColor-LckCompositionRenderPass";
			textureDesc.clearBuffer = false;
			TextureHandle textureHandle = renderGraph.CreateTexture(textureDesc);
			RenderGraphUtils.BlitMaterialParameters blitParameters = new RenderGraphUtils.BlitMaterialParameters(activeColorTexture, textureHandle, this._blitMaterial, 0);
			renderGraph.AddBlitPass(blitParameters, "LckCompositionRenderPass", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Rendering\\LckCompositionRenderPass.cs", 48);
			universalResourceData.cameraColor = textureHandle;
		}

		private const string PassName = "LckCompositionRenderPass";

		private static readonly int OverlayTexID = Shader.PropertyToID("_OverlayTex");

		private Material _blitMaterial;

		private Texture _overlayTexture;
	}
}
