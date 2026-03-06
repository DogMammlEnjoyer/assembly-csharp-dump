using System;
using SouthPointe.Serialization.MessagePack;
using UnityEngine.Scripting;

namespace Liv.Lck.Core.Serialization
{
	[Preserve]
	internal class LckMsgPackSerializer : ILckSerializer
	{
		public SerializationType SerializationType
		{
			get
			{
				return SerializationType.MsgPack;
			}
		}

		[Preserve]
		public LckMsgPackSerializer()
		{
		}

		public byte[] Serialize(object data)
		{
			return this._formatter.Serialize<object>(data);
		}

		public T Deserialize<T>(byte[] data)
		{
			return this._formatter.Deserialize<T>(data);
		}

		private readonly MessagePackFormatter _formatter = new MessagePackFormatter(null);
	}
}
