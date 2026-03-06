using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.XR
{
	public class BoneControl : InputControl<Bone>
	{
		[InputControl(offset = 0U, displayName = "parentBoneIndex")]
		public IntegerControl parentBoneIndex { get; set; }

		[InputControl(offset = 4U, displayName = "Position")]
		public Vector3Control position { get; set; }

		[InputControl(offset = 16U, displayName = "Rotation")]
		public QuaternionControl rotation { get; set; }

		protected override void FinishSetup()
		{
			this.parentBoneIndex = base.GetChildControl<IntegerControl>("parentBoneIndex");
			this.position = base.GetChildControl<Vector3Control>("position");
			this.rotation = base.GetChildControl<QuaternionControl>("rotation");
			base.FinishSetup();
		}

		public unsafe override Bone ReadUnprocessedValueFromState(void* statePtr)
		{
			return new Bone
			{
				parentBoneIndex = (uint)this.parentBoneIndex.ReadUnprocessedValueFromStateWithCaching(statePtr),
				position = this.position.ReadUnprocessedValueFromStateWithCaching(statePtr),
				rotation = this.rotation.ReadUnprocessedValueFromStateWithCaching(statePtr)
			};
		}

		public unsafe override void WriteValueIntoState(Bone value, void* statePtr)
		{
			this.parentBoneIndex.WriteValueIntoState((int)value.parentBoneIndex, statePtr);
			this.position.WriteValueIntoState(value.position, statePtr);
			this.rotation.WriteValueIntoState(value.rotation, statePtr);
		}
	}
}
