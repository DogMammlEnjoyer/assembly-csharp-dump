using System;
using System.Runtime.InteropServices;

namespace Oculus.Platform
{
	public sealed class Packet : IDisposable
	{
		public Packet(IntPtr packetHandle)
		{
			this.packetHandle = packetHandle;
			this.size = (ulong)CAPI.ovr_Packet_GetSize(packetHandle);
		}

		public ulong ReadBytes(byte[] destination)
		{
			if ((long)destination.Length < (long)this.size)
			{
				throw new ArgumentException(string.Format("Destination array was not big enough to hold {0} bytes", this.size));
			}
			Marshal.Copy(CAPI.ovr_Packet_GetBytes(this.packetHandle), destination, 0, (int)this.size);
			return this.size;
		}

		public ulong SenderID
		{
			get
			{
				return CAPI.ovr_Packet_GetSenderID(this.packetHandle);
			}
		}

		public ulong Size
		{
			get
			{
				return this.size;
			}
		}

		~Packet()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			CAPI.ovr_Packet_Free(this.packetHandle);
			GC.SuppressFinalize(this);
		}

		private readonly ulong size;

		private readonly IntPtr packetHandle;
	}
}
