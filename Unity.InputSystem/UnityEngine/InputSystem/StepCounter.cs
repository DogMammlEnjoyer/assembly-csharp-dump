using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Step Counter")]
	public class StepCounter : Sensor
	{
		[InputControl(displayName = "Step Counter", noisy = true)]
		public IntegerControl stepCounter { get; protected set; }

		public static StepCounter current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			StepCounter.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (StepCounter.current == this)
			{
				StepCounter.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.stepCounter = base.GetChildControl<IntegerControl>("stepCounter");
			base.FinishSetup();
		}
	}
}
