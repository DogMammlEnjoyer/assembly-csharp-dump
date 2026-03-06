using System;
using System.Collections.Generic;

namespace Fusion.Protocol
{
	internal class ProtocolSerializer
	{
		public ProtocolSerializer()
		{
			this.RegisterProtocolMsg(1, new Join());
			this.RegisterProtocolMsg(2, new NetworkConfigSync());
			this.RegisterProtocolMsg(3, new ReflexiveInfo());
			this.RegisterProtocolMsg(4, new Disconnect());
			this.RegisterProtocolMsg(5, new Start());
			this.RegisterProtocolMsg(6, new Snapshot());
			this.RegisterProtocolMsg(7, new HostMigration());
			this.RegisterProtocolMsg(8, new PlayerRefMapping());
			this.RegisterProtocolMsg(9, new ChangeMasterClient());
			this.RegisterProtocolMsg(10, new DummyTrafficSync());
		}

		public bool ConvertToMessages(byte[] data, List<Message> messages)
		{
			Assert.Check(data != null, "Data buffer can't be null to convert Messages");
			try
			{
				this._readStream.SetBuffer(data, data.Length);
				this._readStream.Reading = true;
				messages.Clear();
				for (;;)
				{
					Message item;
					bool flag = this.ReadNext(this._readStream, out item);
					if (!flag)
					{
						break;
					}
					messages.Add(item);
				}
				return messages.Count > 0;
			}
			catch (Exception message)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Error(message);
				}
			}
			return false;
		}

		public bool ConvertToBuffer(Message message, out BitStream buffer)
		{
			try
			{
				this._writeStream.Reset();
				this._writeStream.Writing = true;
				bool flag = this.PackNext(message, this._writeStream);
				if (flag)
				{
					buffer = this._writeStream;
					return true;
				}
			}
			catch (IndexOutOfRangeException)
			{
				this._writeStream.Expand();
			}
			catch (Exception message2)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Error(message2);
				}
			}
			buffer = null;
			return false;
		}

		private void RegisterProtocolMsg(byte id, Message message)
		{
			this._idToType.Add(id, message);
			this._typeToId.Add(message.GetType(), id);
		}

		private bool PackNext(Message msg, BitStream stream)
		{
			int position = stream.Position;
			Assert.Check(stream.Writing);
			Assert.Check<Type>(this._typeToId.ContainsKey(msg.GetType()), "Message {0} not registered", msg.GetType());
			stream.WriteByte(this._typeToId[msg.GetType()]);
			msg.Serialize(stream);
			bool overflowing = stream.Overflowing;
			bool result;
			if (overflowing)
			{
				stream.Position = position;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		private bool ReadNext(BitStream stream, out Message msg)
		{
			bool result;
			try
			{
				Assert.Check(stream.Reading);
				bool flag = !stream.CanRead(8);
				if (flag)
				{
					msg = null;
					result = false;
				}
				else
				{
					byte key = stream.ReadByte();
					msg = this._idToType[key].Clone();
					msg.Serialize(stream);
					result = true;
				}
			}
			catch
			{
				msg = null;
				result = false;
			}
			return result;
		}

		private readonly BitStream _writeStream = new BitStream(new byte[8192]);

		private readonly BitStream _readStream = new BitStream(new byte[0]);

		private readonly Dictionary<Type, byte> _typeToId = new Dictionary<Type, byte>();

		private readonly Dictionary<byte, Message> _idToType = new Dictionary<byte, Message>();
	}
}
