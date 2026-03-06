using System;

namespace SouthPointe.Serialization.MessagePack
{
	internal static class ArrayHelper
	{
		internal static void AdjustSize(ref byte[] bytes, int length)
		{
			if (bytes.Length < length)
			{
				int i;
				for (i = bytes.Length; i < length; i *= 2)
				{
				}
				Array.Resize<byte>(ref bytes, i);
			}
		}
	}
}
