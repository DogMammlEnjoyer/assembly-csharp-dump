using System;
using System.Linq;
using Meta.WitAi.Json;

namespace Meta.Voice.Net.Encoding.Wit
{
	public struct WitChunk
	{
		public override bool Equals(object other)
		{
			if (other is WitChunk)
			{
				WitChunk other2 = (WitChunk)other;
				return this.Equals(other2);
			}
			return false;
		}

		private bool Equals(WitChunk other)
		{
			return this.header.Equals(other.header) && this.jsonString == other.jsonString && object.Equals(this.jsonData, other.jsonData) && this.binaryData.SequenceEqual(other.binaryData);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<WitChunkHeader, string, WitResponseNode, byte[]>(this.header, this.jsonString, this.jsonData, this.binaryData);
		}

		public WitChunkHeader header;

		public string jsonString;

		public WitResponseNode jsonData;

		public byte[] binaryData;
	}
}
