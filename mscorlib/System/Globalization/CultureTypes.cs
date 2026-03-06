using System;

namespace System.Globalization
{
	/// <summary>Defines the types of culture lists that can be retrieved using the <see cref="M:System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes)" /> method.</summary>
	[Flags]
	public enum CultureTypes
	{
		/// <summary>Cultures that are associated with a language but are not specific to a country/region.</summary>
		NeutralCultures = 1,
		/// <summary>Cultures that are specific to a country/region.</summary>
		SpecificCultures = 2,
		/// <summary>This member is deprecated. All cultures that are installed in the Windows operating system.</summary>
		InstalledWin32Cultures = 4,
		/// <summary>All cultures that recognized by .NET, including neutral and specific cultures and custom cultures created by the user.
		/// On .NET Framework 4 and later versions and .NET Core running on Windows, it includes the culture data available from the Windows operating system. On .NET Core running on Linux and macOS, it includes culture data defined in the ICU libraries.
		///  <see cref="F:System.Globalization.CultureTypes.AllCultures" /> is a composite field that includes the <see cref="F:System.Globalization.CultureTypes.NeutralCultures" />, <see cref="F:System.Globalization.CultureTypes.SpecificCultures" />, and <see cref="F:System.Globalization.CultureTypes.InstalledWin32Cultures" /> values.</summary>
		AllCultures = 7,
		/// <summary>This member is deprecated. Custom cultures created by the user.</summary>
		UserCustomCulture = 8,
		/// <summary>This member is deprecated. Custom cultures created by the user that replace cultures shipped with the .NET Framework.</summary>
		ReplacementCultures = 16,
		/// <summary>This member is deprecated and is ignored.</summary>
		[Obsolete("This value has been deprecated.  Please use other values in CultureTypes.")]
		WindowsOnlyCultures = 32,
		/// <summary>This member is deprecated; using this value with <see cref="M:System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes)" /> returns neutral and specific cultures shipped with the .NET Framework 2.0.</summary>
		[Obsolete("This value has been deprecated.  Please use other values in CultureTypes.")]
		FrameworkCultures = 64
	}
}
