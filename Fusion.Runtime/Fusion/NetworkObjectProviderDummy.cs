using System;

namespace Fusion
{
	public class NetworkObjectProviderDummy : INetworkObjectProvider
	{
		public NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
		{
			throw new NotImplementedException();
		}

		public void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
		{
			throw new NotImplementedException();
		}

		public NetworkPrefabId GetPrefabId(NetworkRunner runner, NetworkObjectGuid prefabGuid)
		{
			throw new NotImplementedException();
		}

		NetworkObjectAcquireResult INetworkObjectProvider.AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject result)
		{
			return this.AcquirePrefabInstance(runner, context, out result);
		}

		void INetworkObjectProvider.ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
		{
			this.ReleaseInstance(runner, context);
		}
	}
}
