using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.Core.Cosmetics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck.Cosmetics
{
	[Preserve]
	public class LckCosmeticsManager : ILckCosmeticsManager, IDisposable
	{
		[Preserve]
		public LckCosmeticsManager(ILckCosmeticsCoordinator cosmeticsCoordinator)
		{
			this._cosmeticsCoordinator = cosmeticsCoordinator;
			this._cosmeticsCoordinator.OnCosmeticAvailable += this.HandleCosmeticAvailable;
			LckLog.Log("LCK: LckCosmeticsManager initialized.");
		}

		public void RegisterDependant(ILckCosmeticDependant dependant)
		{
			string cosmeticType = dependant.GetCosmeticType();
			if (string.IsNullOrEmpty(cosmeticType))
			{
				return;
			}
			List<ILckCosmeticDependant> list;
			if (!this._dependantRegistry.TryGetValue(cosmeticType, out list))
			{
				list = new List<ILckCosmeticDependant>();
				this._dependantRegistry[cosmeticType] = list;
			}
			if (!list.Contains(dependant))
			{
				list.Add(dependant);
			}
			LckLog.Log(string.Concat(new string[]
			{
				"LCK: RegisterDependant of type ",
				cosmeticType,
				" for player ",
				dependant.PlayerId,
				". "
			}));
			this.CheckAndApplyCachedCosmetics(dependant);
		}

		public void UnregisterDependant(ILckCosmeticDependant dependant)
		{
			string cosmeticType = dependant.GetCosmeticType();
			List<ILckCosmeticDependant> list;
			if (string.IsNullOrEmpty(cosmeticType) || !this._dependantRegistry.TryGetValue(cosmeticType, out list))
			{
				return;
			}
			LckLog.Log(string.Concat(new string[]
			{
				"LCK: UnregisterDependant of type ",
				cosmeticType,
				" for player ",
				dependant.PlayerId,
				"."
			}));
			list.Remove(dependant);
		}

		public void Dispose()
		{
			if (this._cosmeticsCoordinator != null)
			{
				this._cosmeticsCoordinator.OnCosmeticAvailable -= this.HandleCosmeticAvailable;
			}
			foreach (AssetBundle assetBundle in from bundle in this._loadedAssetBundles.Values
			where bundle != null
			select bundle)
			{
				assetBundle.Unload(true);
			}
			this._loadedAssetBundles.Clear();
			LckLog.Log("LCK: LckCosmeticsManager disposed.");
		}

		private void HandleCosmeticAvailable(LckAvailableCosmeticInfo incomingCosmeticInfo)
		{
			LckCosmeticsManager.<HandleCosmeticAvailable>d__11 <HandleCosmeticAvailable>d__;
			<HandleCosmeticAvailable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleCosmeticAvailable>d__.<>4__this = this;
			<HandleCosmeticAvailable>d__.incomingCosmeticInfo = incomingCosmeticInfo;
			<HandleCosmeticAvailable>d__.<>1__state = -1;
			<HandleCosmeticAvailable>d__.<>t__builder.Start<LckCosmeticsManager.<HandleCosmeticAvailable>d__11>(ref <HandleCosmeticAvailable>d__);
		}

		private void UpdateAvailableCosmeticsCache(LckAvailableCosmeticInfo cosmeticInfo)
		{
			Dictionary<string, LckAvailableCosmeticInfo> availableCosmeticsCache = this._availableCosmeticsCache;
			lock (availableCosmeticsCache)
			{
				string cosmeticId = cosmeticInfo.CosmeticInfo.CosmeticId;
				LckAvailableCosmeticInfo lckAvailableCosmeticInfo;
				if (this._availableCosmeticsCache.TryGetValue(cosmeticId, out lckAvailableCosmeticInfo))
				{
					HashSet<string> hashSet = new HashSet<string>(lckAvailableCosmeticInfo.PlayerIds);
					hashSet.UnionWith(cosmeticInfo.PlayerIds);
					lckAvailableCosmeticInfo.PlayerIds = hashSet.ToArray<string>();
					this._availableCosmeticsCache[cosmeticId] = lckAvailableCosmeticInfo;
				}
				else
				{
					this._availableCosmeticsCache[cosmeticId] = new LckAvailableCosmeticInfo
					{
						CosmeticInfo = cosmeticInfo.CosmeticInfo,
						PlayerIds = cosmeticInfo.PlayerIds.ToArray<string>()
					};
				}
			}
		}

		private Task<List<Object>> LoadCosmetic(LckCosmeticInfo cosmeticInfo)
		{
			LckCosmeticsManager.<LoadCosmetic>d__13 <LoadCosmetic>d__;
			<LoadCosmetic>d__.<>t__builder = AsyncTaskMethodBuilder<List<Object>>.Create();
			<LoadCosmetic>d__.<>4__this = this;
			<LoadCosmetic>d__.cosmeticInfo = cosmeticInfo;
			<LoadCosmetic>d__.<>1__state = -1;
			<LoadCosmetic>d__.<>t__builder.Start<LckCosmeticsManager.<LoadCosmetic>d__13>(ref <LoadCosmetic>d__);
			return <LoadCosmetic>d__.<>t__builder.Task;
		}

		private Task<List<Object>> LoadRootsFromAssetBundleAsync(LckCosmeticInfo cosmeticInfo)
		{
			LckCosmeticsManager.<LoadRootsFromAssetBundleAsync>d__14 <LoadRootsFromAssetBundleAsync>d__;
			<LoadRootsFromAssetBundleAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<Object>>.Create();
			<LoadRootsFromAssetBundleAsync>d__.<>4__this = this;
			<LoadRootsFromAssetBundleAsync>d__.cosmeticInfo = cosmeticInfo;
			<LoadRootsFromAssetBundleAsync>d__.<>1__state = -1;
			<LoadRootsFromAssetBundleAsync>d__.<>t__builder.Start<LckCosmeticsManager.<LoadRootsFromAssetBundleAsync>d__14>(ref <LoadRootsFromAssetBundleAsync>d__);
			return <LoadRootsFromAssetBundleAsync>d__.<>t__builder.Task;
		}

		private void DistributeLoadedCosmetic(LckAvailableCosmeticInfo cosmeticInfo, List<Object> assets)
		{
			object obj;
			if (cosmeticInfo.CosmeticInfo.CosmeticMetadata.TryGetValue("CosmeticType", out obj))
			{
				string text = obj.ToString();
				if (text != null)
				{
					List<ILckCosmeticDependant> list;
					if (this._dependantRegistry.TryGetValue(text, out list))
					{
						HashSet<string> entitledPlayerIds = new HashSet<string>(cosmeticInfo.PlayerIds);
						IEnumerable<ILckCosmeticDependant> source = list;
						Func<ILckCosmeticDependant, bool> predicate;
						Func<ILckCosmeticDependant, bool> <>9__0;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = ((ILckCosmeticDependant d) => entitledPlayerIds.Contains(d.PlayerId)));
						}
						foreach (ILckCosmeticDependant lckCosmeticDependant in source.Where(predicate))
						{
							lckCosmeticDependant.OnCosmeticLoaded(assets);
						}
					}
					return;
				}
			}
			LckLog.LogError("LCK: Failed to find CosmeticType in metadata of " + cosmeticInfo.CosmeticInfo.CosmeticId + ".");
		}

		private void CheckAndApplyCachedCosmetics(ILckCosmeticDependant dependant)
		{
			string cosmeticType = dependant.GetCosmeticType();
			string playerId = dependant.PlayerId;
			Dictionary<string, LckAvailableCosmeticInfo> availableCosmeticsCache = this._availableCosmeticsCache;
			lock (availableCosmeticsCache)
			{
				foreach (LckAvailableCosmeticInfo lckAvailableCosmeticInfo in this._availableCosmeticsCache.Values)
				{
					LckCosmeticInfo cosmeticInfo = lckAvailableCosmeticInfo.CosmeticInfo;
					object obj;
					List<Object> assets;
					if (cosmeticInfo.CosmeticMetadata.TryGetValue("CosmeticType", out obj) && !(obj.ToString() != cosmeticType) && lckAvailableCosmeticInfo.PlayerIds.Contains(playerId) && this._loadedCosmeticCache.TryGetValue(cosmeticInfo.CosmeticId, out assets))
					{
						dependant.OnCosmeticLoaded(assets);
					}
				}
			}
		}

		private List<LckCosmeticsManager.CosmeticRootInfo> ParseCosmeticRoots(IReadOnlyDictionary<string, object> metadata)
		{
			object obj;
			if (!metadata.TryGetValue("CosmeticRoots", out obj))
			{
				LckLog.LogWarning("LCK: Cosmetic metadata does not include top level CosmeticRoots key");
				return null;
			}
			IList<object> list = obj as IList<object>;
			if (list == null)
			{
				LckLog.LogWarning(string.Format("LCK: Cosmetic metadata has invalid CosmeticRoots - unexpected type: {0}", obj.GetType()));
				return null;
			}
			List<LckCosmeticsManager.CosmeticRootInfo> list2 = new List<LckCosmeticsManager.CosmeticRootInfo>();
			foreach (object obj2 in list)
			{
				IReadOnlyDictionary<object, object> readOnlyDictionary = obj2 as IReadOnlyDictionary<object, object>;
				if (readOnlyDictionary == null)
				{
					LckLog.LogWarning(string.Format("LCK: Cosmetic metadata has invalid CosmeticRoots item - unexpected type: {0}", obj2.GetType()));
				}
				else
				{
					object obj3;
					if (readOnlyDictionary.TryGetValue("rootPath", out obj3))
					{
						string text = obj3 as string;
						object obj4;
						if (text != null && readOnlyDictionary.TryGetValue("type", out obj4))
						{
							string text2 = obj4 as string;
							if (text2 != null)
							{
								list2.Add(new LckCosmeticsManager.CosmeticRootInfo
								{
									RootPath = text,
									Type = text2
								});
								continue;
							}
						}
					}
					LckLog.LogWarning("LCK: Cosmetic metadata has invalid CosmeticRoots item - missing rootPath / type");
				}
			}
			return list2;
		}

		private Type ResolveType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			type = Type.GetType("UnityEngine." + typeName + ", UnityEngine.CoreModule");
			if (type != null)
			{
				return type;
			}
			type = Type.GetType("UnityEngine." + typeName + ", UnityEngine");
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType("UnityEngine." + typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		private readonly ILckCosmeticsCoordinator _cosmeticsCoordinator;

		private readonly Dictionary<string, List<ILckCosmeticDependant>> _dependantRegistry = new Dictionary<string, List<ILckCosmeticDependant>>();

		private readonly Dictionary<string, LckAvailableCosmeticInfo> _availableCosmeticsCache = new Dictionary<string, LckAvailableCosmeticInfo>();

		private readonly Dictionary<string, List<Object>> _loadedCosmeticCache = new Dictionary<string, List<Object>>();

		private readonly Dictionary<string, Task<List<Object>>> _loadingTasks = new Dictionary<string, Task<List<Object>>>();

		private readonly Dictionary<string, AssetBundle> _loadedAssetBundles = new Dictionary<string, AssetBundle>();

		private struct CosmeticRootInfo
		{
			public string RootPath;

			public string Type;
		}
	}
}
