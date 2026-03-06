using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Wraps a managed object holding a handle to a resource that is passed to unmanaged code using platform invoke.</summary>
	public readonly struct HandleRef
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.HandleRef" /> class with the object to wrap and a handle to the resource used by unmanaged code.</summary>
		/// <param name="wrapper">A managed object that should not be finalized until the platform invoke call returns.</param>
		/// <param name="handle">An <see cref="T:System.IntPtr" /> that indicates a handle to a resource.</param>
		public HandleRef(object wrapper, IntPtr handle)
		{
			this._wrapper = wrapper;
			this._handle = handle;
		}

		/// <summary>Gets the object holding the handle to a resource.</summary>
		/// <returns>The object holding the handle to a resource.</returns>
		public object Wrapper
		{
			get
			{
				return this._wrapper;
			}
		}

		/// <summary>Gets the handle to a resource.</summary>
		/// <returns>The handle to a resource.</returns>
		public IntPtr Handle
		{
			get
			{
				return this._handle;
			}
		}

		/// <summary>Returns the handle to a resource of the specified <see cref="T:System.Runtime.InteropServices.HandleRef" /> object.</summary>
		/// <param name="value">The object that needs a handle.</param>
		/// <returns>The handle to a resource of the specified <see cref="T:System.Runtime.InteropServices.HandleRef" /> object.</returns>
		public static explicit operator IntPtr(HandleRef value)
		{
			return value._handle;
		}

		/// <summary>Returns the internal integer representation of a <see cref="T:System.Runtime.InteropServices.HandleRef" /> object.</summary>
		/// <param name="value">A <see cref="T:System.Runtime.InteropServices.HandleRef" /> object to retrieve an internal integer representation from.</param>
		/// <returns>An <see cref="T:System.IntPtr" /> object that represents a <see cref="T:System.Runtime.InteropServices.HandleRef" /> object.</returns>
		public static IntPtr ToIntPtr(HandleRef value)
		{
			return value._handle;
		}

		private readonly object _wrapper;

		private readonly IntPtr _handle;
	}
}
