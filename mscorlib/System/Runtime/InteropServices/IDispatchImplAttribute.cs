using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Indicates which <see langword="IDispatch" /> implementation the common language runtime uses when exposing dual interfaces and dispinterfaces to COM.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, Inherited = false)]
	[Obsolete("This attribute is deprecated and will be removed in a future version.", false)]
	public sealed class IDispatchImplAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see langword="IDispatchImplAttribute" /> class with specified <see cref="T:System.Runtime.InteropServices.IDispatchImplType" /> value.</summary>
		/// <param name="implType">Indicates which <see cref="T:System.Runtime.InteropServices.IDispatchImplType" /> enumeration will be used.</param>
		public IDispatchImplAttribute(IDispatchImplType implType)
		{
			this._val = implType;
		}

		/// <summary>Initializes a new instance of the <see langword="IDispatchImplAttribute" /> class with specified <see cref="T:System.Runtime.InteropServices.IDispatchImplType" /> value.</summary>
		/// <param name="implType">Indicates which <see cref="T:System.Runtime.InteropServices.IDispatchImplType" /> enumeration will be used.</param>
		public IDispatchImplAttribute(short implType)
		{
			this._val = (IDispatchImplType)implType;
		}

		/// <summary>Gets the <see cref="T:System.Runtime.InteropServices.IDispatchImplType" /> value used by the class.</summary>
		/// <returns>The <see cref="T:System.Runtime.InteropServices.IDispatchImplType" /> value used by the class.</returns>
		public IDispatchImplType Value
		{
			get
			{
				return this._val;
			}
		}

		internal IDispatchImplType _val;
	}
}
