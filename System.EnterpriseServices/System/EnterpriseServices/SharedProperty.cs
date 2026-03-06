using System;
using System.Runtime.InteropServices;
using Unity;

namespace System.EnterpriseServices
{
	/// <summary>Accesses a shared property. This class cannot be inherited.</summary>
	[ComVisible(false)]
	public sealed class SharedProperty
	{
		internal SharedProperty(ISharedProperty property)
		{
			this.property = property;
		}

		/// <summary>Gets or sets the value of the shared property.</summary>
		/// <returns>The value of the shared property.</returns>
		public object Value
		{
			get
			{
				return this.property.Value;
			}
			set
			{
				this.property.Value = value;
			}
		}

		internal SharedProperty()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ISharedProperty property;
	}
}
