using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents an entity-tag header value.</summary>
	public class EntityTagHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> class.</summary>
		/// <param name="tag">A string that contains an <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</param>
		public EntityTagHeaderValue(string tag)
		{
			Parser.Token.CheckQuotedString(tag);
			this.Tag = tag;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> class.</summary>
		/// <param name="tag">A string that contains an  <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</param>
		/// <param name="isWeak">A value that indicates if this entity-tag header is a weak validator. If the entity-tag header is weak validator, then <paramref name="isWeak" /> should be set to <see langword="true" />. If the entity-tag header is a strong validator, then <paramref name="isWeak" /> should be set to <see langword="false" />.</param>
		public EntityTagHeaderValue(string tag, bool isWeak) : this(tag)
		{
			this.IsWeak = isWeak;
		}

		internal EntityTagHeaderValue()
		{
		}

		/// <summary>Gets the entity-tag header value.</summary>
		/// <returns>Returns <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</returns>
		public static EntityTagHeaderValue Any
		{
			get
			{
				return EntityTagHeaderValue.any;
			}
		}

		/// <summary>Gets whether the entity-tag is prefaced by a weakness indicator.</summary>
		/// <returns>
		///   <see langword="true" /> if the entity-tag is prefaced by a weakness indicator; otherwise, <see langword="false" />.</returns>
		public bool IsWeak { get; internal set; }

		/// <summary>Gets the opaque quoted string.</summary>
		/// <returns>An opaque quoted string.</returns>
		public string Tag { get; internal set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			EntityTagHeaderValue entityTagHeaderValue = obj as EntityTagHeaderValue;
			return entityTagHeaderValue != null && entityTagHeaderValue.Tag == this.Tag && string.Equals(entityTagHeaderValue.Tag, this.Tag, StringComparison.Ordinal);
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.IsWeak.GetHashCode() ^ this.Tag.GetHashCode();
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents entity tag header value information.</param>
		/// <returns>An <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid entity tag header value information.</exception>
		public static EntityTagHeaderValue Parse(string input)
		{
			EntityTagHeaderValue result;
			if (EntityTagHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out EntityTagHeaderValue parsedValue)
		{
			Token token;
			if (EntityTagHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		private static bool TryParseElement(Lexer lexer, out EntityTagHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			bool isWeak = false;
			if (t == Token.Type.Token)
			{
				string stringValue = lexer.GetStringValue(t);
				if (stringValue == "*")
				{
					parsedValue = EntityTagHeaderValue.any;
					t = lexer.Scan(false);
					return true;
				}
				if (stringValue != "W" || lexer.PeekChar() != 47)
				{
					return false;
				}
				isWeak = true;
				lexer.EatChar();
				t = lexer.Scan(false);
			}
			if (t != Token.Type.QuotedString)
			{
				return false;
			}
			parsedValue = new EntityTagHeaderValue();
			parsedValue.Tag = lexer.GetStringValue(t);
			parsedValue.IsWeak = isWeak;
			t = lexer.Scan(false);
			return true;
		}

		internal static bool TryParse(string input, int minimalCount, out List<EntityTagHeaderValue> result)
		{
			return CollectionParser.TryParse<EntityTagHeaderValue>(input, minimalCount, new ElementTryParser<EntityTagHeaderValue>(EntityTagHeaderValue.TryParseElement), out result);
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (!this.IsWeak)
			{
				return this.Tag;
			}
			return "W/" + this.Tag;
		}

		private static readonly EntityTagHeaderValue any = new EntityTagHeaderValue
		{
			Tag = "*"
		};
	}
}
