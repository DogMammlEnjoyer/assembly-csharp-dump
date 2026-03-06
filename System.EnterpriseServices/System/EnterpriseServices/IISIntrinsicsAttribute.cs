using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Enables access to ASP intrinsic values from <see cref="M:System.EnterpriseServices.ContextUtil.GetNamedProperty(System.String)" />. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class IISIntrinsicsAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.IISIntrinsicsAttribute" /> class, enabling access to the ASP intrinsic values.</summary>
		public IISIntrinsicsAttribute()
		{
			this.val = true;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.IISIntrinsicsAttribute" /> class, optionally disabling access to the ASP intrinsic values.</summary>
		/// <param name="val">
		///   <see langword="true" /> to enable access to the ASP intrinsic values; otherwise, <see langword="false" />.</param>
		public IISIntrinsicsAttribute(bool val)
		{
			this.val = val;
		}

		/// <summary>Gets a value that indicates whether access to the ASP intrinsic values is enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if access is enabled; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
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
