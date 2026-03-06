using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	[AddComponentMenu("XR/Haptics/Haptic Impulse Player", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.HapticImpulsePlayer.html")]
	public class HapticImpulsePlayer : MonoBehaviour
	{
		public XRInputHapticImpulseProvider hapticOutput
		{
			get
			{
				return this.m_HapticOutput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_HapticOutput, value, this);
			}
		}

		public float amplitudeMultiplier
		{
			get
			{
				return this.m_AmplitudeMultiplier;
			}
			set
			{
				this.m_AmplitudeMultiplier = value;
			}
		}

		protected void Awake()
		{
			XRInputHapticImpulseProvider hapticOutput = this.m_HapticOutput;
			if (hapticOutput != null && hapticOutput.inputSourceMode == XRInputHapticImpulseProvider.InputSourceMode.InputActionReference && this.m_HapticOutput.inputActionReference == null)
			{
				IXRHapticImpulseProvider componentInParent = base.gameObject.GetComponentInParent<IXRHapticImpulseProvider>(true);
				if (componentInParent as Component != null)
				{
					this.m_HapticOutput.SetObjectReference(componentInParent);
					this.m_HapticOutput.inputSourceMode = XRInputHapticImpulseProvider.InputSourceMode.ObjectReference;
				}
			}
		}

		protected void OnEnable()
		{
			this.m_HapticOutput.EnableDirectActionIfModeUsed();
		}

		protected void OnDisable()
		{
			this.m_HapticOutput.DisableDirectActionIfModeUsed();
		}

		public bool SendHapticImpulse(float amplitude, float duration)
		{
			return this.SendHapticImpulse(amplitude, duration, 0f);
		}

		public bool SendHapticImpulse(float amplitude, float duration, float frequency)
		{
			if (!base.isActiveAndEnabled)
			{
				return false;
			}
			IXRHapticImpulseChannelGroup channelGroup = this.m_HapticOutput.GetChannelGroup();
			bool? flag;
			if (channelGroup == null)
			{
				flag = null;
			}
			else
			{
				IXRHapticImpulseChannel channel = channelGroup.GetChannel(0);
				flag = ((channel != null) ? new bool?(channel.SendHapticImpulse(amplitude * this.m_AmplitudeMultiplier, duration, frequency)) : null);
			}
			bool? flag2 = flag;
			return flag2.GetValueOrDefault();
		}

		internal static HapticImpulsePlayer GetOrCreateInHierarchy(GameObject gameObject)
		{
			HapticImpulsePlayer hapticImpulsePlayer = gameObject.GetComponentInParent<HapticImpulsePlayer>(true);
			if (hapticImpulsePlayer == null)
			{
				Component component = gameObject.GetComponentInParent<IXRHapticImpulseProvider>(true) as Component;
				hapticImpulsePlayer = ((component != null) ? component.gameObject.AddComponent<HapticImpulsePlayer>() : gameObject.AddComponent<HapticImpulsePlayer>());
			}
			return hapticImpulsePlayer;
		}

		[SerializeField]
		[Tooltip("Specifies the output haptic control or controller that haptic impulses will be sent to.")]
		private XRInputHapticImpulseProvider m_HapticOutput = new XRInputHapticImpulseProvider("Haptic", false, XRInputHapticImpulseProvider.InputSourceMode.InputActionReference);

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Amplitude multiplier which can be used to dampen the haptic impulses sent by this component.")]
		private float m_AmplitudeMultiplier = 1f;
	}
}
