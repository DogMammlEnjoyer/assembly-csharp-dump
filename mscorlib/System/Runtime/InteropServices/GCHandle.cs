using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Runtime.InteropServices
{
	/// <summary>Provides a way to access a managed object from unmanaged memory.</summary>
	[ComVisible(true)]
	public struct GCHandle
	{
		private GCHandle(IntPtr h)
		{
			this.handle = h;
		}

		private GCHandle(object obj)
		{
			this = new GCHandle(obj, GCHandleType.Normal);
		}

		internal GCHandle(object value, GCHandleType type)
		{
			if (type < GCHandleType.Weak || type > GCHandleType.Pinned)
			{
				type = GCHandleType.Normal;
			}
			this.handle = GCHandle.GetTargetHandle(value, IntPtr.Zero, type);
		}

		/// <summary>Gets a value indicating whether the handle is allocated.</summary>
		/// <returns>
		///   <see langword="true" /> if the handle is allocated; otherwise, <see langword="false" />.</returns>
		public bool IsAllocated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.handle != IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static object GetRef(IntPtr handle)
		{
			return *Unsafe.As<IntPtr, object>(ref *(IntPtr*)((void*)handle));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void SetRef(IntPtr handle, object value)
		{
			*Unsafe.As<IntPtr, object>(ref *(IntPtr*)((void*)handle)) = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool CanDereferenceHandle(IntPtr handle)
		{
			return (handle & (IntPtr)1) == (IntPtr)0;
		}

		/// <summary>Gets or sets the object this handle represents.</summary>
		/// <returns>The object this handle represents.</returns>
		/// <exception cref="T:System.InvalidOperationException">The handle was freed, or never initialized.</exception>
		public object Target
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (!this.IsAllocated)
				{
					throw new InvalidOperationException("Handle is not allocated");
				}
				if (GCHandle.CanDereferenceHandle(this.handle))
				{
					return GCHandle.GetRef(this.handle);
				}
				return GCHandle.GetTarget(this.handle);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (GCHandle.CanDereferenceHandle(this.handle))
				{
					GCHandle.SetRef(this.handle, value);
					return;
				}
				this.handle = GCHandle.GetTargetHandle(value, this.handle, (GCHandleType)(-1));
			}
		}

		/// <summary>Retrieves the address of an object in a <see cref="F:System.Runtime.InteropServices.GCHandleType.Pinned" /> handle.</summary>
		/// <returns>The address of the pinned object as an <see cref="T:System.IntPtr" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The handle is any type other than <see cref="F:System.Runtime.InteropServices.GCHandleType.Pinned" />.</exception>
		public IntPtr AddrOfPinnedObject()
		{
			IntPtr addrOfPinnedObject = GCHandle.GetAddrOfPinnedObject(this.handle);
			if (addrOfPinnedObject == (IntPtr)(-1))
			{
				throw new ArgumentException("Object contains non-primitive or non-blittable data.");
			}
			if (addrOfPinnedObject == (IntPtr)(-2))
			{
				throw new InvalidOperationException("Handle is not pinned.");
			}
			return addrOfPinnedObject;
		}

		/// <summary>Allocates a <see cref="F:System.Runtime.InteropServices.GCHandleType.Normal" /> handle for the specified object.</summary>
		/// <param name="value">The object that uses the <see cref="T:System.Runtime.InteropServices.GCHandle" />.</param>
		/// <returns>A new <see cref="T:System.Runtime.InteropServices.GCHandle" /> that protects the object from garbage collection. This <see cref="T:System.Runtime.InteropServices.GCHandle" /> must be released with <see cref="M:System.Runtime.InteropServices.GCHandle.Free" /> when it is no longer needed.</returns>
		/// <exception cref="T:System.ArgumentException">An instance with nonprimitive (non-blittable) members cannot be pinned.</exception>
		public static GCHandle Alloc(object value)
		{
			return new GCHandle(value);
		}

		/// <summary>Allocates a handle of the specified type for the specified object.</summary>
		/// <param name="value">The object that uses the <see cref="T:System.Runtime.InteropServices.GCHandle" />.</param>
		/// <param name="type">One of the <see cref="T:System.Runtime.InteropServices.GCHandleType" /> values, indicating the type of <see cref="T:System.Runtime.InteropServices.GCHandle" /> to create.</param>
		/// <returns>A new <see cref="T:System.Runtime.InteropServices.GCHandle" /> of the specified type. This <see cref="T:System.Runtime.InteropServices.GCHandle" /> must be released with <see cref="M:System.Runtime.InteropServices.GCHandle.Free" /> when it is no longer needed.</returns>
		/// <exception cref="T:System.ArgumentException">An instance with nonprimitive (non-blittable) members cannot be pinned.</exception>
		public static GCHandle Alloc(object value, GCHandleType type)
		{
			return new GCHandle(value, type);
		}

		/// <summary>Releases a <see cref="T:System.Runtime.InteropServices.GCHandle" />.</summary>
		/// <exception cref="T:System.InvalidOperationException">The handle was freed or never initialized.</exception>
		public void Free()
		{
			IntPtr intPtr = this.handle;
			if (intPtr != IntPtr.Zero && Interlocked.CompareExchange(ref this.handle, IntPtr.Zero, intPtr) == intPtr)
			{
				GCHandle.FreeHandle(intPtr);
				return;
			}
			throw new InvalidOperationException("Handle is not initialized.");
		}

		/// <summary>A <see cref="T:System.Runtime.InteropServices.GCHandle" /> is stored using an internal integer representation.</summary>
		/// <param name="value">The <see cref="T:System.Runtime.InteropServices.GCHandle" /> for which the integer is required.</param>
		/// <returns>The integer value.</returns>
		public static explicit operator IntPtr(GCHandle value)
		{
			return value.handle;
		}

		/// <summary>A <see cref="T:System.Runtime.InteropServices.GCHandle" /> is stored using an internal integer representation.</summary>
		/// <param name="value">An <see cref="T:System.IntPtr" /> that indicates the handle for which the conversion is required.</param>
		/// <returns>The stored <see cref="T:System.Runtime.InteropServices.GCHandle" /> object using an internal integer representation.</returns>
		public static explicit operator GCHandle(IntPtr value)
		{
			if (value == IntPtr.Zero)
			{
				throw new InvalidOperationException("GCHandle value cannot be zero");
			}
			if (!GCHandle.CheckCurrentDomain(value))
			{
				throw new ArgumentException("GCHandle value belongs to a different domain");
			}
			return new GCHandle(value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CheckCurrentDomain(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetTarget(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTargetHandle(object obj, IntPtr handle, GCHandleType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeHandle(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetAddrOfPinnedObject(IntPtr handle);

		/// <summary>Returns a value indicating whether two <see cref="T:System.Runtime.InteropServices.GCHandle" /> objects are equal.</summary>
		/// <param name="a">A <see cref="T:System.Runtime.InteropServices.GCHandle" /> object to compare with the <paramref name="b" /> parameter.</param>
		/// <param name="b">A <see cref="T:System.Runtime.InteropServices.GCHandle" /> object to compare with the <paramref name="a" /> parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="a" /> and <paramref name="b" /> parameters are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(GCHandle a, GCHandle b)
		{
			return a.handle == b.handle;
		}

		/// <summary>Returns a value indicating whether two <see cref="T:System.Runtime.InteropServices.GCHandle" /> objects are not equal.</summary>
		/// <param name="a">A <see cref="T:System.Runtime.InteropServices.GCHandle" /> object to compare with the <paramref name="b" /> parameter.</param>
		/// <param name="b">A <see cref="T:System.Runtime.InteropServices.GCHandle" /> object to compare with the <paramref name="a" /> parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="a" /> and <paramref name="b" /> parameters are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(GCHandle a, GCHandle b)
		{
			return !(a == b);
		}

		/// <summary>Determines whether the specified <see cref="T:System.Runtime.InteropServices.GCHandle" /> object is equal to the current <see cref="T:System.Runtime.InteropServices.GCHandle" /> object.</summary>
		/// <param name="o">The <see cref="T:System.Runtime.InteropServices.GCHandle" /> object to compare with the current <see cref="T:System.Runtime.InteropServices.GCHandle" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Runtime.InteropServices.GCHandle" /> object is equal to the current <see cref="T:System.Runtime.InteropServices.GCHandle" /> object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			return o is GCHandle && this == (GCHandle)o;
		}

		/// <summary>Returns an identifier for the current <see cref="T:System.Runtime.InteropServices.GCHandle" /> object.</summary>
		/// <returns>An identifier for the current <see cref="T:System.Runtime.InteropServices.GCHandle" /> object.</returns>
		public override int GetHashCode()
		{
			return this.handle.GetHashCode();
		}

		/// <summary>Returns a new <see cref="T:System.Runtime.InteropServices.GCHandle" /> object created from a handle to a managed object.</summary>
		/// <param name="value">An <see cref="T:System.IntPtr" /> handle to a managed object to create a <see cref="T:System.Runtime.InteropServices.GCHandle" /> object from.</param>
		/// <returns>A new <see cref="T:System.Runtime.InteropServices.GCHandle" /> object that corresponds to the value parameter.</returns>
		/// <exception cref="T:System.InvalidOperationException">The value of the <paramref name="value" /> parameter is <see cref="F:System.IntPtr.Zero" />.</exception>
		public static GCHandle FromIntPtr(IntPtr value)
		{
			return (GCHandle)value;
		}

		/// <summary>Returns the internal integer representation of a <see cref="T:System.Runtime.InteropServices.GCHandle" /> object.</summary>
		/// <param name="value">A <see cref="T:System.Runtime.InteropServices.GCHandle" /> object to retrieve an internal integer representation from.</param>
		/// <returns>An <see cref="T:System.IntPtr" /> object that represents a <see cref="T:System.Runtime.InteropServices.GCHandle" /> object.</returns>
		public static IntPtr ToIntPtr(GCHandle value)
		{
			return (IntPtr)value;
		}

		private IntPtr handle;
	}
}
