using System;

namespace System.Resources
{
	/// <summary>Instructs a <see cref="T:System.Resources.ResourceManager" /> object to ask for a particular version of a satellite assembly.</summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public sealed class SatelliteContractVersionAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.SatelliteContractVersionAttribute" /> class.</summary>
		/// <param name="version">A string that specifies the version of the satellite assemblies to load.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="version" /> parameter is <see langword="null" />.</exception>
		public SatelliteContractVersionAttribute(string version)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			this.Version = version;
		}

		/// <summary>Gets the version of the satellite assemblies with the required resources.</summary>
		/// <returns>A string that contains the version of the satellite assemblies with the required resources.</returns>
		public string Version { get; }
	}
}
