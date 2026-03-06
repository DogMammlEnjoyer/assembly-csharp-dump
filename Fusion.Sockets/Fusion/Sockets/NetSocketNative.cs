using System;
using Fusion.Encryption;
using Fusion.Sockets.Stun;
using NanoSockets;

namespace Fusion.Sockets
{
	internal class NetSocketNative : INetSocket
	{
		public bool SupportsMultiThreading
		{
			get
			{
				return true;
			}
		}

		public void Initialize(NetConfig config)
		{
			Assert.Always(UDP.Initialize() == Status.Ok, "Unable to initialize Socket");
		}

		public NetSocket Create(NetConfig config)
		{
			Socket socket = UDP.Create(config.SocketSendBuffer, config.SocketRecvBuffer);
			Assert.Always(socket.IsCreated, "Unable to create Socket");
			Assert.Always(UDP.SetNonBlocking(socket) == Status.Ok, "Unable to set Socket as NonBlocking");
			return new NetSocket
			{
				NativeSocket = socket
			};
		}

		public NetAddress Bind(NetSocket socket, NetConfig config)
		{
			Address nativeAddress = config.Address.NativeAddress;
			bool flag = UDP.Bind(socket.NativeSocket.handle, ref nativeAddress) != 0;
			if (flag)
			{
				UDP.Destroy(ref socket.NativeSocket.handle);
				throw new InvalidOperationException(string.Format("Failed to bind socket to {0}", config.Address.NativeAddress));
			}
			nativeAddress = default(Address);
			bool flag2 = UDP.GetAddress(socket.NativeSocket.handle, ref nativeAddress) > Status.Ok;
			if (flag2)
			{
				UDP.Destroy(ref socket.NativeSocket.handle);
				throw new InvalidOperationException("Failed to resolve address for bound socket");
			}
			nativeAddress._address0 = config.Address.NativeAddress._address0;
			nativeAddress._address1 = config.Address.NativeAddress._address1;
			return new NetAddress
			{
				NativeAddress = nativeAddress
			};
		}

		public bool CanFragment(NetAddress address)
		{
			return true;
		}

		public bool Poll(NetSocket socket, long timeout)
		{
			return UDP.Poll(socket.NativeSocket.handle, timeout) > 0;
		}

		public unsafe int Receive(NetSocket socket, NetAddress* address, byte* buffer, int bufferLength)
		{
			int num = UDP.Receive(socket.NativeSocket.handle, &address->NativeAddress, buffer, bufferLength);
			bool flag = num > 0 && StunMessage.IsStunMessage(buffer, bufferLength);
			int result;
			if (flag)
			{
				StunClient.TryParseAndStoreStunMessage(address, buffer, num);
				result = -1;
			}
			else
			{
				EngineProfiler.Begin("Encryption.Socket.Receive");
				bool flag2 = this.HandleEncryptionIngoing(address, ref buffer, bufferLength, ref num);
				EngineProfiler.End();
				bool flag3 = !flag2;
				if (flag3)
				{
					result = -1;
				}
				else
				{
					result = num;
				}
			}
			return result;
		}

		public unsafe int Send(NetSocket socket, NetAddress* address, byte* buffer, int bufferLength, bool reliable = false)
		{
			EngineProfiler.Begin("Encryption.Socket.Send");
			bool flag = this.HandleEncryptionOutgoing(address, ref buffer, ref bufferLength);
			EngineProfiler.End();
			bool flag2 = !flag;
			int result;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				result = UDP.Send(socket.NativeSocket.handle, &address->NativeAddress, buffer, bufferLength);
			}
			return result;
		}

		public void Destroy(NetSocket netSocket)
		{
			this.ResetEncryption();
			UDP.Destroy(ref netSocket.NativeSocket.handle);
		}

