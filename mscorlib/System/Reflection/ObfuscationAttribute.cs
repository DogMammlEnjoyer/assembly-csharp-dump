using System;

namespace System.Reflection
{
	/// <summary>Instructs obfuscation tools to take the specified actions for an assembly, type, or member.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	public sealed class ObfuscationAttribute : Attribute
	{
		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value indicating whether the obfuscation tool should remove this attribute after processing.</summary>
		/// <returns>
		///   <see langword="true" /> if an obfuscation tool should remove the attribute after processing; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool StripAfterObfuscation { get; set; } = true;

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value indicating whether the obfuscation tool should exclude the type or member from obfuscation.</summary>
		/// <returns>
		///   <see langword="true" /> if the type or member to which this attribute is applied should be excluded from obfuscation; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool Exclude { get; set; } = true;

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value indicating whether the attribute of a type is to apply to the members of the type.</summary>
		/// <returns>
		///   <see langword="true" /> if the attribute is to apply to the members of the type; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool ApplyToMembers { get; set; } = true;

		/// <summary>Gets or sets a string value that is recognized by the obfuscation tool, and which specifies processing options.</summary>
		/// <returns>A string value that is recognized by the obfuscation tool, and which specifies processing options. The default is "all".</returns>
		public string Feature { get; set; } = "all";
	}
}
