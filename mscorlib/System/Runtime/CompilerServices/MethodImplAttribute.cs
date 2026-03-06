using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	/// <summary>Specifies the details of how a method is implemented. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class MethodImplAttribute : Attribute
	{
		internal MethodImplAttribute(MethodImplAttributes methodImplAttributes)
		{
			MethodImplOptions methodImplOptions = MethodImplOptions.Unmanaged | MethodImplOptions.ForwardRef | MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall | MethodImplOptions.Synchronized | MethodImplOptions.NoInlining | MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization;
			this._val = (MethodImplOptions)(methodImplAttributes & (MethodImplAttributes)methodImplOptions);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.MethodImplAttribute" /> class with the specified <see cref="T:System.Runtime.CompilerServices.MethodImplOptions" /> value.</summary>
		/// <param name="methodImplOptions">A <see cref="T:System.Runtime.CompilerServices.MethodImplOptions" /> value specifying properties of the attributed method.</param>
		public MethodImplAttribute(MethodImplOptions methodImplOptions)
		{
			this._val = methodImplOptions;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.MethodImplAttribute" /> class with the specified <see cref="T:System.Runtime.CompilerServices.MethodImplOptions" /> value.</summary>
		/// <param name="value">A bitmask representing the desired <see cref="T:System.Runtime.CompilerServices.MethodImplOptions" /> value which specifies properties of the attributed method.</param>
		public MethodImplAttribute(short value)
		{
			this._val = (MethodImplOptions)value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.MethodImplAttribute" /> class.</summary>
		public MethodImplAttribute()
		{
		}

		/// <summary>Gets the <see cref="T:System.Runtime.CompilerServices.MethodImplOptions" /> value describing the attributed method.</summary>
		/// <returns>The <see cref="T:System.Runtime.CompilerServices.MethodImplOptions" /> value describing the attributed method.</returns>
		public MethodImplOptions Value
		{
			get
			{
				return this._val;
			}
		}

		internal MethodImplOptions _val;

		/// <summary>A <see cref="T:System.Runtime.CompilerServices.MethodCodeType" /> value indicating what kind of implementation is provided for this method.</summary>
		public MethodCodeType MethodCodeType;
	}
}
