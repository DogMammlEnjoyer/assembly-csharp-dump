using System;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	/// <summary>Contains information describing an active Compensating Resource Manager (CRM) Clerk object.</summary>
	public sealed class ClerkInfo
	{
		/// <summary>Frees the resources of the current <see cref="T:System.EnterpriseServices.CompensatingResourceManager.ClerkInfo" /> before it is reclaimed by the garbage collector.</summary>
		[MonoTODO]
		~ClerkInfo()
		{
			throw new NotImplementedException();
		}

		internal ClerkInfo()
		{
		}

		/// <summary>Gets the activity ID of the current Compensating Resource Manager (CRM) Worker.</summary>
		/// <returns>Gets the activity ID of the current Compensating Resource Manager (CRM) Worker.</returns>
		[MonoTODO]
		public string ActivityId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets <see cref="F:System.Runtime.InteropServices.UnmanagedType.IUnknown" /> for the current Clerk.</summary>
		/// <returns>
		///   <see cref="F:System.Runtime.InteropServices.UnmanagedType.IUnknown" /> for the current Clerk.</returns>
		[MonoTODO]
		public Clerk Clerk
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the ProgId of the Compensating Resource Manager (CRM) Compensator for the current CRM Clerk.</summary>
		/// <returns>The ProgId of the CRM Compensator for the current CRM Clerk.</returns>
		[MonoTODO]
		public string Compensator
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the description of the Compensating Resource Manager (CRM) Compensator for the current CRM Clerk. The description string is the string that was provided by the <see langword="ICrmLogControl::RegisterCompensator" /> method.</summary>
		/// <returns>The description of the CRM Compensator for the current CRM Clerk.</returns>
		[MonoTODO]
		public string Description
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the instance class ID (CLSID) of the current Compensating Resource Manager (CRM) Clerk.</summary>
		/// <returns>The instance CLSID of the current CRM Clerk.</returns>
		[MonoTODO]
		public string InstanceId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the unit of work (UOW) of the transaction for the current Compensating Resource Manager (CRM) Clerk.</summary>
		/// <returns>The UOW of the transaction for the current CRM Clerk.</returns>
		[MonoTODO]
		public string TransactionUOW
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
