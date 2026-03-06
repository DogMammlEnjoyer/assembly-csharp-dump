using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents authentication information in Authorization, ProxyAuthorization, WWW-Authenticate, and Proxy-Authenticate header values.</summary>
	public class AuthenticationHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> class.</summary>
		/// <param name="scheme">The scheme to use for authorization.</param>
		public AuthenticationHeaderValue(string scheme) : this(scheme, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> class.</summary>
		/// <param name="scheme">The scheme to use for authorization.</param>
		/// <param name="parameter">The credentials containing the authentication information of the user agent for the resource being requested.</param>
		public AuthenticationHeaderValue(string scheme, string parameter)
		{
			Parser.Token.Check(scheme);
			this.Scheme = scheme;
			this.Parameter = parameter;
		}

		private AuthenticationHeaderValue()
		{
		}

		/// <summary>Gets the credentials containing the authentication information of the user agent for the resource being requested.</summary>
		/// <returns>The credentials containing the authentication information.</returns>
		public string Parameter { get; private set; }

		/// <summary>Gets the scheme to use for authorization.</summary>
		/// <returns>The scheme to use for authorization.</returns>
		public string Scheme { get; private set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			AuthenticationHeaderValue authenticationHeaderValue = obj as AuthenticationHeaderValue;
			return authenticationHeaderValue != null && string.Equals(authenticationHeaderValue.Scheme, this.Scheme, StringComparison.OrdinalIgnoreCase) && authenticationHeaderValue.Parameter == this.Parameter;
		}

		/// <summary>Serves as a hash function for an  <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			int num = this.Scheme.ToLowerInvariant().GetHashCode();
			if (!string.IsNullOrEmpty(this.Parameter))
			{
				num ^= this.Parameter.ToLowerInvariant().GetHashCode();
			}
			return num;
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents authentication header value information.</param>
		/// <returns>An <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid authentication header value information.</exception>
		public static AuthenticationHeaderValue Parse(string input)
		{
			AuthenticationHeaderValue result;
			if (AuthenticationHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out AuthenticationHeaderValue parsedValue)
		{
			Token token;
			if (AuthenticationHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<AuthenticationHeaderValue> result)
		{
			return CollectionParser.TryParse<AuthenticationHeaderValue>(input, minimalCount, new ElementTryParser<AuthenticationHeaderValue>(AuthenticationHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out AuthenticationHeaderValue parsedValue, out Token t)
		{
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				parsedValue = null;
				return false;
			}
			parsedValue = new AuthenticationHeaderValue();
			parsedValue.Scheme = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.Token)
			{
				parsedValue.Parameter = lexer.GetRemainingStringValue(t.StartPosition);
				t = new Token(Token.Type.End, 0, 0);
			}
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (this.Parameter == null)
			{
				return this.Scheme;
			}
			return this.Scheme + " " + this.Parameter;
		}
	}
}
