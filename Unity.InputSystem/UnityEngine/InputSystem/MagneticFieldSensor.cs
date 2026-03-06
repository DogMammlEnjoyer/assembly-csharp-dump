using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Magnetic Field")]
	public class MagneticFieldSensor : Sensor
	{
		[InputControl(displayName = "Magnetic Field", noisy = true)]
		public Vector3Control magneticField { get; protected set; }

		public static MagneticFieldSensor current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			MagneticFieldSensor.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (MagneticFieldSensor.current == this)
			{
				MagneticFieldSensor.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.magneticField = base.GetChildControl<Vector3Control>("magneticField");
			base.FinishSetup();
		}
	}
}
