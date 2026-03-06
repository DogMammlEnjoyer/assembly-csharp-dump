using System;
using Unity;

namespace System.Configuration
{
	/// <summary>Manages the path context for the current application. This class cannot be inherited.</summary>
	public sealed class ExeContext
	{
		internal ExeContext(string path, ConfigurationUserLevel level)
		{
			this.path = path;
			this.level = level;
		}

		/// <summary>Gets the current path for the application.</summary>
		/// <returns>A string value containing the current path.</returns>
		public string ExePath
		{
			get
			{
				return this.path;
			}
		}

		/// <summary>Gets an object representing the path level of the current application.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConfigurationUserLevel" /> object representing the path level of the current application.</returns>
		public ConfigurationUserLevel UserLevel
		{
			get
			{
				return this.level;
			}
		}

		internal ExeContext()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string path;

		private ConfigurationUserLevel level;
	}
}
