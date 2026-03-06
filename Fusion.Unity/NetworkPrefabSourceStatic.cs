using System;

namespace Fusion
{
	[Serializable]
	public class NetworkPrefabSourceStatic : NetworkAssetSourceStatic<NetworkObject>, INetworkPrefabSource, INetworkAssetSource<NetworkObject>
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
