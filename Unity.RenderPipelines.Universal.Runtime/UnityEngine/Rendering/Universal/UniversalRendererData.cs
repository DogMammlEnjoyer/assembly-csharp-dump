using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	[ReloadGroup]
	[ExcludeFromPreset]
	[Serializable]
	public class UniversalRendererData : ScriptableRendererData, ISerializationCallbackReceiver
	{
		protected override ScriptableRenderer Create()
		{
			if (!Application.isPlaying)
			{
				this.ReloadAllNullProperties();
			}
			return new UniversalRenderer(this);
		}

		public LayerMask prepassLayerMask
		{
			get
			{
				return this.m_PrepassLayerMask;
			}
			set
			{
				base.SetDirty();
				this.m_PrepassLayerMask = value;
			}
		}

		public LayerMask opaqueLayerMask
		{
			get
			{
				return this.m_OpaqueLayerMask;
			}
			set
			{
				base.SetDirty();
				this.m_OpaqueLayerMask = value;
			}
		}

		public LayerMask transparentLayerMask
		{
			get
			{
				return this.m_TransparentLayerMask;
			}
			set
			{
				base.SetDirty();
				this.m_TransparentLayerMask = value;
			}
		}

		public StencilStateData defaultStencilState
		{
			get
			{
				return this.m_DefaultStencilState;
			}
			set
			{
				base.SetDirty();
				this.m_DefaultStencilState = value;
			}
		}

		public bool shadowTransparentReceive
		{
			get
			{
				return this.m_ShadowTransparentReceive;
			}
			set
			{
				base.SetDirty();
				this.m_ShadowTransparentReceive = value;
			}
		}

		public RenderingMode renderingMode
		{
			get
			{
				return this.m_RenderingMode;
			}
			set
			{
				base.SetDirty();
				this.m_RenderingMode = value;
			}
		}

		public DepthPrimingMode depthPrimingMode
		{
			get
			{
				return this.m_DepthPrimingMode;
			}
			set
			{
				base.SetDirty();
				this.m_DepthPrimingMode = value;
			}
		}

		public CopyDepthMode copyDepthMode
		{
			get
			{
				return this.m_CopyDepthMode;
			}
			set
			{
				base.SetDirty();
				this.m_CopyDepthMode = value;
			}
		}

		public DepthFormat depthAttachmentFormat
		{
			get
			{
				if (this.m_DepthAttachmentFormat != DepthFormat.Default && !SystemInfo.IsFormatSupported((GraphicsFormat)this.m_DepthAttachmentFormat, GraphicsFormatUsage.Render))
				{
					Debug.LogWarning("Selected Depth Attachment Format is not supported on this platform, falling back to Default");
					return DepthFormat.Default;
				}
				return this.m_DepthAttachmentFormat;
			}
			set
			{
				base.SetDirty();
				if (this.renderingMode == RenderingMode.Deferred && !GraphicsFormatUtility.IsStencilFormat((GraphicsFormat)value))
				{
					Debug.LogWarning("Depth format without stencil is not supported on Deferred renderer, falling back to Default");
					this.m_DepthAttachmentFormat = DepthFormat.Default;
					return;
				}
				this.m_DepthAttachmentFormat = value;
			}
		}

		public DepthFormat depthTextureFormat
		{
			get
			{
				if (this.m_DepthTextureFormat != DepthFormat.Default && !SystemInfo.IsFormatSupported((GraphicsFormat)this.m_DepthTextureFormat, GraphicsFormatUsage.Render))
				{
					Debug.LogWarning("Selected Depth Texture Format " + this.m_DepthTextureFormat.ToString() + " is not supported on this platform, falling back to Default");
					return DepthFormat.Default;
				}
				return this.m_DepthTextureFormat;
			}
			set
			{
				base.SetDirty();
				this.m_DepthTextureFormat = value;
			}
		}

		public bool accurateGbufferNormals
		{
			get
			{
				return this.m_AccurateGbufferNormals;
			}
			set
			{
				base.SetDirty();
				this.m_AccurateGbufferNormals = value;
			}
		}

		public IntermediateTextureMode intermediateTextureMode
		{
			get
			{
				return this.m_IntermediateTextureMode;
			}
			set
			{
				base.SetDirty();
				this.m_IntermediateTextureMode = value;
			}
		}

		public bool usesDeferredLighting
		{
			get
			{
				return this.m_RenderingMode == RenderingMode.Deferred || this.m_RenderingMode == RenderingMode.DeferredPlus;
			}
		}

		public bool usesClusterLightLoop
		{
			get
			{
				return this.m_RenderingMode == RenderingMode.ForwardPlus || this.m_RenderingMode == RenderingMode.DeferredPlus;
			}
		}

		internal override bool stripShadowsOffVariants
		{
			get
			{
				return this.m_StripShadowsOffVariants;
			}
			set
			{
				this.m_StripShadowsOffVariants = value;
			}
		}

		internal override bool stripAdditionalLightOffVariants
		{
			get
			{
				return this.m_StripAdditionalLightOffVariants;
			}
			set
			{
				this.m_StripAdditionalLightOffVariants = value;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.ReloadAllNullProperties();
		}

		private void ReloadAllNullProperties()
		{
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_AssetVersion = 3;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.m_AssetVersion <= 1)
			{
				this.m_CopyDepthMode = CopyDepthMode.AfterOpaques;
			}
			if (this.m_AssetVersion <= 2)
			{
				this.m_PrepassLayerMask = this.m_OpaqueLayerMask;
			}
			this.m_AssetVersion = 3;
		}

		[Obsolete("Moved to UniversalRenderPipelineRuntimeXRResources on GraphicsSettings. #from(2023.3)", false)]
		public XRSystemData xrSystemData;

		public PostProcessData postProcessData;

		private const int k_LatestAssetVersion = 3;

		[SerializeField]
		private int m_AssetVersion;

		[SerializeField]
		private LayerMask m_PrepassLayerMask = -1;

		[SerializeField]
		private LayerMask m_OpaqueLayerMask = -1;

		[SerializeField]
		private LayerMask m_TransparentLayerMask = -1;

		[SerializeField]
		private StencilStateData m_DefaultStencilState = new StencilStateData
		{
			passOperation = StencilOp.Replace
		};

		[SerializeField]
		private bool m_ShadowTransparentReceive = true;

		[SerializeField]
		private RenderingMode m_RenderingMode;

		[SerializeField]
		private DepthPrimingMode m_DepthPrimingMode;

		[SerializeField]
		private CopyDepthMode m_CopyDepthMode = CopyDepthMode.AfterTransparents;

		[SerializeField]
		private DepthFormat m_DepthAttachmentFormat;

		[SerializeField]
		private DepthFormat m_DepthTextureFormat;

		[SerializeField]
		private bool m_AccurateGbufferNormals;

		[SerializeField]
		private IntermediateTextureMode m_IntermediateTextureMode = IntermediateTextureMode.Always;

		[NonSerialized]
		private bool m_StripShadowsOffVariants = true;

		[NonSerialized]
		private bool m_StripAdditionalLightOffVariants = true;
	}
}
