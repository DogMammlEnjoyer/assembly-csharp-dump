using System;

namespace System.Runtime.CompilerServices
{
	/// <summary>A class whose static <see cref="M:System.Runtime.CompilerServices.RuntimeFeature.IsSupported(System.String)" /> method checks whether a specified feature is supported by the common language runtime.</summary>
	public static class RuntimeFeature
	{
		/// <summary>Determines whether a specified feature is supported by the common language runtime.</summary>
		/// <param name="feature">The name of the feature.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="feature" /> is supported; otherwise, <see langword="false" />.</returns>
		public static bool IsSupported(string feature)
		{
			if (feature == "PortablePdb" || feature == "DefaultImplementationsOfInterfaces")
			{
				return true;
			}
			if (!(feature == "IsDynamicCodeSupported"))
			{
				return feature == "IsDynamicCodeCompiled" && RuntimeFeature.IsDynamicCodeCompiled;
			}
			return RuntimeFeature.IsDynamicCodeSupported;
		}

		public static bool IsDynamicCodeSupported
		{
			get
			{
				return true;
			}
		}

		public static bool IsDynamicCodeCompiled
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets the name of the portable PDB feature.</summary>
		public const string PortablePdb = "PortablePdb";

		public const string DefaultImplementationsOfInterfaces = "DefaultImplementationsOfInterfaces";
	}
}
