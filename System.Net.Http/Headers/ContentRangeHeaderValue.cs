using System;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers
{
	/// <summary>Represents the value of the Content-Range header.</summary>
	public class ContentRangeHeaderValue : ICloneable
	{
		private ContentRangeHeaderValue()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
		/// <param name="length">The starting or ending point of the range, in bytes.</param>
		public ContentRangeHeaderValue(long length)
		{
			if (length < 0L)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			this.Length = new long?(length);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
		/// <param name="from">The position, in bytes, at which to start sending data.</param>
		/// <param name="to">The position, in bytes, at which to stop sending data.</param>
		public ContentRangeHeaderValue(long from, long to)
		{
			if (from < 0L || from > to)
			{
				throw new ArgumentOutOfRangeException("from");
			}
			this.From = new long?(from);
			this.To = new long?(to);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
		/// <param name="from">The position, in bytes, at which to start sending data.</param>
		/// <param name="to">The position, in bytes, at which to stop sending data.</param>
		/// <param name="length">The starting or ending point of the range, in bytes.</param>
		public ContentRangeHeaderValue(long from, long to, long length) : this(from, to)
		{
			if (length < 0L)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (to > length)
			{
				throw new ArgumentOutOfRangeException("to");
			}
			this.Length = new long?(length);
		}

		/// <summary>Gets the position at which to start sending data.</summary>
		/// <returns>The position, in bytes, at which to start sending data.</returns>
		public long? From { get; private set; }

		/// <summary>Gets whether the Content-Range header has a length specified.</summary>
		/// <returns>
		///   <see langword="true" /> if the Content-Range has a length specified; otherwise, <see langword="false" />.</returns>
		public bool HasLength
		{
			get
			{
				return this.Length != null;
			}
		}

		/// <summary>Gets whether the Content-Range has a range specified.</summary>
		/// <returns>
		///   <see langword="true" /> if the Content-Range has a range specified; otherwise, <see langword="false" />.</returns>
		public bool HasRange
		{
			get
			{
				return this.From != null;
			}
		}

		/// <summary>Gets the length of the full entity-body.</summary>
		/// <returns>The length of the full entity-body.</returns>
		public long? Length { get; private set; }

		/// <summary>Gets the position at which to stop sending data.</summary>
		/// <returns>The position at which to stop sending data.</returns>
		public long? To { get; private set; }

		/// <summary>The range units used.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains range units.</returns>
		public string Unit
		{
			get
			{
				return this.unit;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Unit");
				}
				Parser.Token.Check(value);
				this.unit = value;
			}
		}

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified Object is equal to the current <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			ContentRangeHeaderValue contentRangeHeaderValue = obj as ContentRangeHeaderValue;
			if (contentRangeHeaderValue == null)
			{
				return false;
			}
			long? num = contentRangeHeaderValue.Length;
			long? num2 = this.Length;
			if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
			{
				num2 = contentRangeHeaderValue.From;
				num = this.From;
				if (num2.GetValueOrDefault() == num.GetValueOrDefault() & num2 != null == (num != null))
				{
					num = contentRangeHeaderValue.To;
					num2 = this.To;
					if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
					{
						return string.Equals(contentRangeHeaderValue.unit, this.unit, StringComparison.OrdinalIgnoreCase);
					}
				}
			}
			return false;
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.Unit.GetHashCode() ^ this.Length.GetHashCode() ^ this.From.GetHashCode() ^ this.To.GetHashCode() ^ this.unit.ToLowerInvariant().GetHashCode();
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents content range header value information.</param>
		/// <returns>An <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid content range header value information.</exception>
		public static ContentRangeHeaderValue Parse(string input)
		{
			ContentRangeHeaderValue result;
			if (ContentRangeHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out ContentRangeHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return false;
			}
			ContentRangeHeaderValue contentRangeHeaderValue = new ContentRangeHeaderValue();
			contentRangeHeaderValue.unit = lexer.GetStringValue(token);
			token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return false;
			}
			if (!lexer.IsStarStringValue(token))
			{
				long value;
				if (!lexer.TryGetNumericValue(token, out value))
				{
					string stringValue = lexer.GetStringValue(token);
					if (stringValue.Length < 3)
					{
						return false;
					}
					string[] array = stringValue.Split('-', StringSplitOptions.None);
					if (array.Length != 2)
					{
						return false;
					}
					if (!long.TryParse(array[0], NumberStyles.None, CultureInfo.InvariantCulture, out value))
					{
						return false;
					}
					contentRangeHeaderValue.From = new long?(value);
					if (!long.TryParse(array[1], NumberStyles.None, CultureInfo.InvariantCulture, out value))
					{
						return false;
					}
					contentRangeHeaderValue.To = new long?(value);
				}
				else
				{
					contentRangeHeaderValue.From = new long?(value);
					token = lexer.Scan(true);
					if (token != Token.Type.SeparatorDash)
					{
						return false;
					}
					token = lexer.Scan(false);
					if (!lexer.TryGetNumericValue(token, out value))
					{
						return false;
					}
					contentRangeHeaderValue.To = new long?(value);
				}
			}
			token = lexer.Scan(false);
			if (token != Token.Type.SeparatorSlash)
			{
				return false;
			}
			token = lexer.Scan(false);
			if (!lexer.IsStarStringValue(token))
			{
				long value2;
				if (!lexer.TryGetNumericValue(token, out value2))
				{
					return false;
				}
				contentRangeHeaderValue.Length = new long?(value2);
			}
			token = lexer.Scan(false);
			if (token != Token.Type.End)
			{
				return false;
			}
			parsedValue = contentRangeHeaderValue;
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.unit);
			stringBuilder.Append(" ");
			if (this.From == null)
			{
				stringBuilder.Append("*");
			}
			else
			{
				stringBuilder.Append(this.From.Value.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append("-");
				stringBuilder.Append(this.To.Value.ToString(CultureInfo.InvariantCulture));
			}
			stringBuilder.Append("/");
			stringBuilder.Append((this.Length == null) ? "*" : this.Length.Value.ToString(CultureInfo.InvariantCulture));
			return stringBuilder.ToString();
		}

		private string unit = "bytes";
	}
}
