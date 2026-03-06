using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.X509Certificates
{
	internal static class OSX509Certificates
	{
		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCreateWithData(IntPtr allocator, IntPtr nsdataRef);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern int SecTrustCreateWithCertificates(IntPtr certOrCertArray, IntPtr policies, out IntPtr sectrustref);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern int SecTrustSetAnchorCertificates(IntPtr trust, IntPtr anchorCertificates);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecPolicyCreateSSL([MarshalAs(UnmanagedType.I1)] bool server, IntPtr cfStringHostname);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern int SecTrustEvaluate(IntPtr secTrustRef, out OSX509Certificates.SecTrustResult secTrustResultTime);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern IntPtr CFStringCreateWithCharacters(IntPtr allocator, string str, IntPtr count);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private unsafe static extern IntPtr CFDataCreate(IntPtr allocator, byte* bytes, IntPtr length);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern void CFRetain(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern void CFRelease(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFArrayCreate(IntPtr allocator, IntPtr values, IntPtr numValues, IntPtr callbacks);

		private unsafe static IntPtr MakeCFData(byte[] data)
		{
			fixed (byte* ptr = &data[0])
			{
				byte* bytes = ptr;
				return OSX509Certificates.CFDataCreate(IntPtr.Zero, bytes, (IntPtr)data.Length);
			}
		}

		private unsafe static IntPtr FromIntPtrs(IntPtr[] values)
		{
			IntPtr* value;
			if (values == null || values.Length == 0)
			{
				value = null;
			}
			else
			{
				value = &values[0];
			}
			return OSX509Certificates.CFArrayCreate(IntPtr.Zero, (IntPtr)((void*)value), (IntPtr)values.Length, IntPtr.Zero);
		}

		private static IntPtr GetCertificate(X509Certificate certificate)
		{
			IntPtr intPtr = certificate.Impl.GetNativeAppleCertificate();
			if (intPtr != IntPtr.Zero)
			{
				OSX509Certificates.CFRetain(intPtr);
				return intPtr;
			}
			IntPtr intPtr2 = OSX509Certificates.MakeCFData(certificate.GetRawCertData());
			intPtr = OSX509Certificates.SecCertificateCreateWithData(IntPtr.Zero, intPtr2);
			OSX509Certificates.CFRelease(intPtr2);
			return intPtr;
		}

		public static OSX509Certificates.SecTrustResult TrustEvaluateSsl(X509CertificateCollection certificates, X509CertificateCollection anchors, string host)
		{
			if (certificates == null)
			{
				return OSX509Certificates.SecTrustResult.Deny;
			}
			OSX509Certificates.SecTrustResult result;
			try
			{
				result = OSX509Certificates._TrustEvaluateSsl(certificates, anchors, host);
			}
			catch
			{
				result = OSX509Certificates.SecTrustResult.Deny;
			}
			return result;
		}

		private static OSX509Certificates.SecTrustResult _TrustEvaluateSsl(X509CertificateCollection certificates, X509CertificateCollection anchors, string hostName)
		{
			int count = certificates.Count;
			int num = (anchors != null) ? anchors.Count : 0;
			IntPtr[] array = new IntPtr[count];
			IntPtr[] array2 = new IntPtr[num];
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr intPtr3 = IntPtr.Zero;
			IntPtr intPtr4 = IntPtr.Zero;
			IntPtr zero = IntPtr.Zero;
			OSX509Certificates.SecTrustResult secTrustResult = OSX509Certificates.SecTrustResult.Deny;
			OSX509Certificates.SecTrustResult result;
			try
			{
				for (int i = 0; i < count; i++)
				{
					array[i] = OSX509Certificates.GetCertificate(certificates[i]);
					if (array[i] == IntPtr.Zero)
					{
						return OSX509Certificates.SecTrustResult.Deny;
					}
				}
				for (int j = 0; j < num; j++)
				{
					array2[j] = OSX509Certificates.GetCertificate(anchors[j]);
					if (array2[j] == IntPtr.Zero)
					{
						return OSX509Certificates.SecTrustResult.Deny;
					}
				}
				intPtr = OSX509Certificates.FromIntPtrs(array);
				if (hostName != null)
				{
					intPtr4 = OSX509Certificates.CFStringCreateWithCharacters(IntPtr.Zero, hostName, (IntPtr)hostName.Length);
				}
				intPtr3 = OSX509Certificates.SecPolicyCreateSSL(true, intPtr4);
				if (OSX509Certificates.SecTrustCreateWithCertificates(intPtr, intPtr3, out zero) != 0)
				{
					result = OSX509Certificates.SecTrustResult.Deny;
				}
				else
				{
					if (num > 0)
					{
						intPtr2 = OSX509Certificates.FromIntPtrs(array2);
						OSX509Certificates.SecTrustSetAnchorCertificates(zero, intPtr2);
					}
					OSX509Certificates.SecTrustEvaluate(zero, out secTrustResult);
					result = secTrustResult;
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					OSX509Certificates.CFRelease(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					OSX509Certificates.CFRelease(intPtr2);
				}
				for (int k = 0; k < count; k++)
				{
					if (array[k] != IntPtr.Zero)
					{
						OSX509Certificates.CFRelease(array[k]);
					}
				}
				for (int l = 0; l < num; l++)
				{
					if (array2[l] != IntPtr.Zero)
					{
						OSX509Certificates.CFRelease(array2[l]);
					}
				}
				if (intPtr3 != IntPtr.Zero)
				{
					OSX509Certificates.CFRelease(intPtr3);
				}
				if (intPtr4 != IntPtr.Zero)
				{
					OSX509Certificates.CFRelease(intPtr4);
				}
				if (zero != IntPtr.Zero)
				{
					OSX509Certificates.CFRelease(zero);
				}
			}
			return result;
		}

		public const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";

		public const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

		public enum SecTrustResult
		{
			Invalid,
			Proceed,
			Confirm,
			Deny,
			Unspecified,
			RecoverableTrustFailure,
			FatalTrustFailure,
			ResultOtherError
		}
	}
}
