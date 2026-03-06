using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Fusion
{
	public class CustomNetworkObjectProvider : NetworkObjectProviderDefault
	{
		private static NetworkObjectBaker Baker
		{
			get
			{
				NetworkObjectBaker result;
				if ((result = CustomNetworkObjectProvider._baker) == null)
				{
					result = (CustomNetworkObjectProvider._baker = new NetworkObjectBaker());
				}
				return result;
			}
		}

		public static void RegisterCustomNetworkObject(uint customPrefabID, Func<GameObject> func)
		{
			if (CustomNetworkObjectProvider.CustomSpawnDict.ContainsKey(customPrefabID))
			{
				Debug.LogError(string.Format("The requested customPrefabID {0} already existed, aborting registration", customPrefabID));
			}
			CustomNetworkObjectProvider.CustomSpawnDict[customPrefabID] = func;
		}

		public override NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject result)
		{
			Func<GameObject> func;
			if (CustomNetworkObjectProvider.CustomSpawnDict.TryGetValue(context.PrefabId.RawValue, out func))
			{
				GameObject gameObject = func();
				NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
				if (networkObject == null)
				{
					networkObject = gameObject.AddComponent<NetworkObject>();
				}
				CustomNetworkObjectProvider.Baker.Bake(gameObject);
				if (context.DontDestroyOnLoad)
				{
					runner.MakeDontDestroyOnLoad(gameObject);
				}
				else
				{
					runner.MoveToRunnerScene(gameObject, null);
				}
				result = networkObject;
				return NetworkObjectAcquireResult.Success;
			}
			return base.AcquirePrefabInstance(runner, context, out result);
		}

		private static NetworkObjectBaker _baker;

		private static readonly Dictionary<uint, Func<GameObject>> CustomSpawnDict = new Dictionary<uint, Func<GameObject>>();
	}
}
