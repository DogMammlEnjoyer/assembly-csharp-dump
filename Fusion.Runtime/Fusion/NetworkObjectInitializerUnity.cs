using System;

namespace Fusion
{
	public class NetworkObjectInitializerUnity : INetworkObjectInitializer
	{
		public void InitializeNetworkState(NetworkObject networkObject)
		{
			foreach (NetworkBehaviour networkBehaviour in networkObject.NetworkedBehaviours)
			{
				networkBehaviour.CopyBackingFieldsToState(true);
			}
		}
	}
}
