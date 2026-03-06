using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents an Accept-Encoding header value.with optional quality factor.</summary>
	public sealed class TransferCodingWithQualityHeaderValue : TransferCodingHeaderValue
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> class.</summary>
		/// <param name="value">A string used to initialize the new instance.</param>
		public TransferCodingWithQualityHeaderValue(string value) : base(value)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> class.</summary>
		/// <param name="value">A string used to initialize the new instance.</param>
		/// <param name="quality">A value for the quality factor.</param>
		public TransferCodingWithQualityHeaderValue(string value, double quality) : this(value)
		{
			this.Quality = new double?(quality);
		}

		private TransferCodingWithQualityHeaderValue()
		{
		}

		/// <summary>Gets the quality factor from the <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" />.</summary>
		/// <returns>The quality factor from the <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" />.</returns>
		public double? Quality
		{
			get
			{
				return QualityValue.GetValue(this.parameters);
			}
			set
			{
				QualityValue.SetValue(ref this.parameters, value);
			}
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents transfer-coding value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid transfer-coding with quality header value information.</exception>
		public new static TransferCodingWithQualityHeaderValue Parse(string input)
		{
			TransferCodingWithQualityHeaderValue result;
			if (TransferCodingWithQualityHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException();
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out TransferCodingWithQualityHeaderValue parsedValue)
		{
			Token token;
			if (TransferCodingWithQualityHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<TransferCodingWithQualityHeaderValue> result)
		{
			return CollectionParser.TryParse<TransferCodingWithQualityHeaderValue>(input, minimalCount, new ElementTryParser<TransferCodingWithQualityHeaderValue>(TransferCodingWithQualityHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out TransferCodingWithQualityHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			TransferCodingWithQualityHeaderValue transferCodingWithQualityHeaderValue = new TransferCodingWithQualityHeaderValue();
			transferCodingWithQualityHeaderValue.value = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorSemicolon && (!NameValueHeaderValue.TryParseParameters(lexer, out transferCodingWithQualityHeaderValue.parameters, out t) || t != Token.Type.End))
			{
				return false;
			}
			parsedValue = transferCodingWithQualityHeaderValue;
			return true;
		}
	}
}
