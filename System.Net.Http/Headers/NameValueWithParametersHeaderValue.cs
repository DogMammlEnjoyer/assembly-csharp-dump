using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a name/value pair with parameters used in various headers as defined in RFC 2616.</summary>
	public class NameValueWithParametersHeaderValue : NameValueHeaderValue, ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
		/// <param name="name">The header name.</param>
		public NameValueWithParametersHeaderValue(string name) : base(name)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
		/// <param name="name">The header name.</param>
		/// <param name="value">The header value.</param>
		public NameValueWithParametersHeaderValue(string name, string value) : base(name, value)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
		/// <param name="source">A <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object used to initialize the new instance.</param>
		protected NameValueWithParametersHeaderValue(NameValueWithParametersHeaderValue source) : base(source)
		{
			if (source.parameters != null)
			{
				foreach (NameValueHeaderValue item in source.parameters)
				{
					this.Parameters.Add(item);
				}
			}
		}

		private NameValueWithParametersHeaderValue()
		{
		}

		/// <summary>Gets the parameters from the <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
		/// <returns>A collection containing the parameters.</returns>
		public ICollection<NameValueHeaderValue> Parameters
		{
			get
			{
				List<NameValueHeaderValue> result;
				if ((result = this.parameters) == null)
				{
					result = (this.parameters = new List<NameValueHeaderValue>());
				}
				return result;
			}
		}

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return new NameValueWithParametersHeaderValue(this);
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			NameValueWithParametersHeaderValue nameValueWithParametersHeaderValue = obj as NameValueWithParametersHeaderValue;
			return nameValueWithParametersHeaderValue != null && base.Equals(obj) && nameValueWithParametersHeaderValue.parameters.SequenceEqual(this.parameters);
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ HashCodeCalculator.Calculate<NameValueHeaderValue>(this.parameters);
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents name value with parameter header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid name value with parameter header value information.</exception>
		public new static NameValueWithParametersHeaderValue Parse(string input)
		{
			NameValueWithParametersHeaderValue result;
			if (NameValueWithParametersHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (this.parameters == null || this.parameters.Count == 0)
			{
				return base.ToString();
			}
			return base.ToString() + this.parameters.ToString<NameValueHeaderValue>();
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out NameValueWithParametersHeaderValue parsedValue)
		{
			Token token;
			if (NameValueWithParametersHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<NameValueWithParametersHeaderValue> result)
		{
			return CollectionParser.TryParse<NameValueWithParametersHeaderValue>(input, minimalCount, new ElementTryParser<NameValueWithParametersHeaderValue>(NameValueWithParametersHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out NameValueWithParametersHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			parsedValue = new NameValueWithParametersHeaderValue
			{
				Name = lexer.GetStringValue(t)
			};
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorEqual)
			{
				t = lexer.Scan(false);
				if (t != Token.Type.Token && t != Token.Type.QuotedString)
				{
					return false;
				}
				parsedValue.value = lexer.GetStringValue(t);
				t = lexer.Scan(false);
			}
			if (t == Token.Type.SeparatorSemicolon)
			{
				List<NameValueHeaderValue> list;
				if (!NameValueHeaderValue.TryParseParameters(lexer, out list, out t))
				{
					return false;
				}
				parsedValue.parameters = list;
			}
			return true;
		}

		private List<NameValueHeaderValue> parameters;
	}
}
