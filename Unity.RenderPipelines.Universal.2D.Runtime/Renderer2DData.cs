using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.Universal
{
	[ReloadGroup]
	[ExcludeFromPreset]
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.Universal", "Unity.RenderPipelines.Universal.Runtime", null)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest/index.html?subfolder=/manual/2DRendererData-overview.html")]
	[Serializable]
	public class Renderer2DData : ScriptableRendererData
	{
		public float hdrEmulationScale
		{
			get
			{
				return this.m_HDREmulationScale;
			}
		}

		internal float lightRenderTextureScale
		{
			get
			{
				return this.m_LightRenderTextureScale;
			}
		}

		public Light2DBlendStyle[] lightBlendStyles
		{
			get
			{
				return this.m_LightBlendStyles;
			}
		}

		internal bool useDepthStencilBuffer
		{
			get
			{
				return this.m_UseDepthStencilBuffer;
			}
		}

		internal PostProcessData postProcessData
		{
			get
			{
				return this.m_PostProcessData;
			}
			set
			{
				this.m_PostProcessData = value;
			}
		}

		internal TransparencySortMode transparencySortMode
		{
			get
			{
				return this.m_TransparencySortMode;
			}
		}

		internal Vector3 transparencySortAxis
		{
			get
			{
				return this.m_TransparencySortAxis;
			}
		}

		internal uint lightRenderTextureMemoryBudget
		{
			get
			{
				return this.m_MaxLightRenderTextureCount;
			}
		}

		internal uint shadowRenderTextureMemoryBudget
		{
			get
			{
				return this.m_MaxShadowRenderTextureCount;
			}
		}

		internal bool useCameraSortingLayerTexture
		{
			get
			{
				return this.m_UseCameraSortingLayersTexture;
			}
		}

		internal int cameraSortingLayerTextureBound
		{
			get
			{
				return this.m_CameraSortingLayersTextureBound;
			}
		}

		internal Downsampling cameraSortingLayerDownsamplingMethod
		{
			get
			{
				return this.m_CameraSortingLayerDownsamplingMethod;
			}
		}

		internal LayerMask layerMask
		{
			get
			{
				return this.m_LayerMask;
			}
		}

		protected override ScriptableRenderer Create()
		{
			return new Renderer2D(this);
		}

		internal void Dispose()
		{
			for (int i = 0; i < this.m_LightBlendStyles.Length; i++)
			{
				RTHandle renderTargetHandle = this.m_LightBlendStyles[i].renderTargetHandle;
				if (renderTargetHandle != null)
				{
					renderTargetHandle.Release();
				}
			}
			foreach (KeyValuePair<uint, Material> keyValuePair in this.lightMaterials)
			{
				CoreUtils.Destroy(keyValuePair.Value);
			}
			this.lightMaterials.Clear();
			CoreUtils.Destroy(this.spriteSelfShadowMaterial);
			CoreUtils.Destroy(this.spriteUnshadowMaterial);
			CoreUtils.Destroy(this.geometrySelfShadowMaterial);
			CoreUtils.Destroy(this.geometryUnshadowMaterial);
			CoreUtils.Destroy(this.projectedShadowMaterial);
			CoreUtils.Destroy(this.projectedUnshadowMaterial);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			for (int i = 0; i < this.m_LightBlendStyles.Length; i++)
			{
				this.m_LightBlendStyles[i].renderTargetHandleId = Shader.PropertyToID(string.Format("_ShapeLightTexture{0}", i));
				this.m_LightBlendStyles[i].renderTargetHandle = RTHandles.Alloc(this.m_LightBlendStyles[i].renderTargetHandleId, string.Format("_ShapeLightTexture{0}", i));
			}
			this.geometrySelfShadowMaterial = null;
			this.geometryUnshadowMaterial = null;
			this.spriteSelfShadowMaterial = null;
			this.spriteUnshadowMaterial = null;
			this.projectedShadowMaterial = null;
			this.projectedUnshadowMaterial = null;
		}

		internal Dictionary<uint, Material> lightMaterials { get; } = new Dictionary<uint, Material>();

		internal Material spriteSelfShadowMaterial { get; set; }

		internal Material spriteUnshadowMaterial { get; set; }

		internal Material geometrySelfShadowMaterial { get; set; }

		internal Material geometryUnshadowMaterial { get; set; }

		internal Material projectedShadowMaterial { get; set; }

		internal Material projectedUnshadowMaterial { get; set; }

		internal ILight2DCullResult lightCullResult { get; set; }

		[SerializeField]
		private LayerMask m_LayerMask = -1;

		[SerializeField]
		private TransparencySortMode m_TransparencySortMode;

		[SerializeField]
		private Vector3 m_TransparencySortAxis = Vector3.up;

		[SerializeField]
		private float m_HDREmulationScale = 1f;

		[SerializeField]
		[Range(0.01f, 1f)]
		private float m_LightRenderTextureScale = 0.5f;

		[SerializeField]
		[FormerlySerializedAs("m_LightOperations")]
		private Light2DBlendStyle[] m_LightBlendStyles;

		[SerializeField]
		private bool m_UseDepthStencilBuffer = true;

		[SerializeField]
		private bool m_UseCameraSortingLayersTexture;

		[SerializeField]
		private int m_CameraSortingLayersTextureBound;

		[SerializeField]
		private Downsampling m_CameraSortingLayerDownsamplingMethod;

		[SerializeField]
		private uint m_MaxLightRenderTextureCount = 16U;

		[SerializeField]
		private uint m_MaxShadowRenderTextureCount = 1U;

		[SerializeField]
		private PostProcessData m_PostProcessData;

		internal RTHandle normalsRenderTarget;

		internal RTHandle cameraSortingLayerRenderTarget;

		internal enum Renderer2DDefaultMaterialType
		{
			Lit,
			Unlit,
			Custom
		}
	}
}
