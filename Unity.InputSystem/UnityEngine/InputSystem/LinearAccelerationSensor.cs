using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(LinearAccelerationState), displayName = "Linear Acceleration")]
	public class LinearAccelerationSensor : Sensor
	{
		public Vector3Control acceleration { get; protected set; }

		public static LinearAccelerationSensor current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			LinearAccelerationSensor.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (LinearAccelerationSensor.current == this)
			{
				LinearAccelerationSensor.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.acceleration = base.GetChildControl<Vector3Control>("acceleration");
			base.FinishSetup();
		}
	}
}
