using System;

namespace System.Net
{
	internal static class ContextFlagsAdapterPal
	{
		internal static ContextFlagsPal GetContextFlagsPalFromInterop(Interop.SspiCli.ContextFlags win32Flags)
		{
			ContextFlagsPal contextFlagsPal = ContextFlagsPal.None;
			foreach (ContextFlagsAdapterPal.ContextFlagMapping contextFlagMapping in ContextFlagsAdapterPal.s_contextFlagMapping)
			{
				if ((win32Flags & contextFlagMapping.Win32Flag) == contextFlagMapping.Win32Flag)
				{
					contextFlagsPal |= contextFlagMapping.ContextFlag;
				}
			}
			return contextFlagsPal;
		}

		internal static Interop.SspiCli.ContextFlags GetInteropFromContextFlagsPal(ContextFlagsPal flags)
		{
			Interop.SspiCli.ContextFlags contextFlags = Interop.SspiCli.ContextFlags.Zero;
			foreach (ContextFlagsAdapterPal.ContextFlagMapping contextFlagMapping in ContextFlagsAdapterPal.s_contextFlagMapping)
			{
				if ((flags & contextFlagMapping.ContextFlag) == contextFlagMapping.ContextFlag)
				{
					contextFlags |= contextFlagMapping.Win32Flag;
				}
			}
			return contextFlags;
		}

		private static readonly ContextFlagsAdapterPal.ContextFlagMapping[] s_contextFlagMapping = new ContextFlagsAdapterPal.ContextFlagMapping[]
		{
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.AcceptExtendedError, ContextFlagsPal.AcceptExtendedError),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.InitManualCredValidation, ContextFlagsPal.InitManualCredValidation),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.AcceptIntegrity, ContextFlagsPal.AcceptIntegrity),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.AcceptStream, ContextFlagsPal.AcceptStream),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.AllocateMemory, ContextFlagsPal.AllocateMemory),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.AllowMissingBindings, ContextFlagsPal.AllowMissingBindings),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.Confidentiality, ContextFlagsPal.Confidentiality),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.Connection, ContextFlagsPal.Connection),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.Delegate, ContextFlagsPal.Delegate),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.InitExtendedError, ContextFlagsPal.InitExtendedError),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.AcceptIntegrity, ContextFlagsPal.AcceptIntegrity),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.InitManualCredValidation, ContextFlagsPal.InitManualCredValidation),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.AcceptStream, ContextFlagsPal.AcceptStream),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.AcceptExtendedError, ContextFlagsPal.AcceptExtendedError),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.InitUseSuppliedCreds, ContextFlagsPal.InitUseSuppliedCreds),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.MutualAuth, ContextFlagsPal.MutualAuth),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.ProxyBindings, ContextFlagsPal.ProxyBindings),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.ReplayDetect, ContextFlagsPal.ReplayDetect),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.SequenceDetect, ContextFlagsPal.SequenceDetect),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.UnverifiedTargetName, ContextFlagsPal.UnverifiedTargetName),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.UseSessionKey, ContextFlagsPal.UseSessionKey),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.SspiCli.ContextFlags.Zero, ContextFlagsPal.None)
		};

		private readonly struct ContextFlagMapping
		{
			public ContextFlagMapping(Interop.SspiCli.ContextFlags win32Flag, ContextFlagsPal contextFlag)
			{
				this.Win32Flag = win32Flag;
				this.ContextFlag = contextFlag;
			}

			public readonly Interop.SspiCli.ContextFlags Win32Flag;

			public readonly ContextFlagsPal ContextFlag;
		}
	}
}
