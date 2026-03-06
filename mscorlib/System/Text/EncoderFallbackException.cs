using System;
using System.Runtime.Serialization;

namespace System.Text
{
	/// <summary>The exception that is thrown when an encoder fallback operation fails. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class EncoderFallbackException : ArgumentException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Text.EncoderFallbackException" /> class.</summary>
		public EncoderFallbackException() : base("Value does not fall within the expected range.")
		{
			base.HResult = -2147024809;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Text.EncoderFallbackException" /> class. A parameter specifies the error message.</summary>
		/// <param name="message">An error message.</param>
		public EncoderFallbackException(string message) : base(message)
		{
			base.HResult = -2147024809;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Text.EncoderFallbackException" /> class. Parameters specify the error message and the inner exception that is the cause of this exception.</summary>
		/// <param name="message">An error message.</param>
		/// <param name="innerException">The exception that caused this exception.</param>
		public EncoderFallbackException(string message, Exception innerException) : base(message, innerException)
		{
			base.HResult = -2147024809;
		}

		internal EncoderFallbackException(string message, char charUnknown, int index) : base(message)
		{
			this._charUnknown = charUnknown;
			this._index = index;
		}

		internal EncoderFallbackException(string message, char charUnknownHigh, char charUnknownLow, int index) : base(message)
		{
			if (!char.IsHighSurrogate(charUnknownHigh))
			{
				throw new ArgumentOutOfRangeException("charUnknownHigh", SR.Format("Valid values are between {0} and {1}, inclusive.", 55296, 56319));
			}
			if (!char.IsLowSurrogate(charUnknownLow))
			{
				throw new ArgumentOutOfRangeException("CharUnknownLow", SR.Format("Valid values are between {0} and {1}, inclusive.", 56320, 57343));
			}
			this._charUnknownHigh = charUnknownHigh;
			this._charUnknownLow = charUnknownLow;
			this._index = index;
		}

		private EncoderFallbackException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
		}

		/// <summary>Gets the input character that caused the exception.</summary>
		/// <returns>The character that cannot be encoded.</returns>
		public char CharUnknown
		{
			get
			{
				return this._charUnknown;
			}
		}

		/// <summary>Gets the high component character of the surrogate pair that caused the exception.</summary>
		/// <returns>The high component character of the surrogate pair that cannot be encoded.</returns>
		public char CharUnknownHigh
		{
			get
			{
				return this._charUnknownHigh;
			}
		}

		/// <summary>Gets the low component character of the surrogate pair that caused the exception.</summary>
		/// <returns>The low component character of the surrogate pair that cannot be encoded.</returns>
		public char CharUnknownLow
		{
			get
			{
				return this._charUnknownLow;
			}
		}

		/// <summary>Gets the index position in the input buffer of the character that caused the exception.</summary>
		/// <returns>The index position in the input buffer of the character that cannot be encoded.</returns>
		public int Index
		{
			get
			{
				return this._index;
			}
		}

		/// <summary>Indicates whether the input that caused the exception is a surrogate pair.</summary>
		/// <returns>
		///   <see langword="true" /> if the input was a surrogate pair; otherwise, <see langword="false" />.</returns>
		public bool IsUnknownSurrogate()
		{
			return this._charUnknownHigh > '\0';
		}

		private char _charUnknown;

		private char _charUnknownHigh;

		private char _charUnknownLow;

		private int _index;
	}
}
