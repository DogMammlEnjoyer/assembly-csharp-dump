using System;

namespace System.Text
{
	/// <summary>Provides a failure-handling mechanism, called a fallback, for an input character that cannot be converted to an output byte sequence. The fallback throws an exception if an input character cannot be converted to an output byte sequence. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class EncoderExceptionFallback : EncoderFallback
	{
		/// <summary>Returns an encoder fallback buffer that throws an exception if it cannot convert a character sequence to a byte sequence.</summary>
		/// <returns>An encoder fallback buffer that throws an exception when it cannot encode a character sequence.</returns>
		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			return new EncoderExceptionFallbackBuffer();
		}

		/// <summary>Gets the maximum number of characters this instance can return.</summary>
		/// <returns>The return value is always zero.</returns>
		public override int MaxCharCount
		{
			get
			{
				return 0;
			}
		}

		/// <summary>Indicates whether the current <see cref="T:System.Text.EncoderExceptionFallback" /> object and a specified object are equal.</summary>
		/// <param name="value">An object that derives from the <see cref="T:System.Text.EncoderExceptionFallback" /> class.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not <see langword="null" /> (<see langword="Nothing" /> in Visual Basic .NET) and is a <see cref="T:System.Text.EncoderExceptionFallback" /> object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object value)
		{
			return value is EncoderExceptionFallback;
		}

		/// <summary>Retrieves the hash code for this instance.</summary>
		/// <returns>The return value is always the same arbitrary value, and has no special significance.</returns>
		public override int GetHashCode()
		{
			return 654;
		}
	}
}
