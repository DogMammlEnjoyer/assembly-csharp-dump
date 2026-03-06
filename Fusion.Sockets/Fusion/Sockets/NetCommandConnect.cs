using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct NetCommandConnect
	{
		public static int ClampTokenLength(int tokenLength)
		{
			bool flag = tokenLength < 0;
			if (flag)
			{
				LogStream logWarn = InternalLogStreams.LogWarn;
				if (logWarn != null)
				{
					logWarn.Log("Connection token length can't be negative");
				}
			}
			bool flag2 = tokenLength > 128;
			if (flag2)
			{
				LogStream logWarn2 = InternalLogStreams.LogWarn;
				if (logWarn2 != null)
				{
					logWarn2.Log(string.Format("Connection token length to large, truncated to {0} bytes.", 128));
				}
			}
			return Maths.Clamp(tokenLength, 0, 128);
		}

		public unsafe static byte[] GetTokenDataAsArray(NetCommandConnect command)
		{
			int num = NetCommandConnect.ClampTokenLength(command.TokenLength);
			bool flag = num == 0;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				byte[] array = new byte[num];
				byte[] array2;
				byte* destination;
				if ((array2 = array) == null || array2.Length == 0)
				{
					destination = null;
				}
				else
				{
					destination = &array2[0];
				}
				Native.MemCpy((void*)destination, (void*)(&command.TokenData.FixedElementField), num);
				array2 = null;
				result = array;
			}
			return result;
		}

		public unsafe static byte[] GetUniqueIdAsArray(NetCommandConnect command)
		{
			byte[] array = new byte[8];
			byte[] array2;
			byte* destination;
			if ((array2 = array) == null || array2.Length == 0)
			{
				destination = null;
			}
			else
			{
				destination = &array2[0];
			}
			Native.MemCpy((void*)destination, (void*)(&command.UniqueId.FixedElementField), 8);
			array2 = null;
			return array;
		}

		public unsafe static NetCommandConnect Create(NetConnectionId id, byte* token = null, int tokenLength = 0, byte* uniqueId = null)
		{
			tokenLength = NetCommandConnect.ClampTokenLength(tokenLength);
			NetCommandConnect netCommandConnect = new NetCommandConnect
			{
				Header = NetCommands.Connect,
				ConnectionId = id,
				TokenLength = tokenLength
			};
			bool flag = netCommandConnect.TokenLength > 0;
			if (flag)
			{
				Assert.Check(token != null);
				Native.MemCpy((void*)(&netCommandConnect.TokenData.FixedElementField), (void*)token, netCommandConnect.TokenLength);
			}
			bool flag2 = uniqueId != null;
			if (flag2)
			{
				Native.MemCpy((void*)(&netCommandConnect.UniqueId.FixedElementField), (void*)uniqueId, 8);
			}
			return netCommandConnect;
		}

		public const int TOKEN_MAX_LENGTH_BYTES = 128;

		public const int UNIQUE_ID_LENGTH_BYTES = 8;

		public const int SIZE_BYTES = 152;

		public const int SIZE_BITS = 1216;

		[FieldOffset(0)]
		public NetCommandHeader Header;

		[FieldOffset(4)]
		public int TokenLength;

		[FieldOffset(8)]
		public NetConnectionId ConnectionId;

		[FixedBuffer(typeof(byte), 128)]
		[FieldOffset(16)]
		public NetCommandConnect.<TokenData>e__FixedBuffer TokenData;

		[FixedBuffer(typeof(byte), 8)]
		[FieldOffset(144)]
		public NetCommandConnect.<UniqueId>e__FixedBuffer UniqueId;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct <TokenData>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		public struct <UniqueId>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
