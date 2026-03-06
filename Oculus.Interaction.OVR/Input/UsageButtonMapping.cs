using System;

namespace Oculus.Interaction.Input
{
	internal class UsageButtonMapping : IUsage
	{
		public ControllerButtonUsage Usage { get; }

		public OVRInput.Button Button { get; }

		public UsageButtonMapping(ControllerButtonUsage usage, OVRInput.Button button)
		{
			this.Usage = usage;
			this.Button = button;
		}

		public void Apply(ControllerDataAsset controllerDataAsset, OVRInput.Controller controllerMask)
		{
			bool value = OVRInput.Get(this.Button, controllerMask);
			controllerDataAsset.Input.SetButton(this.Usage, value);
		}
	}
}
