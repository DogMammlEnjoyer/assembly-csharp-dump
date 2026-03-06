using System;

namespace System.Runtime.Versioning
{
	/// <summary>Identifies the version of the .NET Framework that a particular assembly was compiled against.</summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public sealed class TargetFrameworkAttribute : Attribute
	{
		/// <summary>Initializes an instance of the <see cref="T:System.Runtime.Versioning.TargetFrameworkAttribute" /> class by specifying the .NET Framework version against which an assembly was built.</summary>
		/// <param name="frameworkName">The version of the .NET Framework against which the assembly was built.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="frameworkName" /> is <see langword="null" />.</exception>
		public TargetFrameworkAttribute(string frameworkName)
		{
			if (frameworkName == null)
			{
				throw new ArgumentNullException("frameworkName");
			}
			this._frameworkName = frameworkName;
		}

		/// <summary>Gets the name of the .NET Framework version against which a particular assembly was compiled.</summary>
		/// <returns>The name of the .NET Framework version with which the assembly was compiled.</returns>
		public string FrameworkName
		{
			get
			{
				return this._frameworkName;
			}
		}

		/// <summary>Gets the display name of the .NET Framework version against which an assembly was built.</summary>
		/// <returns>The display name of the .NET Framework version.</returns>
		public string FrameworkDisplayName
		{
			get
			{
				return this._frameworkDisplayName;
			}
			set
			{
				this._frameworkDisplayName = value;
			}
		}

		private string _frameworkName;

		private string _frameworkDisplayName;
	}
}
