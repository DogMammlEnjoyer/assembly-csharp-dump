using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct NetCommandDisconnect
	{
		public unsafe static NetCommandDisconnect Create(NetDisconnectReason reason, byte[] token)
		{
			int num = Math.Min(128, ((token != null) ? new int?(token.Length) : null).GetValueOrDefault());
			NetCommandDisconnect result = new NetCommandDisconnect
			{
				Header = NetCommands.Disconnect,
				Reason = reason,
				TokenLength = num
			};
			for (int i = 0; i < num; i++)
			{
				*(ref result.TokenData.FixedElementField + i) = token[i];
			}
			return result;
		}

		public unsafe static NetCommandDisconnect Create(NetDisconnectReason reason, byte* token, int tokenLength)
		{
			tokenLength = Math.Min(128, tokenLength);
			NetCommandDisconnect result = new NetCommandDisconnect
			{
				Header = NetCommands.Disconnect,
				Reason = reason,
				TokenLength = tokenLength
			};
			for (int i = 0; i < tokenLength; i++)
			{
				*(ref result.TokenData.FixedElementField + i) = token[i];
			}
			return result;
		}

		public const int TOKEN_MAX_LENGTH_BYTES = 128;

		[FieldOffset(0)]
		public NetCommandHeader Header;

		[FieldOffset(2)]
		public NetDisconnectReason Reason;

		[FieldOffset(4)]
		public int TokenLength;

		[FixedBuffer(typeof(byte), 128)]
		[FieldOffset(8)]
		public NetCommandDisconnect.<TokenData>e__FixedBuffer TokenData;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct <TokenData>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
