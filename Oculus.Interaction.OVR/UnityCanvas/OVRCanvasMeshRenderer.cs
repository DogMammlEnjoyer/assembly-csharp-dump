using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.UnityCanvas
{
	[Feature(Feature.Interaction)]
	public class OVRCanvasMeshRenderer : CanvasMeshRenderer
	{
		private OVRRenderingMode RenderingMode
		{
			get
			{
				return (OVRRenderingMode)this._renderingMode;
			}
		}

		public bool ShouldUseOVROverlay
		{
			get
			{
				OVRRenderingMode renderingMode = this.RenderingMode;
				return renderingMode - OVRRenderingMode.Overlay <= 1 && !this.UseEditorEmulation();
			}
		}

		protected override string GetShaderName()
		{
			OVRRenderingMode renderingMode = this.RenderingMode;
			if (renderingMode == OVRRenderingMode.Overlay)
			{
				return "Hidden/Imposter_AlphaCutout";
			}
			if (renderingMode != OVRRenderingMode.Underlay)
			{
				return base.GetShaderName();
			}
			if (this.UseEditorEmulation())
			{
				return "Hidden/Imposter_AlphaCutout";
			}
			if (this._doUnderlayAntiAliasing)
			{
				return "Hidden/Imposter_Underlay_AA";
			}
			return "Hidden/Imposter_Underlay";
		}

		protected override float GetAlphaCutoutThreshold()
		{
			OVRRenderingMode renderingMode = this.RenderingMode;
			if (renderingMode == OVRRenderingMode.Overlay)
			{
				return 1f;
			}
			if (renderingMode != OVRRenderingMode.Underlay)
			{
				return base.GetAlphaCutoutThreshold();
			}
			if (!this.UseEditorEmulation())
			{
				return 1f;
			}
			return 0.5f;
		}

		protected override void HandleUpdateRenderTexture(Texture texture)
		{
			base.HandleUpdateRenderTexture(texture);
			this.UpdateOverlay(texture);
		}

		private bool UseEditorEmulation()
		{
			return Application.isEditor && this._emulateWhileInEditor;
		}

		private bool GetOverlayParameters(out OVROverlay.OverlayShape shape, out Vector3 position, out Vector3 scale)
		{
			CanvasCylinder canvasCylinder = this._canvasMesh as CanvasCylinder;
			if (canvasCylinder != null)
			{
				shape = OVROverlay.OverlayShape.Cylinder;
				Vector2Int baseResolutionToUse = this._canvasRenderTexture.GetBaseResolutionToUse();
				position = new Vector3(0f, 0f, -canvasCylinder.Radius) - this._runtimeOffset;
				scale = new Vector3(this._canvasRenderTexture.PixelsToUnits((float)baseResolutionToUse.x) / canvasCylinder.transform.lossyScale.x, this._canvasRenderTexture.PixelsToUnits((float)baseResolutionToUse.y) / canvasCylinder.transform.lossyScale.y, canvasCylinder.Radius);
				return true;
			}
			if (this._canvasMesh is CanvasRect)
			{
				shape = OVROverlay.OverlayShape.Quad;
				Vector2Int baseResolutionToUse2 = this._canvasRenderTexture.GetBaseResolutionToUse();
				position = -this._runtimeOffset;
				scale = new Vector3(this._canvasRenderTexture.PixelsToUnits((float)baseResolutionToUse2.x), this._canvasRenderTexture.PixelsToUnits((float)baseResolutionToUse2.y), 1f);
				return true;
			}
			shape = OVROverlay.OverlayShape.Quad;
			position = Vector3.zero;
			scale = Vector3.zero;
			return false;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected void UpdateOverlay(Texture texture)
		{
			try
			{
				if (!this.ShouldUseOVROverlay)
				{
					OVROverlay overlay = this._overlay;
					if (overlay != null)
					{
						GameObject gameObject = overlay.gameObject;
						if (gameObject != null)
						{
							gameObject.SetActive(false);
						}
					}
				}
				else
				{
					if (this._overlay == null)
					{
						GameObject gameObject2 = this.CreateChildObject("__Overlay");
						this._overlay = gameObject2.AddComponent<OVROverlay>();
						this._overlay.isAlphaPremultiplied = !Application.isMobilePlatform;
					}
					else
					{
						this._overlay.gameObject.SetActive(true);
					}
					OVROverlay.OverlayShape currentOverlayShape;
					Vector3 localPosition;
					Vector3 localScale;
					if (!this.GetOverlayParameters(out currentOverlayShape, out localPosition, out localScale))
					{
						this._overlay.gameObject.SetActive(false);
					}
					else
					{
						bool flag = this.RenderingMode == OVRRenderingMode.Underlay;
						this._overlay.textures = new Texture[]
						{
							texture
						};
						this._overlay.noDepthBufferTesting = flag;
						this._overlay.currentOverlayType = (flag ? OVROverlay.OverlayType.Underlay : OVROverlay.OverlayType.Overlay);
						this._overlay.currentOverlayShape = currentOverlayShape;
						this._overlay.useExpensiveSuperSample = this._enableSuperSampling;
						this._overlay.transform.localPosition = localPosition;
						this._overlay.transform.localScale = localScale;
					}
				}
			}
			finally
			{
			}
		}

		protected GameObject CreateChildObject(string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			return gameObject;
		}

		public void InjectAllOVRCanvasMeshRenderer(CanvasRenderTexture canvasRenderTexture, MeshRenderer meshRenderer, CanvasMesh canvasMesh)
		{
			base.InjectAllCanvasMeshRenderer(canvasRenderTexture, meshRenderer);
			this.InjectCanvasMesh(canvasMesh);
		}

		public void InjectCanvasMesh(CanvasMesh canvasMesh)
		{
			this._canvasMesh = canvasMesh;
		}

		public void InjectOptionalRenderingMode(OVRRenderingMode ovrRenderingMode)
		{
			this._renderingMode = (int)ovrRenderingMode;
		}

		public void InjectOptionalDoUnderlayAntiAliasing(bool doUnderlayAntiAliasing)
		{
			this._doUnderlayAntiAliasing = doUnderlayAntiAliasing;
		}

		public void InjectOptionalEnableSuperSampling(bool enableSuperSampling)
		{
			this._enableSuperSampling = enableSuperSampling;
		}

		[SerializeField]
		protected CanvasMesh _canvasMesh;

		[Tooltip("If non-zero it will cause the position of the overlay to be offset by this amount at runtime, while the renderer will remain where it was at edit time. This can be used to prevent the two representations from overlapping.")]
		[SerializeField]
		protected Vector3 _runtimeOffset = new Vector3(0f, 0f, 0f);

		[Tooltip("Uses a more expensive image sampling technique for improved quality at the cost of performance.")]
		[SerializeField]
		protected bool _enableSuperSampling = true;

		[Tooltip("Attempts to anti-alias the edges of the underlay by using alpha blending.  Can cause borders of darkness around partially transparent objects.")]
		[SerializeField]
		private bool _doUnderlayAntiAliasing;

		[Tooltip("OVR Layers can provide a buggy or less ideal workflow while in the editor.  This option allows you emulate the layer rendering while in the editor, while still using the OVR Layer rendering in a build.")]
		[SerializeField]
		private bool _emulateWhileInEditor = true;

		protected OVROverlay _overlay;

		public new static class Properties
		{
			public static readonly string CanvasRenderTexture = "_canvasRenderTexture";

			public static readonly string CanvasMesh = "_canvasMesh";

			public static readonly string EnableSuperSampling = "_enableSuperSampling";

			public static readonly string EmulateWhileInEditor = "_emulateWhileInEditor";

			public static readonly string DoUnderlayAntiAliasing = "_doUnderlayAntiAliasing";

			public static readonly string RuntimeOffset = "_runtimeOffset";
		}
	}
}
