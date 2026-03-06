using System;
using System.Threading;

namespace System.Text
{
	/// <summary>Provides a failure-handling mechanism, called a fallback, for an encoded input byte sequence that cannot be converted to an output character.</summary>
	[Serializable]
	public abstract class DecoderFallback
	{
		/// <summary>Gets an object that outputs a substitute string in place of an input byte sequence that cannot be decoded.</summary>
		/// <returns>A type derived from the <see cref="T:System.Text.DecoderFallback" /> class. The default value is a <see cref="T:System.Text.DecoderReplacementFallback" /> object that emits the QUESTION MARK character ("?", U+003F) in place of unknown byte sequences.</returns>
		public static DecoderFallback ReplacementFallback
		{
			get
			{
				DecoderFallback result;
				if ((result = DecoderFallback.s_replacementFallback) == null)
				{
					result = (Interlocked.CompareExchange<DecoderFallback>(ref DecoderFallback.s_replacementFallback, new DecoderReplacementFallback(), null) ?? DecoderFallback.s_replacementFallback);
				}
				return result;
			}
		}

		/// <summary>Gets an object that throws an exception when an input byte sequence cannot be decoded.</summary>
		/// <returns>A type derived from the <see cref="T:System.Text.DecoderFallback" /> class. The default value is a <see cref="T:System.Text.DecoderExceptionFallback" /> object.</returns>
		public static DecoderFallback ExceptionFallback
		{
			get
			{
				DecoderFallback result;
				if ((result = DecoderFallback.s_exceptionFallback) == null)
				{
					result = (Interlocked.CompareExchange<DecoderFallback>(ref DecoderFallback.s_exceptionFallback, new DecoderExceptionFallback(), null) ?? DecoderFallback.s_exceptionFallback);
				}
				return result;
			}
		}

		/// <summary>When overridden in a derived class, initializes a new instance of the <see cref="T:System.Text.DecoderFallbackBuffer" /> class.</summary>
		/// <returns>An object that provides a fallback buffer for a decoder.</returns>
		public abstract DecoderFallbackBuffer CreateFallbackBuffer();

		/// <summary>When overridden in a derived class, gets the maximum number of characters the current <see cref="T:System.Text.DecoderFallback" /> object can return.</summary>
		/// <returns>The maximum number of characters the current <see cref="T:System.Text.DecoderFallback" /> object can return.</returns>
		public abstract int MaxCharCount { get; }

		private static DecoderFallback s_replacementFallback;

		private static DecoderFallback s_exceptionFallback;
	}
}
