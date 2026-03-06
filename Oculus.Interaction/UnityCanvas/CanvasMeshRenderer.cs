using System;
using UnityEngine;

namespace Oculus.Interaction.UnityCanvas
{
	public class CanvasMeshRenderer : MonoBehaviour
	{
		private RenderingMode RenderingMode
		{
			get
			{
				return (RenderingMode)this._renderingMode;
			}
		}

		protected virtual string GetShaderName()
		{
			switch (this.RenderingMode)
			{
			case RenderingMode.AlphaBlended:
				return "Hidden/Imposter_AlphaBlended";
			case RenderingMode.AlphaCutout:
				if (this._useAlphaToMask)
				{
					return "Hidden/Imposter_AlphaToMask";
				}
				return "Hidden/Imposter_AlphaCutout";
			}
			return "Hidden/Imposter_Opaque";
		}

		protected virtual void SetAdditionalProperties(MaterialPropertyBlock block)
		{
			block.SetFloat("_Cutoff", this.GetAlphaCutoutThreshold());
		}

		protected virtual float GetAlphaCutoutThreshold()
		{
			if (this.RenderingMode == RenderingMode.AlphaCutout && !this._useAlphaToMask)
			{
				return this._alphaCutoutThreshold;
			}
			return 1f;
		}

		protected virtual void HandleUpdateRenderTexture(Texture texture)
		{
			this._meshRenderer.material = this._material;
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			this._meshRenderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetTexture(CanvasMeshRenderer.MainTexShaderID, texture);
			this.SetAdditionalProperties(materialPropertyBlock);
			this._meshRenderer.SetPropertyBlock(materialPropertyBlock);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				try
				{
					this._material = new Material(Shader.Find(this.GetShaderName()));
				}
				finally
				{
				}
				CanvasRenderTexture canvasRenderTexture = this._canvasRenderTexture;
				canvasRenderTexture.OnUpdateRenderTexture = (Action<Texture>)Delegate.Combine(canvasRenderTexture.OnUpdateRenderTexture, new Action<Texture>(this.HandleUpdateRenderTexture));
				if (this._canvasRenderTexture.Texture != null)
				{
					this.HandleUpdateRenderTexture(this._canvasRenderTexture.Texture);
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				if (this._material != null)
				{
					Object.Destroy(this._material);
					this._material = null;
				}
				CanvasRenderTexture canvasRenderTexture = this._canvasRenderTexture;
				canvasRenderTexture.OnUpdateRenderTexture = (Action<Texture>)Delegate.Remove(canvasRenderTexture.OnUpdateRenderTexture, new Action<Texture>(this.HandleUpdateRenderTexture));
			}
		}

		public void InjectAllCanvasMeshRenderer(CanvasRenderTexture canvasRenderTexture, MeshRenderer meshRenderer)
		{
			this.InjectCanvasRenderTexture(canvasRenderTexture);
			this.InjectMeshRenderer(meshRenderer);
		}

		public void InjectCanvasRenderTexture(CanvasRenderTexture canvasRenderTexture)
		{
			this._canvasRenderTexture = canvasRenderTexture;
		}

		public void InjectMeshRenderer(MeshRenderer meshRenderer)
		{
			this._meshRenderer = meshRenderer;
		}

		public void InjectOptionalRenderingMode(RenderingMode renderingMode)
		{
			this._renderingMode = (int)renderingMode;
		}

		public void InjectOptionalAlphaCutoutThreshold(float alphaCutoutThreshold)
		{
			this._alphaCutoutThreshold = alphaCutoutThreshold;
		}

		public void InjectOptionalUseAlphaToMask(bool useAlphaToMask)
		{
			this._useAlphaToMask = useAlphaToMask;
		}

		private static readonly int MainTexShaderID = Shader.PropertyToID("_MainTex");

		[Tooltip("The canvas texture that will be rendered.")]
		[SerializeField]
		protected CanvasRenderTexture _canvasRenderTexture;

		[Tooltip("The mesh renderer that will be driven.")]
		[SerializeField]
		protected MeshRenderer _meshRenderer;

		[Tooltip("Determines the shader used for rendering. For details on these rendering modes, see the Curved Canvas topic in the documentation.")]
		[SerializeField]
		protected int _renderingMode = 1;

		[Tooltip("Requires MSAA. Provides limited transparency useful for anti-aliasing soft edges of UI elements.")]
		[SerializeField]
		private bool _useAlphaToMask = true;

		[Tooltip("Select the alpha cutoff used for the cutout rendering.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float _alphaCutoutThreshold = 0.5f;

		protected Material _material;

		protected bool _started;

		public static class Properties
		{
			public static readonly string RenderingMode = "_renderingMode";

			public static readonly string UseAlphaToMask = "_useAlphaToMask";

			public static readonly string AlphaCutoutThreshold = "_alphaCutoutThreshold";
		}
	}
}