		public void SetupEncryption(byte[] key, byte[] encryptedKey)
		{
			bool flag = this._encryptionManager != null;
			if (flag)
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn("SetupEncryption: already setup, ignoring...");
				}
			}
			else
			{
				bool flag2;
				if (key != null && key.Length != 0)
				{
					flag2 = Array.TrueForAll<byte>(key, (byte b) => b == 0);
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					TraceLogStream logTraceEncryption2 = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption2 != null)
					{
						logTraceEncryption2.Warn("SetupEncryption: no key, ignoring...");
					}
				}
				else
				{
					this._encryptionManager = new EncryptionManager<NetAddress, DataEncryptor>();
					this._encryptionManager.RegisterEncryptionKey(default(NetAddress), key);
					this._encryptionToken = new EncryptionToken
					{
						Key = key,
						KeyEncrypted = encryptedKey
					};
					this._encryptionBuffer = Native.MallocAndClearArray<byte>(2048);
					TraceLogStream logTraceEncryption3 = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption3 != null)
					{
						logTraceEncryption3.Log(string.Format("SetupEncryption: {0}", this._encryptionToken));
					}
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Log("Encryption is enabled");
					}
				}
			}
		}

		private unsafe bool HandleEncryptionOutgoing(NetAddress* address, ref byte* buffer, ref int bufferLength)
		{
			bool flag = this._encryptionManager != null && bufferLength > 1;
			if (flag)
			{
				NetAddress netAddress = *address;
				int num = 0;
				Assert.Check(bufferLength <= 2048, "Buffer is too big");
				Native.MemClear((void*)this._encryptionBuffer, 2048);
				Native.MemCpy((void*)this._encryptionBuffer, buffer, Math.Min(bufferLength, 2048));
				buffer = this._encryptionBuffer;
				bool flag2 = !this._encryptionManager.HasEncryptionForHandle(netAddress);
				if (flag2)
				{
					TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption != null)
					{
						logTraceEncryption.Warn(string.Format("Encryption Handler not found: {0}", netAddress));
					}
					bool flag3 = this._encryptionToken.KeyEncrypted != null;
					if (!flag3)
					{
						TraceLogStream logTraceEncryption2 = InternalLogStreams.LogTraceEncryption;
						if (logTraceEncryption2 != null)
						{
							logTraceEncryption2.Warn("Encryption failed. Invalid encryption handler.");
						}
						return false;
					}
					int num2 = this._encryptionToken.KeyEncrypted.Length;
					Assert.Check(num2 <= 255, "KeyEncrypted is too big");
					num = num2 + 1;
					Native.MemMove(buffer + (IntPtr)num, buffer, bufferLength);
					*buffer = (byte)num2;
					Native.CopyFromArray<byte>(buffer + 1, this._encryptionToken.KeyEncrypted);
					this._remoteEncryptionHandler = netAddress;
					netAddress = default(NetAddress);
					TraceLogStream logTraceEncryption3 = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption3 != null)
					{
						logTraceEncryption3.Log(string.Format("Sending encrypted key: {0}", netAddress));
					}
				}
				bool flag4 = this._encryptionManager.Wrap(netAddress, buffer + (IntPtr)num, ref bufferLength, 2048);
				if (!flag4)
				{
					TraceLogStream logTraceEncryption4 = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption4 != null)
					{
						logTraceEncryption4.Warn("Encryption failed. Unable to wrap data.");
					}
					return false;
				}
				bufferLength += num;
			}
			return true;
		}

		private unsafe bool HandleEncryptionIngoing(NetAddress* address, ref byte* buffer, int bufferLength, ref int received)
		{
			bool flag = this._encryptionManager != null && received > 1;
			if (flag)
			{
				NetAddress netAddress = *address;
				bool flag2 = !this._encryptionManager.HasEncryptionForHandle(netAddress);
				if (flag2)
				{
					TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption != null)
					{
						logTraceEncryption.Warn(string.Format("Encryption Handler not found: {0}/{1}", address->ToString(), netAddress));
					}
					bool flag3 = this._encryptionToken.KeyEncrypted != null;
					if (flag3)
					{
						bool flag4 = this._remoteEncryptionHandler.Equals(netAddress);
						if (!flag4)
						{
							TraceLogStream logTraceEncryption2 = InternalLogStreams.LogTraceEncryption;
							if (logTraceEncryption2 != null)
							{
								logTraceEncryption2.Warn("Encryption failed. Invalid encryption handler.");
							}
							return false;
						}
						this._encryptionManager.RegisterEncryptionKey(netAddress, this._encryptionToken.Key);
						this._remoteEncryptionHandler = default(NetAddress);
						TraceLogStream logTraceEncryption3 = InternalLogStreams.LogTraceEncryption;
						if (logTraceEncryption3 != null)
						{
							logTraceEncryption3.Log(string.Format("Encryption Handler registered: {0}/{1}", address->ToString(), netAddress));
						}
					}
					else
					{
						int num = (int)(*buffer);
						byte* ptr = buffer + 1;
						int num2 = num + 1;
						bool flag5 = this._encryptionManager.Unwrap(default(NetAddress), ptr, ref num, num);
						if (!flag5)
						{
							TraceLogStream logTraceEncryption4 = InternalLogStreams.LogTraceEncryption;
							if (logTraceEncryption4 != null)
							{
								logTraceEncryption4.Warn("Encryption failed. Unable to unwrap keys.");
							}
							return false;
						}
						byte[] array = new byte[num];
						Native.CopyToArray<byte>(array, (void*)ptr);
						this._encryptionManager.RegisterEncryptionKey(netAddress, array);
						received -= num2;
						Native.MemMove(buffer, buffer + (IntPtr)num2, received);
					}
				}
				bool flag6 = !this._encryptionManager.Unwrap(netAddress, buffer, ref received, bufferLength);
				if (flag6)
				{
					TraceLogStream logTraceEncryption5 = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption5 != null)
					{
						logTraceEncryption5.Warn("Encryption failed. Unable to unwrap data.");
					}
					return false;
				}
			}
			return true;
		}

		private void ResetEncryption()
		{
			TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
			if (logTraceEncryption != null)
			{
				logTraceEncryption.Log("ResetEncryption");
			}
			EncryptionManager<NetAddress, DataEncryptor> encryptionManager = this._encryptionManager;
			if (encryptionManager != null)
			{
				encryptionManager.Dispose();
			}
			this._encryptionManager = null;
			this._encryptionToken = null;
			this._remoteEncryptionHandler = default(NetAddress);
			Native.Free<byte>(ref this._encryptionBuffer);
		}

		public void DeleteEncryptionKey(NetAddress address)
		{
			EncryptionManager<NetAddress, DataEncryptor> encryptionManager = this._encryptionManager;
			if (encryptionManager != null)
			{
				encryptionManager.DeleteEncryptionKey(address);
			}
		}

		private EncryptionManager<NetAddress, DataEncryptor> _encryptionManager;

		private EncryptionToken _encryptionToken;

		private NetAddress _remoteEncryptionHandler;

		private unsafe byte* _encryptionBuffer = null;

		private const int EncryptionBufferLength = 2048;
	}
}
