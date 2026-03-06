using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering.Universal.UTess
{
	[DebuggerDisplay("Length = {Length}")]
	[DebuggerTypeProxy(typeof(ArraySliceDebugView<>))]
	internal struct ArraySlice<T> : IEquatable<ArraySlice<T>> where T : struct
	{
		public unsafe ArraySlice(NativeArray<T> array, int start, int length)
		{
			this.m_Stride = UnsafeUtility.SizeOf<T>();
			byte* buffer = (byte*)array.GetUnsafePtr<T>() + this.m_Stride * start;
			this.m_Buffer = buffer;
			this.m_Length = length;
		}

		public bool Equals(ArraySlice<T> other)
		{
			return this.m_Buffer == other.m_Buffer && this.m_Stride == other.m_Stride && this.m_Length == other.m_Length;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is ArraySlice<T> && this.Equals((ArraySlice<T>)obj);
		}

		public override int GetHashCode()
		{
			return (this.m_Buffer * 397 ^ this.m_Stride) * 397 ^ this.m_Length;
		}

		public static bool operator ==(ArraySlice<T> left, ArraySlice<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ArraySlice<T> left, ArraySlice<T> right)
		{
			return !left.Equals(right);
		}

		public unsafe static ArraySlice<T> ConvertExistingDataToArraySlice(void* dataPointer, int stride, int length)
		{
			if (length < 0)
			{
				throw new ArgumentException(string.Format("Invalid length of '{0}'. It must be greater than 0.", length), "length");
			}
			if (stride < 0)
			{
				throw new ArgumentException(string.Format("Invalid stride '{0}'. It must be greater than 0.", stride), "stride");
			}
			return new ArraySlice<T>
			{
				m_Stride = stride,
				m_Buffer = (byte*)dataPointer,
				m_Length = length
			};
		}

		public unsafe T this[int index]
		{
			get
			{
				return UnsafeUtility.ReadArrayElementWithStride<T>((void*)this.m_Buffer, index, this.m_Stride);
			}
			[WriteAccessRequired]
			set
			{
				UnsafeUtility.WriteArrayElementWithStride<T>((void*)this.m_Buffer, index, this.m_Stride, value);
			}
		}

		internal unsafe void* GetUnsafeReadOnlyPtr()
		{
			return (void*)this.m_Buffer;
		}

		internal unsafe void CopyTo(T[] array)
		{
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			IntPtr value = gchandle.AddrOfPinnedObject();
			int num = UnsafeUtility.SizeOf<T>();
			UnsafeUtility.MemCpyStride((void*)value, num, this.GetUnsafeReadOnlyPtr(), this.Stride, num, this.m_Length);
			gchandle.Free();
		}

		internal T[] ToArray()
		{
			T[] array = new T[this.Length];
			this.CopyTo(array);
			return array;
		}

		public int Stride
		{
			get
			{
				return this.m_Stride;
			}
		}

		public int Length
		{
			get
			{
				return this.m_Length;
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe byte* m_Buffer;

		internal int m_Stride;

		internal int m_Length;
	}
}
