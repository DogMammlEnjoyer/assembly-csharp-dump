using System;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	/// <summary>The ManagementProbe attribute indicates that a property or field represents a read-only WMI property.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManagementProbeAttribute : ManagementMemberAttribute
	{
		/// <summary>Gets or sets a value that defines the type of output that the property that is marked with the ManagementProbe attribute will output.</summary>
		/// <returns>A <see cref="T:System.Type" /> value that indicates the type of output that the property that is marked with the ManagementProbe attribute will output.</returns>
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
