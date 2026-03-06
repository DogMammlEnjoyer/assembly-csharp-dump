using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Provides different methods for preventing derivation.</summary>
	[Flags]
	public enum XmlSchemaDerivationMethod
	{
		/// <summary>Override default derivation method to allow any derivation.</summary>
		[XmlEnum("")]
		Empty = 0,
		/// <summary>Refers to derivations by <see langword="Substitution" />.</summary>
		[XmlEnum("substitution")]
		Substitution = 1,
		/// <summary>Refers to derivations by <see langword="Extension" />.</summary>
		[XmlEnum("extension")]
		Extension = 2,
		/// <summary>Refers to derivations by <see langword="Restriction" />.</summary>
		[XmlEnum("restriction")]
		Restriction = 4,
		/// <summary>Refers to derivations by <see langword="List" />.</summary>
		[XmlEnum("list")]
		List = 8,
		/// <summary>Refers to derivations by <see langword="Union" />.</summary>
		[XmlEnum("union")]
		Union = 16,
		/// <summary>
		///     <see langword="#all" />. Refers to all derivation methods.</summary>
		[XmlEnum("#all")]
		All = 255,
		/// <summary>Accepts the default derivation method.</summary>
		[XmlIgnore]
		None = 256
	}
}
