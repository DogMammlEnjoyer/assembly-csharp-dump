using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Indicates to a COM client that all classes in the current version of an assembly are compatible with classes in an earlier version of the assembly.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class ComCompatibleVersionAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.ComCompatibleVersionAttribute" /> class with the major version, minor version, build, and revision numbers of the assembly.</summary>
		/// <param name="major">The major version number of the assembly.</param>
		/// <param name="minor">The minor version number of the assembly.</param>
		/// <param name="build">The build number of the assembly.</param>
		/// <param name="revision">The revision number of the assembly.</param>
		public ComCompatibleVersionAttribute(int major, int minor, int build, int revision)
		{
			this._major = major;
			this._minor = minor;
			this._build = build;
			this._revision = revision;
		}

		/// <summary>Gets the major version number of the assembly.</summary>
		/// <returns>The major version number of the assembly.</returns>
		public int MajorVersion
		{
			get
			{
				return this._major;
			}
		}

		/// <summary>Gets the minor version number of the assembly.</summary>
		/// <returns>The minor version number of the assembly.</returns>
		public int MinorVersion
		{
			get
			{
				return this._minor;
			}
		}

		/// <summary>Gets the build number of the assembly.</summary>
		/// <returns>The build number of the assembly.</returns>
		public int BuildNumber
		{
			get
			{
				return this._build;
			}
		}

		/// <summary>Gets the revision number of the assembly.</summary>
		/// <returns>The revision number of the assembly.</returns>
		public int RevisionNumber
		{
			get
			{
				return this._revision;
			}
		}

		internal int _major;

		internal int _minor;

		internal int _build;

		internal int _revision;
	}
}
