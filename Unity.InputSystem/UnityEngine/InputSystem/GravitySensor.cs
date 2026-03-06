using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(GravityState), displayName = "Gravity")]
	public class GravitySensor : Sensor
	{
		public Vector3Control gravity { get; protected set; }

		public static GravitySensor current { get; private set; }

		protected override void FinishSetup()
		{
			this.gravity = base.GetChildControl<Vector3Control>("gravity");
			base.FinishSetup();
		}

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			GravitySensor.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (GravitySensor.current == this)
			{
				GravitySensor.current = null;
			}
		}
	}
}
