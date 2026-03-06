using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
	[AddComponentMenu("XR/Locomotion/Locomotion Mediator", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionMediator.html")]
	[RequireComponent(typeof(XRBodyTransformer))]
	public class LocomotionMediator : MonoBehaviour
	{
		public XROrigin xrOrigin
		{
			get
			{
				return this.m_XRBodyTransformer.xrOrigin;
			}
			set
			{
				this.m_XRBodyTransformer.xrOrigin = value;
			}
		}

		public XRBodyTransformer bodyTransformer
		{
			get
			{
				return this.m_XRBodyTransformer;
			}
		}

		protected void Awake()
		{
			this.m_XRBodyTransformer = base.GetComponent<XRBodyTransformer>();
		}

		protected void Update()
		{
			LocomotionMediator.s_ProvidersToRemove.Clear();
			foreach (KeyValuePair<LocomotionProvider, LocomotionMediator.LocomotionProviderData> keyValuePair in this.m_ProviderDataMap)
			{
				LocomotionProvider key = keyValuePair.Key;
				if (key == null)
				{
					LocomotionMediator.s_ProvidersToRemove.Add(key);
				}
				else
				{
					LocomotionMediator.LocomotionProviderData value = keyValuePair.Value;
					if (value.state == LocomotionState.Preparing && key.canStartMoving)
					{
						this.ChangeState(key, value, LocomotionState.Moving);
					}
					else if (value.state == LocomotionState.Ended && Time.frameCount > value.locomotionEndFrame)
					{
						this.ChangeState(key, value, LocomotionState.Idle);
					}
				}
			}
			if (LocomotionMediator.s_ProvidersToRemove.Count > 0)
			{
				foreach (LocomotionProvider key2 in LocomotionMediator.s_ProvidersToRemove)
				{
					this.m_ProviderDataMap.Remove(key2);
				}
			}
		}

		internal bool TryPrepareLocomotion(LocomotionProvider provider)
		{
			LocomotionMediator.LocomotionProviderData locomotionProviderData;
			if (!this.m_ProviderDataMap.TryGetValue(provider, out locomotionProviderData))
			{
				locomotionProviderData = new LocomotionMediator.LocomotionProviderData();
				this.m_ProviderDataMap[provider] = locomotionProviderData;
			}
			else if (this.GetProviderLocomotionState(provider).IsActive())
			{
				return false;
			}
			this.ChangeState(provider, locomotionProviderData, LocomotionState.Preparing);
			return true;
		}

		internal bool TryStartLocomotion(LocomotionProvider provider)
		{
			LocomotionMediator.LocomotionProviderData locomotionProviderData;
			if (!this.m_ProviderDataMap.TryGetValue(provider, out locomotionProviderData))
			{
				locomotionProviderData = new LocomotionMediator.LocomotionProviderData();
				this.m_ProviderDataMap[provider] = locomotionProviderData;
			}
			else if (this.GetProviderLocomotionState(provider) == LocomotionState.Moving)
			{
				return false;
			}
			this.ChangeState(provider, locomotionProviderData, LocomotionState.Moving);
			return true;
		}

		internal bool TryEndLocomotion(LocomotionProvider provider)
		{
			LocomotionMediator.LocomotionProviderData locomotionProviderData;
			if (!this.m_ProviderDataMap.TryGetValue(provider, out locomotionProviderData))
			{
				return false;
			}
			if (!locomotionProviderData.state.IsActive())
			{
				return false;
			}
			this.ChangeState(provider, locomotionProviderData, LocomotionState.Ended);
			return true;
		}

		private void ChangeState(LocomotionProvider provider, LocomotionMediator.LocomotionProviderData providerData, LocomotionState state)
		{
			if (providerData.state == state)
			{
				return;
			}
			LocomotionState state2 = providerData.state;
			providerData.state = state;
			if (state == LocomotionState.Ended)
			{
				providerData.locomotionEndFrame = Time.frameCount;
			}
			provider.OnLocomotionStateChanging(state2, state, this.m_XRBodyTransformer);
		}

		public LocomotionState GetProviderLocomotionState(LocomotionProvider provider)
		{
			LocomotionMediator.LocomotionProviderData locomotionProviderData;
			if (!this.m_ProviderDataMap.TryGetValue(provider, out locomotionProviderData))
			{
				return LocomotionState.Idle;
			}
			return locomotionProviderData.state;
		}

		private XRBodyTransformer m_XRBodyTransformer;

		private readonly Dictionary<LocomotionProvider, LocomotionMediator.LocomotionProviderData> m_ProviderDataMap = new Dictionary<LocomotionProvider, LocomotionMediator.LocomotionProviderData>();

		private static readonly List<LocomotionProvider> s_ProvidersToRemove = new List<LocomotionProvider>();

		private class LocomotionProviderData
		{
			public LocomotionState state;

			public int locomotionEndFrame;
		}
	}
}
