using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
	[ScriptHelp]
	[FusionGlobalScriptableObject("Assets/Photon/Fusion/Resources/NetworkProjectConfig.fusion", DefaultContentsGeneratorMethod = "GenerateDefaultContents")]
	public class NetworkProjectConfigAsset : FusionGlobalScriptableObject<NetworkProjectConfigAsset>
	{
		public static NetworkProjectConfigAsset Global
		{
			get
			{
				return FusionGlobalScriptableObject<NetworkProjectConfigAsset>.GlobalInternal;
			}
		}

		public static bool TryGetGlobal(out NetworkProjectConfigAsset global)
		{
			return FusionGlobalScriptableObject<NetworkProjectConfigAsset>.TryGetGlobalInternal(out global);
		}

		public static bool IsGlobalLoaded
		{
			get
			{
				return FusionGlobalScriptableObject<NetworkProjectConfigAsset>.IsGlobalLoadedInternal;
			}
		}

		public static void UnloadGlobal()
		{
			FusionGlobalScriptableObject<NetworkProjectConfigAsset>.UnloadGlobalInternal();
		}

		private void OnEnable()
		{
			this.Config.PrefabTable.Clear();
			this.Config.ExecutionOrderOverrides.Clear();
			NetworkPrefabTable prefabTable = this.Config.PrefabTable;
			foreach (INetworkPrefabSource networkPrefabSource in this.Prefabs)
			{
				NetworkPrefabId networkPrefabId;
				bool flag = prefabTable.TryAddSource(networkPrefabSource, out networkPrefabId);
				if (!flag)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(string.Format("Failed to add prefab asset {0}, there is already a prefab entry with same guid", networkPrefabSource.AssetGuid));
					}
				}
			}
			prefabTable.Options = this.PrefabOptions;
			foreach (NetworkProjectConfigAsset.SerializableSimulationBehaviourMeta serializableSimulationBehaviourMeta in this.BehaviourMeta)
			{
				Type type = serializableSimulationBehaviourMeta.Type;
				bool flag2 = type == null;
				if (flag2)
				{
					LogStream logError2 = InternalLogStreams.LogError;
					if (logError2 != null)
					{
						logError2.Log(string.Format("Failed to resolve type: {0}", serializableSimulationBehaviourMeta.Type));
					}
				}
				else
				{
					this.Config.ExecutionOrderOverrides.Add(type, serializableSimulationBehaviourMeta.ExecutionOrder);
				}
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			NetworkPrefabTable prefabTable = this.Config.PrefabTable;
			if (prefabTable != null)
			{
				prefabTable.Clear();
			}
			this.Config.ExecutionOrderOverrides.Clear();
		}

		private static string GenerateDefaultContents()
		{
			return JsonUtility.ToJson(new NetworkProjectConfig(), true);
		}

		[SerializeField]
		[DrawInline]
		public NetworkProjectConfig Config = new NetworkProjectConfig();

		[ResolveNetworkPrefabSource]
		[SerializeReference]
		[HideArrayElementLabel]
		[InlineHelp]
		public List<INetworkPrefabSource> Prefabs = new List<INetworkPrefabSource>();

		public NetworkPrefabTableOptions PrefabOptions = NetworkPrefabTableOptions.Default;

		[ReadOnly]
		[InlineHelp]
		[SerializeField]
		public NetworkProjectConfigAsset.SerializableSimulationBehaviourMeta[] BehaviourMeta = Array.Empty<NetworkProjectConfigAsset.SerializableSimulationBehaviourMeta>();

		[Serializable]
		public struct SerializableSimulationBehaviourMeta
		{
			public SerializableType<SimulationBehaviour> Type;

			public int ExecutionOrder;
		}
	}
}
