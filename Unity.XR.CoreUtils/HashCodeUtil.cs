using System;

namespace Unity.XR.CoreUtils
{
	internal static class HashCodeUtil
	{
		public static int Combine(int hash1, int hash2)
		{
			return hash1 * 486187739 + hash2;
		}

		public static int ReferenceHash(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetHashCode();
		}

		public static int Combine(int hash1, int hash2, int hash3)
		{
			return HashCodeUtil.Combine(HashCodeUtil.Combine(hash1, hash2), hash3);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4)
		{
			return HashCodeUtil.Combine(HashCodeUtil.Combine(hash1, hash2, hash3), hash4);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4, int hash5)
		{
			return HashCodeUtil.Combine(HashCodeUtil.Combine(hash1, hash2, hash3, hash4), hash5);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6)
		{
			return HashCodeUtil.Combine(HashCodeUtil.Combine(hash1, hash2, hash3, hash4, hash5), hash6);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7)
		{
			return HashCodeUtil.Combine(HashCodeUtil.Combine(hash1, hash2, hash3, hash4, hash5, hash6), hash7);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8)
		{
			return HashCodeUtil.Combine(HashCodeUtil.Combine(hash1, hash2, hash3, hash4, hash5, hash6, hash7), hash8);
		}
	}
}
