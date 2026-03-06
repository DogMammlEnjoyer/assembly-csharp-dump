using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.Win32;

namespace System.Security.Cryptography
{
	/// <summary>The exception that is thrown when an error occurs during a cryptographic operation.</summary>
	[ComVisible(true)]
	[Serializable]
	public class CryptographicException : SystemException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CryptographicException" /> class with default properties.</summary>
		public CryptographicException() : base(Environment.GetResourceString("Error occurred during a cryptographic operation."))
		{
			base.SetErrorCode(-2146233296);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CryptographicException" /> class with a specified error message.</summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public CryptographicException(string message) : base(message)
		{
			base.SetErrorCode(-2146233296);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CryptographicException" /> class with a specified error message in the specified format.</summary>
		/// <param name="format">The format used to output the error message.</param>
		/// <param name="insert">The error message that explains the reason for the exception.</param>
		public CryptographicException(string format, string insert) : base(string.Format(CultureInfo.CurrentCulture, format, insert))
		{
			base.SetErrorCode(-2146233296);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CryptographicException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="inner">The exception that is the cause of the current exception. If the <paramref name="inner" /> parameter is not <see langword="null" />, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public CryptographicException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233296);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CryptographicException" /> class with the specified <see langword="HRESULT" /> error code.</summary>
		/// <param name="hr">The <see langword="HRESULT" /> error code.</param>
		[SecuritySafeCritical]
		public CryptographicException(int hr) : this(Win32Native.GetMessage(hr))
		{
			if (((long)hr & (long)((ulong)-2147483648)) != (long)((ulong)-2147483648))
			{
				hr = ((hr & 65535) | -2147024896);
			}
			base.SetErrorCode(hr);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CryptographicException" /> class with serialized data.</summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected CryptographicException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private static void ThrowCryptographicException(int hr)
		{
			throw new CryptographicException(hr);
		}

		private const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

		private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

		private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;
	}
}
