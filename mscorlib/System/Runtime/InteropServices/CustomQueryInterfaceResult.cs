using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Provides return values for the <see cref="M:System.Runtime.InteropServices.ICustomQueryInterface.GetInterface(System.Guid@,System.IntPtr@)" /> method.</summary>
	[ComVisible(false)]
	[Serializable]
	public enum CustomQueryInterfaceResult
	{
		/// <summary>The interface pointer that is returned from the <see cref="M:System.Runtime.InteropServices.ICustomQueryInterface.GetInterface(System.Guid@,System.IntPtr@)" /> method can be used as the result of IUnknown::QueryInterface.</summary>
		Handled,
		/// <summary>The custom <see langword="QueryInterface" /> was not used. Instead, the default implementation of IUnknown::QueryInterface should be used.</summary>
		NotHandled,
		/// <summary>The interface for a specific interface ID is not available. In this case, the returned interface is <see langword="null" />. E_NOINTERFACE is returned to the caller of IUnknown::QueryInterface.</summary>
		Failed
	}
}
