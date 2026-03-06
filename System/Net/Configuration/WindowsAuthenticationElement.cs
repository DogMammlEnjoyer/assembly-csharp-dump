using System;
using System.Configuration;
using Unity;

namespace System.Net.Configuration
{
	/// <summary>Represents the Windows authentication element in a configuration file. This class cannot be inherited.</summary>
	public sealed class WindowsAuthenticationElement : ConfigurationElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.WindowsAuthenticationElement" /> class.</summary>
		public WindowsAuthenticationElement()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Defines the default size of the Windows credential handle cache.</summary>
		/// <returns>The default size of the Windows credential handle cache.</returns>
		public int DefaultCredentialsHandleCacheSize
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}
	}
}
