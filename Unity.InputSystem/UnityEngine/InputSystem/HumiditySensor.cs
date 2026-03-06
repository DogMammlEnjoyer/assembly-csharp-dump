using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Humidity")]
	public class HumiditySensor : Sensor
	{
		[InputControl(displayName = "Relative Humidity", noisy = true)]
		public AxisControl relativeHumidity { get; protected set; }

		public static HumiditySensor current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			HumiditySensor.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (HumiditySensor.current == this)
			{
				HumiditySensor.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.relativeHumidity = base.GetChildControl<AxisControl>("relativeHumidity");
			base.FinishSetup();
		}
	}
}
