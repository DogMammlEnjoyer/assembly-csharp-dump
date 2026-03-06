using System;

namespace System.Reflection
{
	/// <summary>Provides a remotable version of the <see langword="AssemblyName" />.</summary>
	public class AssemblyNameProxy : MarshalByRefObject
	{
		/// <summary>Gets the <see langword="AssemblyName" /> for a given file.</summary>
		/// <param name="assemblyFile">The assembly file for which to get the <see langword="AssemblyName" />.</param>
		/// <returns>An <see langword="AssemblyName" /> object representing the given file.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="assemblyFile" /> is empty.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> is not found.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.</exception>
		public AssemblyName GetAssemblyName(string assemblyFile)
		{
			return AssemblyName.GetAssemblyName(assemblyFile);
		}
	}
}
