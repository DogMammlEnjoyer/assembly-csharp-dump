using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.InputSystem
{
	public abstract class InputBindingComposite<TValue> : InputBindingComposite where TValue : struct
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

		public abstract TValue ReadValue(ref InputBindingCompositeContext context);

		public unsafe override void ReadValue(ref InputBindingCompositeContext context, void* buffer, int bufferSize)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = UnsafeUtility.SizeOf<TValue>();
			if (bufferSize < num)
			{
				throw new ArgumentException(string.Format("Expected buffer of at least {0} bytes but got buffer of only {1} bytes instead", UnsafeUtility.SizeOf<TValue>(), bufferSize), "bufferSize");
			}
			TValue tvalue = this.ReadValue(ref context);
			void* source = UnsafeUtility.AddressOf<TValue>(ref tvalue);
			UnsafeUtility.MemCpy(buffer, source, (long)num);
		}

		public unsafe override object ReadValueAsObject(ref InputBindingCompositeContext context)
		{
			TValue tvalue = default(TValue);
			void* buffer = UnsafeUtility.AddressOf<TValue>(ref tvalue);
			this.ReadValue(ref context, buffer, UnsafeUtility.SizeOf<TValue>());
			return tvalue;
		}
	}
}
