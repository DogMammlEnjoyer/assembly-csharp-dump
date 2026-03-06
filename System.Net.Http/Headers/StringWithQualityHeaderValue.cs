using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a string header value with an optional quality.</summary>
	public class StringWithQualityHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> class.</summary>
		/// <param name="value">The string used to initialize the new instance.</param>
		public StringWithQualityHeaderValue(string value)
		{
			Parser.Token.Check(value);
			this.Value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> class.</summary>
		/// <param name="value">A string used to initialize the new instance.</param>
		/// <param name="quality">A quality factor used to initialize the new instance.</param>
		public StringWithQualityHeaderValue(string value, double quality) : this(value)
		{
			if (quality < 0.0 || quality > 1.0)
			{
				throw new ArgumentOutOfRangeException("quality");
			}
			this.Quality = new double?(quality);
		}

		private StringWithQualityHeaderValue()
		{
		}

		/// <summary>Gets the quality factor from the <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</summary>
		/// <returns>The quality factor from the <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</returns>
		public double? Quality { get; private set; }

		/// <summary>Gets the string value from the <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</summary>
		/// <returns>The string value from the <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</returns>
		public string Value { get; private set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified Object is equal to the current <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			StringWithQualityHeaderValue stringWithQualityHeaderValue = obj as StringWithQualityHeaderValue;
			if (stringWithQualityHeaderValue != null && string.Equals(stringWithQualityHeaderValue.Value, this.Value, StringComparison.OrdinalIgnoreCase))
			{
				double? quality = stringWithQualityHeaderValue.Quality;
				double? quality2 = this.Quality;
				return quality.GetValueOrDefault() == quality2.GetValueOrDefault() & quality != null == (quality2 != null);
			}
			return false;
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.Value.ToLowerInvariant().GetHashCode() ^ this.Quality.GetHashCode();
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents quality header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid string with quality header value information.</exception>
		public static StringWithQualityHeaderValue Parse(string input)
		{
			StringWithQualityHeaderValue result;
			if (StringWithQualityHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out StringWithQualityHeaderValue parsedValue)
		{
			Token token;
			if (StringWithQualityHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<StringWithQualityHeaderValue> result)
		{
			return CollectionParser.TryParse<StringWithQualityHeaderValue>(input, minimalCount, new ElementTryParser<StringWithQualityHeaderValue>(StringWithQualityHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out StringWithQualityHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			StringWithQualityHeaderValue stringWithQualityHeaderValue = new StringWithQualityHeaderValue();
			stringWithQualityHeaderValue.Value = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorSemicolon)
			{
				t = lexer.Scan(false);
				if (t != Token.Type.Token)
				{
					return false;
				}
				string stringValue = lexer.GetStringValue(t);
				if (stringValue != "q" && stringValue != "Q")
				{
					return false;
				}
				t = lexer.Scan(false);
				if (t != Token.Type.SeparatorEqual)
				{
					return false;
				}
				t = lexer.Scan(false);
				double num;
				if (!lexer.TryGetDoubleValue(t, out num))
				{
					return false;
				}
				if (num > 1.0)
				{
					return false;
				}
				stringWithQualityHeaderValue.Quality = new double?(num);
				t = lexer.Scan(false);
			}
			parsedValue = stringWithQualityHeaderValue;
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (this.Quality != null)
			{
				return this.Value + "; q=" + this.Quality.Value.ToString("0.0##", CultureInfo.InvariantCulture);
			}
			return this.Value;
		}
	}
}
