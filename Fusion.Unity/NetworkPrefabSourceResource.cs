using System;

namespace Fusion
{
	[Serializable]
	public class NetworkPrefabSourceResource : NetworkAssetSourceResource<NetworkObject>, INetworkPrefabSource, INetworkAssetSource<NetworkObject>
	{
		NetworkObjectGuid INetworkPrefabSource.AssetGuid
		{
			get
			{
				return this.AssetGuid;
			}
		}

		public NetworkObjectGuid AssetGuid;
	}
}
