using System;

namespace System.Reflection
{
	/// <summary>Specifies that the assembly is not fully signed when created.</summary>
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyDelaySignAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.AssemblyDelaySignAttribute" /> class.</summary>
		/// <param name="delaySign">
		///   <see langword="true" /> if the feature this attribute represents is activated; otherwise, <see langword="false" />.</param>
		public AssemblyDelaySignAttribute(bool delaySign)
		{
			this.DelaySign = delaySign;
		}

		/// <summary>Gets a value indicating the state of the attribute.</summary>
		/// <returns>
		///   <see langword="true" /> if this assembly has been built as delay-signed; otherwise, <see langword="false" />.</returns>
		public bool DelaySign { get; }
	}
}
