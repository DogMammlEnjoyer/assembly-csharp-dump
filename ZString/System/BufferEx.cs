using System;

namespace System
{
	internal static class BufferEx
	{
		internal unsafe static void ZeroMemory(byte* dest, uint len)
		{
			if (len == 0U)
			{
				return;
			}
			int num = 0;
			while ((long)num < (long)((ulong)len))
			{
				dest[num] = 0;
				num++;
			}
		}

		internal unsafe static void Memcpy(byte* dest, byte* src, int len)
		{
			if (len == 0)
			{
				return;
			}
			for (int i = 0; i < len; i++)
			{
				dest[i] = src[i];
			}
		}
	}
}
