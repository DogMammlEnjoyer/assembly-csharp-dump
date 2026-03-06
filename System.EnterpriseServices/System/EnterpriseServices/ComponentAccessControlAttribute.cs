using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Enables security checking on calls to a component. This class cannot be inherited.</summary>
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ComponentAccessControlAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.ComponentAccessControlAttribute" /> class.</summary>
		public ComponentAccessControlAttribute()
		{
			this.val = false;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.ComponentAccessControlAttribute" /> class and sets the <see cref="P:System.EnterpriseServices.ComponentAccessControlAttribute.Value" /> property to indicate whether to enable COM+ security configuration.</summary>
		/// <param name="val">
		///   <see langword="true" /> to enable security checking on calls to a component; otherwise, <see langword="false" />.</param>
		public ComponentAccessControlAttribute(bool val)
		{
			this.val = val;
		}

		/// <summary>Gets a value indicating whether to enable security checking on calls to a component.</summary>
		/// <returns>
		///   <see langword="true" /> if security checking is to be enabled; otherwise, <see langword="false" />.</returns>
		public bool Value
		{
			get
			{
				return this.val;
			}
		}

		private bool val;
	}
}
