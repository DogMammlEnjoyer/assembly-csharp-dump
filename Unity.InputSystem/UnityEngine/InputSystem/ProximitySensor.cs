using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Proximity")]
	public class ProximitySensor : Sensor
	{
		[InputControl(displayName = "Distance", noisy = true)]
		public AxisControl distance { get; protected set; }

		public static ProximitySensor current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			ProximitySensor.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (ProximitySensor.current == this)
			{
				ProximitySensor.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.distance = base.GetChildControl<AxisControl>("distance");
			base.FinishSetup();
		}
	}
}
