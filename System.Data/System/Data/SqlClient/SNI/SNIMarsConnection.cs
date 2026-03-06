using System;
using System.Collections.Generic;

namespace System.Data.SqlClient.SNI
{
	internal class SNIMarsConnection
	{
		public Guid ConnectionId
		{
			get
			{
				return this._connectionId;
			}
		}

		public SNIMarsConnection(SNIHandle lowerHandle)
		{
			this._lowerHandle = lowerHandle;
			this._lowerHandle.SetAsyncCallbacks(new SNIAsyncCallback(this.HandleReceiveComplete), new SNIAsyncCallback(this.HandleSendComplete));
		}

		public SNIMarsHandle CreateMarsSession(object callbackObject, bool async)
		{
			SNIMarsHandle result;
			lock (this)
			{
				ushort nextSessionId = this._nextSessionId;
				this._nextSessionId = nextSessionId + 1;
				ushort num = nextSessionId;
				SNIMarsHandle snimarsHandle = new SNIMarsHandle(this, num, callbackObject, async);
				this._sessions.Add((int)num, snimarsHandle);
				result = snimarsHandle;
			}
			return result;
		}

		public uint StartReceive()
		{
			SNIPacket snipacket = null;
			if (this.ReceiveAsync(ref snipacket) == 997U)
			{
				return 997U;
			}
			return SNICommon.ReportSNIError(SNIProviders.SMUX_PROV, 0U, 19U, string.Empty);
		}

		public uint Send(SNIPacket packet)
		{
			uint result;
			lock (this)
			{
				result = this._lowerHandle.Send(packet);
			}
			return result;
		}

		public uint SendAsync(SNIPacket packet, SNIAsyncCallback callback)
		{
			uint result;
			lock (this)
			{
				result = this._lowerHandle.SendAsync(packet, false, callback);
			}
			return result;
		}

		public uint ReceiveAsync(ref SNIPacket packet)
		{
			uint result;
			lock (this)
			{
				result = this._lowerHandle.ReceiveAsync(ref packet);
			}
			return result;
		}

		public uint CheckConnection()
		{
			uint result;
			lock (this)
			{
				result = this._lowerHandle.CheckConnection();
			}
			return result;
		}

		public void HandleReceiveError(SNIPacket packet)
		{
			foreach (SNIMarsHandle snimarsHandle in this._sessions.Values)
			{
				snimarsHandle.HandleReceiveError(packet);
			}
		}

		public void HandleSendComplete(SNIPacket packet, uint sniErrorCode)
		{
			packet.InvokeCompletionCallback(sniErrorCode);
		}

		public void HandleReceiveComplete(SNIPacket packet, uint sniErrorCode)
		{
			SNISMUXHeader snismuxheader = null;
			SNIPacket packet2 = null;
			SNIMarsHandle snimarsHandle = null;
			if (sniErrorCode != 0U)
			{
				SNIMarsConnection obj = this;
				lock (obj)
				{
					this.HandleReceiveError(packet);
					return;
				}
			}
			for (;;)
			{
				SNIMarsConnection obj = this;
				lock (obj)
				{
					if (this._currentHeaderByteCount != 16)
					{
						snismuxheader = null;
						packet2 = null;
						snimarsHandle = null;
						while (this._currentHeaderByteCount != 16)
						{
							int num = packet.TakeData(this._headerBytes, this._currentHeaderByteCount, 16 - this._currentHeaderByteCount);
							this._currentHeaderByteCount += num;
							if (num == 0)
							{
								sniErrorCode = this.ReceiveAsync(ref packet);
								if (sniErrorCode == 997U)
								{
									return;
								}
								this.HandleReceiveError(packet);
								return;
							}
						}
						this._currentHeader = new SNISMUXHeader
						{
							SMID = this._headerBytes[0],
							flags = this._headerBytes[1],
							sessionId = BitConverter.ToUInt16(this._headerBytes, 2),
							length = BitConverter.ToUInt32(this._headerBytes, 4) - 16U,
							sequenceNumber = BitConverter.ToUInt32(this._headerBytes, 8),
							highwater = BitConverter.ToUInt32(this._headerBytes, 12)
						};
						this._dataBytesLeft = (int)this._currentHeader.length;
						this._currentPacket = new SNIPacket((int)this._currentHeader.length);
					}
					snismuxheader = this._currentHeader;
					packet2 = this._currentPacket;
					if (this._currentHeader.flags == 8 && this._dataBytesLeft > 0)
					{
						int num2 = packet.TakeData(this._currentPacket, this._dataBytesLeft);
						this._dataBytesLeft -= num2;
						if (this._dataBytesLeft > 0)
						{
							sniErrorCode = this.ReceiveAsync(ref packet);
							if (sniErrorCode == 997U)
							{
								break;
							}
							this.HandleReceiveError(packet);
							break;
						}
					}
					this._currentHeaderByteCount = 0;
					if (!this._sessions.ContainsKey((int)this._currentHeader.sessionId))
					{
						SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.SMUX_PROV, 0U, 5U, string.Empty);
						this.HandleReceiveError(packet);
						this._lowerHandle.Dispose();
						this._lowerHandle = null;
						break;
					}
					if (this._currentHeader.flags == 4)
					{
						this._sessions.Remove((int)this._currentHeader.sessionId);
					}
					else
					{
						snimarsHandle = this._sessions[(int)this._currentHeader.sessionId];
					}
				}
				if (snismuxheader.flags == 8)
				{
					snimarsHandle.HandleReceiveComplete(packet2, snismuxheader);
				}
				if (this._currentHeader.flags == 2)
				{
					try
					{
						snimarsHandle.HandleAck(snismuxheader.highwater);
					}
					catch (Exception sniException)
					{
						SNICommon.ReportSNIError(SNIProviders.SMUX_PROV, 35U, sniException);
					}
				}
				obj = this;
				lock (obj)
				{
					if (packet.DataLeft != 0)
					{
						continue;
					}
					sniErrorCode = this.ReceiveAsync(ref packet);
					if (sniErrorCode != 997U)
					{
						this.HandleReceiveError(packet);
					}
				}
				break;
			}
		}

		public uint EnableSsl(uint options)
		{
			return this._lowerHandle.EnableSsl(options);
		}

		public void DisableSsl()
		{
			this._lowerHandle.DisableSsl();
		}

		private readonly Guid _connectionId = Guid.NewGuid();

		private readonly Dictionary<int, SNIMarsHandle> _sessions = new Dictionary<int, SNIMarsHandle>();

		private readonly byte[] _headerBytes = new byte[16];

		private SNIHandle _lowerHandle;

		private ushort _nextSessionId;

		private int _currentHeaderByteCount;

		private int _dataBytesLeft;

		private SNISMUXHeader _currentHeader;

		private SNIPacket _currentPacket;
	}
}
