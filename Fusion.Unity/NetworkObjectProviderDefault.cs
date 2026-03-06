using System;
using UnityEngine;

namespace Fusion
{
	public class NetworkObjectProviderDefault : Behaviour, INetworkObjectProvider
	{
		public virtual NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
		{
			instance = null;
			if (this.DelayIfSceneManagerIsBusy && runner.SceneManager.IsBusy)
			{
				return NetworkObjectAcquireResult.Retry;
			}
			NetworkObject networkObject;
			try
			{
				networkObject = runner.Prefabs.Load(context.PrefabId, context.IsSynchronous);
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("Failed to load prefab: {0}", arg));
				return NetworkObjectAcquireResult.Failed;
			}
			if (!networkObject)
			{
				return NetworkObjectAcquireResult.Retry;
			}
			instance = this.InstantiatePrefab(runner, networkObject);
			if (context.DontDestroyOnLoad)
			{
				runner.MakeDontDestroyOnLoad(instance.gameObject);
			}
			else
			{
				runner.MoveToRunnerScene(instance.gameObject, null);
			}
			runner.Prefabs.AddInstance(context.PrefabId);
			return NetworkObjectAcquireResult.Success;
		}

		public virtual void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
		{
			NetworkObject @object = context.Object;
			if (!context.IsBeingDestroyed)
			{
				if (context.TypeId.IsPrefab)
				{
					this.DestroyPrefabInstance(runner, context.TypeId.AsPrefabId, @object);
				}
				else if (context.TypeId.IsSceneObject)
				{
					this.DestroySceneObject(runner, context.TypeId.AsSceneObjectId, @object);
				}
				else
				{
					if (!context.IsNestedObject)
					{
						throw new NotImplementedException(string.Format("Unknown type id {0}", context.TypeId));
					}
					this.DestroyPrefabNestedObject(runner, @object);
				}
			}
			if (context.TypeId.IsPrefab)
			{
				runner.Prefabs.RemoveInstance(context.TypeId.AsPrefabId);
			}
		}

		public NetworkPrefabId GetPrefabId(NetworkRunner runner, NetworkObjectGuid prefabGuid)
		{
			return runner.Prefabs.GetId(prefabGuid);
		}

		protected virtual NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
		{
			return Object.Instantiate<NetworkObject>(prefab);
		}

		protected virtual void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
		{
			Object.Destroy(instance.gameObject);
		}

		protected virtual void DestroyPrefabNestedObject(NetworkRunner runner, NetworkObject instance)
		{
			Object.Destroy(instance.gameObject);
		}

		protected virtual void DestroySceneObject(NetworkRunner runner, NetworkSceneObjectId sceneObjectId, NetworkObject instance)
		{
			Object.Destroy(instance.gameObject);
		}

		[InlineHelp]
		public bool DelayIfSceneManagerIsBusy = true;
	}
}
