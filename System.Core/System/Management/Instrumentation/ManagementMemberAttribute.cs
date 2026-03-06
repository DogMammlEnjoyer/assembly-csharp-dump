using System;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	/// <summary>This class is used by the WMI.NET Provider Extensions framework. It is the base class for all the management attributes that can be applied to members.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[AttributeUsage(AttributeTargets.All)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public abstract class ManagementMemberAttribute : Attribute
	{
		/// <summary>Gets or sets the name of the management attribute.</summary>
		/// <returns>Returns a string which is the name of the management attribute.</returns>
		public string Name
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
