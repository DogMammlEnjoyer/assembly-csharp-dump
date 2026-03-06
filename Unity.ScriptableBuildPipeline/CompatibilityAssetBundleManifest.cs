using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.Build.Pipeline
{
	[Serializable]
	public class CompatibilityAssetBundleManifest : ScriptableObject, ISerializationCallbackReceiver
	{
		public void SetResults(Dictionary<string, BundleDetails> results)
		{
			this.m_Details = new Dictionary<string, BundleDetails>(results);
		}

		public string[] GetAllAssetBundles()
		{
			string[] array = this.m_Details.Keys.ToArray<string>();
			Array.Sort<string>(array);
			return array;
		}

		public string[] GetAllAssetBundlesWithVariant()
		{
			return new string[0];
		}

		public Hash128 GetAssetBundleHash(string assetBundleName)
		{
			BundleDetails bundleDetails;
			if (this.m_Details.TryGetValue(assetBundleName, out bundleDetails))
			{
				return bundleDetails.Hash;
			}
			return default(Hash128);
		}

		public uint GetAssetBundleCrc(string assetBundleName)
		{
			BundleDetails bundleDetails;
			if (this.m_Details.TryGetValue(assetBundleName, out bundleDetails))
			{
				return bundleDetails.Crc;
			}
			return 0U;
		}

		public string[] GetDirectDependencies(string assetBundleName)
		{
			return this.GetAllDependencies(assetBundleName);
		}

		public string[] GetAllDependencies(string assetBundleName)
		{
			BundleDetails bundleDetails;
			if (this.m_Details.TryGetValue(assetBundleName, out bundleDetails))
			{
				return bundleDetails.Dependencies.ToArray<string>();
			}
			return new string[0];
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ManifestFileVersion: 1\n");
			stringBuilder.Append("CompatibilityAssetBundleManifest:\n");
			if (this.m_Details != null && this.m_Details.Count > 0)
			{
				stringBuilder.Append("  AssetBundleInfos:\n");
				int num = 0;
				using (Dictionary<string, BundleDetails>.Enumerator enumerator = this.m_Details.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, BundleDetails> keyValuePair = enumerator.Current;
						stringBuilder.AppendFormat("    Info_{0}:\n", num++);
						stringBuilder.AppendFormat("      Name: {0}\n", keyValuePair.Key);
						stringBuilder.AppendFormat("      Hash: {0}\n", keyValuePair.Value.Hash);
						stringBuilder.AppendFormat("      CRC: {0}\n", keyValuePair.Value.Crc);
						int num2 = 0;
						if (keyValuePair.Value.Dependencies != null && keyValuePair.Value.Dependencies.Length != 0)
						{
							stringBuilder.Append("      Dependencies: {}\n");
							foreach (string arg in keyValuePair.Value.Dependencies)
							{
								stringBuilder.AppendFormat("        Dependency_{0}: {1}\n", num2++, arg);
							}
						}
						else
						{
							stringBuilder.Append("      Dependencies: {}\n");
						}
					}
					goto IL_17C;
				}
			}
			stringBuilder.Append("  AssetBundleInfos: {}\n");
			IL_17C:
			return stringBuilder.ToString();
		}

		public void OnBeforeSerialize()
		{
			this.m_Keys = new List<string>();
			this.m_Values = new List<BundleDetails>();
			foreach (KeyValuePair<string, BundleDetails> keyValuePair in this.m_Details)
			{
				this.m_Keys.Add(keyValuePair.Key);
				this.m_Values.Add(keyValuePair.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			this.m_Details = new Dictionary<string, BundleDetails>();
			for (int num = 0; num != Math.Min(this.m_Keys.Count, this.m_Values.Count); num++)
			{
				this.m_Details.Add(this.m_Keys[num], this.m_Values[num]);
			}
		}

		private Dictionary<string, BundleDetails> m_Details;

		[SerializeField]
		private List<string> m_Keys;

		[SerializeField]
		private List<BundleDetails> m_Values;
	}
}
