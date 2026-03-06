using System;
using System.Security.Permissions;

namespace System.Management.Instrumentation
{
	/// <summary>The ManagementKey attribute identifies the key properties of a WMI class.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManagementKeyAttribute : ManagementMemberAttribute
	{
	}
}
