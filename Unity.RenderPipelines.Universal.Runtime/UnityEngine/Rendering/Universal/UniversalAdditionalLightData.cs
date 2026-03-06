using System;

namespace UnityEngine.Rendering.Universal
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Light))]
	public class UniversalAdditionalLightData : MonoBehaviour, ISerializationCallbackReceiver, IAdditionalData
	{
		public bool usePipelineSettings
		{
			get
			{
				return this.m_UsePipelineSettings;
			}
			set
			{
				this.m_UsePipelineSettings = value;
			}
		}

		internal Light light
		{
			get
			{
				if (!this.m_Light)
				{
					base.TryGetComponent<Light>(out this.m_Light);
				}
				return this.m_Light;
			}
		}

		public int additionalLightsShadowResolutionTier
		{
			get
			{
				return this.m_AdditionalLightsShadowResolutionTier;
			}
		}

		public bool customShadowLayers
		{
			get
			{
				return this.m_CustomShadowLayers;
			}
			set
			{
				if (this.m_CustomShadowLayers != value)
				{
					this.m_CustomShadowLayers = value;
					this.SyncLightAndShadowLayers();
				}
			}
		}

		[Tooltip("Controls the size of the cookie mask currently assigned to the light.")]
		public Vector2 lightCookieSize
		{
			get
			{
				return this.m_LightCookieSize;
			}
			set
			{
				this.m_LightCookieSize = value;
			}
		}

		[Tooltip("Controls the offset of the cookie mask currently assigned to the light.")]
		public Vector2 lightCookieOffset
		{
			get
			{
				return this.m_LightCookieOffset;
			}
			set
			{
				this.m_LightCookieOffset = value;
			}
		}

		[Tooltip("Controls the filtering quality of soft shadows. Higher quality has lower performance.")]
		public SoftShadowQuality softShadowQuality
		{
			get
			{
				return this.m_SoftShadowQuality;
			}
			set
			{
				this.m_SoftShadowQuality = value;
			}
		}

		public RenderingLayerMask renderingLayers
		{
			get
			{
				return this.m_RenderingLayersMask;
			}
			set
			{
				if (this.m_RenderingLayersMask == value)
				{
					return;
				}
				this.m_RenderingLayersMask = value;
				this.SyncLightAndShadowLayers();
			}
		}

		public RenderingLayerMask shadowRenderingLayers
		{
			get
			{
				return this.m_ShadowRenderingLayersMask;
			}
			set
			{
				if (value == this.m_ShadowRenderingLayersMask)
				{
					return;
				}
				this.m_ShadowRenderingLayersMask = value;
				this.SyncLightAndShadowLayers();
			}
		}

		private void SyncLightAndShadowLayers()
		{
			if (this.light)
			{
				this.light.renderingLayerMask = (this.m_CustomShadowLayers ? this.m_ShadowRenderingLayersMask : this.m_RenderingLayersMask);
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (this.m_Version == UniversalAdditionalLightData.Version.Count)
			{
				this.m_Version = UniversalAdditionalLightData.Version.RenderingLayersMask;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.m_Version == UniversalAdditionalLightData.Version.Count)
			{
				this.m_Version = UniversalAdditionalLightData.Version.Initial;
			}
			if (this.m_Version < UniversalAdditionalLightData.Version.RenderingLayers)
			{
				this.m_RenderingLayers = (uint)this.m_LightLayerMask;
				this.m_ShadowRenderingLayers = (uint)this.m_ShadowLayerMask;
				this.m_Version = UniversalAdditionalLightData.Version.RenderingLayers;
			}
			if (this.m_Version < UniversalAdditionalLightData.Version.SoftShadowQuality)
			{
				this.m_SoftShadowQuality = (SoftShadowQuality)Math.Clamp((int)(this.m_SoftShadowQuality + 1), 0, 3);
				this.m_Version = UniversalAdditionalLightData.Version.SoftShadowQuality;
			}
			if (this.m_Version < UniversalAdditionalLightData.Version.RenderingLayersMask)
			{
				this.m_RenderingLayersMask = this.m_RenderingLayers;
				this.m_ShadowRenderingLayersMask = this.m_ShadowRenderingLayers;
				this.m_Version = UniversalAdditionalLightData.Version.RenderingLayersMask;
			}
		}

		[Obsolete("This is obsolete, please use renderingLayerMask instead. #from(2023.1)", true)]
		public LightLayerEnum lightLayerMask
		{
			get
			{
				return this.m_LightLayerMask;
			}
			set
			{
				this.m_LightLayerMask = value;
			}
		}

		[Obsolete("This is obsolete, please use shadowRenderingLayerMask instead. #from(2023.1)", true)]
		public LightLayerEnum shadowLayerMask
		{
			get
			{
				return this.m_ShadowLayerMask;
			}
			set
			{
				this.m_ShadowLayerMask = value;
			}
		}

		[Tooltip("Controls if light Shadow Bias parameters use pipeline settings.")]
		[SerializeField]
		private bool m_UsePipelineSettings = true;

		public static readonly int AdditionalLightsShadowResolutionTierCustom = -1;

		public static readonly int AdditionalLightsShadowResolutionTierLow = 0;

		public static readonly int AdditionalLightsShadowResolutionTierMedium = 1;

		public static readonly int AdditionalLightsShadowResolutionTierHigh = 2;

		public static readonly int AdditionalLightsShadowDefaultResolutionTier = UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierHigh;

		public static readonly int AdditionalLightsShadowDefaultCustomResolution = 128;

		[NonSerialized]
		private Light m_Light;

		public static readonly int AdditionalLightsShadowMinimumResolution = 128;

		[Tooltip("Controls if light shadow resolution uses pipeline settings.")]
		[SerializeField]
		private int m_AdditionalLightsShadowResolutionTier = UniversalAdditionalLightData.AdditionalLightsShadowDefaultResolutionTier;

		[SerializeField]
		private bool m_CustomShadowLayers;

		[SerializeField]
		private Vector2 m_LightCookieSize = Vector2.one;

		[SerializeField]
		private Vector2 m_LightCookieOffset = Vector2.zero;

		[SerializeField]
		private SoftShadowQuality m_SoftShadowQuality;

		[SerializeField]
		private RenderingLayerMask m_RenderingLayersMask = RenderingLayerMask.defaultRenderingLayerMask;

		[SerializeField]
		private RenderingLayerMask m_ShadowRenderingLayersMask = RenderingLayerMask.defaultRenderingLayerMask;

		[SerializeField]
		private UniversalAdditionalLightData.Version m_Version = UniversalAdditionalLightData.Version.Count;

		[Obsolete("This is obsolete, please use m_RenderingLayerMask instead. #from(2023.1)", false)]
		[SerializeField]
		private LightLayerEnum m_LightLayerMask = LightLayerEnum.LightLayerDefault;

		[Obsolete("This is obsolete, please use m_RenderingLayerMask instead. #from(2023.1)", false)]
		[SerializeField]
		private LightLayerEnum m_ShadowLayerMask = LightLayerEnum.LightLayerDefault;

		[SerializeField]
		[Obsolete("This is obsolete, please use m_RenderingLayersMask instead. #from(6000.2)", false)]
		private uint m_RenderingLayers = 1U;

		[SerializeField]
		[Obsolete("This is obsolete, please use renderingLayersMask instead. #from(6000.2)", false)]
		private uint m_ShadowRenderingLayers = 1U;

		private enum Version
		{
			Initial,
			RenderingLayers = 2,
			SoftShadowQuality,
			RenderingLayersMask,
			Count
		}
	}
}
