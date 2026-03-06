using System;

namespace Pathfinding.Util
{
	public class Checksum
	{
		public static uint GetChecksum(byte[] arr, uint hash)
		{
			hash ^= 2166136261U;
			for (int i = 0; i < arr.Length; i++)
			{
				hash = (hash ^ (uint)arr[i]) * 16777619U;
			}
			return hash;
		}
	}
}
