using System;

namespace System.Transactions
{
	/// <summary>Contains additional information that specifies transaction behaviors.</summary>
	public struct TransactionOptions
	{
		internal TransactionOptions(IsolationLevel level, TimeSpan timeout)
		{
			this.level = level;
			this.timeout = timeout;
		}

		/// <summary>Gets or sets the isolation level of the transaction.</summary>
		/// <returns>A <see cref="T:System.Transactions.IsolationLevel" /> enumeration that specifies the isolation level of the transaction.</returns>
		public IsolationLevel IsolationLevel
		{
			get
			{
				return this.level;
			}
			set
			{
				this.level = value;
			}
		}

		/// <summary>Gets or sets the timeout period for the transaction.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> value that specifies the timeout period for the transaction.</returns>
		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		/// <summary>Tests whether two specified <see cref="T:System.Transactions.TransactionOptions" /> instances are equivalent.</summary>
		/// <param name="x">The <see cref="T:System.Transactions.TransactionOptions" /> instance that is to the left of the equality operator.</param>
		/// <param name="y">The <see cref="T:System.Transactions.TransactionOptions" /> instance that is to the right of the equality operator.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="x" /> and <paramref name="y" /> are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(TransactionOptions x, TransactionOptions y)
		{
			return x.level == y.level && x.timeout == y.timeout;
		}

		/// <summary>Returns a value that indicates whether two <see cref="T:System.Transactions.TransactionOptions" /> instances are not equal.</summary>
		/// <param name="x">The <see cref="T:System.Transactions.TransactionOptions" /> instance that is to the left of the equality operator.</param>
		/// <param name="y">The <see cref="T:System.Transactions.TransactionOptions" /> instance that is to the right of the equality operator.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="x" /> and <paramref name="y" /> are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(TransactionOptions x, TransactionOptions y)
		{
			return x.level != y.level || x.timeout != y.timeout;
		}

		/// <summary>Determines whether this <see cref="T:System.Transactions.TransactionOptions" /> instance and the specified object are equal.</summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> and this <see cref="T:System.Transactions.TransactionOptions" /> instance are identical; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj is TransactionOptions && this == (TransactionOptions)obj;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return (int)(this.level ^ (IsolationLevel)this.timeout.GetHashCode());
		}

		private IsolationLevel level;

		private TimeSpan timeout;
	}
}
