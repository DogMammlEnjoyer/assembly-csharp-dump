using System;
using System.Runtime.InteropServices;
using Unity;

namespace System.Reflection.Emit
{
	/// <summary>Represents the class that describes how to marshal a field from managed to unmanaged code. This class cannot be inherited.</summary>
	[ComVisible(true)]
	[Obsolete("An alternate API is available: Emit the MarshalAs custom attribute instead.")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UnmanagedMarshal
	{
		private UnmanagedMarshal(UnmanagedType maint, int cnt)
		{
			this.count = cnt;
			this.t = maint;
			this.tbase = maint;
		}

		private UnmanagedMarshal(UnmanagedType maint, UnmanagedType elemt)
		{
			this.count = 0;
			this.t = maint;
			this.tbase = elemt;
		}

		/// <summary>Gets an unmanaged base type. This property is read-only.</summary>
		/// <returns>An <see langword="UnmanagedType" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">The unmanaged type is not an <see langword="LPArray" /> or a <see langword="SafeArray" />.</exception>
		public UnmanagedType BaseType
		{
			get
			{
				if (this.t == UnmanagedType.LPArray)
				{
					throw new ArgumentException();
				}
				if (this.t == UnmanagedType.SafeArray)
				{
					throw new ArgumentException();
				}
				return this.tbase;
			}
		}

		/// <summary>Gets a number element. This property is read-only.</summary>
		/// <returns>An integer indicating the element count.</returns>
		/// <exception cref="T:System.ArgumentException">The argument is not an unmanaged element count.</exception>
		public int ElementCount
		{
			get
			{
				return this.count;
			}
		}

		/// <summary>Indicates an unmanaged type. This property is read-only.</summary>
		/// <returns>An <see cref="T:System.Runtime.InteropServices.UnmanagedType" /> object.</returns>
		public UnmanagedType GetUnmanagedType
		{
			get
			{
				return this.t;
			}
		}

		/// <summary>Gets a GUID. This property is read-only.</summary>
		/// <returns>A <see cref="T:System.Guid" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">The argument is not a custom marshaler.</exception>
		public Guid IIDGuid
		{
			get
			{
				return new Guid(this.guid);
			}
		}

		/// <summary>Specifies a fixed-length array (ByValArray) to marshal to unmanaged code.</summary>
		/// <param name="elemCount">The number of elements in the fixed-length array.</param>
		/// <returns>An <see cref="T:System.Reflection.Emit.UnmanagedMarshal" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">The argument is not a simple native type.</exception>
		public static UnmanagedMarshal DefineByValArray(int elemCount)
		{
			return new UnmanagedMarshal(UnmanagedType.ByValArray, elemCount);
		}

		/// <summary>Specifies a string in a fixed array buffer (ByValTStr) to marshal to unmanaged code.</summary>
		/// <param name="elemCount">The number of elements in the fixed array buffer.</param>
		/// <returns>An <see cref="T:System.Reflection.Emit.UnmanagedMarshal" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">The argument is not a simple native type.</exception>
		public static UnmanagedMarshal DefineByValTStr(int elemCount)
		{
			return new UnmanagedMarshal(UnmanagedType.ByValTStr, elemCount);
		}

		/// <summary>Specifies an <see langword="LPArray" /> to marshal to unmanaged code. The length of an <see langword="LPArray" /> is determined at runtime by the size of the actual marshaled array.</summary>
		/// <param name="elemType">The unmanaged type to which to marshal the array.</param>
		/// <returns>An <see cref="T:System.Reflection.Emit.UnmanagedMarshal" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">The argument is not a simple native type.</exception>
		public static UnmanagedMarshal DefineLPArray(UnmanagedType elemType)
		{
			return new UnmanagedMarshal(UnmanagedType.LPArray, elemType);
		}

		/// <summary>Specifies a <see langword="SafeArray" /> to marshal to unmanaged code.</summary>
		/// <param name="elemType">The base type or the <see langword="UnmanagedType" /> of each element of the array.</param>
		/// <returns>An <see cref="T:System.Reflection.Emit.UnmanagedMarshal" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">The argument is not a simple native type.</exception>
		public static UnmanagedMarshal DefineSafeArray(UnmanagedType elemType)
		{
			return new UnmanagedMarshal(UnmanagedType.SafeArray, elemType);
		}

		/// <summary>Specifies a given type that is to be marshaled to unmanaged code.</summary>
		/// <param name="unmanagedType">The unmanaged type to which the type is to be marshaled.</param>
		/// <returns>An <see cref="T:System.Reflection.Emit.UnmanagedMarshal" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">The argument is not a simple native type.</exception>
		public static UnmanagedMarshal DefineUnmanagedMarshal(UnmanagedType unmanagedType)
		{
			return new UnmanagedMarshal(unmanagedType, unmanagedType);
		}

		internal static UnmanagedMarshal DefineCustom(Type typeref, string cookie, string mtype, Guid id)
		{
			UnmanagedMarshal unmanagedMarshal = new UnmanagedMarshal(UnmanagedType.CustomMarshaler, UnmanagedType.CustomMarshaler);
			unmanagedMarshal.mcookie = cookie;
			unmanagedMarshal.marshaltype = mtype;
			unmanagedMarshal.marshaltyperef = typeref;
			if (id == Guid.Empty)
			{
				unmanagedMarshal.guid = string.Empty;
			}
			else
			{
				unmanagedMarshal.guid = id.ToString();
			}
			return unmanagedMarshal;
		}

		internal static UnmanagedMarshal DefineLPArrayInternal(UnmanagedType elemType, int sizeConst, int sizeParamIndex)
		{
			return new UnmanagedMarshal(UnmanagedType.LPArray, elemType)
			{
				count = sizeConst,
				param_num = sizeParamIndex,
				has_size = true
			};
		}

		internal UnmanagedMarshal()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private int count;

		private UnmanagedType t;

		private UnmanagedType tbase;

		private string guid;

		private string mcookie;

		private string marshaltype;

		internal Type marshaltyperef;

		private int param_num;

		private bool has_size;
	}
}
