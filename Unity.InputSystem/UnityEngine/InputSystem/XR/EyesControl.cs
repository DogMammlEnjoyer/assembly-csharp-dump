using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.XR
{
	public class EyesControl : InputControl<Eyes>
	{
		[InputControl(offset = 0U, displayName = "LeftEyePosition")]
		public Vector3Control leftEyePosition { get; set; }

		[InputControl(offset = 12U, displayName = "LeftEyeRotation")]
		public QuaternionControl leftEyeRotation { get; set; }

		[InputControl(offset = 28U, displayName = "RightEyePosition")]
		public Vector3Control rightEyePosition { get; set; }

		[InputControl(offset = 40U, displayName = "RightEyeRotation")]
		public QuaternionControl rightEyeRotation { get; set; }

		[InputControl(offset = 56U, displayName = "FixationPoint")]
		public Vector3Control fixationPoint { get; set; }

		[InputControl(offset = 68U, displayName = "LeftEyeOpenAmount")]
		public AxisControl leftEyeOpenAmount { get; set; }

		[InputControl(offset = 72U, displayName = "RightEyeOpenAmount")]
		public AxisControl rightEyeOpenAmount { get; set; }

		protected override void FinishSetup()
		{
			this.leftEyePosition = base.GetChildControl<Vector3Control>("leftEyePosition");
			this.leftEyeRotation = base.GetChildControl<QuaternionControl>("leftEyeRotation");
			this.rightEyePosition = base.GetChildControl<Vector3Control>("rightEyePosition");
			this.rightEyeRotation = base.GetChildControl<QuaternionControl>("rightEyeRotation");
			this.fixationPoint = base.GetChildControl<Vector3Control>("fixationPoint");
			this.leftEyeOpenAmount = base.GetChildControl<AxisControl>("leftEyeOpenAmount");
			this.rightEyeOpenAmount = base.GetChildControl<AxisControl>("rightEyeOpenAmount");
			base.FinishSetup();
		}

		public unsafe override Eyes ReadUnprocessedValueFromState(void* statePtr)
		{
			return new Eyes
			{
				leftEyePosition = this.leftEyePosition.ReadUnprocessedValueFromStateWithCaching(statePtr),
				leftEyeRotation = this.leftEyeRotation.ReadUnprocessedValueFromStateWithCaching(statePtr),
				rightEyePosition = this.rightEyePosition.ReadUnprocessedValueFromStateWithCaching(statePtr),
				rightEyeRotation = this.rightEyeRotation.ReadUnprocessedValueFromStateWithCaching(statePtr),
				fixationPoint = this.fixationPoint.ReadUnprocessedValueFromStateWithCaching(statePtr),
				leftEyeOpenAmount = this.leftEyeOpenAmount.ReadUnprocessedValueFromStateWithCaching(statePtr),
				rightEyeOpenAmount = this.rightEyeOpenAmount.ReadUnprocessedValueFromStateWithCaching(statePtr)
			};
		}

		public unsafe override void WriteValueIntoState(Eyes value, void* statePtr)
		{
			this.leftEyePosition.WriteValueIntoState(value.leftEyePosition, statePtr);
			this.leftEyeRotation.WriteValueIntoState(value.leftEyeRotation, statePtr);
			this.rightEyePosition.WriteValueIntoState(value.rightEyePosition, statePtr);
			this.rightEyeRotation.WriteValueIntoState(value.rightEyeRotation, statePtr);
			this.fixationPoint.WriteValueIntoState(value.fixationPoint, statePtr);
			this.leftEyeOpenAmount.WriteValueIntoState(value.leftEyeOpenAmount, statePtr);
			this.rightEyeOpenAmount.WriteValueIntoState(value.rightEyeOpenAmount, statePtr);
		}
	}
}
