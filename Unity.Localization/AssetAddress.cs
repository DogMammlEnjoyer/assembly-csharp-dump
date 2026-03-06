using System;

namespace UnityEngine.Localization
{
	internal static class AssetAddress
	{
		public static bool IsSubAsset(string address)
		{
			return address != null && address.EndsWith("]");
		}

		public static string GetGuid(string address)
		{
			if (!AssetAddress.IsSubAsset(address))
			{
				return address;
			}
			int length = address.IndexOf("[");
			return address.Substring(0, length);
		}

		public static string GetSubAssetName(string address)
		{
			if (!AssetAddress.IsSubAsset(address))
			{
				return null;
			}
			int num = address.IndexOf("[");
			int length = address.Length - num - 2;
			return address.Substring(num + 1, length);
		}

		public static string FormatAddress(string guid, string subAssetName)
		{
			return guid + "[" + subAssetName + "]";
		}

		private const string k_SubAssetEntryStartBracket = "[";

		private const string k_SubAssetEntryEndBracket = "]";
	}
}
