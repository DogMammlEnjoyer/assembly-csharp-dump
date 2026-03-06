using System;

namespace System.ComponentModel
{
	/// <summary>Specifies whether the Visual Studio Custom Action Installer or the Installutil.exe (Installer Tool) should be invoked when the assembly is installed.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class RunInstallerAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.RunInstallerAttribute" /> class.</summary>
		/// <param name="runInstaller">
		///   <see langword="true" /> if an installer should be invoked during installation of an assembly; otherwise, <see langword="false" />.</param>
		public RunInstallerAttribute(bool runInstaller)
		{
			this.RunInstaller = runInstaller;
		}

		/// <summary>Gets a value indicating whether an installer should be invoked during installation of an assembly.</summary>
		/// <returns>
		///   <see langword="true" /> if an installer should be invoked during installation of an assembly; otherwise, <see langword="false" />.</returns>
		public bool RunInstaller { get; }

		/// <summary>Determines whether the value of the specified <see cref="T:System.ComponentModel.RunInstallerAttribute" /> is equivalent to the current <see cref="T:System.ComponentModel.RunInstallerAttribute" />.</summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.ComponentModel.RunInstallerAttribute" /> is equal to the current <see cref="T:System.ComponentModel.RunInstallerAttribute" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			RunInstallerAttribute runInstallerAttribute = obj as RunInstallerAttribute;
			return runInstallerAttribute != null && runInstallerAttribute.RunInstaller == this.RunInstaller;
		}

		/// <summary>Generates a hash code for the current <see cref="T:System.ComponentModel.RunInstallerAttribute" />.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.RunInstallerAttribute" />.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Determines if this attribute is the default.</summary>
		/// <returns>
		///   <see langword="true" /> if the attribute is the default value for this attribute class; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Equals(RunInstallerAttribute.Default);
		}

		/// <summary>Specifies that the Visual Studio Custom Action Installer or the Installutil.exe (Installer Tool) should be invoked when the assembly is installed. This <see langword="static" /> field is read-only.</summary>
		public static readonly RunInstallerAttribute Yes = new RunInstallerAttribute(true);

		/// <summary>Specifies that the Visual Studio Custom Action Installer or the Installutil.exe (Installer Tool) should not be invoked when the assembly is installed. This <see langword="static" /> field is read-only.</summary>
		public static readonly RunInstallerAttribute No = new RunInstallerAttribute(false);

		/// <summary>Specifies the default visiblity, which is <see cref="F:System.ComponentModel.RunInstallerAttribute.No" />. This <see langword="static" /> field is read-only.</summary>
		public static readonly RunInstallerAttribute Default = RunInstallerAttribute.No;
	}
}
