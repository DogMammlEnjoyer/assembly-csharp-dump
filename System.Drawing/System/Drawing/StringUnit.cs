using System;

namespace System.Drawing
{
	/// <summary>Specifies the units of measure for a text string.</summary>
	public enum StringUnit
	{
		/// <summary>Specifies world units as the unit of measure.</summary>
		World,
		/// <summary>Specifies the device unit as the unit of measure.</summary>
		Display,
		/// <summary>Specifies a pixel as the unit of measure.</summary>
		Pixel,
		/// <summary>Specifies a printer's point (1/72 inch) as the unit of measure.</summary>
		Point,
		/// <summary>Specifies an inch as the unit of measure.</summary>
		Inch,
		/// <summary>Specifies 1/300 of an inch as the unit of measure.</summary>
		Document,
		/// <summary>Specifies a millimeter as the unit of measure</summary>
		Millimeter,
		/// <summary>Specifies a printer's em size of 32 as the unit of measure.</summary>
		Em = 32
	}
}
