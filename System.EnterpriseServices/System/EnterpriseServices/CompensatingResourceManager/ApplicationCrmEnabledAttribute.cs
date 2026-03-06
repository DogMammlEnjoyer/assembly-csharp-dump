using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	/// <summary>Enables Compensating Resource Manger (CRM) on the tagged application.</summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(false)]
	[ProgId("System.EnterpriseServices.Crm.ApplicationCrmEnabledAttribute")]
	public sealed class ApplicationCrmEnabledAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.CompensatingResourceManager.ApplicationCrmEnabledAttribute" /> class, setting the <see cref="P:System.EnterpriseServices.CompensatingResourceManager.ApplicationCrmEnabledAttribute.Value" /> property to <see langword="true" />.</summary>
		public ApplicationCrmEnabledAttribute()
		{
			this.val = true;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.CompensatingResourceManager.ApplicationCrmEnabledAttribute" /> class, optionally setting the <see cref="P:System.EnterpriseServices.CompensatingResourceManager.ApplicationCrmEnabledAttribute.Value" /> property to <see langword="false" />.</summary>
		/// <param name="val">
		///   <see langword="true" /> to enable Compensating Resource Manager (CRM); otherwise, <see langword="false" />.</param>
		public ApplicationCrmEnabledAttribute(bool val)
		{
			this.val = val;
		}

		/// <summary>Enables or disables Compensating Resource Manager (CRM) on the tagged application.</summary>
		/// <returns>
		///   <see langword="true" /> if CRM is enabled; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
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
