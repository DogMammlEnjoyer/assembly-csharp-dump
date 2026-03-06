using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(GyroscopeState))]
	public class Gyroscope : Sensor
	{
		public Vector3Control angularVelocity { get; protected set; }

		public static Gyroscope current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Gyroscope.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (Gyroscope.current == this)
			{
				Gyroscope.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.angularVelocity = base.GetChildControl<Vector3Control>("angularVelocity");
			base.FinishSetup();
		}
	}
}
