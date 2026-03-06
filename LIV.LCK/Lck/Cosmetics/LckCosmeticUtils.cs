using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Liv.Lck.Cosmetics
{
	public static class LckCosmeticUtils
	{
		public static List<LckCosmeticUtils.CosmeticRootInfo> ParseRootsFromMetadata(IReadOnlyDictionary<string, object> metadata)
		{
			object obj;
			if (!metadata.TryGetValue("CosmeticRoots", out obj))
			{
				Debug.LogWarning("LCK: Cosmetic metadata does not include top level CosmeticRoots key");
				return null;
			}
			IList<object> list = obj as IList<object>;
			if (list == null)
			{
				Debug.LogWarning(string.Format("LCK: Cosmetic metadata has invalid CosmeticRoots - unexpected type: {0}", obj.GetType()));
				return null;
			}
			List<LckCosmeticUtils.CosmeticRootInfo> list2 = new List<LckCosmeticUtils.CosmeticRootInfo>();
			foreach (object obj2 in list)
			{
				IReadOnlyDictionary<object, object> readOnlyDictionary = obj2 as IReadOnlyDictionary<object, object>;
				if (readOnlyDictionary == null)
				{
					Debug.LogWarning(string.Format("LCK: Cosmetic metadata has invalid CosmeticRoots item - unexpected type: {0}", obj2.GetType()));
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
								list2.Add(new LckCosmeticUtils.CosmeticRootInfo
								{
									RootPath = text,
									Type = text2
								});
								continue;
							}
						}
					}
					Debug.LogWarning("LCK: Cosmetic metadata has invalid CosmeticRoots item - missing rootPath / type");
				}
			}
			return list2;
		}

		public static List<LckCosmeticUtils.CosmeticRootInfo> ParseRootsFromTomlString(string tomlContent)
		{
			List<LckCosmeticUtils.CosmeticRootInfo> list = new List<LckCosmeticUtils.CosmeticRootInfo>();
			string[] array = tomlContent.Split(new char[]
			{
				'\r',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			string text = null;
			string text2 = null;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text3 = array2[i].Trim();
				if (text3.StartsWith("[[CosmeticRoots]]"))
				{
					text = null;
					text2 = null;
				}
				else if (text3.StartsWith("rootPath"))
				{
					text = text3.Split(new char[]
					{
						'='
					}, 2)[1].Trim().Trim('"');
				}
				else if (text3.StartsWith("type"))
				{
					text2 = text3.Split(new char[]
					{
						'='
					}, 2)[1].Trim().Trim('"');
				}
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
				{
					list.Add(new LckCosmeticUtils.CosmeticRootInfo
					{
						RootPath = text,
						Type = text2
					});
					text = null;
					text2 = null;
				}
			}
			return list;
		}

		public static Task<List<Object>> LoadRootsFromBundleAsync(AssetBundle bundle, List<LckCosmeticUtils.CosmeticRootInfo> rootInfos, string cosmeticIdForLogging)
		{
			LckCosmeticUtils.<LoadRootsFromBundleAsync>d__3 <LoadRootsFromBundleAsync>d__;
			<LoadRootsFromBundleAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<Object>>.Create();
			<LoadRootsFromBundleAsync>d__.bundle = bundle;
			<LoadRootsFromBundleAsync>d__.rootInfos = rootInfos;
			<LoadRootsFromBundleAsync>d__.cosmeticIdForLogging = cosmeticIdForLogging;
			<LoadRootsFromBundleAsync>d__.<>1__state = -1;
			<LoadRootsFromBundleAsync>d__.<>t__builder.Start<LckCosmeticUtils.<LoadRootsFromBundleAsync>d__3>(ref <LoadRootsFromBundleAsync>d__);
			return <LoadRootsFromBundleAsync>d__.<>t__builder.Task;
		}

		public static List<Object> LoadRootsFromBundle(AssetBundle bundle, List<LckCosmeticUtils.CosmeticRootInfo> rootInfos, string cosmeticIdForLogging)
		{
			List<Object> list = new List<Object>();
			foreach (LckCosmeticUtils.CosmeticRootInfo cosmeticRootInfo in rootInfos)
			{
				Type type = LckCosmeticUtils.ResolveType(cosmeticRootInfo.Type);
				if (type == null)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"LCK: Could not resolve type '",
						cosmeticRootInfo.Type,
						"' for asset '",
						cosmeticRootInfo.RootPath,
						"' in cosmetic '",
						cosmeticIdForLogging,
						"'. Skipping."
					}));
				}
				else
				{
					Object @object = bundle.LoadAsset(cosmeticRootInfo.RootPath, type);
					if (@object != null)
					{
						list.Add(@object);
					}
					else
					{
						Debug.LogWarning(string.Concat(new string[]
						{
							"LCK: Failed to load asset at path '",
							cosmeticRootInfo.RootPath,
							"' of type '",
							type.Name,
							"' from bundle for cosmetic '",
							cosmeticIdForLogging,
							"'."
						}));
					}
				}
			}
			return list;
		}

		private static Type ResolveType(string typeName)
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
			Debug.LogWarning("LCK: Could not resolve type '" + typeName + "' in any loaded assembly.");
			return null;
		}

		public struct CosmeticRootInfo
		{
			public string RootPath;

			public string Type;
		}
	}
}
