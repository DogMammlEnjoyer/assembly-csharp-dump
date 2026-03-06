using System;

namespace Meta.WitAi.Data
{
	[Serializable]
	public class AudioEncoding
	{
		public override string ToString()
		{
			return string.Format("audio/raw;bits={0};rate={1}k;encoding={2};endian={3}", new object[]
			{
				this.bits,
				this.samplerate / 1000,
				this.encoding,
				this.endian.ToString().ToLower()
			});
		}

		public int numChannels = 1;

		public int samplerate = 16000;

		public string encoding = "signed-integer";

		public const string ENCODING_SIGNED = "signed-integer";

		public const string ENCODING_UNSIGNED = "unsigned-integer";

		public int bits = 16;

		public const int BITS_BYTE = 8;

		public const int BITS_SHORT = 16;

		public const int BITS_INT = 32;

		public const int BITS_LONG = 64;

		public AudioEncoding.Endian endian = AudioEncoding.Endian.Little;

		public enum Endian
		{
			Big,
			Little
		}
	}
}
