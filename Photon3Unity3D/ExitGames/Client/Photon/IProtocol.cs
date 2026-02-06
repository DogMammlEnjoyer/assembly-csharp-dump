using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public abstract class IProtocol
	{
		public abstract string ProtocolType { get; }

		public abstract byte[] VersionBytes { get; }

		public abstract void Serialize(StreamBuffer dout, object serObject, bool setType);

		public abstract void SerializeShort(StreamBuffer dout, short serObject, bool setType);

		public abstract void SerializeString(StreamBuffer dout, string serObject, bool setType);

		public abstract void SerializeEventData(StreamBuffer stream, EventData serObject, bool setType);

		[Obsolete("Use ParameterDictionary instead.")]
		public abstract void SerializeOperationRequest(StreamBuffer stream, byte operationCode, Dictionary<byte, object> parameters, bool setType);

		public abstract void SerializeOperationRequest(StreamBuffer stream, byte operationCode, ParameterDictionary parameters, bool setType);

		public abstract void SerializeOperationResponse(StreamBuffer stream, OperationResponse serObject, bool setType);

		public abstract object Deserialize(StreamBuffer din, byte type, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None);

		public abstract short DeserializeShort(StreamBuffer din);

		public abstract byte DeserializeByte(StreamBuffer din);

		public abstract EventData DeserializeEventData(StreamBuffer din, EventData target = null, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None);

		public abstract OperationRequest DeserializeOperationRequest(StreamBuffer din, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None);

		public abstract OperationResponse DeserializeOperationResponse(StreamBuffer stream, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None);

		public abstract DisconnectMessage DeserializeDisconnectMessage(StreamBuffer stream);

		public byte[] Serialize(object obj)
		{
			StreamBuffer streamBuffer = new StreamBuffer(64);
			this.Serialize(streamBuffer, obj, true);
			return streamBuffer.ToArray();
		}

		public object Deserialize(StreamBuffer stream)
		{
			return this.Deserialize(stream, stream.ReadByte(), IProtocol.DeserializationFlags.None);
		}

		public object Deserialize(byte[] serializedData)
		{
			StreamBuffer streamBuffer = new StreamBuffer(serializedData);
			return this.Deserialize(streamBuffer, streamBuffer.ReadByte(), IProtocol.DeserializationFlags.None);
		}

		public object DeserializeMessage(StreamBuffer stream)
		{
			return this.Deserialize(stream, stream.ReadByte(), IProtocol.DeserializationFlags.None);
		}

		internal void SerializeMessage(StreamBuffer ms, object msg)
		{
			this.Serialize(ms, msg, true);
		}

		public readonly ByteArraySlicePool ByteArraySlicePool = new ByteArraySlicePool();

		public enum DeserializationFlags
		{
			None,
			AllowPooledByteArray,
			WrapIncomingStructs
		}
	}
}
