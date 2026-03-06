using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a name/value pair used in various headers as defined in RFC 2616.</summary>
	public class NameValueHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
		/// <param name="name">The header name.</param>
		public NameValueHeaderValue(string name) : this(name, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
		/// <param name="name">The header name.</param>
		/// <param name="value">The header value.</param>
		public NameValueHeaderValue(string name, string value)
		{
			Parser.Token.Check(name);
			this.Name = name;
			this.Value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
		/// <param name="source">A <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> object used to initialize the new instance.</param>
		protected internal NameValueHeaderValue(NameValueHeaderValue source)
		{
			this.Name = source.Name;
			this.value = source.value;
		}

		internal NameValueHeaderValue()
		{
		}

		/// <summary>Gets the header name.</summary>
		/// <returns>The header name.</returns>
		public string Name { get; internal set; }

		/// <summary>Gets the header value.</summary>
		/// <returns>The header value.</returns>
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					Lexer lexer = new Lexer(value);
					Token token = lexer.Scan(false);
					if (lexer.Scan(false) != Token.Type.End || (token != Token.Type.Token && token != Token.Type.QuotedString))
					{
						throw new FormatException();
					}
					value = lexer.GetStringValue(token);
				}
				this.value = value;
			}
		}

		internal static NameValueHeaderValue Create(string name, string value)
		{
			return new NameValueHeaderValue
			{
				Name = name,
				value = value
			};
		}

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return new NameValueHeaderValue(this);
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			int num = this.Name.ToLowerInvariant().GetHashCode();
			if (!string.IsNullOrEmpty(this.value))
			{
				num ^= this.value.ToLowerInvariant().GetHashCode();
			}
			return num;
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			NameValueHeaderValue nameValueHeaderValue = obj as NameValueHeaderValue;
			if (nameValueHeaderValue == null || !string.Equals(nameValueHeaderValue.Name, this.Name, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.value))
			{
				return string.IsNullOrEmpty(nameValueHeaderValue.value);
			}
			return string.Equals(nameValueHeaderValue.value, this.value, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents name value header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid name value header value information.</exception>
		public static NameValueHeaderValue Parse(string input)
		{
			NameValueHeaderValue result;
			if (NameValueHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		internal static bool TryParsePragma(string input, int minimalCount, out List<NameValueHeaderValue> result)
		{
			return CollectionParser.TryParse<NameValueHeaderValue>(input, minimalCount, new ElementTryParser<NameValueHeaderValue>(NameValueHeaderValue.TryParseElement), out result);
		}

		internal static bool TryParseParameters(Lexer lexer, out List<NameValueHeaderValue> result, out Token t)
		{
			List<NameValueHeaderValue> list = new List<NameValueHeaderValue>();
			result = null;
			for (;;)
			{
				Token token = lexer.Scan(false);
				if (token != Token.Type.Token)
				{
					break;
				}
				string text = null;
				t = lexer.Scan(false);
				if (t == Token.Type.SeparatorEqual)
				{
					t = lexer.Scan(false);
					if (t != Token.Type.Token && t != Token.Type.QuotedString)
					{
						return false;
					}
					text = lexer.GetStringValue(t);
					t = lexer.Scan(false);
				}
				list.Add(new NameValueHeaderValue
				{
					Name = lexer.GetStringValue(token),
					value = text
				});
				if (t != Token.Type.SeparatorSemicolon)
				{
					goto Block_5;
				}
			}
			t = Token.Empty;
			return false;
			Block_5:
			result = list;
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.value))
			{
				return this.Name;
			}
			return this.Name + "=" + this.value;
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out NameValueHeaderValue parsedValue)
		{
			Token token;
			if (NameValueHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		private static bool TryParseElement(Lexer lexer, out NameValueHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			parsedValue = new NameValueHeaderValue
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
			return true;
		}

		internal string value;
	}
}
