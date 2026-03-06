using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.XRInputDeviceHapticImpulseProvider.html")]
	[CreateAssetMenu(fileName = "XRInputDeviceHapticImpulseProvider", menuName = "XR/Input Device Haptic Impulse Provider")]
	public class XRInputDeviceHapticImpulseProvider : ScriptableObject, IXRHapticImpulseProvider
	{
		public IXRHapticImpulseChannelGroup GetChannelGroup()
		{
			this.RefreshInputDeviceIfNeeded();
			if (this.m_ChannelGroup == null)
			{
				this.m_ChannelGroup = new XRInputDeviceHapticImpulseChannelGroup();
			}
			this.m_ChannelGroup.Initialize(this.m_InputDevice);
			return this.m_ChannelGroup;
		}

		private void RefreshInputDeviceIfNeeded()
		{
			if (!this.m_InputDevice.isValid)
			{
				XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(this.m_Characteristics, out this.m_InputDevice);
			}
		}

		[SerializeField]
		private InputDeviceCharacteristics m_Characteristics;

		private XRInputDeviceHapticImpulseChannelGroup m_ChannelGroup;

		private InputDevice m_InputDevice;
	}
}
