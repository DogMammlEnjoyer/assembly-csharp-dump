using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Specifies whether the type should be marshaled using the Automation marshaler or a custom proxy and stub.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public sealed class AutomationProxyAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.AutomationProxyAttribute" /> class.</summary>
		/// <param name="val">
		///   <see langword="true" /> if the class should be marshaled using the Automation Marshaler; <see langword="false" /> if a proxy stub marshaler should be used.</param>
		public AutomationProxyAttribute(bool val)
		{
			this._val = val;
		}

		/// <summary>Gets a value indicating the type of marshaler to use.</summary>
		/// <returns>
		///   <see langword="true" /> if the class should be marshaled using the Automation Marshaler; <see langword="false" /> if a proxy stub marshaler should be used.</returns>
		public bool Value
		{
			get
			{
				return this._val;
			}
		}

		internal bool _val;
	}
}
