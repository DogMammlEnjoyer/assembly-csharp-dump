using System;
using System.Text;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.WitAi;
using Meta.WitAi.Json;
using UnityEngine;

namespace Meta.Voice.Net.Encoding.Wit
{
	[LogCategory(LogCategory.Encoding)]
	public class WitChunkConverter : ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Encoding, null);

		private bool IsHeaderDecoded
		{
			get
			{
				return this._headerDecoded >= 17;
			}
		}

		private bool IsJsonDecoded
		{
			get
			{
				return this._jsonDecoded >= this._currentChunk.header.jsonLength;
			}
		}

		private bool IsBinaryDecoded
		{
			get
			{
				return this._binaryDecoded >= this._currentChunk.header.binaryLength;
			}
		}

		private void ResetChunk()
		{
			this._headerDecoded = 0;
			this._jsonDecoded = 0;
			this._jsonBuilder.Clear();
			this._binaryDecoded = 0UL;
			this._currentChunk.jsonString = null;
			this._currentChunk.jsonData = null;
			this._currentChunk.binaryData = null;
		}

		public void Decode(byte[] buffer, int bufferOffset, int bufferLength, Action<WitChunk> onChunkDecoded, Action<byte[], int, int> customBinaryDecoder = null)
		{
			while (bufferLength > 0)
			{
				int num = this.DecodeChunk(buffer, bufferOffset, bufferLength, onChunkDecoded, customBinaryDecoder);
				bufferOffset += num;
				bufferLength -= num;
			}
		}

		private int DecodeChunk(byte[] buffer, int bufferOffset, int bufferLength, Action<WitChunk> onChunkDecoded, Action<byte[], int, int> customBinaryDecoder)
		{
			int num = 0;
			if (!this.IsHeaderDecoded)
			{
				num = this.DecodeHeader(buffer, bufferOffset, bufferLength);
				if (!this.IsHeaderDecoded)
				{
					return num;
				}
				bufferOffset += num;
				bufferLength -= num;
				if (this._currentChunk.header.invalid)
				{
					this.Logger.Error("WitChunk Header Decode Failed: Header is invalid\nHeader: {0}", new object[]
					{
						WitRequestSettings.GetByteString(this._headerBytes, 0, 17)
					});
					this.ResetChunk();
					return num;
				}
				if (customBinaryDecoder == null)
				{
					byte[] binaryData = this._currentChunk.binaryData;
					int num2 = (binaryData != null) ? binaryData.Length : 0;
					int num3 = (int)this._currentChunk.header.binaryLength;
					if (num2 != num3)
					{
						this._currentChunk.binaryData = new byte[num3];
					}
				}
			}
			if (!this.IsJsonDecoded)
			{
				int num4 = this.DecodeJson(buffer, bufferOffset, bufferLength);
				num += num4;
				bufferOffset += num4;
				bufferLength -= num4;
				if (this.IsJsonDecoded && customBinaryDecoder != null && onChunkDecoded != null)
				{
					onChunkDecoded(this._currentChunk);
				}
			}
			if (!this.IsBinaryDecoded)
			{
				int num5 = this.DecodeBinary(buffer, bufferOffset, bufferLength, customBinaryDecoder);
				num += num5;
			}
			if (this.IsJsonDecoded && this.IsBinaryDecoded)
			{
				if (customBinaryDecoder == null && onChunkDecoded != null)
				{
					onChunkDecoded(this._currentChunk);
				}
				this.ResetChunk();
			}
			return num;
		}

		private int DecodeHeader(byte[] buffer, int bufferOffset, int bufferLength)
		{
			int headerDecoded = this._headerDecoded;
			int b = 17 - this._headerDecoded;
			int num = Mathf.Min(bufferLength, b);
			Array.Copy(buffer, bufferOffset, this._headerBytes, headerDecoded, num);
			this._headerDecoded += num;
			if (this.IsHeaderDecoded)
			{
				this._currentChunk.header = WitChunkConverter.GetHeader(this._headerBytes, 0);
			}
			return num;
		}

		private int DecodeJson(byte[] buffer, int bufferOffset, int bufferLength)
		{
			int jsonDecoded = this._jsonDecoded;
			int b = this._currentChunk.header.jsonLength - jsonDecoded;
			int num = Mathf.Min(bufferLength, b);
			if (num <= 0)
			{
				return 0;
			}
			string value = WitChunkConverter.DecodeString(buffer, bufferOffset, num);
			this._jsonBuilder.Append(value);
			this._jsonDecoded += num;
			if (this.IsJsonDecoded)
			{
				string jsonString = this._jsonBuilder.ToString();
				this._currentChunk.jsonString = jsonString;
				this._currentChunk.jsonData = JsonConvert.DeserializeToken(jsonString);
			}
			return num;
		}

		private int DecodeBinary(byte[] buffer, int bufferOffset, int bufferLength, Action<byte[], int, int> customBinaryDecoder)
		{
			ulong binaryDecoded = this._binaryDecoded;
			ulong num = this._currentChunk.header.binaryLength - binaryDecoded;
			int num2 = Mathf.Min(bufferLength, (int)num);
			if (num2 <= 0)
			{
				return 0;
			}
			if (customBinaryDecoder != null)
			{
				customBinaryDecoder(buffer, bufferOffset, num2);
			}
			else if (this._currentChunk.binaryData != null)
			{
				Array.Copy(buffer, bufferOffset, this._currentChunk.binaryData, (int)this._binaryDecoded, num2);
			}
			this._binaryDecoded += (ulong)((long)num2);
			return num2;
		}

		public static string DecodeString(byte[] rawData, int offset, int length)
		{
			return WitChunkConverter.TextEncoding.GetString(rawData, offset, length);
		}

		public static byte[] Encode(WitChunk chunkData)
		{
			if (string.IsNullOrEmpty(chunkData.jsonString))
			{
				WitResponseNode jsonData = chunkData.jsonData;
				chunkData.jsonString = ((jsonData != null) ? jsonData.ToString() : null);
			}
			return WitChunkConverter.Encode(chunkData.jsonString, chunkData.binaryData);
		}

		public static byte[] Encode(byte[] binaryData)
		{
			return WitChunkConverter.Encode(null, binaryData);
		}

		public static byte[] Encode(WitResponseNode jsonToken, byte[] binaryData = null)
		{
			return WitChunkConverter.Encode((jsonToken != null) ? jsonToken.ToString() : null, binaryData);
		}

		public static byte[] Encode(string jsonString, byte[] binaryData = null)
		{
			return WitChunkConverter.Encode(WitChunkConverter.EncodeString(jsonString), binaryData);
		}

		public static byte[] Encode(byte[] jsonData, byte[] binaryData)
		{
			int num = (jsonData != null) ? jsonData.Length : 0;
			int num2 = (binaryData != null) ? binaryData.Length : 0;
			byte[] array = new byte[17 + num + num2];
			array[0] = WitChunkConverter.EncodeFlag(num > 0, num2 > 0);
			int num3 = 1;
			WitChunkConverter.EncodeLength(array, ref num3, (long)num);
			WitChunkConverter.EncodeLength(array, ref num3, (long)num2);
			WitChunkConverter.EncodeBytes(array, ref num3, jsonData);
			WitChunkConverter.EncodeBytes(array, ref num3, binaryData);
			return array;
		}

		public static byte[] EncodeString(string stringData)
		{
			if (!string.IsNullOrEmpty(stringData))
			{
				return WitChunkConverter.TextEncoding.GetBytes(stringData);
			}
			return null;
		}

		private static byte EncodeFlag(bool hasJson, bool hasBinary)
		{
			if (hasJson)
			{
				if (!hasBinary)
				{
					return 2;
				}
				return 3;
			}
			else
			{
				if (!hasBinary)
				{
					return 0;
				}
				return 1;
			}
		}

		private static void EncodeLength(byte[] destination, ref int offset, long length)
		{
			byte[] bytes = BitConverter.GetBytes(length);
			WitChunkConverter.EncodeBytes(destination, ref offset, bytes);
		}

		private static void EncodeBytes(byte[] destination, ref int offset, byte[] source)
		{
			if (source == null || source.Length == 0)
			{
				return;
			}
			Array.Copy(source, 0, destination, offset, source.Length);
			offset += source.Length;
		}

		private static WitChunkHeader GetHeader(byte[] bytes, int offset)
		{
			WitChunkHeader result = default(WitChunkHeader);
			byte b = bytes[offset];
			bool flag = (b & 1) > 0;
			bool flag2 = (WitChunkConverter.SafeShift(b, 1) & 1) != 0;
			result.invalid = ((WitChunkConverter.SafeShift(b, 2) & 63) != 0);
			long num = BitConverter.ToInt64(bytes, offset + 1);
			result.jsonLength = (int)num;
			result.invalid |= (num < 0L && flag2);
			result.invalid |= (num > 0L && !flag2);
			long num2 = BitConverter.ToInt64(bytes, offset + 1 + 8);
			result.binaryLength = (ulong)num2;
			result.invalid |= (num2 < 0L && flag);
			result.invalid |= (num2 > 0L && !flag);
			return result;
		}

		private static int SafeShift(byte flags, int index)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return (int)flags << index;
			}
			return flags >> index;
		}

		private static readonly UTF8Encoding TextEncoding = new UTF8Encoding();

		private WitChunk _currentChunk;

		private int _headerDecoded;

		private byte[] _headerBytes = new byte[17];

		private int _jsonDecoded;

		private StringBuilder _jsonBuilder = new StringBuilder();

		private ulong _binaryDecoded;

		private const int FLAG_SIZE = 1;

		private const int LONG_SIZE = 8;

		private const int HEADER_SIZE = 17;

		private const byte FLAG_NO_JSON_NO_BINARY = 0;

		private const byte FLAG_NO_JSON_YES_BINARY = 1;

		private const byte FLAG_YES_JSON_NO_BINARY = 2;

		private const byte FLAG_YES_JSON_YES_BINARY = 3;
	}
}
