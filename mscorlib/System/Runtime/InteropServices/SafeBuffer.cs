using System;
using System.Runtime.CompilerServices;
using Microsoft.Win32.SafeHandles;

namespace System.Runtime.InteropServices
{
	/// <summary>Provides a controlled memory buffer that can be used for reading and writing. Attempts to access memory outside the controlled buffer (underruns and overruns) raise exceptions.</summary>
	public abstract class SafeBuffer : SafeHandleZeroOrMinusOneIsInvalid
	{
		/// <summary>Creates a new instance of the <see cref="T:System.Runtime.InteropServices.SafeBuffer" /> class, and specifies whether the buffer handle is to be reliably released.</summary>
		/// <param name="ownsHandle">
		///   <see langword="true" /> to reliably release the handle during the finalization phase; <see langword="false" /> to prevent reliable release (not recommended).</param>
		protected SafeBuffer(bool ownsHandle) : base(ownsHandle)
		{
			this._numBytes = SafeBuffer.Uninitialized;
		}

		/// <summary>Defines the allocation size of the memory region in bytes. You must call this method before you use the <see cref="T:System.Runtime.InteropServices.SafeBuffer" /> instance.</summary>
		/// <param name="numBytes">The number of bytes in the buffer.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="numBytes" /> is less than zero.  
		/// -or-  
		/// <paramref name="numBytes" /> is greater than the available address space.</exception>
		[CLSCompliant(false)]
		public void Initialize(ulong numBytes)
		{
			if (IntPtr.Size == 4 && numBytes > (ulong)-1)
			{
				throw new ArgumentOutOfRangeException("numBytes", "The number of bytes cannot exceed the virtual address space on a 32 bit machine.");
			}
			if (numBytes >= (ulong)SafeBuffer.Uninitialized)
			{
				throw new ArgumentOutOfRangeException("numBytes", "The length of the buffer must be less than the maximum UIntPtr value for your platform.");
			}
			this._numBytes = (UIntPtr)numBytes;
		}

		/// <summary>Specifies the allocation size of the memory buffer by using the specified number of elements and element size. You must call this method before you use the <see cref="T:System.Runtime.InteropServices.SafeBuffer" /> instance.</summary>
		/// <param name="numElements">The number of elements in the buffer.</param>
		/// <param name="sizeOfEachElement">The size of each element in the buffer.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="numElements" /> is less than zero.  
		/// -or-  
		/// <paramref name="sizeOfEachElement" /> is less than zero.  
		/// -or-  
		/// <paramref name="numElements" /> multiplied by <paramref name="sizeOfEachElement" /> is greater than the available address space.</exception>
		[CLSCompliant(false)]
		public void Initialize(uint numElements, uint sizeOfEachElement)
		{
			if (IntPtr.Size == 4 && numElements * sizeOfEachElement > 4294967295U)
			{
				throw new ArgumentOutOfRangeException("numBytes", "The number of bytes cannot exceed the virtual address space on a 32 bit machine.");
			}
			if ((ulong)(numElements * sizeOfEachElement) >= (ulong)SafeBuffer.Uninitialized)
			{
				throw new ArgumentOutOfRangeException("numElements", "The length of the buffer must be less than the maximum UIntPtr value for your platform.");
			}
			this._numBytes = (UIntPtr)(checked(numElements * sizeOfEachElement));
		}

		/// <summary>Defines the allocation size of the memory region by specifying the number of value types. You must call this method before you use the <see cref="T:System.Runtime.InteropServices.SafeBuffer" /> instance.</summary>
		/// <param name="numElements">The number of elements of the value type to allocate memory for.</param>
		/// <typeparam name="T">The value type to allocate memory for.</typeparam>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="numElements" /> is less than zero.  
		/// -or-  
		/// <paramref name="numElements" /> multiplied by the size of each element is greater than the available address space.</exception>
		[CLSCompliant(false)]
		public void Initialize<T>(uint numElements) where T : struct
		{
			this.Initialize(numElements, SafeBuffer.AlignedSizeOf<T>());
		}

