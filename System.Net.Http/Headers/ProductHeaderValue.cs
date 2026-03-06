using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a product token value in a User-Agent header.</summary>
	public class ProductHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> class.</summary>
		/// <param name="name">The product name.</param>
		public ProductHeaderValue(string name)
		{
			Parser.Token.Check(name);
			this.Name = name;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> class.</summary>
		/// <param name="name">The product name value.</param>
		/// <param name="version">The product version value.</param>
		public ProductHeaderValue(string name, string version) : this(name)
		{
			if (!string.IsNullOrEmpty(version))
			{
				Parser.Token.Check(version);
			}
			this.Version = version;
		}

		internal ProductHeaderValue()
		{
		}

		/// <summary>Gets the name of the product token.</summary>
		/// <returns>The name of the product token.</returns>
		public string Name { get; internal set; }

		/// <summary>Gets the version of the product token.</summary>
		/// <returns>The version of the product token.</returns>
		public string Version { get; internal set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			ProductHeaderValue productHeaderValue = obj as ProductHeaderValue;
			return productHeaderValue != null && string.Equals(productHeaderValue.Name, this.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(productHeaderValue.Version, this.Version, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			int num = this.Name.ToLowerInvariant().GetHashCode();
			if (this.Version != null)
			{
				num ^= this.Version.ToLowerInvariant().GetHashCode();
			}
			return num;
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents product header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> instance.</returns>
		public static ProductHeaderValue Parse(string input)
		{
			ProductHeaderValue result;
			if (ProductHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out ProductHeaderValue parsedValue)
		{
			Token token;
			if (ProductHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<ProductHeaderValue> result)
		{
			return CollectionParser.TryParse<ProductHeaderValue>(input, minimalCount, new ElementTryParser<ProductHeaderValue>(ProductHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out ProductHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			parsedValue = new ProductHeaderValue();
			parsedValue.Name = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorSlash)
			{
				t = lexer.Scan(false);
				if (t != Token.Type.Token)
				{
					return false;
				}
				parsedValue.Version = lexer.GetStringValue(t);
				t = lexer.Scan(false);
			}
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (this.Version != null)
			{
				return this.Name + "/" + this.Version;
			}
			return this.Name;
		}
	}
}
