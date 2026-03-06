using System;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Controls
{
	public class AxisControl : InputControl<float>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected float Preprocess(float value)
		{
			if (this.scale)
			{
				value *= this.scaleFactor;
			}
			if (this.clamp == AxisControl.Clamp.ToConstantBeforeNormalize)
			{
				if (value < this.clampMin || value > this.clampMax)
				{
					value = this.clampConstant;
				}
			}
			else if (this.clamp == AxisControl.Clamp.BeforeNormalize)
			{
				value = Mathf.Clamp(value, this.clampMin, this.clampMax);
			}
			if (this.normalize)
			{
				value = NormalizeProcessor.Normalize(value, this.normalizeMin, this.normalizeMax, this.normalizeZero);
			}
			if (this.clamp == AxisControl.Clamp.AfterNormalize)
			{
				value = Mathf.Clamp(value, this.clampMin, this.clampMax);
			}
			if (this.invert)
			{
				value *= -1f;
			}
			return value;
		}

		private float Unpreprocess(float value)
		{
			if (this.invert)
			{
				value *= -1f;
			}
			if (this.normalize)
			{
				value = NormalizeProcessor.Denormalize(value, this.normalizeMin, this.normalizeMax, this.normalizeZero);
			}
			if (this.scale)
			{
				value /= this.scaleFactor;
			}
			return value;
		}

		public AxisControl()
		{
			this.m_StateBlock.format = InputStateBlock.FormatFloat;
		}

		protected override void FinishSetup()
		{
			base.FinishSetup();
			if (!base.hasDefaultState && this.normalize && Mathf.Abs(this.normalizeZero) > Mathf.Epsilon)
			{
				this.m_DefaultState = base.stateBlock.FloatToPrimitiveValue(this.normalizeZero);
			}
		}

		public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
		{
			int num = this.m_OptimizedControlDataType;
			if (num != 1113150533)
			{
				if (num == 1179407392)
				{
					return *(float*)((byte*)statePtr + this.m_StateBlock.m_ByteOffset);
				}
				float value = base.stateBlock.ReadFloat(statePtr);
				return this.Preprocess(value);
			}
			else
			{
				if (((byte*)statePtr)[this.m_StateBlock.m_ByteOffset] == 0)
				{
					return 0f;
				}
				return 1f;
			}
		}

		public unsafe override void WriteValueIntoState(float value, void* statePtr)
		{
			int num = this.m_OptimizedControlDataType;
			if (num == 1113150533)
			{
				((byte*)statePtr)[this.m_StateBlock.m_ByteOffset] = ((value >= 0.5f) ? 1 : 0);
				return;
			}
			if (num == 1179407392)
			{
				*(float*)((byte*)statePtr + this.m_StateBlock.m_ByteOffset) = value;
				return;
			}
			value = this.Unpreprocess(value);
			base.stateBlock.WriteFloat(statePtr, value);
		}

		public unsafe override bool CompareValue(void* firstStatePtr, void* secondStatePtr)
		{
			float a = base.ReadValueFromState(firstStatePtr);
			float b = base.ReadValueFromState(secondStatePtr);
			return !Mathf.Approximately(a, b);
		}

		public unsafe override float EvaluateMagnitude(void* statePtr)
		{
			return this.EvaluateMagnitude(base.ReadValueFromStateWithCaching(statePtr));
		}

		private float EvaluateMagnitude(float value)
		{
			if (this.m_MinValue.isEmpty || this.m_MaxValue.isEmpty)
			{
				return Mathf.Abs(value);
			}
			float num = this.m_MinValue.ToSingle(null);
			float max = this.m_MaxValue.ToSingle(null);
			float num2 = Mathf.Clamp(value, num, max);
			if (num >= 0f)
			{
				return NormalizeProcessor.Normalize(num2, num, max, 0f);
			}
			if (num2 < 0f)
			{
				return NormalizeProcessor.Normalize(Mathf.Abs(num2), 0f, Mathf.Abs(num), 0f);
			}
			return NormalizeProcessor.Normalize(num2, 0f, max, 0f);
		}

		protected override FourCC CalculateOptimizedControlDataType()
		{
			bool flag = this.clamp == AxisControl.Clamp.None && !this.invert && !this.normalize && !this.scale;
			if (flag && this.m_StateBlock.format == InputStateBlock.FormatFloat && this.m_StateBlock.sizeInBits == 32U && this.m_StateBlock.bitOffset == 0U)
			{
				return InputStateBlock.FormatFloat;
			}
			if (flag && this.m_StateBlock.format == InputStateBlock.FormatBit && this.m_StateBlock.sizeInBits == 8U && this.m_StateBlock.bitOffset == 0U)
			{
				return InputStateBlock.FormatByte;
			}
			return InputStateBlock.FormatInvalid;
		}

		public AxisControl.Clamp clamp;

		public float clampMin;

		public float clampMax;

		public float clampConstant;

		public bool invert;

		public bool normalize;

		public float normalizeMin;

		public float normalizeMax;

		public float normalizeZero;

		public bool scale;

		public float scaleFactor;

		public enum Clamp
		{
			None,
			BeforeNormalize,
			AfterNormalize,
			ToConstantBeforeNormalize
		}
	}
}