		/// <summary>Obtains a pointer from a <see cref="T:System.Runtime.InteropServices.SafeBuffer" /> object for a block of memory.</summary>
		/// <param name="pointer">A byte pointer, passed by reference, to receive the pointer from within the <see cref="T:System.Runtime.InteropServices.SafeBuffer" /> object. You must set this pointer to <see langword="null" /> before you call this method.</param>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Overload:System.Runtime.InteropServices.SafeBuffer.Initialize" /> method has not been called.</exception>
		[CLSCompliant(false)]
		public unsafe void AcquirePointer(ref byte* pointer)
		{
			if (this._numBytes == SafeBuffer.Uninitialized)
			{
				throw SafeBuffer.NotInitialized();
			}
			pointer = (IntPtr)((UIntPtr)0);
			bool flag = false;
			base.DangerousAddRef(ref flag);
			pointer = (void*)this.handle;
		}

		/// <summary>Releases a pointer that was obtained by the <see cref="M:System.Runtime.InteropServices.SafeBuffer.AcquirePointer(System.Byte*@)" /> method.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Overload:System.Runtime.InteropServices.SafeBuffer.Initialize" /> method has not been called.</exception>
		public void ReleasePointer()
		{
			if (this._numBytes == SafeBuffer.Uninitialized)
			{
				throw SafeBuffer.NotInitialized();
			}
			base.DangerousRelease();
		}

