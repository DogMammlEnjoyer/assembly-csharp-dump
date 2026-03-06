using System;

namespace Oculus.Interaction.Input
{
	internal class UsageTouchMapping : IUsage
	{
		public ControllerButtonUsage Usage { get; }

		public OVRInput.Touch Touch { get; }

		public UsageTouchMapping(ControllerButtonUsage usage, OVRInput.Touch touch)
		{
			this.Usage = usage;
			this.Touch = touch;
		}

		public void Apply(ControllerDataAsset controllerDataAsset, OVRInput.Controller controllerMask)
		{
			bool value = OVRInput.Get(this.Touch, controllerMask);
			controllerDataAsset.Input.SetButton(this.Usage, value);
		}
	}
}
