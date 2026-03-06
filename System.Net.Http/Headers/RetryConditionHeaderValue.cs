using System;
using System.Globalization;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a Retry-After header value which can either be a date/time or a timespan value.</summary>
	public class RetryConditionHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> class.</summary>
		/// <param name="date">The date and time offset used to initialize the new instance.</param>
		public RetryConditionHeaderValue(DateTimeOffset date)
		{
			this.Date = new DateTimeOffset?(date);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> class.</summary>
		/// <param name="delta">The delta, in seconds, used to initialize the new instance.</param>
		public RetryConditionHeaderValue(TimeSpan delta)
		{
			if (delta.TotalSeconds > 4294967295.0)
			{
				throw new ArgumentOutOfRangeException("delta");
			}
			this.Delta = new TimeSpan?(delta);
		}

		/// <summary>Gets the date and time offset from the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
		/// <returns>The date and time offset from the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</returns>
		public DateTimeOffset? Date { get; private set; }

		/// <summary>Gets the delta in seconds from the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
		/// <returns>The delta in seconds from the <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</returns>
		public TimeSpan? Delta { get; private set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			RetryConditionHeaderValue retryConditionHeaderValue = obj as RetryConditionHeaderValue;
			return retryConditionHeaderValue != null && retryConditionHeaderValue.Date == this.Date && retryConditionHeaderValue.Delta == this.Delta;
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.Date.GetHashCode() ^ this.Delta.GetHashCode();
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents retry condition header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid retry condition header value information.</exception>
		public static RetryConditionHeaderValue Parse(string input)
		{
			RetryConditionHeaderValue result;
			if (RetryConditionHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out RetryConditionHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return false;
			}
			TimeSpan? timeSpan = lexer.TryGetTimeSpanValue(token);
			if (timeSpan != null)
			{
				if (lexer.Scan(false) != Token.Type.End)
				{
					return false;
				}
				parsedValue = new RetryConditionHeaderValue(timeSpan.Value);
			}
			else
			{
				DateTimeOffset date;
				if (!Lexer.TryGetDateValue(input, out date))
				{
					return false;
				}
				parsedValue = new RetryConditionHeaderValue(date);
			}
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (this.Delta == null)
			{
				return this.Date.Value.ToString("r", CultureInfo.InvariantCulture);
			}
			return this.Delta.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture);
		}
	}
}