		/// <summary>Reads a value type from memory at the specified offset.</summary>
		/// <param name="byteOffset">The location from which to read the value type. You may have to consider alignment issues.</param>
		/// <typeparam name="T">The value type to read.</typeparam>
		/// <returns>The value type that was read from memory.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Overload:System.Runtime.InteropServices.SafeBuffer.Initialize" /> method has not been called.</exception>
		[CLSCompliant(false)]
		public unsafe T Read<T>(ulong byteOffset) where T : struct
		{
			if (this._numBytes == SafeBuffer.Uninitialized)
			{
				throw SafeBuffer.NotInitialized();
			}
			uint num = SafeBuffer.SizeOf<T>();
			byte* ptr = (byte*)((void*)this.handle) + byteOffset;
			this.SpaceCheck(ptr, (ulong)num);
			T result = default(T);
			bool flag = false;
			try
			{
				base.DangerousAddRef(ref flag);
				try
				{
					fixed (byte* ptr2 = Unsafe.As<T, byte>(ref result))
					{
						Buffer.Memmove(ptr2, ptr, num);
					}
				}
				finally
				{
					byte* ptr2 = null;
				}
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return result;
		}

		/// <summary>Reads the specified number of value types from memory starting at the offset, and writes them into an array starting at the index.</summary>
		/// <param name="byteOffset">The location from which to start reading.</param>
		/// <param name="array">The output array to write to.</param>
		/// <param name="index">The location in the output array to begin writing to.</param>
		/// <param name="count">The number of value types to read from the input array and to write to the output array.</param>
		/// <typeparam name="T">The value type to read.</typeparam>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.  
		/// -or-  
		/// <paramref name="count" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The length of the array minus the index is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Overload:System.Runtime.InteropServices.SafeBuffer.Initialize" /> method has not been called.</exception>
		[CLSCompliant(false)]
		public unsafe void ReadArray<T>(ulong byteOffset, T[] array, int index, int count) where T : struct
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Buffer cannot be null.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (array.Length - index < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			if (this._numBytes == SafeBuffer.Uninitialized)
			{
				throw SafeBuffer.NotInitialized();
			}
			uint num = SafeBuffer.SizeOf<T>();
			uint num2 = SafeBuffer.AlignedSizeOf<T>();
			byte* ptr = (byte*)((void*)this.handle) + byteOffset;
			this.SpaceCheck(ptr, checked(unchecked((ulong)num2) * (ulong)(unchecked((long)count))));
			bool flag = false;
			try
			{
				base.DangerousAddRef(ref flag);
				if (count > 0)
				{
					try
					{
						fixed (byte* ptr2 = Unsafe.As<T, byte>(ref array[index]))
						{
							byte* ptr3 = ptr2;
							for (int i = 0; i < count; i++)
							{
								Buffer.Memmove(ptr3 + (ulong)num * (ulong)((long)i), ptr + (ulong)num2 * (ulong)((long)i), num);
							}
						}
					}
					finally
					{
						byte* ptr2 = null;
					}
				}
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		/// <summary>Writes a value type to memory at the given location.</summary>
		/// <param name="byteOffset">The location at which to start writing. You may have to consider alignment issues.</param>
		/// <param name="value">The value to write.</param>
		/// <typeparam name="T">The value type to write.</typeparam>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Overload:System.Runtime.InteropServices.SafeBuffer.Initialize" /> method has not been called.</exception>
		[CLSCompliant(false)]
		public unsafe void Write<T>(ulong byteOffset, T value) where T : struct
		{
			if (this._numBytes == SafeBuffer.Uninitialized)
			{
				throw SafeBuffer.NotInitialized();
			}
			uint num = SafeBuffer.SizeOf<T>();
			byte* ptr = (byte*)((void*)this.handle) + byteOffset;
			this.SpaceCheck(ptr, (ulong)num);
			bool flag = false;
			try
			{
				base.DangerousAddRef(ref flag);
				try
				{
					fixed (byte* ptr2 = Unsafe.As<T, byte>(ref value))
					{
						byte* src = ptr2;
						Buffer.Memmove(ptr, src, num);
					}
				}
				finally
				{
					byte* ptr2 = null;
				}
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		/// <summary>Writes the specified number of value types to a memory location by reading bytes starting from the specified location in the input array.</summary>
		/// <param name="byteOffset">The location in memory to write to.</param>
		/// <param name="array">The input array.</param>
		/// <param name="index">The offset in the array to start reading from.</param>
		/// <param name="count">The number of value types to write.</param>
		/// <typeparam name="T">The value type to write.</typeparam>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">The length of the input array minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Overload:System.Runtime.InteropServices.SafeBuffer.Initialize" /> method has not been called.</exception>
		[CLSCompliant(false)]
		public unsafe void WriteArray<T>(ulong byteOffset, T[] array, int index, int count) where T : struct
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Buffer cannot be null.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (array.Length - index < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			if (this._numBytes == SafeBuffer.Uninitialized)
			{
				throw SafeBuffer.NotInitialized();
			}
			uint num = SafeBuffer.SizeOf<T>();
			uint num2 = SafeBuffer.AlignedSizeOf<T>();
			byte* ptr = (byte*)((void*)this.handle) + byteOffset;
			this.SpaceCheck(ptr, checked(unchecked((ulong)num2) * (ulong)(unchecked((long)count))));
			bool flag = false;
			try
			{
				base.DangerousAddRef(ref flag);
				if (count > 0)
				{
					try
					{
						fixed (byte* ptr2 = Unsafe.As<T, byte>(ref array[index]))
						{
							byte* ptr3 = ptr2;
							for (int i = 0; i < count; i++)
							{
								Buffer.Memmove(ptr + (ulong)num2 * (ulong)((long)i), ptr3 + (ulong)num * (ulong)((long)i), num);
							}
						}
					}
					finally
					{
						byte* ptr2 = null;
					}
				}
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
		}

		/// <summary>Gets the size of the buffer, in bytes.</summary>
		/// <returns>The number of bytes in the memory buffer.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Overload:System.Runtime.InteropServices.SafeBuffer.Initialize" /> method has not been called.</exception>
		[CLSCompliant(false)]
		public ulong ByteLength
		{
			get
			{
				if (this._numBytes == SafeBuffer.Uninitialized)
				{
					throw SafeBuffer.NotInitialized();
				}
				return (ulong)this._numBytes;
			}
		}

		private unsafe void SpaceCheck(byte* ptr, ulong sizeInBytes)
		{
			if ((ulong)this._numBytes < sizeInBytes)
			{
				SafeBuffer.NotEnoughRoom();
			}
			if ((long)((byte*)ptr - (byte*)((void*)this.handle)) > (long)((ulong)this._numBytes - sizeInBytes))
			{
				SafeBuffer.NotEnoughRoom();
			}
		}

		private static void NotEnoughRoom()
		{
			throw new ArgumentException("Not enough space available in the buffer.");
		}

		private static InvalidOperationException NotInitialized()
		{
			return new InvalidOperationException("You must call Initialize on this object instance before using it.");
		}

		internal static uint AlignedSizeOf<T>() where T : struct
		{
			uint num = SafeBuffer.SizeOf<T>();
			if (num == 1U || num == 2U)
			{
				return num;
			}
			return (uint)((ulong)(num + 3U) & 18446744073709551612UL);
		}

		internal static uint SizeOf<T>() where T : struct
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				throw new ArgumentException("The specified Type must be a struct containing no references.");
			}
			return (uint)Unsafe.SizeOf<T>();
		}

		private static readonly UIntPtr Uninitialized = (UIntPtr.Size == 4) ? ((UIntPtr)uint.MaxValue) : ((UIntPtr)ulong.MaxValue);

		private UIntPtr _numBytes;
	}
}
