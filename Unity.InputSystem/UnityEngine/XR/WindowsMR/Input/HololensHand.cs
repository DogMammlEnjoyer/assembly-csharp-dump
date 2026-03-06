using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace UnityEngine.XR.WindowsMR.Input
{
	[InputControlLayout(displayName = "HoloLens Hand", commonUsages = new string[]
	{
		"LeftHand",
		"RightHand"
	}, hideInUI = true)]
	public class HololensHand : XRController
	{
		[InputControl(noisy = true, aliases = new string[]
		{
			"gripVelocity"
		})]
		public Vector3Control deviceVelocity { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"triggerbutton"
		})]
		public ButtonControl airTap { get; protected set; }

		[InputControl(noisy = true)]
		public AxisControl sourceLossRisk { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control sourceLossMitigationDirection { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.airTap = base.GetChildControl<ButtonControl>("airTap");
			this.deviceVelocity = base.GetChildControl<Vector3Control>("deviceVelocity");
			this.sourceLossRisk = base.GetChildControl<AxisControl>("sourceLossRisk");
			this.sourceLossMitigationDirection = base.GetChildControl<Vector3Control>("sourceLossMitigationDirection");
		}
	}
}
