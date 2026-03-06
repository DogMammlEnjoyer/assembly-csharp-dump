using System;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	/// <summary>The ManagementTask attribute indicates that the target method implements a WMI method.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManagementTaskAttribute : ManagementMemberAttribute
	{
		/// <summary>Gets or sets a value that defines the type of output that the method that is marked with the ManagementTask attribute will output.</summary>
		/// <returns>A <see cref="T:System.Type" /> value that indicates the type of output that the method that is marked with the ManagementTask attribute will output.</returns>
		public Type Schema
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
			}
		}
	}
}
