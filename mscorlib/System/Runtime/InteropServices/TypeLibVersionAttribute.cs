using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Specifies the version number of an exported type library.</summary>
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class TypeLibVersionAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.TypeLibVersionAttribute" /> class with the major and minor version numbers of the type library.</summary>
		/// <param name="major">The major version number of the type library.</param>
		/// <param name="minor">The minor version number of the type library.</param>
		public TypeLibVersionAttribute(int major, int minor)
		{
			this._major = major;
			this._minor = minor;
		}

		/// <summary>Gets the major version number of the type library.</summary>
		/// <returns>The major version number of the type library.</returns>
		public int MajorVersion
		{
			get
			{
				return this._major;
			}
		}

		/// <summary>Gets the minor version number of the type library.</summary>
		/// <returns>The minor version number of the type library.</returns>
		public int MinorVersion
		{
			get
			{
				return this._minor;
			}
		}

		internal int _major;

		internal int _minor;
	}
}
