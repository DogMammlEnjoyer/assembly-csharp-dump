using System;
using System.Globalization;

namespace System.Net.Http.Headers
{
	/// <summary>Represents an If-Range header value which can either be a date/time or an entity-tag value.</summary>
	public class RangeConditionHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
		/// <param name="date">A date value used to initialize the new instance.</param>
		public RangeConditionHeaderValue(DateTimeOffset date)
		{
			this.Date = new DateTimeOffset?(date);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
		/// <param name="entityTag">An <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" /> object used to initialize the new instance.</param>
		public RangeConditionHeaderValue(EntityTagHeaderValue entityTag)
		{
			if (entityTag == null)
			{
				throw new ArgumentNullException("entityTag");
			}
			this.EntityTag = entityTag;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
		/// <param name="entityTag">An entity tag represented as a string used to initialize the new instance.</param>
		public RangeConditionHeaderValue(string entityTag) : this(new EntityTagHeaderValue(entityTag))
		{
		}

		/// <summary>Gets the date from the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
		/// <returns>The date from the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</returns>
		public DateTimeOffset? Date { get; private set; }

		/// <summary>Gets the entity tag from the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
		/// <returns>The entity tag from the <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</returns>
		public EntityTagHeaderValue EntityTag { get; private set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			RangeConditionHeaderValue rangeConditionHeaderValue = obj as RangeConditionHeaderValue;
			if (rangeConditionHeaderValue == null)
			{
				return false;
			}
			if (this.EntityTag == null)
			{
				return this.Date == rangeConditionHeaderValue.Date;
			}
			return this.EntityTag.Equals(rangeConditionHeaderValue.EntityTag);
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			if (this.EntityTag == null)
			{
				return this.Date.GetHashCode();
			}
			return this.EntityTag.GetHashCode();
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents range condition header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid range Condition header value information.</exception>
		public static RangeConditionHeaderValue Parse(string input)
		{
			RangeConditionHeaderValue result;
			if (RangeConditionHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out RangeConditionHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			bool isWeak;
			if (token == Token.Type.Token)
			{
				if (lexer.GetStringValue(token) != "W")
				{
					DateTimeOffset date;
					if (!Lexer.TryGetDateValue(input, out date))
					{
						return false;
					}
					parsedValue = new RangeConditionHeaderValue(date);
					return true;
				}
				else
				{
					if (lexer.PeekChar() != 47)
					{
						return false;
					}
					isWeak = true;
					lexer.EatChar();
					token = lexer.Scan(false);
				}
			}
			else
			{
				isWeak = false;
			}
			if (token != Token.Type.QuotedString)
			{
				return false;
			}
			if (lexer.Scan(false) != Token.Type.End)
			{
				return false;
			}
			parsedValue = new RangeConditionHeaderValue(new EntityTagHeaderValue
			{
				Tag = lexer.GetStringValue(token),
				IsWeak = isWeak
			});
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (this.EntityTag != null)
			{
				return this.EntityTag.ToString();
			}
			return this.Date.Value.ToString("r", CultureInfo.InvariantCulture);
		}
	}
}
