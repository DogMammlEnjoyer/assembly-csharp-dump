using System;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	/// <summary>The ManagementEntity attribute indicates that a class provides management information exposed through a WMI provider.Note: the WMI .NET libraries are now considered in final state, and no further development, enhancements, or updates will be available for non-security related issues affecting these libraries. The MI APIs should be used for all new development.</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManagementEntityAttribute : Attribute
	{
		/// <summary>Gets or sets a value that specifies whether the class represents a WMI class in a provider implemented external to the current assembly.</summary>
		/// <returns>A boolean value that is true if the class represents an external WMI class and false otherwise.</returns>
		public bool External
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
			}
		}

		/// <summary>Gets or sets the name of the WMI class.</summary>
		/// <returns>A string that contains the name of the WMI class.</returns>
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

		/// <summary>Specifies whether the associated class represents a singleton WMI class.</summary>
		/// <returns>A boolean value that is true if the class represents a singleton WMI class and false otherwise.</returns>
		public bool Singleton
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
			}
		}
	}
}
