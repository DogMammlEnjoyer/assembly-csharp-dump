using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(AccelerometerState))]
	public class Accelerometer : Sensor
	{
		public Vector3Control acceleration { get; protected set; }

		public static Accelerometer current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Accelerometer.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (Accelerometer.current == this)
			{
				Accelerometer.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.acceleration = base.GetChildControl<Vector3Control>("acceleration");
			base.FinishSetup();
		}
	}
}
