using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.XRInputDeviceFloatValueReader.html")]
	[CreateAssetMenu(fileName = "XRInputDeviceFloatValueReader", menuName = "XR/Input Value Reader/float")]
	public class XRInputDeviceFloatValueReader : XRInputDeviceValueReader<float>
	{
		public override float ReadValue()
		{
			return base.ReadFloatValue();
		}

		public override bool TryReadValue(out float value)
		{
			return base.TryReadFloatValue(out value);
		}
	}
}
