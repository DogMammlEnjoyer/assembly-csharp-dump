using System;

namespace Oculus.Platform.Models
{
	public class ManagedInfo
	{
		public ManagedInfo(IntPtr o)
		{
			this.Department = CAPI.ovr_ManagedInfo_GetDepartment(o);
			this.Email = CAPI.ovr_ManagedInfo_GetEmail(o);
			this.EmployeeNumber = CAPI.ovr_ManagedInfo_GetEmployeeNumber(o);
			this.ExternalId = CAPI.ovr_ManagedInfo_GetExternalId(o);
			this.Location = CAPI.ovr_ManagedInfo_GetLocation(o);
			this.Manager = CAPI.ovr_ManagedInfo_GetManager(o);
			this.Name = CAPI.ovr_ManagedInfo_GetName(o);
			this.OrganizationId = CAPI.ovr_ManagedInfo_GetOrganizationId(o);
			this.OrganizationName = CAPI.ovr_ManagedInfo_GetOrganizationName(o);
			this.Position = CAPI.ovr_ManagedInfo_GetPosition(o);
		}

		public readonly string Department;

		public readonly string Email;

		public readonly string EmployeeNumber;

		public readonly string ExternalId;

		public readonly string Location;

		public readonly string Manager;

		public readonly string Name;

		public readonly string OrganizationId;

		public readonly string OrganizationName;

		public readonly string Position;
	}
}
