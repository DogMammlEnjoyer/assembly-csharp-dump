using System;
using System.Linq;

namespace Fusion.Encryption
{
	internal class EncryptionToken
	{
		public override string ToString()
		{
			string[] array = new string[5];
			array[0] = "[EncryptionToken: Key=";
			int num = 1;
			byte[] key = this.Key;
			array[num] = BinUtils.BytesToHex((key != null) ? key.Take(5).ToArray<byte>() : null, 16);
			array[2] = ", KeyEncrypted=";
			int num2 = 3;
			byte[] keyEncrypted = this.KeyEncrypted;
			array[num2] = BinUtils.BytesToHex((keyEncrypted != null) ? keyEncrypted.Take(5).ToArray<byte>() : null, 16);
			array[4] = "]";
			return string.Concat(array);
		}

		public byte[] Key;

		public byte[] KeyEncrypted;
	}
}
