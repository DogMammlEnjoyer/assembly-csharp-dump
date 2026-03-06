using System;
using Fusion.Protocol;

namespace Fusion.Sockets
{
	internal class NetSocketRelay : INetSocket
	{
		public bool SupportsMultiThreading
		{
			get
			{
				return false;
			}
		}

		public NetAddress LocalAddress
		{
			get
			{
				return NetAddress.FromActorId(this._communicator.CommunicatorID);
			}
		}

		public NetSocketRelay(ICommunicator communicator)
		{
			this._communicator = communicator;
		}

		public NetAddress Bind(NetSocket socket, NetConfig config)
		{
			Assert.Check(socket.Handle == this._handle);
			return this.LocalAddress;
		}

		public bool CanFragment(NetAddress address)
		{
			return true;
		}

		public NetSocket Create(NetConfig config)
		{
			return new NetSocket
			{
				Handle = this._handle
			};
		}

		public void Destroy(NetSocket netSocket)
		{
			this._handle = 0L;
		}

		public void DeleteEncryptionKey(NetAddress address)
		{
		}

		public void SetupEncryption(byte[] key, byte[] encryptedKey)
		{
		}

		public void Initialize(NetConfig config)
		{
			this._handle = (long)Environment.TickCount;
		}

		public bool Poll(NetSocket socket, long timeout)
		{
			Assert.Check(this._communicator != null);
			Assert.Check(this._handle != 0L);
			Assert.Check(this._handle == socket.Handle);
			return this._communicator.Poll();
		}

		public unsafe int Receive(NetSocket socket, NetAddress* address, byte* buffer, int bufferLength)
		{
			Assert.Check(this._communicator != null);
			Assert.Check(this._handle != 0L);
			Assert.Check(this._handle == socket.Handle);
			int actorId;
			int num = this._communicator.ReceivePackage(out actorId, buffer, bufferLength);
			bool flag = num > 0;
			if (flag)
			{
				*address = NetAddress.FromActorId(actorId);
			}
			return num;
		}

		public unsafe int Send(NetSocket socket, NetAddress* address, byte* buffer, int bufferLength, bool reliable = false)
		{
			Assert.Check(this._communicator != null);
			Assert.Check(this._handle != 0L);
			Assert.Check(this._handle == socket.Handle);
			bool flag = address->IsRelayAddr && this._communicator != null && this._communicator.SendPackage(101, address->ActorId, reliable, buffer, bufferLength);
			int result;
			if (flag)
			{
				result = bufferLength;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private long _handle;

		private readonly ICommunicator _communicator;
	}
}
