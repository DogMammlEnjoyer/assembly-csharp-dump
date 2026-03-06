using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Controls whether Unicode characters are converted to the closest matching ANSI characters.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public sealed class BestFitMappingAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.BestFitMappingAttribute" /> class set to the value of the <see cref="P:System.Runtime.InteropServices.BestFitMappingAttribute.BestFitMapping" /> property.</summary>
		/// <param name="BestFitMapping">
		///   <see langword="true" /> to indicate that best-fit mapping is enabled; otherwise, <see langword="false" />. The default is <see langword="true" />.</param>
		public BestFitMappingAttribute(bool BestFitMapping)
		{
			this._bestFitMapping = BestFitMapping;
		}

		/// <summary>Gets the best-fit mapping behavior when converting Unicode characters to ANSI characters.</summary>
		/// <returns>
		///   <see langword="true" /> if best-fit mapping is enabled; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool BestFitMapping
		{
			get
			{
				return this._bestFitMapping;
			}
		}

		internal bool _bestFitMapping;

		/// <summary>Enables or disables the throwing of an exception on an unmappable Unicode character that is converted to an ANSI '?' character.</summary>
		public bool ThrowOnUnmappableChar;
	}
}
