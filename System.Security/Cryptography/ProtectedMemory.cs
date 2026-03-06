using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	/// <summary>Provides methods for protecting and unprotecting memory. This class cannot be inherited.</summary>
	public sealed class ProtectedMemory
	{
		private ProtectedMemory()
		{
		}

		/// <summary>Protects the specified data.</summary>
		/// <param name="userData">The byte array containing data in memory to protect. The array must be a multiple of 16 bytes.</param>
		/// <param name="scope">One of the enumeration values that specifies the scope of memory protection.</param>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///   <paramref name="userData" /> must be 16 bytes in length or in multiples of 16 bytes.</exception>
		/// <exception cref="T:System.NotSupportedException">The operating system does not support this method. This method can be used only with the Windows 2000 or later operating systems.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="userData" /> is <see langword="null" />.</exception>
		[MonoTODO("only supported on Windows 2000 SP3 and later")]
		public static void Protect(byte[] userData, MemoryProtectionScope scope)
		{
			if (userData == null)
			{
				throw new ArgumentNullException("userData");
			}
			ProtectedMemory.Check(userData.Length, scope);
			try
			{
				uint cbData = (uint)userData.Length;
				ProtectedMemory.MemoryProtectionImplementation memoryProtectionImplementation = ProtectedMemory.impl;
				if (memoryProtectionImplementation != ProtectedMemory.MemoryProtectionImplementation.Win32RtlEncryptMemory)
				{
					if (memoryProtectionImplementation != ProtectedMemory.MemoryProtectionImplementation.Win32CryptoProtect)
					{
						throw new PlatformNotSupportedException();
					}
					if (!ProtectedMemory.CryptProtectMemory(userData, cbData, (uint)scope))
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
				}
				else
				{
					int num = ProtectedMemory.RtlEncryptMemory(userData, cbData, (uint)scope);
					if (num < 0)
					{
						throw new CryptographicException(Locale.GetText("Error. NTSTATUS = {0}.", new object[]
						{
							num
						}));
					}
				}
			}
			catch
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Unsupported;
				throw new PlatformNotSupportedException();
			}
		}

		/// <summary>Unprotects data in memory that was protected using the <see cref="M:System.Security.Cryptography.ProtectedMemory.Protect(System.Byte[],System.Security.Cryptography.MemoryProtectionScope)" /> method.</summary>
		/// <param name="encryptedData">The byte array in memory to unencrypt.</param>
		/// <param name="scope">One of the enumeration values that specifies the scope of memory protection.</param>
		/// <exception cref="T:System.NotSupportedException">The operating system does not support this method. This method can be used only with the Windows 2000 or later operating systems.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="encryptedData" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///   <paramref name="encryptedData" /> is empty.  
		/// -or-  
		/// This call was not implemented.  
		/// -or-  
		/// NTSTATUS contains an error.</exception>
		[MonoTODO("only supported on Windows 2000 SP3 and later")]
		public static void Unprotect(byte[] encryptedData, MemoryProtectionScope scope)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			ProtectedMemory.Check(encryptedData.Length, scope);
			try
			{
				uint cbData = (uint)encryptedData.Length;
				ProtectedMemory.MemoryProtectionImplementation memoryProtectionImplementation = ProtectedMemory.impl;
				if (memoryProtectionImplementation != ProtectedMemory.MemoryProtectionImplementation.Win32RtlEncryptMemory)
				{
					if (memoryProtectionImplementation != ProtectedMemory.MemoryProtectionImplementation.Win32CryptoProtect)
					{
						throw new PlatformNotSupportedException();
					}
					if (!ProtectedMemory.CryptUnprotectMemory(encryptedData, cbData, (uint)scope))
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
				}
				else
				{
					int num = ProtectedMemory.RtlDecryptMemory(encryptedData, cbData, (uint)scope);
					if (num < 0)
					{
						throw new CryptographicException(Locale.GetText("Error. NTSTATUS = {0}.", new object[]
						{
							num
						}));
					}
				}
			}
			catch
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Unsupported;
				throw new PlatformNotSupportedException();
			}
		}

		private static void Detect()
		{
			OperatingSystem osversion = Environment.OSVersion;
			if (osversion.Platform != PlatformID.Win32NT)
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Unsupported;
				return;
			}
			Version version = osversion.Version;
			if (version.Major < 5)
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Unsupported;
				return;
			}
			if (version.Major != 5)
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Win32CryptoProtect;
				return;
			}
			if (version.Minor < 2)
			{
				ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Win32RtlEncryptMemory;
				return;
			}
			ProtectedMemory.impl = ProtectedMemory.MemoryProtectionImplementation.Win32CryptoProtect;
		}

		private static void Check(int size, MemoryProtectionScope scope)
		{
			if (size % 16 != 0)
			{
				throw new CryptographicException(Locale.GetText("Not a multiple of {0} bytes.", new object[]
				{
					16
				}));
			}
			if (scope < MemoryProtectionScope.SameProcess || scope > MemoryProtectionScope.SameLogon)
			{
				throw new ArgumentException(Locale.GetText("Invalid enum value for '{0}'.", new object[]
				{
					"MemoryProtectionScope"
				}), "scope");
			}
			ProtectedMemory.MemoryProtectionImplementation memoryProtectionImplementation = ProtectedMemory.impl;
			if (memoryProtectionImplementation == ProtectedMemory.MemoryProtectionImplementation.Unsupported)
			{
				throw new PlatformNotSupportedException();
			}
			if (memoryProtectionImplementation == ProtectedMemory.MemoryProtectionImplementation.Unknown)
			{
				ProtectedMemory.Detect();
				return;
			}
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "SystemFunction040", SetLastError = true)]
		private static extern int RtlEncryptMemory(byte[] pData, uint cbData, uint dwFlags);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "SystemFunction041", SetLastError = true)]
		private static extern int RtlDecryptMemory(byte[] pData, uint cbData, uint dwFlags);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("crypt32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptProtectMemory(byte[] pData, uint cbData, uint dwFlags);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("crypt32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptUnprotectMemory(byte[] pData, uint cbData, uint dwFlags);

		private const int BlockSize = 16;

		private static ProtectedMemory.MemoryProtectionImplementation impl;

		private enum MemoryProtectionImplementation
		{
			Unknown,
			Win32RtlEncryptMemory,
			Win32CryptoProtect,
			Unsupported = -2147483648
		}
	}
}
