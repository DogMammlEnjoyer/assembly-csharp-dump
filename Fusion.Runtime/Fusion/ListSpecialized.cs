using System;
using System.Collections.Generic;

namespace Fusion
{
	internal static class ListSpecialized
	{
		public static int BinarySearchSpecialized(this List<NetworkId> list, NetworkId value)
		{
			int i = 0;
			int num = list.Count - 1;
			while (i <= num)
			{
				int num2 = i + (num - i >> 1);
				NetworkId networkId = list[num2];
				int num3 = (int)(networkId.Raw - value.Raw);
				bool flag = num3 == 0;
				if (flag)
				{
					return num2;
				}
				bool flag2 = num3 < 0;
				if (flag2)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return ~i;
		}

		public static bool AddUnique(this List<NetworkId> list, NetworkId value)
		{
			int num = list.BinarySearchSpecialized(value);
			bool flag = num >= 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				list.Insert(~num, value);
				result = true;
			}
			return result;
		}

		public static bool RemoveUnique(this List<NetworkId> list, NetworkId value)
		{
			int num = list.BinarySearchSpecialized(value);
			bool flag = num < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				list.RemoveAt(num);
				result = true;
			}
			return result;
		}
	}
}
