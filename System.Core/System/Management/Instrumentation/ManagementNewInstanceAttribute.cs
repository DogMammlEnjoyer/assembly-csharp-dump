using System;
using System.Security.Permissions;

namespace System.Management.Instrumentation
{
	/// <summary>The base class for management attributes that have only run-time functionality and no schema representation. The management attribute classes ManagementBindAttribute, ManagementCreateAttribute and ManagementEnumeratorAttribute are all derived from this class.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public abstract class ManagementNewInstanceAttribute : ManagementMemberAttribute
	{
	}
}
