using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.InputSystem
{
	public abstract class InputProcessor<TValue> : InputProcessor where TValue : struct
	{
		public abstract TValue Process(TValue value, InputControl control);

		public override object ProcessAsObject(object value, InputControl control)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is TValue))
			{
				throw new ArgumentException(string.Format("Expecting value of type '{0}' but got value '{1}' of type '{2}'", typeof(TValue).Name, value, value.GetType().Name), "value");
			}
			TValue value2 = (TValue)((object)value);
			return this.Process(value2, control);
		}

		public unsafe override void Process(void* buffer, int bufferSize, InputControl control)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("Expected buffer of at least {0} bytes but got buffer with just {1} bytes", num, bufferSize), "bufferSize");
			}
			TValue value = default(TValue);
			void* ptr = UnsafeUtility.AddressOf<TValue>(ref value);
			UnsafeUtility.MemCpy(ptr, buffer, (long)num);
			value = this.Process(value, control);
			ptr = UnsafeUtility.AddressOf<TValue>(ref value);
			UnsafeUtility.MemCpy(buffer, ptr, (long)num);
		}
	}
}
