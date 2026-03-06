using System;

namespace System
{
	/// <summary>Specifies how mathematical rounding methods should process a number that is midway between two numbers.</summary>
	public enum MidpointRounding
	{
		/// <summary>When a number is halfway between two others, it is rounded toward the nearest even number.</summary>
		ToEven,
		/// <summary>When a number is halfway between two others, it is rounded toward the nearest number that is away from zero.</summary>
		AwayFromZero
	}
}
