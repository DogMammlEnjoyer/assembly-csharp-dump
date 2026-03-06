using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(AttitudeState), displayName = "Attitude")]
	public class AttitudeSensor : Sensor
	{
		public QuaternionControl attitude { get; protected set; }

		public static AttitudeSensor current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			AttitudeSensor.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (AttitudeSensor.current == this)
			{
				AttitudeSensor.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.attitude = base.GetChildControl<QuaternionControl>("attitude");
			base.FinishSetup();
		}
	}
}
