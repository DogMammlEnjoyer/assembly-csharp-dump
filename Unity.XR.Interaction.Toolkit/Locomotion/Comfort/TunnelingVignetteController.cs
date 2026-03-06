using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort
{
	[AddComponentMenu("XR/Locomotion/Tunneling Vignette Controller", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort.TunnelingVignetteController.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class TunnelingVignetteController : MonoBehaviour
	{
		public VignetteParameters defaultParameters
		{
			get
			{
				return this.m_DefaultParameters;
			}
			set
			{
				this.m_DefaultParameters = value;
			}
		}

		public VignetteParameters currentParameters
		{
			get
			{
				return this.m_CurrentParameters;
			}
		}

		public List<LocomotionVignetteProvider> locomotionVignetteProviders
		{
			get
			{
				return this.m_LocomotionVignetteProviders;
			}
			set
			{
				this.m_LocomotionVignetteProviders = value;
			}
		}

		internal static event Action<ITunnelingVignetteProvider> vignetteProviderQueued;

		public void BeginTunnelingVignette(ITunnelingVignetteProvider provider)
		{
			foreach (TunnelingVignetteController.ProviderRecord providerRecord in this.m_ProviderRecords)
			{
				if (providerRecord.provider == provider)
				{
					providerRecord.easeState = EaseState.EasingIn;
					return;
				}
			}
			this.m_ProviderRecords.Add(new TunnelingVignetteController.ProviderRecord(provider)
			{
				easeState = EaseState.EasingIn
			});
			Action<ITunnelingVignetteProvider> action = TunnelingVignetteController.vignetteProviderQueued;
			if (action == null)
			{
				return;
			}
			action(provider);
		}

		public void EndTunnelingVignette(ITunnelingVignetteProvider provider)
		{
			VignetteParameters vignetteParameters = provider.vignetteParameters ?? this.m_DefaultParameters;
			foreach (TunnelingVignetteController.ProviderRecord providerRecord in this.m_ProviderRecords)
			{
				if (providerRecord.provider == provider)
				{
					providerRecord.easeState = ((vignetteParameters.easeInTimeLock && !providerRecord.easeInLockEnded) ? EaseState.EasingInHoldBeforeEasingOut : ((vignetteParameters.easeOutDelayTime > 0f && providerRecord.dynamicEaseOutDelayTime < vignetteParameters.easeOutDelayTime) ? EaseState.EasingOutDelay : EaseState.EasingOut));
					return;
				}
			}
			EaseState easeState = vignetteParameters.easeInTimeLock ? EaseState.EasingInHoldBeforeEasingOut : ((vignetteParameters.easeOutDelayTime > 0f) ? EaseState.EasingOutDelay : EaseState.EasingOut);
			this.m_ProviderRecords.Add(new TunnelingVignetteController.ProviderRecord(provider)
			{
				easeState = easeState
			});
		}

		[Conditional("UNITY_EDITOR")]
		internal void PreviewInEditor(VignetteParameters previewParameters)
		{
			if (!Application.isPlaying && base.gameObject.activeInHierarchy)
			{
				this.UpdateTunnelingVignette(previewParameters);
			}
		}

		protected virtual void Awake()
		{
			this.m_CurrentParameters.CopyFrom(VignetteParameters.Defaults.noEffect);
			this.UpdateTunnelingVignette(VignetteParameters.Defaults.noEffect);
		}

		[Conditional("UNITY_EDITOR")]
		protected virtual void Reset()
		{
			this.m_DefaultParameters.CopyFrom(VignetteParameters.Defaults.defaultEffect);
			this.m_CurrentParameters.CopyFrom(VignetteParameters.Defaults.noEffect);
			this.UpdateTunnelingVignette(this.m_DefaultParameters);
		}

		protected virtual void Update()
		{
			if (this.m_LocomotionVignetteProviders.Count > 0)
			{
				foreach (LocomotionVignetteProvider locomotionVignetteProvider in this.m_LocomotionVignetteProviders)
				{
					LocomotionProvider locomotionProvider = locomotionVignetteProvider.locomotionProvider;
					if (locomotionVignetteProvider.enabled && !(locomotionProvider == null))
					{
						if (locomotionProvider.isLocomotionActive)
						{
							this.BeginTunnelingVignette(locomotionVignetteProvider);
						}
						else if (locomotionProvider.locomotionState == LocomotionState.Ended)
						{
							this.EndTunnelingVignette(locomotionVignetteProvider);
						}
					}
				}
			}
			if (this.m_ProviderRecords.Count == 0)
			{
				return;
			}
			foreach (TunnelingVignetteController.ProviderRecord providerRecord in this.m_ProviderRecords)
			{
				VignetteParameters vignetteParameters = providerRecord.provider.vignetteParameters ?? this.m_DefaultParameters;
				float dynamicApertureSize = providerRecord.dynamicApertureSize;
				switch (providerRecord.easeState)
				{
				case EaseState.NotEasing:
					providerRecord.dynamicApertureSize = 1f;
					providerRecord.dynamicEaseOutDelayTime = 0f;
					providerRecord.easeInLockEnded = false;
					continue;
				case EaseState.EasingIn:
				{
					float num = Mathf.Max(vignetteParameters.easeInTime, 0f);
					float apertureSize = vignetteParameters.apertureSize;
					providerRecord.easeInLockEnded = false;
					if (num > 0f && dynamicApertureSize > apertureSize)
					{
						float num2 = dynamicApertureSize + (apertureSize - 1f) / num * Time.unscaledDeltaTime;
						providerRecord.dynamicApertureSize = ((num2 < apertureSize) ? apertureSize : num2);
						continue;
					}
					providerRecord.dynamicApertureSize = apertureSize;
					continue;
				}
				case EaseState.EasingInHoldBeforeEasingOut:
					if (!providerRecord.easeInLockEnded)
					{
						float num3 = Mathf.Max(vignetteParameters.easeInTime, 0f);
						float apertureSize2 = vignetteParameters.apertureSize;
						if (num3 > 0f && dynamicApertureSize > apertureSize2)
						{
							float num4 = dynamicApertureSize + (apertureSize2 - 1f) / num3 * Time.unscaledDeltaTime;
							providerRecord.dynamicApertureSize = ((num4 < apertureSize2) ? apertureSize2 : num4);
							continue;
						}
						providerRecord.easeInLockEnded = true;
						if (vignetteParameters.easeOutDelayTime <= 0f || providerRecord.dynamicEaseOutDelayTime >= vignetteParameters.easeOutDelayTime)
						{
							providerRecord.easeState = EaseState.EasingOut;
							goto IL_2A3;
						}
						providerRecord.easeState = EaseState.EasingOutDelay;
					}
					else
					{
						if (vignetteParameters.easeOutDelayTime <= 0f)
						{
							providerRecord.easeState = EaseState.EasingOutDelay;
							goto IL_2A3;
						}
						providerRecord.easeState = EaseState.EasingOutDelay;
					}
					break;
				case EaseState.EasingOutDelay:
					break;
				case EaseState.EasingOut:
					goto IL_2A3;
				default:
					continue;
				}
				float num5 = providerRecord.dynamicEaseOutDelayTime;
				float num6 = Mathf.Max(vignetteParameters.easeOutDelayTime, 0f);
				if (num6 > 0f && num5 < num6)
				{
					num5 += Time.unscaledDeltaTime;
					providerRecord.dynamicEaseOutDelayTime = ((num5 > num6) ? num6 : num5);
				}
				if (providerRecord.dynamicEaseOutDelayTime < num6)
				{
					continue;
				}
				providerRecord.easeState = EaseState.EasingOut;
				IL_2A3:
				float num7 = Mathf.Max(vignetteParameters.easeOutTime, 0f);
				float apertureSize3 = vignetteParameters.apertureSize;
				if (num7 > 0f && dynamicApertureSize < 1f)
				{
					float num8 = dynamicApertureSize + (1f - apertureSize3) / num7 * Time.unscaledDeltaTime;
					providerRecord.dynamicApertureSize = ((num8 > 1f) ? 1f : num8);
				}
				else
				{
					providerRecord.dynamicApertureSize = 1f;
				}
				if (providerRecord.dynamicApertureSize >= 1f)
				{
					providerRecord.easeState = EaseState.NotEasing;
				}
			}
			float num9 = 1f;
			TunnelingVignetteController.ProviderRecord providerRecord2 = null;
			foreach (TunnelingVignetteController.ProviderRecord providerRecord3 in this.m_ProviderRecords)
			{
				float dynamicApertureSize2 = providerRecord3.dynamicApertureSize;
				if (dynamicApertureSize2 < num9)
				{
					providerRecord2 = providerRecord3;
					num9 = dynamicApertureSize2;
				}
			}
			if (providerRecord2 != null)
			{
				this.m_CurrentParameters.CopyFrom(providerRecord2.provider.vignetteParameters ?? this.m_DefaultParameters);
			}
			this.m_CurrentParameters.apertureSize = num9;
			this.UpdateTunnelingVignette(this.m_CurrentParameters);
		}

		private void UpdateTunnelingVignette(VignetteParameters parameters)
		{
			if (parameters == null)
			{
				parameters = this.m_DefaultParameters;
			}
			if (this.TrySetUpMaterial())
			{
				this.m_MeshRender.GetPropertyBlock(this.m_VignettePropertyBlock);
				this.m_VignettePropertyBlock.SetFloat(TunnelingVignetteController.ShaderPropertyLookup.apertureSize, parameters.apertureSize);
				this.m_VignettePropertyBlock.SetFloat(TunnelingVignetteController.ShaderPropertyLookup.featheringEffect, parameters.featheringEffect);
				this.m_VignettePropertyBlock.SetColor(TunnelingVignetteController.ShaderPropertyLookup.vignetteColor, parameters.vignetteColor);
				this.m_VignettePropertyBlock.SetColor(TunnelingVignetteController.ShaderPropertyLookup.vignetteColorBlend, parameters.vignetteColorBlend);
				this.m_MeshRender.SetPropertyBlock(this.m_VignettePropertyBlock);
			}
			Transform transform = base.transform;
			Vector3 localPosition = transform.localPosition;
			if (!Mathf.Approximately(localPosition.y, parameters.apertureVerticalPosition))
			{
				localPosition.y = parameters.apertureVerticalPosition;
				transform.localPosition = localPosition;
			}
		}

		private bool TrySetUpMaterial()
		{
			if (this.m_MeshRender == null)
			{
				this.m_MeshRender = base.GetComponent<MeshRenderer>();
			}
			if (this.m_MeshRender == null)
			{
				this.m_MeshRender = base.gameObject.AddComponent<MeshRenderer>();
			}
			if (this.m_VignettePropertyBlock == null)
			{
				this.m_VignettePropertyBlock = new MaterialPropertyBlock();
			}
			if (this.m_MeshFilter == null)
			{
				this.m_MeshFilter = base.GetComponent<MeshFilter>();
			}
			if (this.m_MeshFilter == null)
			{
				this.m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (this.m_MeshFilter.sharedMesh == null)
			{
				Debug.LogWarning("The default mesh for the TunnelingVignetteController is not set. Make sure to import it from the Tunneling Vignette Sample of XR Interaction Toolkit.", this);
				return false;
			}
			if (this.m_MeshRender.sharedMaterial == null)
			{
				Shader shader = Shader.Find("VR/TunnelingVignette");
				if (shader == null)
				{
					Debug.LogWarning("The default material for the TunnelingVignetteController is not set, and the default Shader: VR/TunnelingVignette cannot be found. Make sure they are imported from the Tunneling Vignette Sample of XR Interaction Toolkit.", this);
					return false;
				}
				Debug.LogWarning("The default material for the TunnelingVignetteController is not set. Make sure it is imported from the Tunneling Vignette Sample of XR Interaction Toolkit. + Try creating a material using the default Shader: VR/TunnelingVignette", this);
				this.m_SharedMaterial = new Material(shader)
				{
					name = "DefaultTunnelingVignette"
				};
				this.m_MeshRender.sharedMaterial = this.m_SharedMaterial;
			}
			else
			{
				this.m_SharedMaterial = this.m_MeshRender.sharedMaterial;
			}
			return true;
		}

		private const string k_DefaultShader = "VR/TunnelingVignette";

		[SerializeField]
		private VignetteParameters m_DefaultParameters = new VignetteParameters();

		[SerializeField]
		private VignetteParameters m_CurrentParameters = new VignetteParameters();

		[SerializeField]
		private List<LocomotionVignetteProvider> m_LocomotionVignetteProviders = new List<LocomotionVignetteProvider>();

		private readonly List<TunnelingVignetteController.ProviderRecord> m_ProviderRecords = new List<TunnelingVignetteController.ProviderRecord>();

		private MeshRenderer m_MeshRender;

		private MeshFilter m_MeshFilter;

		private Material m_SharedMaterial;

		private MaterialPropertyBlock m_VignettePropertyBlock;

		private static class ShaderPropertyLookup
		{
			public static readonly int apertureSize = Shader.PropertyToID("_ApertureSize");

			public static readonly int featheringEffect = Shader.PropertyToID("_FeatheringEffect");

			public static readonly int vignetteColor = Shader.PropertyToID("_VignetteColor");

			public static readonly int vignetteColorBlend = Shader.PropertyToID("_VignetteColorBlend");
		}

		private class ProviderRecord
		{
			public ITunnelingVignetteProvider provider { get; }

			public EaseState easeState { get; set; }

			public float dynamicApertureSize { get; set; } = 1f;

			public bool easeInLockEnded { get; set; }

			public float dynamicEaseOutDelayTime { get; set; }

			public ProviderRecord(ITunnelingVignetteProvider provider)
			{
				this.provider = provider;
			}
		}
	}
}
