using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	internal static class SerializationUtils
	{
		internal static string SerializeToString<T>(T obj)
		{
			if (obj == null)
			{
				return null;
			}
			DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractSerializer.WriteObject(memoryStream, obj);
				result = Convert.ToBase64String(SerializationUtils.Compress(memoryStream.ToArray()));
			}
			return result;
		}

		internal static T DeserializeFromString<T>(string base64)
		{
			byte[] array = SerializationUtils.Decompress(Convert.FromBase64String(base64));
			T result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				XmlObjectSerializer xmlObjectSerializer = new DataContractSerializer(typeof(T));
				memoryStream.Write(array, 0, array.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				result = (T)((object)xmlObjectSerializer.ReadObject(memoryStream));
			}
			return result;
		}

		private static byte[] Compress(byte[] data)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
				{
					deflateStream.Write(data, 0, data.Length);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		private static byte[] Decompress(byte[] data)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (DeflateStream deflateStream = new DeflateStream(new MemoryStream(data), CompressionMode.Decompress))
				{
					deflateStream.CopyTo(memoryStream);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}
	}
}
