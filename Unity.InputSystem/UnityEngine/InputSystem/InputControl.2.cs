using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public abstract class InputControl<TValue> : InputControl where TValue : struct
	{
		public override Type valueType
		{
			get
			{
				return typeof(TValue);
			}
		}

		public override int valueSizeInBytes
		{
			get
			{
				return UnsafeUtility.SizeOf<TValue>();
			}
		}

		public unsafe ref readonly TValue value
		{
			get
			{
				if (!InputSystem.s_Manager.readValueCachingFeatureEnabled || this.m_CachedValueIsStale || this.evaluateProcessorsEveryRead)
				{
					this.m_CachedValue = this.ProcessValue(*this.unprocessedValue);
					this.m_CachedValueIsStale = false;
				}
				return ref this.m_CachedValue;
			}
		}

		internal ref readonly TValue unprocessedValue
		{
			get
			{
				if (base.currentStatePtr == null)
				{
					return ref this.m_UnprocessedCachedValue;
				}
				if (!InputSystem.s_Manager.readValueCachingFeatureEnabled || this.m_UnprocessedCachedValueIsStale)
				{
					this.m_UnprocessedCachedValue = this.ReadUnprocessedValueFromState(base.currentStatePtr);
					this.m_UnprocessedCachedValueIsStale = false;
				}
				return ref this.m_UnprocessedCachedValue;
			}
		}

		public unsafe TValue ReadValue()
		{
			return *this.value;
		}

		public TValue ReadValueFromPreviousFrame()
		{
			return this.ReadValueFromState(base.previousFrameStatePtr);
		}

		public TValue ReadDefaultValue()
		{
			return this.ReadValueFromState(base.defaultStatePtr);
		}

		public unsafe TValue ReadValueFromState(void* statePtr)
		{
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return this.ProcessValue(this.ReadUnprocessedValueFromState(statePtr));
		}

		public unsafe TValue ReadValueFromStateWithCaching(void* statePtr)
		{
			if (statePtr != base.currentStatePtr)
			{
				return this.ReadValueFromState(statePtr);
			}
			return *this.value;
		}

		public unsafe TValue ReadUnprocessedValueFromStateWithCaching(void* statePtr)
		{
			if (statePtr != base.currentStatePtr)
			{
				return this.ReadUnprocessedValueFromState(statePtr);
			}
			return *this.unprocessedValue;
		}

		public unsafe TValue ReadUnprocessedValue()
		{
			return *this.unprocessedValue;
		}

		public unsafe abstract TValue ReadUnprocessedValueFromState(void* statePtr);

		public unsafe override object ReadValueFromStateAsObject(void* statePtr)
		{
			return this.ReadValueFromState(statePtr);
		}

		public unsafe override void ReadValueFromStateIntoBuffer(void* statePtr, void* bufferPtr, int bufferSize)
		{
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			if (bufferPtr == null)
			{
				throw new ArgumentNullException("bufferPtr");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("bufferSize={0} < sizeof(TValue)={1}", bufferSize, num), "bufferSize");
			}
			TValue tvalue = this.ReadValueFromState(statePtr);
			void* source = UnsafeUtility.AddressOf<TValue>(ref tvalue);
			UnsafeUtility.MemCpy(bufferPtr, source, (long)num);
		}

		public unsafe override void WriteValueFromBufferIntoState(void* bufferPtr, int bufferSize, void* statePtr)
		{
			if (bufferPtr == null)
			{
				throw new ArgumentNullException("bufferPtr");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("bufferSize={0} < sizeof(TValue)={1}", bufferSize, num), "bufferSize");
			}
			TValue value = default(TValue);
			UnsafeUtility.MemCpy(UnsafeUtility.AddressOf<TValue>(ref value), bufferPtr, (long)num);
			this.WriteValueIntoState(value, statePtr);
		}

		public unsafe override void WriteValueFromObjectIntoState(object value, void* statePtr)
		{
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is TValue))
			{
				value = Convert.ChangeType(value, typeof(TValue));
			}
			TValue value2 = (TValue)((object)value);
			this.WriteValueIntoState(value2, statePtr);
		}

		public unsafe virtual void WriteValueIntoState(TValue value, void* statePtr)
		{
			throw new NotSupportedException(string.Format("Control '{0}' does not support writing", this));
		}

		public unsafe override object ReadValueFromBufferAsObject(void* buffer, int bufferSize)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("Expecting buffer of at least {0} bytes for value of type {1} but got buffer of only {2} bytes instead", num, typeof(TValue).Name, bufferSize), "bufferSize");
			}
			TValue tvalue = default(TValue);
			UnsafeUtility.MemCpy(UnsafeUtility.AddressOf<TValue>(ref tvalue), buffer, (long)num);
			return tvalue;
		}

		private unsafe static bool CompareValue(ref TValue firstValue, ref TValue secondValue)
		{
			void* ptr = UnsafeUtility.AddressOf<TValue>(ref firstValue);
			void* ptr2 = UnsafeUtility.AddressOf<TValue>(ref secondValue);
			return UnsafeUtility.MemCmp(ptr, ptr2, (long)UnsafeUtility.SizeOf<TValue>()) != 0;
		}

		public unsafe override bool CompareValue(void* firstStatePtr, void* secondStatePtr)
		{
			TValue tvalue = this.ReadValueFromState(firstStatePtr);
			TValue tvalue2 = this.ReadValueFromState(secondStatePtr);
			return InputControl<TValue>.CompareValue(ref tvalue, ref tvalue2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue ProcessValue(TValue value)
		{
			this.ProcessValue(ref value);
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ProcessValue(ref TValue value)
		{
			if (this.m_ProcessorStack.length <= 0)
			{
				return;
			}
			value = this.m_ProcessorStack.firstValue.Process(value, this);
			if (this.m_ProcessorStack.additionalValues == null)
			{
				return;
			}
			for (int i = 0; i < this.m_ProcessorStack.length - 1; i++)
			{
				value = this.m_ProcessorStack.additionalValues[i].Process(value, this);
			}
		}

		internal TProcessor TryGetProcessor<TProcessor>() where TProcessor : InputProcessor<TValue>
		{
			if (this.m_ProcessorStack.length > 0)
			{
				TProcessor tprocessor = this.m_ProcessorStack.firstValue as TProcessor;
				if (tprocessor != null)
				{
					return tprocessor;
				}
				if (this.m_ProcessorStack.additionalValues != null)
				{
					for (int i = 0; i < this.m_ProcessorStack.length - 1; i++)
					{
						TProcessor tprocessor2 = this.m_ProcessorStack.additionalValues[i] as TProcessor;
						if (tprocessor2 != null)
						{
							return tprocessor2;
						}
					}
				}
			}
			return default(TProcessor);
		}

		internal override void AddProcessor(object processor)
		{
			InputProcessor<TValue> inputProcessor = processor as InputProcessor<TValue>;
			if (inputProcessor == null)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Cannot add processor of type '",
					processor.GetType().Name,
					"' to control of type '",
					base.GetType().Name,
					"'"
				}), "processor");
			}
			this.m_ProcessorStack.Append(inputProcessor);
		}

		protected override void FinishSetup()
		{
			using (IEnumerator<InputProcessor<TValue>> enumerator = this.m_ProcessorStack.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.cachingPolicy == InputProcessor.CachingPolicy.EvaluateOnEveryRead)
					{
						this.evaluateProcessorsEveryRead = true;
					}
				}
			}
			base.FinishSetup();
		}

		internal InputProcessor<TValue>[] processors
		{
			get
			{
				return this.m_ProcessorStack.ToArray();
			}
		}

		internal InlinedArray<InputProcessor<TValue>> m_ProcessorStack;

		private TValue m_CachedValue;

		private TValue m_UnprocessedCachedValue;

		internal bool evaluateProcessorsEveryRead;
	}
}
