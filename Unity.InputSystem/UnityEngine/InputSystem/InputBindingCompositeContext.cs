using System;
using System.Collections.Generic;

namespace UnityEngine.InputSystem
{
	public struct InputBindingCompositeContext
	{
		public unsafe IEnumerable<InputBindingCompositeContext.PartBinding> controls
		{
			get
			{
				if (this.m_State == null)
				{
					yield break;
				}
				int totalBindingCount = this.m_State.totalBindingCount;
				int num;
				for (int bindingIndex = this.m_BindingIndex + 1; bindingIndex < totalBindingCount; bindingIndex = num)
				{
					InputActionState.BindingState bindingState = *this.m_State.GetBindingState(bindingIndex);
					if (!bindingState.isPartOfComposite)
					{
						break;
					}
					int controlStartIndex = bindingState.controlStartIndex;
					for (int i = 0; i < bindingState.controlCount; i = num)
					{
						InputControl control = this.m_State.controls[controlStartIndex + i];
						yield return new InputBindingCompositeContext.PartBinding
						{
							part = bindingState.partIndex,
							control = control
						};
						num = i + 1;
					}
					num = bindingIndex + 1;
				}
				yield break;
			}
		}

		public float EvaluateMagnitude(int partNumber)
		{
			return this.m_State.EvaluateCompositePartMagnitude(this.m_BindingIndex, partNumber);
		}

		public TValue ReadValue<TValue>(int partNumber) where TValue : struct, IComparable<TValue>
		{
			if (this.m_State == null)
			{
				return default(TValue);
			}
			int num;
			return this.m_State.ReadCompositePartValue<TValue, InputBindingCompositeContext.DefaultComparer<TValue>>(this.m_BindingIndex, partNumber, null, out num, default(InputBindingCompositeContext.DefaultComparer<TValue>));
		}

		public TValue ReadValue<TValue>(int partNumber, out InputControl sourceControl) where TValue : struct, IComparable<TValue>
		{
			if (this.m_State == null)
			{
				sourceControl = null;
				return default(TValue);
			}
			int num;
			TValue result = this.m_State.ReadCompositePartValue<TValue, InputBindingCompositeContext.DefaultComparer<TValue>>(this.m_BindingIndex, partNumber, null, out num, default(InputBindingCompositeContext.DefaultComparer<TValue>));
			if (num != -1)
			{
				sourceControl = this.m_State.controls[num];
				return result;
			}
			sourceControl = null;
			return result;
		}

		public TValue ReadValue<TValue, TComparer>(int partNumber, TComparer comparer = default(TComparer)) where TValue : struct where TComparer : IComparer<TValue>
		{
			if (this.m_State == null)
			{
				return default(TValue);
			}
			int num;
			return this.m_State.ReadCompositePartValue<TValue, TComparer>(this.m_BindingIndex, partNumber, null, out num, comparer);
		}

		public TValue ReadValue<TValue, TComparer>(int partNumber, out InputControl sourceControl, TComparer comparer = default(TComparer)) where TValue : struct where TComparer : IComparer<TValue>
		{
			if (this.m_State == null)
			{
				sourceControl = null;
				return default(TValue);
			}
			int num;
			TValue result = this.m_State.ReadCompositePartValue<TValue, TComparer>(this.m_BindingIndex, partNumber, null, out num, comparer);
			if (num != -1)
			{
				sourceControl = this.m_State.controls[num];
				return result;
			}
			sourceControl = null;
			return result;
		}

		public unsafe bool ReadValueAsButton(int partNumber)
		{
			if (this.m_State == null)
			{
				return false;
			}
			bool result = false;
			int num;
			this.m_State.ReadCompositePartValue<float, InputBindingCompositeContext.DefaultComparer<float>>(this.m_BindingIndex, partNumber, &result, out num, default(InputBindingCompositeContext.DefaultComparer<float>));
			return result;
		}

		public unsafe void ReadValue(int partNumber, void* buffer, int bufferSize)
		{
			InputActionState state = this.m_State;
			if (state == null)
			{
				return;
			}
			state.ReadCompositePartValue(this.m_BindingIndex, partNumber, buffer, bufferSize);
		}

		public object ReadValueAsObject(int partNumber)
		{
			return this.m_State.ReadCompositePartValueAsObject(this.m_BindingIndex, partNumber);
		}

		public double GetPressTime(int partNumber)
		{
			return this.m_State.GetCompositePartPressTime(this.m_BindingIndex, partNumber);
		}

		internal InputActionState m_State;

		internal int m_BindingIndex;

		public struct PartBinding
		{
			public int part { readonly get; set; }

			public InputControl control { readonly get; set; }
		}

		private struct DefaultComparer<TValue> : IComparer<TValue> where TValue : IComparable<TValue>
		{
			public int Compare(TValue x, TValue y)
			{
				return x.CompareTo(y);
			}
		}
	}
}
