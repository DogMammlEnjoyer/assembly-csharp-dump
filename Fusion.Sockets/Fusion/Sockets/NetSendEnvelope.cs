using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Fusion.Sockets
{
	public struct NetSendEnvelope
	{
		[return: NotNull]
		public unsafe T* TakeUserData<[IsUnmanaged] T>() where T : struct, ValueType
		{
			Assert.Always<IntPtr>(this.UserData != null, (IntPtr)this.UserData);
			T* userData = (T*)this.UserData;
			this.UserData = null;
			return userData;
		}

		public unsafe void* UserData;

		public double SendTime;

		public ushort Sequence;

		internal NetPacketType PacketType;
	}
}
