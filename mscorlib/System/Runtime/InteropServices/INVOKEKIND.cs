using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Use <see cref="T:System.Runtime.InteropServices.ComTypes.INVOKEKIND" /> instead.</summary>
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.INVOKEKIND instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Serializable]
	public enum INVOKEKIND
	{
		/// <summary>The member is called using a normal function invocation syntax.</summary>
		INVOKE_FUNC = 1,
		/// <summary>The function is invoked using a normal property-access syntax.</summary>
		INVOKE_PROPERTYGET,
		/// <summary>The function is invoked using a property value assignment syntax.</summary>
		INVOKE_PROPERTYPUT = 4,
		/// <summary>The function is invoked using a property reference assignment syntax.</summary>
		INVOKE_PROPERTYPUTREF = 8
	}
}
