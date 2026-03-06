using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Indicates which <see langword="IDispatch" /> implementation to use for a particular class.</summary>
	[ComVisible(true)]
	[Obsolete("The IDispatchImplAttribute is deprecated.", false)]
	[Serializable]
	public enum IDispatchImplType
	{
		/// <summary>Specifies that the common language runtime decides which <see langword="IDispatch" /> implementation to use.</summary>
		SystemDefinedImpl,
		/// <summary>Specifies that the <see langword="IDispatch" /> implementation is supplied by the runtime.</summary>
		InternalImpl,
		/// <summary>Specifies that the <see langword="IDispatch" /> implementation is supplied by passing the type information for the object to the COM <see langword="CreateStdDispatch" /> API method.</summary>
		CompatibleImpl
	}
}
