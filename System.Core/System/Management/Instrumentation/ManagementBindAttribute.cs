using System;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	/// <summary>The ManagementBind attribute indicates that a method is used to return the instance of a WMI class associated with a specific key value.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManagementBindAttribute : ManagementNewInstanceAttribute
	{
		/// <summary>Gets or sets a value that defines the type of output that the method that is marked with the ManagementEnumerator attribute will output.</summary>
		/// <returns>A <see cref="T:System.Type" /> value that indicates the type of output that the method marked with the <see cref="ManagementBind" /> attribute will output.</returns>
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
