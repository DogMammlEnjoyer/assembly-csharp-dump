using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.XRInputDeviceBoolValueReader.html")]
	[CreateAssetMenu(fileName = "XRInputDeviceBoolValueReader", menuName = "XR/Input Value Reader/bool")]
	public class XRInputDeviceBoolValueReader : XRInputDeviceValueReader<bool>
	{
		public override bool ReadValue()
		{
			return base.ReadBoolValue();
		}

		public override bool TryReadValue(out bool value)
		{
			return base.TryReadBoolValue(out value);
		}
	}
}
