using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using Unity;

namespace System.Threading
{
	/// <summary>Provides methods for setting and capturing the compressed stack on the current thread. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class CompressedStack : ISerializable
	{
		internal CompressedStack(int length)
		{
			if (length > 0)
			{
				this._list = new ArrayList(length);
			}
		}

		internal CompressedStack(CompressedStack cs)
		{
			if (cs != null && cs._list != null)
			{
				this._list = (ArrayList)cs._list.Clone();
			}
		}

		/// <summary>Creates a copy of the current compressed stack.</summary>
		/// <returns>A <see cref="T:System.Threading.CompressedStack" /> object representing the current compressed stack.</returns>
		[ComVisible(false)]
		public CompressedStack CreateCopy()
		{
			return new CompressedStack(this);
		}

		/// <summary>Captures the compressed stack from the current thread.</summary>
		/// <returns>A <see cref="T:System.Threading.CompressedStack" /> object.</returns>
		public static CompressedStack Capture()
		{
			throw new NotSupportedException();
		}

		/// <summary>Gets the compressed stack for the current thread.</summary>
		/// <returns>A <see cref="T:System.Threading.CompressedStack" /> for the current thread.</returns>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call chain does not have permission to access unmanaged code.  
		///  -or-  
		///  The request for <see cref="T:System.Security.Permissions.StrongNameIdentityPermission" /> failed.</exception>
		[SecurityCritical]
		public static CompressedStack GetCompressedStack()
		{
			throw new NotSupportedException();
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the logical context information needed to recreate an instance of this execution context.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object to be populated with serialization information.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure representing the destination context of the serialization.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		[MonoTODO("incomplete")]
		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
		}

		/// <summary>Runs a method in the specified compressed stack on the current thread.</summary>
		/// <param name="compressedStack">The <see cref="T:System.Threading.CompressedStack" /> to set.</param>
		/// <param name="callback">A <see cref="T:System.Threading.ContextCallback" /> that represents the method to be run in the specified security context.</param>
		/// <param name="state">The object to be passed to the callback method.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="compressedStack" /> is <see langword="null" />.</exception>
		[SecurityCritical]
		public static void Run(CompressedStack compressedStack, ContextCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		internal bool Equals(CompressedStack cs)
		{
			if (this.IsEmpty())
			{
				return cs.IsEmpty();
			}
			return !cs.IsEmpty() && this._list.Count == cs._list.Count;
		}

		internal bool IsEmpty()
		{
			return this._list == null || this._list.Count == 0;
		}

		internal IList List
		{
			get
			{
				return this._list;
			}
		}

		internal CompressedStack()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ArrayList _list;
	}
}
