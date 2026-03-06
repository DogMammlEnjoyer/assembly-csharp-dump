using System;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a byte range in a Range header value.</summary>
	public class RangeItemHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> class.</summary>
		/// <param name="from">The position at which to start sending data.</param>
		/// <param name="to">The position at which to stop sending data.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="from" /> is greater than <paramref name="to" />  
		/// -or-  
		/// <paramref name="from" /> or <paramref name="to" /> is less than 0.</exception>
		public RangeItemHeaderValue(long? from, long? to)
		{
			if (from == null && to == null)
			{
				throw new ArgumentException();
			}
			long? num2;
			if (from != null && to != null)
			{
				long? num = from;
				num2 = to;
				if (num.GetValueOrDefault() > num2.GetValueOrDefault() & (num != null & num2 != null))
				{
					throw new ArgumentOutOfRangeException("from");
				}
			}
			num2 = from;
			long num3 = 0L;
			if (num2.GetValueOrDefault() < num3 & num2 != null)
			{
				throw new ArgumentOutOfRangeException("from");
			}
			num2 = to;
			num3 = 0L;
			if (num2.GetValueOrDefault() < num3 & num2 != null)
			{
				throw new ArgumentOutOfRangeException("to");
			}
			this.From = from;
			this.To = to;
		}

		/// <summary>Gets the position at which to start sending data.</summary>
		/// <returns>The position at which to start sending data.</returns>
		public long? From { get; private set; }

		/// <summary>Gets the position at which to stop sending data.</summary>
		/// <returns>The position at which to stop sending data.</returns>
		public long? To { get; private set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			RangeItemHeaderValue rangeItemHeaderValue = obj as RangeItemHeaderValue;
			if (rangeItemHeaderValue != null)
			{
				long? num = rangeItemHeaderValue.From;
				long? num2 = this.From;
				if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
				{
					num2 = rangeItemHeaderValue.To;
					num = this.To;
					return num2.GetValueOrDefault() == num.GetValueOrDefault() & num2 != null == (num != null);
				}
			}
			return false;
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.From.GetHashCode() ^ this.To.GetHashCode();
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (this.From == null)
			{
				return "-" + this.To.Value.ToString();
			}
			if (this.To == null)
			{
				return this.From.Value.ToString() + "-";
			}
			return this.From.Value.ToString() + "-" + this.To.Value.ToString();
		}
	}
}
