using System;
using System.IO;
using System.IO.Compression;

namespace Fusion
{
	internal static class CompressionUtils
	{
		public static byte[] Compress(byte[] data)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					gzipStream.Write(data, 0, data.Length);
					gzipStream.Close();
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		public static byte[] Decompress(byte[] data)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						gzipStream.CopyTo(memoryStream2);
						result = memoryStream2.ToArray();
					}
				}
			}
			return result;
		}

		public unsafe static void SnapshotCompress(int* Current, int* Previous, int* Result, int totalLenght, out int count)
		{
			count = 0;
			for (int i = 0; i < totalLenght; i++)
			{
				bool flag = Current[i] != Previous[i];
				if (flag)
				{
					int num = count;
					count = num + 1;
					Result[num] = i;
					num = count;
					count = num + 1;
					Result[num] = Current[i];
				}
			}
		}

		public unsafe static void SnapshotDecompress(int* previous, int* delta, int length)
		{
			int i = 0;
			while (i < length)
			{
				int num = delta[i++];
				int num2 = delta[i++];
				previous[num] = num2;
			}
		}
	}
}
