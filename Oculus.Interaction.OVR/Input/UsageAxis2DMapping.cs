using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	internal class UsageAxis2DMapping : IUsage
	{
		public ControllerAxis2DUsage Usage { get; }

		public OVRInput.Axis2D Axis2D { get; }

		public UsageAxis2DMapping(ControllerAxis2DUsage usage, OVRInput.Axis2D axis2D)
		{
			this.Usage = usage;
			this.Axis2D = axis2D;
		}

		public void Apply(ControllerDataAsset controllerDataAsset, OVRInput.Controller controllerMask)
		{
			Vector2 value = OVRInput.Get(this.Axis2D, controllerMask);
			controllerDataAsset.Input.SetAxis2D(this.Usage, value);
		}
	}
}
