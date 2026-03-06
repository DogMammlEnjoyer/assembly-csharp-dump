using System;
using Fusion.Protocol;

namespace Fusion.Sockets
{
	internal class NetSocketHybrid : INetSocket
	{
		public bool SupportsMultiThreading
		{
			get
			{
				return false;
			}
		}

		public NetSocketHybrid(ICommunicator client)
		{
			this._client = client;
			this._relaySocket = new NetSocketRelay(this._client);
			this._nativeSocket = new NetSocketNative();
		}

		public void Initialize(NetConfig config)
		{
			this._relaySocket.Initialize(config);
			this._nativeSocket.Initialize(config);
		}

		public NetSocket Create(NetConfig config)
		{
			this._relayNetSocketRef = this._relaySocket.Create(config);
			return this._nativeSocket.Create(config);
		}

		public void Destroy(NetSocket netSocket)
		{
			this._relaySocket.Destroy(this._relayNetSocketRef);
			this._nativeSocket.Destroy(netSocket);
		}

		public void DeleteEncryptionKey(NetAddress address)
		{
			this._relaySocket.DeleteEncryptionKey(address);
			this._nativeSocket.DeleteEncryptionKey(address);
		}

		public void SetupEncryption(byte[] key, byte[] encryptedKey)
		{
			this._relaySocket.SetupEncryption(key, encryptedKey);
			this._nativeSocket.SetupEncryption(key, encryptedKey);
		}

		public NetAddress Bind(NetSocket socket, NetConfig config)
		{
			this._relayAddress = this._relaySocket.Bind(this._relayNetSocketRef, config);
			return this._nativeSocket.Bind(socket, config);
		}

		public bool CanFragment(NetAddress address)
		{
			return address.IsRelayAddr ? this._relaySocket.CanFragment(address) : this._nativeSocket.CanFragment(address);
		}

		public bool Poll(NetSocket socket, long timeout)
		{
			return this._relaySocket.Poll(this._relayNetSocketRef, timeout) || this._nativeSocket.Poll(socket, timeout);
		}

		public unsafe int Receive(NetSocket socket, NetAddress* address, byte* buffer, int bufferLength)
		{
			Assert.Check(address->Equals(default(NetAddress)), address->ToString());
			int num = this._nativeSocket.Receive(socket, address, buffer, bufferLength);
			bool flag = num < 0;
			if (flag)
			{
				num = this._relaySocket.Receive(this._relayNetSocketRef, address, buffer, bufferLength);
			}
			return num;
		}

		public unsafe int Send(NetSocket socket, NetAddress* address, byte* buffer, int bufferLength, bool reliable = false)
		{
			bool isRelayAddr = address->IsRelayAddr;
			int result;
			if (isRelayAddr)
			{
				result = this._relaySocket.Send(this._relayNetSocketRef, address, buffer, bufferLength, reliable);
			}
			else
			{
				result = this._nativeSocket.Send(socket, address, buffer, bufferLength, reliable);
			}
			return result;
		}

		private NetSocket _relayNetSocketRef;

		private NetAddress _relayAddress;

		private readonly NetSocketRelay _relaySocket;

		private readonly NetSocketNative _nativeSocket;

		private readonly ICommunicator _client;
	}
}
