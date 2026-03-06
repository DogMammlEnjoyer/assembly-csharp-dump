using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a warning value used by the Warning header.</summary>
	public class WarningHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> class.</summary>
		/// <param name="code">The specific warning code.</param>
		/// <param name="agent">The host that attached the warning.</param>
		/// <param name="text">A quoted-string containing the warning text.</param>
		public WarningHeaderValue(int code, string agent, string text)
		{
			if (!WarningHeaderValue.IsCodeValid(code))
			{
				throw new ArgumentOutOfRangeException("code");
			}
			Parser.Uri.Check(agent);
			Parser.Token.CheckQuotedString(text);
			this.Code = code;
			this.Agent = agent;
			this.Text = text;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> class.</summary>
		/// <param name="code">The specific warning code.</param>
		/// <param name="agent">The host that attached the warning.</param>
		/// <param name="text">A quoted-string containing the warning text.</param>
		/// <param name="date">The date/time stamp of the warning.</param>
		public WarningHeaderValue(int code, string agent, string text, DateTimeOffset date) : this(code, agent, text)
		{
			this.Date = new DateTimeOffset?(date);
		}

		private WarningHeaderValue()
		{
		}

		/// <summary>Gets the host that attached the warning.</summary>
		/// <returns>The host that attached the warning.</returns>
		public string Agent { get; private set; }

		/// <summary>Gets the specific warning code.</summary>
		/// <returns>The specific warning code.</returns>
		public int Code { get; private set; }

		/// <summary>Gets the date/time stamp of the warning.</summary>
		/// <returns>The date/time stamp of the warning.</returns>
		public DateTimeOffset? Date { get; private set; }

		/// <summary>Gets a quoted-string containing the warning text.</summary>
		/// <returns>A quoted-string containing the warning text.</returns>
		public string Text { get; private set; }

		private static bool IsCodeValid(int code)
		{
			return code >= 0 && code < 1000;
		}

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> instance.</summary>
		/// <returns>Returns a copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			WarningHeaderValue warningHeaderValue = obj as WarningHeaderValue;
			return warningHeaderValue != null && (this.Code == warningHeaderValue.Code && string.Equals(warningHeaderValue.Agent, this.Agent, StringComparison.OrdinalIgnoreCase) && this.Text == warningHeaderValue.Text) && this.Date == warningHeaderValue.Date;
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.Code.GetHashCode() ^ this.Agent.ToLowerInvariant().GetHashCode() ^ this.Text.GetHashCode() ^ this.Date.GetHashCode();
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents authentication header value information.</param>
		/// <returns>Returns a <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid authentication header value information.</exception>
		public static WarningHeaderValue Parse(string input)
		{
			WarningHeaderValue result;
			if (WarningHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out WarningHeaderValue parsedValue)
		{
			Token token;
			if (WarningHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<WarningHeaderValue> result)
		{
			return CollectionParser.TryParse<WarningHeaderValue>(input, minimalCount, new ElementTryParser<WarningHeaderValue>(WarningHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out WarningHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			int code;
			if (!lexer.TryGetNumericValue(t, out code) || !WarningHeaderValue.IsCodeValid(code))
			{
				return false;
			}
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			Token token = t;
			if (lexer.PeekChar() == 58)
			{
				lexer.EatChar();
				token = lexer.Scan(false);
				if (token != Token.Type.Token)
				{
					return false;
				}
			}
			WarningHeaderValue warningHeaderValue = new WarningHeaderValue();
			warningHeaderValue.Code = code;
			warningHeaderValue.Agent = lexer.GetStringValue(t, token);
			t = lexer.Scan(false);
			if (t != Token.Type.QuotedString)
			{
				return false;
			}
			warningHeaderValue.Text = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.QuotedString)
			{
				DateTimeOffset value;
				if (!lexer.TryGetDateValue(t, out value))
				{
					return false;
				}
				warningHeaderValue.Date = new DateTimeOffset?(value);
				t = lexer.Scan(false);
			}
			parsedValue = warningHeaderValue;
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.WarningHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			string text = string.Concat(new string[]
			{
				this.Code.ToString("000"),
				" ",
				this.Agent,
				" ",
				this.Text
			});
			if (this.Date != null)
			{
				text = text + " \"" + this.Date.Value.ToString("r", CultureInfo.InvariantCulture) + "\"";
			}
			return text;
		}
	}
}
