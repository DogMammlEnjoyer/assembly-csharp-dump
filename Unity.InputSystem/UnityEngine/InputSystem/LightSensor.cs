using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Light")]
	public class LightSensor : Sensor
	{
		[InputControl(displayName = "Light Level", noisy = true)]
		public AxisControl lightLevel { get; protected set; }

		public static LightSensor current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			LightSensor.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (LightSensor.current == this)
			{
				LightSensor.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.lightLevel = base.GetChildControl<AxisControl>("lightLevel");
			base.FinishSetup();
		}
	}
}
