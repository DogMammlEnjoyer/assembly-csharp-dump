using System;
using System.Net.Security;
using System.Runtime.InteropServices;

namespace System.Net
{
	internal class SSPISecureChannelType : SSPIInterface
	{
		public SecurityPackageInfoClass[] SecurityPackages
		{
			get
			{
				return SSPISecureChannelType.s_securityPackages;
			}
			set
			{
				SSPISecureChannelType.s_securityPackages = value;
			}
		}

		public int EnumerateSecurityPackages(out int pkgnum, out SafeFreeContextBuffer pkgArray)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, null, "EnumerateSecurityPackages");
			}
			return SafeFreeContextBuffer.EnumeratePackages(out pkgnum, out pkgArray);
		}

		public int AcquireCredentialsHandle(string moduleName, Interop.SspiCli.CredentialUse usage, ref Interop.SspiCli.SEC_WINNT_AUTH_IDENTITY_W authdata, out SafeFreeCredentials outCredential)
		{
			return SafeFreeCredentials.AcquireCredentialsHandle(moduleName, usage, ref authdata, out outCredential);
		}

		public int AcquireCredentialsHandle(string moduleName, Interop.SspiCli.CredentialUse usage, ref SafeSspiAuthDataHandle authdata, out SafeFreeCredentials outCredential)
		{
			return SafeFreeCredentials.AcquireCredentialsHandle(moduleName, usage, ref authdata, out outCredential);
		}

		public int AcquireDefaultCredential(string moduleName, Interop.SspiCli.CredentialUse usage, out SafeFreeCredentials outCredential)
		{
			return SafeFreeCredentials.AcquireDefaultCredential(moduleName, usage, out outCredential);
		}

		public int AcquireCredentialsHandle(string moduleName, Interop.SspiCli.CredentialUse usage, ref Interop.SspiCli.SCHANNEL_CRED authdata, out SafeFreeCredentials outCredential)
		{
			return SafeFreeCredentials.AcquireCredentialsHandle(moduleName, usage, ref authdata, out outCredential);
		}

		public int AcceptSecurityContext(ref SafeFreeCredentials credential, ref SafeDeleteContext context, SecurityBuffer inputBuffer, Interop.SspiCli.ContextFlags inFlags, Interop.SspiCli.Endianness endianness, SecurityBuffer outputBuffer, ref Interop.SspiCli.ContextFlags outFlags)
		{
			return SafeDeleteContext.AcceptSecurityContext(ref credential, ref context, inFlags, endianness, inputBuffer, null, outputBuffer, ref outFlags);
		}

		public int AcceptSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, SecurityBuffer[] inputBuffers, Interop.SspiCli.ContextFlags inFlags, Interop.SspiCli.Endianness endianness, SecurityBuffer outputBuffer, ref Interop.SspiCli.ContextFlags outFlags)
		{
			return SafeDeleteContext.AcceptSecurityContext(ref credential, ref context, inFlags, endianness, null, inputBuffers, outputBuffer, ref outFlags);
		}

		public int InitializeSecurityContext(ref SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, Interop.SspiCli.ContextFlags inFlags, Interop.SspiCli.Endianness endianness, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, ref Interop.SspiCli.ContextFlags outFlags)
		{
			return SafeDeleteContext.InitializeSecurityContext(ref credential, ref context, targetName, inFlags, endianness, inputBuffer, null, outputBuffer, ref outFlags);
		}

		public int InitializeSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, Interop.SspiCli.ContextFlags inFlags, Interop.SspiCli.Endianness endianness, SecurityBuffer[] inputBuffers, SecurityBuffer outputBuffer, ref Interop.SspiCli.ContextFlags outFlags)
		{
			return SafeDeleteContext.InitializeSecurityContext(ref credential, ref context, targetName, inFlags, endianness, null, inputBuffers, outputBuffer, ref outFlags);
		}

		public int EncryptMessage(SafeDeleteContext context, ref Interop.SspiCli.SecBufferDesc inputOutput, uint sequenceNumber)
		{
			int result;
			try
			{
				bool flag = false;
				context.DangerousAddRef(ref flag);
				result = Interop.SspiCli.EncryptMessage(ref context._handle, 0U, ref inputOutput, sequenceNumber);
			}
			finally
			{
				context.DangerousRelease();
			}
			return result;
		}

		public int DecryptMessage(SafeDeleteContext context, ref Interop.SspiCli.SecBufferDesc inputOutput, uint sequenceNumber)
		{
			int result;
			try
			{
				bool flag = false;
				context.DangerousAddRef(ref flag);
				result = Interop.SspiCli.DecryptMessage(ref context._handle, ref inputOutput, sequenceNumber, null);
			}
			finally
			{
				context.DangerousRelease();
			}
			return result;
		}

		public int MakeSignature(SafeDeleteContext context, ref Interop.SspiCli.SecBufferDesc inputOutput, uint sequenceNumber)
		{
			throw NotImplemented.ByDesignWithMessage("This method is not implemented by this class.");
		}

		public int VerifySignature(SafeDeleteContext context, ref Interop.SspiCli.SecBufferDesc inputOutput, uint sequenceNumber)
		{
			throw NotImplemented.ByDesignWithMessage("This method is not implemented by this class.");
		}

		public unsafe int QueryContextChannelBinding(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute attribute, out SafeFreeContextBufferChannelBinding refHandle)
		{
			refHandle = SafeFreeContextBufferChannelBinding.CreateEmptyHandle();
			SecPkgContext_Bindings secPkgContext_Bindings = default(SecPkgContext_Bindings);
			return SafeFreeContextBufferChannelBinding.QueryContextChannelBinding(phContext, attribute, &secPkgContext_Bindings, refHandle);
		}

		public unsafe int QueryContextAttributes(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute attribute, byte[] buffer, Type handleType, out SafeHandle refHandle)
		{
			refHandle = null;
			if (handleType != null)
			{
				if (handleType == typeof(SafeFreeContextBuffer))
				{
					refHandle = SafeFreeContextBuffer.CreateEmptyHandle();
				}
				else
				{
					if (!(handleType == typeof(SafeFreeCertContext)))
					{
						throw new ArgumentException(SR.Format("'{0}' is not a supported handle type.", handleType.FullName), "handleType");
					}
					refHandle = new SafeFreeCertContext();
				}
			}
			byte* buffer2;
			if (buffer == null || buffer.Length == 0)
			{
				buffer2 = null;
			}
			else
			{
				buffer2 = &buffer[0];
			}
			return SafeFreeContextBuffer.QueryContextAttributes(phContext, attribute, buffer2, refHandle);
		}

		public int SetContextAttributes(SafeDeleteContext phContext, Interop.SspiCli.ContextAttribute attribute, byte[] buffer)
		{
			return SafeFreeContextBuffer.SetContextAttributes(phContext, attribute, buffer);
		}

		public int QuerySecurityContextToken(SafeDeleteContext phContext, out SecurityContextTokenHandle phToken)
		{
			throw new NotSupportedException();
		}

		public int CompleteAuthToken(ref SafeDeleteContext refContext, SecurityBuffer[] inputBuffers)
		{
			throw new NotSupportedException();
		}

		public int ApplyControlToken(ref SafeDeleteContext refContext, SecurityBuffer[] inputBuffers)
		{
			return SafeDeleteContext.ApplyControlToken(ref refContext, inputBuffers);
		}

		private static volatile SecurityPackageInfoClass[] s_securityPackages;
	}
}
