using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Modio.API;
using Modio.API.SchemaDefinitions;
using Modio.Mods;

namespace Modio.Caching
{
	internal static class ModCache
	{
		internal static Mod GetMod(ModId modId)
		{
			Mod result;
			if (!ModCache.Mods.TryGetValue(modId, out result))
			{
				return ModCache.Mods[modId] = new Mod(modId);
			}
			return result;
		}

		internal static Mod GetMod(ModObject modObject)
		{
			Mod mod;
			if (!ModCache.Mods.TryGetValue(modObject.Id, out mod))
			{
				return ModCache.Mods[modObject.Id] = new Mod(modObject);
			}
			return mod.ApplyDetailsFromModObject(modObject);
		}

		internal static bool TryGetMod(ModId modId, out Mod mod)
		{
			return ModCache.Mods.TryGetValue(modId, out mod);
		}

		static ModCache()
		{
			ModioClient.OnShutdown += ModCache.Clear;
		}

		public static void Clear()
		{
			ModCache.Mods.Clear();
			ModCache.ModSearches.Clear();
			ModCache.SearchesNotInCache = 0;
			ModCache.SearchesSavedByCache = 0;
		}

		public static void RemoveModFromCache(ModId modId)
		{
			ModCache.Mods.Remove(modId);
		}

		internal static bool GetCachedModSearch(ModioAPI.Mods.GetModsFilter filter, string searchKey, out Mod[] cachedMods, out long resultTotal)
		{
			ModCache.ModQueryCachedResponse modQueryCachedResponse;
			if (ModCache.ModSearches.TryGetValue(searchKey, out modQueryCachedResponse) && modQueryCachedResponse.Results.TryGetValue((long)filter.PageIndex, out cachedMods))
			{
				resultTotal = modQueryCachedResponse.ResultTotal;
				ModCache.SearchesSavedByCache++;
				return true;
			}
			cachedMods = null;
			resultTotal = 0L;
			ModCache.SearchesNotInCache++;
			return false;
		}

		internal static void CacheModSearch(string searchKey, Mod[] mods, long pageIndex, long resultTotal)
		{
			ModCache.ModQueryCachedResponse modQueryCachedResponse;
			if (!ModCache.ModSearches.TryGetValue(searchKey, out modQueryCachedResponse))
			{
				modQueryCachedResponse = (ModCache.ModSearches[searchKey] = new ModCache.ModQueryCachedResponse());
			}
			modQueryCachedResponse.AddResults(mods, pageIndex, resultTotal);
		}

		internal static void RemoveCachedModSearch(string searchKey)
		{
			ModCache.ModSearches.Remove(searchKey);
		}

		internal static void ClearModSearchCache()
		{
			ModCache.ModSearches.Clear();
			ModCache.SearchesNotInCache = 0;
			ModCache.SearchesSavedByCache = 0;
		}

		internal static string ConstructFilterKey(ModioAPI.Mods.GetModsFilter filter)
		{
			ModCache.StringBuilder.Clear();
			ModCache.StringBuilder.Append("pageSize:");
			ModCache.StringBuilder.Append(filter.PageSize);
			ModCache.StringBuilder.Append(",index:");
			ModCache.StringBuilder.Append(filter.PageIndex);
			foreach (KeyValuePair<string, object> keyValuePair in filter.Parameters)
			{
				if (!(keyValuePair.Value is string))
				{
					IEnumerable enumerable = keyValuePair.Value as IEnumerable;
					if (enumerable != null)
					{
						ModCache.StringBuilder.AppendFormat(",{0}:[", keyValuePair.Key);
						bool flag = true;
						foreach (object value in enumerable)
						{
							if (!flag)
							{
								ModCache.StringBuilder.Append(',');
							}
							flag = false;
							ModCache.StringBuilder.Append(value);
						}
						ModCache.StringBuilder.Append(']');
						continue;
					}
				}
				ModCache.StringBuilder.AppendFormat(",{0}:{1}", keyValuePair.Key, keyValuePair.Value);
			}
			string result = ModCache.StringBuilder.ToString();
			ModCache.StringBuilder.Clear();
			return result;
		}

		public static Mod CreateHiddenModFromCachedIndexData(ModId modId, ModIndex tempIndex = null)
		{
			return ModCache.GetMod(ModInstallationManagement.GetHiddenModObjectFromIndex(modId, tempIndex));
		}

		public static Task<Error> RefreshPotentiallyHiddenCachedMods()
		{
			ModCache.<RefreshPotentiallyHiddenCachedMods>d__18 <RefreshPotentiallyHiddenCachedMods>d__;
			<RefreshPotentiallyHiddenCachedMods>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<RefreshPotentiallyHiddenCachedMods>d__.<>1__state = -1;
			<RefreshPotentiallyHiddenCachedMods>d__.<>t__builder.Start<ModCache.<RefreshPotentiallyHiddenCachedMods>d__18>(ref <RefreshPotentiallyHiddenCachedMods>d__);
			return <RefreshPotentiallyHiddenCachedMods>d__.<>t__builder.Task;
		}

		private static readonly Dictionary<ModId, Mod> Mods = new Dictionary<ModId, Mod>();

		private static readonly Dictionary<string, ModCache.ModQueryCachedResponse> ModSearches = new Dictionary<string, ModCache.ModQueryCachedResponse>();

		internal static int SearchesNotInCache;

		internal static int SearchesSavedByCache;

		private static readonly StringBuilder StringBuilder = new StringBuilder();

		private class ModQueryCachedResponse
		{
			internal long ResultTotal { get; private set; }

			public void AddResults(Mod[] mods, long pageIndex, long resultTotal)
			{
				this.ResultTotal = resultTotal;
				this.Results[pageIndex] = mods;
			}

			internal readonly Dictionary<long, Mod[]> Results = new Dictionary<long, Mod[]>();
		}
	}
}
