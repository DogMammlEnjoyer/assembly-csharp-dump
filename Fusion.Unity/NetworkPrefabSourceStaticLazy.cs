using System;

namespace Fusion
{
	[Serializable]
	public class NetworkPrefabSourceStaticLazy : NetworkAssetSourceStaticLazy<NetworkObject>, INetworkPrefabSource, INetworkAssetSource<NetworkObject>
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
