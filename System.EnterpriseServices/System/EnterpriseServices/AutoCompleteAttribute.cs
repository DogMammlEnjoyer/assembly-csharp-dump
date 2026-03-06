using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Marks the attributed method as an <see langword="AutoComplete" /> object. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Method)]
	[ComVisible(false)]
	public sealed class AutoCompleteAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.AutoCompleteAttribute" /> class, specifying that the application should automatically call <see cref="M:System.EnterpriseServices.ContextUtil.SetComplete" /> if the transaction completes successfully.</summary>
		public AutoCompleteAttribute()
		{
			this.val = true;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.AutoCompleteAttribute" /> class, specifying whether COM+ <see langword="AutoComplete" /> is enabled.</summary>
		/// <param name="val">
		///   <see langword="true" /> to enable <see langword="AutoComplete" /> in the COM+ object; otherwise, <see langword="false" />.</param>
		public AutoCompleteAttribute(bool val)
		{
			this.val = val;
		}

		/// <summary>Gets a value indicating the setting of the <see langword="AutoComplete" /> option in COM+.</summary>
		/// <returns>
		///   <see langword="true" /> if <see langword="AutoComplete" /> is enabled in COM+; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
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
