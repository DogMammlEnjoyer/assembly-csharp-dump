using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Hinge Angle")]
	public class HingeAngle : Sensor
	{
		public AxisControl angle { get; protected set; }

		public static HingeAngle current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			HingeAngle.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (HingeAngle.current == this)
			{
				HingeAngle.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.angle = base.GetChildControl<AxisControl>("angle");
			base.FinishSetup();
		}
	}
}
