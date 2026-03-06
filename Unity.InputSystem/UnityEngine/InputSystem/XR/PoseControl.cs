using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;
using UnityEngine.XR;

namespace UnityEngine.InputSystem.XR
{
	[Preserve]
	[InputControlLayout(stateType = typeof(PoseState))]
	public class PoseControl : InputControl<PoseState>
	{
		public ButtonControl isTracked { get; set; }

		public IntegerControl trackingState { get; set; }

		public Vector3Control position { get; set; }

		public QuaternionControl rotation { get; set; }

		public Vector3Control velocity { get; set; }

		public Vector3Control angularVelocity { get; set; }

		public PoseControl()
		{
			this.m_StateBlock.format = PoseState.s_Format;
		}

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

		public unsafe override PoseState ReadUnprocessedValueFromState(void* statePtr)
		{
			if (this.m_OptimizedControlDataType == 1349481317)
			{
				return *(PoseState*)((byte*)statePtr + this.m_StateBlock.byteOffset);
			}
			return new PoseState
			{
				isTracked = (this.isTracked.ReadUnprocessedValueFromStateWithCaching(statePtr) > 0.5f),
				trackingState = (InputTrackingState)this.trackingState.ReadUnprocessedValueFromStateWithCaching(statePtr),
				position = this.position.ReadUnprocessedValueFromStateWithCaching(statePtr),
				rotation = this.rotation.ReadUnprocessedValueFromStateWithCaching(statePtr),
				velocity = this.velocity.ReadUnprocessedValueFromStateWithCaching(statePtr),
				angularVelocity = this.angularVelocity.ReadUnprocessedValueFromStateWithCaching(statePtr)
			};
		}

		public unsafe override void WriteValueIntoState(PoseState value, void* statePtr)
		{
			if (this.m_OptimizedControlDataType == 1349481317)
			{
				*(PoseState*)((byte*)statePtr + this.m_StateBlock.byteOffset) = value;
				return;
			}
			this.isTracked.WriteValueIntoState(value.isTracked, statePtr);
			this.trackingState.WriteValueIntoState((uint)value.trackingState, statePtr);
			this.position.WriteValueIntoState(value.position, statePtr);
			this.rotation.WriteValueIntoState(value.rotation, statePtr);
			this.velocity.WriteValueIntoState(value.velocity, statePtr);
			this.angularVelocity.WriteValueIntoState(value.angularVelocity, statePtr);
		}

		protected override FourCC CalculateOptimizedControlDataType()
		{
			if (this.m_StateBlock.sizeInBits == 480U && this.m_StateBlock.bitOffset == 0U && this.isTracked.optimizedControlDataType == 1113150533 && this.trackingState.optimizedControlDataType == 1229870112 && this.position.optimizedControlDataType == 1447379763 && this.rotation.optimizedControlDataType == 1364541780 && this.velocity.optimizedControlDataType == 1447379763 && this.angularVelocity.optimizedControlDataType == 1447379763 && this.trackingState.m_StateBlock.byteOffset == this.isTracked.m_StateBlock.byteOffset + 4U && this.position.m_StateBlock.byteOffset == this.isTracked.m_StateBlock.byteOffset + 8U && this.rotation.m_StateBlock.byteOffset == this.isTracked.m_StateBlock.byteOffset + 20U && this.velocity.m_StateBlock.byteOffset == this.isTracked.m_StateBlock.byteOffset + 36U && this.angularVelocity.m_StateBlock.byteOffset == this.isTracked.m_StateBlock.byteOffset + 48U)
			{
				return 1349481317;
			}
			return 0;
		}
	}
}
