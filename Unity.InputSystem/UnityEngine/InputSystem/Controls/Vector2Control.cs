using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Controls
{
	public class Vector2Control : InputControl<Vector2>
	{
		[InputControl(offset = 0U, displayName = "X")]
		public AxisControl x { get; set; }

		[InputControl(offset = 4U, displayName = "Y")]
		public AxisControl y { get; set; }

		public Vector2Control()
		{
			this.m_StateBlock.format = InputStateBlock.FormatVector2;
		}

		protected override void FinishSetup()
		{
			this.x = base.GetChildControl<AxisControl>("x");
			this.y = base.GetChildControl<AxisControl>("y");
			base.FinishSetup();
		}

		public unsafe override Vector2 ReadUnprocessedValueFromState(void* statePtr)
		{
			if (this.m_OptimizedControlDataType == 1447379762)
			{
				return *(Vector2*)((byte*)statePtr + this.m_StateBlock.byteOffset);
			}
			return new Vector2(this.x.ReadUnprocessedValueFromStateWithCaching(statePtr), this.y.ReadUnprocessedValueFromStateWithCaching(statePtr));
		}

		public unsafe override void WriteValueIntoState(Vector2 value, void* statePtr)
		{
			if (this.m_OptimizedControlDataType == 1447379762)
			{
				*(Vector2*)((byte*)statePtr + this.m_StateBlock.byteOffset) = value;
				return;
			}
			this.x.WriteValueIntoState(value.x, statePtr);
			this.y.WriteValueIntoState(value.y, statePtr);
		}

		public unsafe override float EvaluateMagnitude(void* statePtr)
		{
			return base.ReadValueFromStateWithCaching(statePtr).magnitude;
		}

		protected override FourCC CalculateOptimizedControlDataType()
		{
			if (this.m_StateBlock.sizeInBits == 64U && this.m_StateBlock.bitOffset == 0U && this.x.optimizedControlDataType == InputStateBlock.FormatFloat && this.y.optimizedControlDataType == InputStateBlock.FormatFloat && this.y.m_StateBlock.byteOffset == this.x.m_StateBlock.byteOffset + 4U)
			{
				return InputStateBlock.FormatVector2;
			}
			return InputStateBlock.FormatInvalid;
		}
	}
}
