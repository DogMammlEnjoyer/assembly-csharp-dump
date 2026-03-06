using System;
using Unity;

namespace System.Configuration
{
	/// <summary>Encapsulates the context information that is associated with a <see cref="T:System.Configuration.ConfigurationElement" /> object. This class cannot be inherited.</summary>
	public sealed class ContextInformation
	{
		internal ContextInformation(Configuration config, object ctx)
		{
			this.ctx = ctx;
			this.config = config;
		}

		/// <summary>Provides an object containing configuration-section information based on the specified section name.</summary>
		/// <param name="sectionName">The name of the configuration section.</param>
		/// <returns>An object containing the specified section within the configuration.</returns>
		public object GetSection(string sectionName)
		{
			return this.config.GetSection(sectionName);
		}

		/// <summary>Gets the context of the environment where the configuration property is being evaluated.</summary>
		/// <returns>An object specifying the environment where the configuration property is being evaluated.</returns>
		public object HostingContext
		{
			get
			{
				return this.ctx;
			}
		}

		/// <summary>Gets a value specifying whether the configuration property is being evaluated at the machine configuration level.</summary>
		/// <returns>
		///   <see langword="true" /> if the configuration property is being evaluated at the machine configuration level; otherwise, <see langword="false" />.</returns>
		[MonoInternalNote("should this use HostingContext instead?")]
		public bool IsMachineLevel
		{
			get
			{
				return this.config.ConfigPath == "machine";
			}
		}

		internal ContextInformation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private object ctx;

		private Configuration config;
	}
}
