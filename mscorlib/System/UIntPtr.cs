using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	/// <summary>A platform-specific type that is used to represent a pointer or a handle.</summary>
	[ComVisible(true)]
	[CLSCompliant(false)]
	[Serializable]
	public readonly struct UIntPtr : ISerializable, IEquatable<UIntPtr>
	{
		/// <summary>Initializes a new instance of <see cref="T:System.UIntPtr" /> using the specified 64-bit pointer or handle.</summary>
		/// <param name="value">A pointer or handle contained in a 64-bit unsigned integer.</param>
		/// <exception cref="T:System.OverflowException">On a 32-bit platform, <paramref name="value" /> is too large to represent as an <see cref="T:System.UIntPtr" />.</exception>
		public UIntPtr(ulong value)
		{
			if (value > (ulong)-1 && UIntPtr.Size < 8)
			{
				throw new OverflowException(Locale.GetText("This isn't a 64bits machine."));
			}
			this._pointer = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.UIntPtr" /> structure using the specified 32-bit pointer or handle.</summary>
		/// <param name="value">A pointer or handle contained in a 32-bit unsigned integer.</param>
		public UIntPtr(uint value)
		{
			this._pointer = value;
		}

		/// <summary>Initializes a new instance of <see cref="T:System.UIntPtr" /> using the specified pointer to an unspecified type.</summary>
		/// <param name="value">A pointer to an unspecified type.</param>
		[CLSCompliant(false)]
		public unsafe UIntPtr(void* value)
		{
			this._pointer = value;
		}

		/// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
		/// <param name="obj">An object to compare with this instance or <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is an instance of <see cref="T:System.UIntPtr" /> and equals the value of this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj is UIntPtr)
			{
				UIntPtr uintPtr = (UIntPtr)obj;
				return this._pointer == uintPtr._pointer;
			}
			return false;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this._pointer;
		}

		/// <summary>Converts the value of this instance to a 32-bit unsigned integer.</summary>
		/// <returns>A 32-bit unsigned integer equal to the value of this instance.</returns>
		/// <exception cref="T:System.OverflowException">On a 64-bit platform, the value of this instance is too large to represent as a 32-bit unsigned integer.</exception>
		public uint ToUInt32()
		{
			return this._pointer;
		}

		/// <summary>Converts the value of this instance to a 64-bit unsigned integer.</summary>
		/// <returns>A 64-bit unsigned integer equal to the value of this instance.</returns>
		public ulong ToUInt64()
		{
			return this._pointer;
		}

		/// <summary>Converts the value of this instance to a pointer to an unspecified type.</summary>
		/// <returns>A pointer to <see cref="T:System.Void" />; that is, a pointer to memory containing data of an unspecified type.</returns>
		[CLSCompliant(false)]
		public unsafe void* ToPointer()
		{
			return this._pointer;
		}

		/// <summary>Converts the numeric value of this instance to its equivalent string representation.</summary>
		/// <returns>The string representation of the value of this instance.</returns>
		public override string ToString()
		{
			if (UIntPtr.Size >= 8)
			{
				return this._pointer.ToString();
			}
			return this._pointer.ToString();
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the data needed to serialize the current <see cref="T:System.UIntPtr" /> object.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object to populate with data.</param>
		/// <param name="context">The destination for this serialization. (This parameter is not used; specify <see langword="null" />.)</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("pointer", this._pointer);
		}

		/// <summary>Determines whether two specified instances of <see cref="T:System.UIntPtr" /> are equal.</summary>
		/// <param name="value1">The first pointer or handle to compare.</param>
		/// <param name="value2">The second pointer or handle to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value1" /> equals <paramref name="value2" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(UIntPtr value1, UIntPtr value2)
		{
			return value1._pointer == value2._pointer;
		}

		/// <summary>Determines whether two specified instances of <see cref="T:System.UIntPtr" /> are not equal.</summary>
		/// <param name="value1">The first pointer or handle to compare.</param>
		/// <param name="value2">The second pointer or handle to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value1" /> does not equal <paramref name="value2" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(UIntPtr value1, UIntPtr value2)
		{
			return value1._pointer != value2._pointer;
		}

		/// <summary>Converts the value of the specified <see cref="T:System.UIntPtr" /> to a 64-bit unsigned integer.</summary>
		/// <param name="value">The pointer or handle to convert.</param>
		/// <returns>The contents of <paramref name="value" />.</returns>
		public static explicit operator ulong(UIntPtr value)
		{
			return value._pointer;
		}

		/// <summary>Converts the value of the specified <see cref="T:System.UIntPtr" /> to a 32-bit unsigned integer.</summary>
		/// <param name="value">The pointer or handle to convert.</param>
		/// <returns>The contents of <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">On a 64-bit platform, the value of <paramref name="value" /> is too large to represent as a 32-bit unsigned integer.</exception>
		public static explicit operator uint(UIntPtr value)
		{
			return value._pointer;
		}

		/// <summary>Converts the value of a 64-bit unsigned integer to an <see cref="T:System.UIntPtr" />.</summary>
		/// <param name="value">A 64-bit unsigned integer.</param>
		/// <returns>A new instance of <see cref="T:System.UIntPtr" /> initialized to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">On a 32-bit platform, <paramref name="value" /> is too large to represent as an <see cref="T:System.UIntPtr" />.</exception>
		public static explicit operator UIntPtr(ulong value)
		{
			return new UIntPtr(value);
		}

		/// <summary>Converts the specified pointer to an unspecified type to an <see cref="T:System.UIntPtr" />.  
		///  This API is not CLS-compliant.</summary>
		/// <param name="value">A pointer to an unspecified type.</param>
		/// <returns>A new instance of <see cref="T:System.UIntPtr" /> initialized to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public unsafe static explicit operator UIntPtr(void* value)
		{
			return new UIntPtr(value);
		}

		/// <summary>Converts the value of the specified <see cref="T:System.UIntPtr" /> to a pointer to an unspecified type.  
		///  This API is not CLS-compliant.</summary>
		/// <param name="value">The pointer or handle to convert.</param>
		/// <returns>The contents of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public unsafe static explicit operator void*(UIntPtr value)
		{
			return value.ToPointer();
		}

		/// <summary>Converts the value of a 32-bit unsigned integer to an <see cref="T:System.UIntPtr" />.</summary>
		/// <param name="value">A 32-bit unsigned integer.</param>
		/// <returns>A new instance of <see cref="T:System.UIntPtr" /> initialized to <paramref name="value" />.</returns>
		public static explicit operator UIntPtr(uint value)
		{
			return new UIntPtr(value);
		}

		/// <summary>Gets the size of this instance.</summary>
		/// <returns>The size of a pointer or handle on this platform, measured in bytes. The value of this property is 4 on a 32-bit platform, and 8 on a 64-bit platform.</returns>
		public unsafe static int Size
		{
			get
			{
				return sizeof(void*);
			}
		}

		/// <summary>Adds an offset to the value of an unsigned pointer.</summary>
		/// <param name="pointer">The unsigned pointer to add the offset to.</param>
		/// <param name="offset">The offset to add.</param>
		/// <returns>A new unsigned pointer that reflects the addition of <paramref name="offset" /> to <paramref name="pointer" />.</returns>
		public unsafe static UIntPtr Add(UIntPtr pointer, int offset)
		{
			return (UIntPtr)((void*)((byte*)((void*)pointer) + offset));
		}

		/// <summary>Subtracts an offset from the value of an unsigned pointer.</summary>
		/// <param name="pointer">The unsigned pointer to subtract the offset from.</param>
		/// <param name="offset">The offset to subtract.</param>
		/// <returns>A new unsigned pointer that reflects the subtraction of <paramref name="offset" /> from <paramref name="pointer" />.</returns>
		public unsafe static UIntPtr Subtract(UIntPtr pointer, int offset)
		{
			return (UIntPtr)((void*)((byte*)((void*)pointer) - offset));
		}

		/// <summary>Adds an offset to the value of an unsigned pointer.</summary>
		/// <param name="pointer">The unsigned pointer to add the offset to.</param>
		/// <param name="offset">The offset to add.</param>
		/// <returns>A new unsigned pointer that reflects the addition of <paramref name="offset" /> to <paramref name="pointer" />.</returns>
		public unsafe static UIntPtr operator +(UIntPtr pointer, int offset)
		{
			return (UIntPtr)((void*)((byte*)((void*)pointer) + offset));
		}

		/// <summary>Subtracts an offset from the value of an unsigned pointer.</summary>
		/// <param name="pointer">The unsigned pointer to subtract the offset from.</param>
		/// <param name="offset">The offset to subtract.</param>
		/// <returns>A new unsigned pointer that reflects the subtraction of <paramref name="offset" /> from <paramref name="pointer" />.</returns>
		public unsafe static UIntPtr operator -(UIntPtr pointer, int offset)
		{
			return (UIntPtr)((void*)((byte*)((void*)pointer) - offset));
		}

		bool IEquatable<UIntPtr>.Equals(UIntPtr other)
		{
			return this._pointer == other._pointer;
		}

		/// <summary>A read-only field that represents a pointer or handle that has been initialized to zero.</summary>
		public static readonly UIntPtr Zero = new UIntPtr(0U);

		private unsafe readonly void* _pointer;
	}
}
