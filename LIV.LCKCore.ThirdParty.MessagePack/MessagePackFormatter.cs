using System;
using System.IO;

namespace SouthPointe.Serialization.MessagePack
{
	public class MessagePackFormatter
	{
		public SerializationContext Context { get; set; }

		public MessagePackFormatter(SerializationContext context = null)
		{
			this.Context = (context ?? SerializationContext.Default);
		}

		public T Deserialize<T>(byte[] bytes)
		{
			return (T)((object)this.Deserialize(typeof(T), bytes));
		}

		public T Deserialize<T>(Stream stream)
		{
			return (T)((object)this.Deserialize(typeof(T), stream));
		}

		public object Deserialize(Type type, byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
			{
				return null;
			}
			return this.Deserialize(type, new MemoryStream(bytes));
		}

		public object Deserialize(Type type, Stream stream)
		{
			object result;
			try
			{
				FormatReader formatReader = new FormatReader(stream);
				result = this.Context.TypeHandlers.Get(type).Read(formatReader.ReadFormat(), formatReader);
			}
			catch (FormatException ex)
			{
				if (stream.CanSeek)
				{
					MemoryStream memoryStream = new MemoryStream((int)stream.Position);
					byte[] array = new byte[16384];
					stream.Position = 0L;
					int num;
					for (int i = memoryStream.Capacity; i > 0; i -= num)
					{
						num = stream.Read(array, 0, array.Length);
						memoryStream.Write(array, 0, num);
					}
					memoryStream.Position = 0L;
					ex.Source = JsonConverter.Encode(memoryStream, null);
					memoryStream.Close();
				}
				throw;
			}
			return result;
		}

		public byte[] Serialize<T>(T obj)
		{
			return this.Serialize(typeof(T), obj);
		}

		public byte[] Serialize(Type type, object obj)
		{
			MemoryStream memoryStream = new MemoryStream();
			this.Serialize(memoryStream, type, obj);
			return memoryStream.ToArray();
		}

		public void Serialize<T>(Stream stream, T obj)
		{
			Type type = (obj != null) ? obj.GetType() : typeof(T);
			this.Serialize(stream, type, obj);
		}

		public void Serialize(Stream stream, Type type, object obj)
		{
			this.Context.TypeHandlers.Get(type).Write(obj, new FormatWriter(stream));
		}

		public string AsJson(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return null;
			}
			return this.AsJson(new MemoryStream(data));
		}

		public string AsJson(Stream stream)
		{
			return JsonConverter.Encode(stream, this.Context);
		}
	}
}
