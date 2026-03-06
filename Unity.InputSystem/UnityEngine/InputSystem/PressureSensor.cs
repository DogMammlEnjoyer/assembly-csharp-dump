using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Pressure")]
	public class PressureSensor : Sensor
	{
		[InputControl(displayName = "Atmospheric Pressure", noisy = true)]
		public AxisControl atmosphericPressure { get; protected set; }

		public static PressureSensor current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			PressureSensor.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (PressureSensor.current == this)
			{
				PressureSensor.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.atmosphericPressure = base.GetChildControl<AxisControl>("atmosphericPressure");
			base.FinishSetup();
		}
	}
}
