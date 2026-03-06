using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	/// <summary>Represents the abstract class from which all implementations of cryptographic random number generators derive.</summary>
	[ComVisible(true)]
	public abstract class RandomNumberGenerator : IDisposable
	{
		/// <summary>Creates an instance of the default implementation of a cryptographic random number generator that can be used to generate random data.</summary>
		/// <returns>A new instance of a cryptographic random number generator.</returns>
		public static RandomNumberGenerator Create()
		{
			return RandomNumberGenerator.Create("System.Security.Cryptography.RandomNumberGenerator");
		}

		/// <summary>Creates an instance of the specified implementation of a cryptographic random number generator.</summary>
		/// <param name="rngName">The name of the random number generator implementation to use.</param>
		/// <returns>A new instance of a cryptographic random number generator.</returns>
		public static RandomNumberGenerator Create(string rngName)
		{
			return (RandomNumberGenerator)CryptoConfig.CreateFromName(rngName);
		}

		/// <summary>When overridden in a derived class, releases all resources used by the current instance of the <see cref="T:System.Security.Cryptography.RandomNumberGenerator" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>When overridden in a derived class, releases the unmanaged resources used by the <see cref="T:System.Security.Cryptography.RandomNumberGenerator" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>When overridden in a derived class, fills an array of bytes with a cryptographically strong random sequence of values.</summary>
		/// <param name="data">The array to fill with cryptographically strong random bytes.</param>
		public abstract void GetBytes(byte[] data);

		/// <summary>Fills the specified byte array with a cryptographically strong random sequence of values.</summary>
		/// <param name="data">The array to fill with cryptographically strong random bytes.</param>
		/// <param name="offset">The index of the array to start the fill operation.</param>
		/// <param name="count">The number of bytes to fill.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is less than 0</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset" /> plus <paramref name="count" /> exceeds the length of <paramref name="data" />.</exception>
		public virtual void GetBytes(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (offset + count > data.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (count > 0)
			{
				byte[] array = new byte[count];
				this.GetBytes(array);
				Array.Copy(array, 0, data, offset, count);
			}
		}

		/// <summary>When overridden in a derived class, fills an array of bytes with a cryptographically strong random sequence of nonzero values.</summary>
		/// <param name="data">The array to fill with cryptographically strong random nonzero bytes.</param>
		public virtual void GetNonZeroBytes(byte[] data)
		{
			throw new NotImplementedException();
		}

		public static void Fill(Span<byte> data)
		{
			RandomNumberGenerator.FillSpan(data);
		}

		internal unsafe static void FillSpan(Span<byte> data)
		{
			if (data.Length > 0)
			{
				fixed (byte* pinnableReference = data.GetPinnableReference())
				{
					Interop.GetRandomBytes(pinnableReference, data.Length);
				}
			}
		}

		public virtual void GetBytes(Span<byte> data)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(data.Length);
			try
			{
				this.GetBytes(array, 0, data.Length);
				new ReadOnlySpan<byte>(array, 0, data.Length).CopyTo(data);
			}
			finally
			{
				Array.Clear(array, 0, data.Length);
				ArrayPool<byte>.Shared.Return(array, false);
			}
		}

		public virtual void GetNonZeroBytes(Span<byte> data)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(data.Length);
			try
			{
				this.GetNonZeroBytes(array);
				new ReadOnlySpan<byte>(array, 0, data.Length).CopyTo(data);
			}
			finally
			{
				Array.Clear(array, 0, data.Length);
				ArrayPool<byte>.Shared.Return(array, false);
			}
		}

		public unsafe static int GetInt32(int fromInclusive, int toExclusive)
		{
			if (fromInclusive >= toExclusive)
			{
				throw new ArgumentException("Range of random number does not contain at least one possibility.");
			}
			uint num = (uint)(toExclusive - fromInclusive - 1);
			if (num == 0U)
			{
				return fromInclusive;
			}
			uint num2 = num;
			num2 |= num2 >> 1;
			num2 |= num2 >> 2;
			num2 |= num2 >> 4;
			num2 |= num2 >> 8;
			num2 |= num2 >> 16;
			Span<uint> span = new Span<uint>(stackalloc byte[(UIntPtr)4], 1);
			uint num3;
			do
			{
				RandomNumberGenerator.FillSpan(MemoryMarshal.AsBytes<uint>(span));
				num3 = (num2 & *span[0]);
			}
			while (num3 > num);
			return (int)(num3 + (uint)fromInclusive);
		}

		public static int GetInt32(int toExclusive)
		{
			if (toExclusive <= 0)
			{
				throw new ArgumentOutOfRangeException("toExclusive", "Positive number required.");
			}
			return RandomNumberGenerator.GetInt32(0, toExclusive);
		}
	}
}
