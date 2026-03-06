using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Liv.Lck.Rendering
{
	public class LckCompositionRenderFeature : ScriptableRendererFeature
	{
		public override void Create()
		{
			this.m_Pass = new LckCompositionRenderPass();
			this.m_Pass.renderPassEvent = this._renderPassEvent;
		}

		public unsafe override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			LckCamera lckCamera;
			if (!renderingData.cameraData.camera->TryGetComponent<LckCamera>(out lckCamera) && !this._previewInGameWindow)
			{
				return;
			}
			if (this._material == null)
			{
				return;
			}
			if (this._compositionProfile == null || this._compositionProfile.Layers == null)
			{
				return;
			}
			List<ILckCompositionLayer> activeLayers = this._compositionProfile.GetActiveLayers();
			if (activeLayers.Count == 0)
			{
				return;
			}
			ILckCompositionLayer lckCompositionLayer = activeLayers[0];
			if (lckCompositionLayer == null)
			{
				return;
			}
			lckCompositionLayer.BlendMaterial.SetTexture(LckCompositionRenderFeature.OverlayTexID, lckCompositionLayer.CurrentTexture);
			Texture texture = (lckCompositionLayer != null) ? lckCompositionLayer.CurrentTexture : null;
			if (texture == null)
			{
				return;
			}
			this.m_Pass.Setup(this._material, texture);
			renderer.EnqueuePass(this.m_Pass);
		}

		public static readonly int OverlayTexID = Shader.PropertyToID("_OverlayTex");

		[Tooltip("The LCK Composition Profile to source layers from.")]
		[SerializeField]
		private LckCompositionProfile _compositionProfile;

		[Tooltip("The material used when making the blit operation.")]
		[SerializeField]
		private Material _material;

		[Tooltip("The event where to inject the pass.")]
		[SerializeField]
		private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

		[Tooltip("Display the pass on the Game preview windows in editor.")]
		[SerializeField]
		private bool _previewInGameWindow;

		private LckCompositionRenderPass m_Pass;
	}
}
