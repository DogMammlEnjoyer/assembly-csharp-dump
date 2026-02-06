using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using UnityEngine;

public class CustomObjectProvider : NetworkObjectProviderDefault
{
	private static NetworkObjectBaker Baker
	{
		get
		{
			NetworkObjectBaker result;
			if ((result = CustomObjectProvider.baker) == null)
			{
				result = (CustomObjectProvider.baker = new NetworkObjectBaker());
			}
			return result;
		}
	}

	public override NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
	{
		NetworkObjectAcquireResult networkObjectAcquireResult = base.AcquirePrefabInstance(runner, context, out instance);
		if (networkObjectAcquireResult == NetworkObjectAcquireResult.Success)
		{
			this.IsGameMode(instance);
			return networkObjectAcquireResult;
		}
		instance = null;
		return networkObjectAcquireResult;
	}

	private void IsGameMode(NetworkObject instance)
	{
		if (instance.gameObject.GetComponent<GameModeSerializer>() != null)
		{
			GorillaGameModes.GameMode.GetGameModeInstance(GorillaGameModes.GameMode.GetGameModeKeyFromRoomProp()).AddFusionDataBehaviour(instance);
			CustomObjectProvider.Baker.Bake(instance.gameObject);
		}
	}

	protected override void DestroySceneObject(NetworkRunner runner, NetworkSceneObjectId sceneObjectId, NetworkObject instance)
	{
		if (this.SceneObjects != null && this.SceneObjects.Contains(instance.gameObject))
		{
			return;
		}
		base.DestroySceneObject(runner, sceneObjectId, instance);
	}

	protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
	{
		base.DestroyPrefabInstance(runner, prefabId, instance);
	}

	public const int GameModeFlag = 1;

	public const int PlayerFlag = 2;

	private static NetworkObjectBaker baker;

	internal List<GameObject> SceneObjects;
}
