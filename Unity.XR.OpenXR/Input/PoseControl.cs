using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.XR.OpenXR.Input
{
	[Obsolete("OpenXR.Input.PoseControl is deprecated. Please use UnityEngine.InputSystem.XR.PoseControl instead.", false)]
	public class PoseControl : InputControl<Pose>
	{
		[Preserve]
		[InputControl(offset = 0U)]
		public ButtonControl isTracked { get; private set; }

		[Preserve]
		[InputControl(offset = 4U)]
		public IntegerControl trackingState { get; private set; }

		[Preserve]
		[InputControl(offset = 8U, noisy = true)]
		public Vector3Control position { get; private set; }

		[Preserve]
		[InputControl(offset = 20U, noisy = true)]
		public QuaternionControl rotation { get; private set; }

		[Preserve]
		[InputControl(offset = 36U, noisy = true)]
		public Vector3Control velocity { get; private set; }

		[Preserve]
		[InputControl(offset = 48U, noisy = true)]
		public Vector3Control angularVelocity { get; private set; }

		protected override void FinishSetup()
		{
			this.isTracked = base.GetChildControl<ButtonControl>("isTracked");
			this.trackingState = base.GetChildControl<IntegerControl>("trackingState");
			this.position = base.GetChildControl<Vector3Control>("position");
			this.rotation = base.GetChildControl<QuaternionControl>("rotation");
			this.velocity = base.GetChildControl<Vector3Control>("velocity");
			this.angularVelocity = base.GetChildControl<Vector3Control>("angularVelocity");
			base.FinishSetup();
		}

		public unsafe override Pose ReadUnprocessedValueFromState(void* statePtr)
		{
			return new Pose
			{
				isTracked = (this.isTracked.ReadUnprocessedValueFromState(statePtr) > 0.5f),
				trackingState = (InputTrackingState)this.trackingState.ReadUnprocessedValueFromState(statePtr),
				position = this.position.ReadUnprocessedValueFromState(statePtr),
				rotation = this.rotation.ReadUnprocessedValueFromState(statePtr),
				velocity = this.velocity.ReadUnprocessedValueFromState(statePtr),
				angularVelocity = this.angularVelocity.ReadUnprocessedValueFromState(statePtr)
			};
		}

		public unsafe override void WriteValueIntoState(Pose value, void* statePtr)
		{
			this.isTracked.WriteValueIntoState(value.isTracked, statePtr);
			this.trackingState.WriteValueIntoState((uint)value.trackingState, statePtr);
			this.position.WriteValueIntoState(value.position, statePtr);
			this.rotation.WriteValueIntoState(value.rotation, statePtr);
			this.velocity.WriteValueIntoState(value.velocity, statePtr);
			this.angularVelocity.WriteValueIntoState(value.angularVelocity, statePtr);
		}
	}
}
